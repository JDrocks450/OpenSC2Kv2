using OpenSC2Kv2.API.Graphics.Win95;
using OpenSC2Kv2.API.Prim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenSC2Kv2.API.Graphics.Win95.SC2GraphicResource;

namespace OpenSC2Kv2.API.Graphics
{
    public class SC2SpriteHeader
    {
        /// <summary>
        /// The ID of this sprite
        /// </summary>
        public ushort ID { get; set; }
        /// <summary>
        /// The offset in the data file where this sprite image data begins.
        /// </summary>
        public uint Start { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public ushort Height { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public ushort Width { get; set; }
        public ushort? Next { get; set; }
        public string ImageName { get; internal set; }
        public uint End { get; internal set; }
        public uint Size { get; internal set; }
        public byte[] Data { get; internal set; }

        public SC2GraphicBlock Block = new();

        public override string ToString()
        {
            return $"Header: Size {Size}, Width {Width}, Height {Height}";
        }
    }
    public class SC2SpriteArchive
    {
        public ushort SpriteCount { get; set; }
        public Dictionary<int, SC2GraphicResource> Graphics = new();

        public SC2GraphicResource? GetGraphicByID(ushort TextureID)
        {
            return Graphics.Values.FirstOrDefault(x => x.Header.ID == TextureID);
        }

        public bool TryGetGraphicByID(ushort TextureID, out SC2GraphicResource Resource)
        {
            Resource = GetGraphicByID(TextureID);
            return Resource != null;
        }
    }
    /// <summary>
    /// Extracts graphics from a resource file.
    /// </summary>
    public class SPRExtractor
    {
        public SPRExtractor(Uri FilePath)
        {
            this.FilePath = FilePath;
        }

        public Uri FilePath { get; }

        public ActionResult<SC2SpriteArchive> ExtractAll()
        {
            if (FilePath == null) return default;
            if (!FilePath.IsAbsoluteUri) return default;
            if (!File.Exists(FilePath.LocalPath)) return default;

            List<string> errors = new();

            SC2SpriteArchive sc2File = new SC2SpriteArchive();

            using (var sprFile = File.OpenRead(FilePath.LocalPath))
            {
                if (!LoadAll(in sprFile, ref errors, ref sc2File))
                    return new ActionResult<SC2SpriteArchive>(errors, null);
            }
            return new ActionResult<SC2SpriteArchive>(errors, sc2File);
        }

        private bool LoadAll(in FileStream sprFile, ref List<string> Errors, ref SC2SpriteArchive archive)
        {
            ushort imageCount = FileTools.ReadUShort(sprFile);
            archive.SpriteCount = imageCount;

            byte readOffset = 2;
            int readLen = imageCount * 10;

            // calculate image ids, offsets and dimensions
            // each image header is stored as a 10 byte chunk
            // only store unique images (1204 and 1183 are duplicated)
            for (int offset = readOffset; offset < readLen + readOffset; offset += 10)
            {
                sprFile.Seek(offset, SeekOrigin.Begin);
                ushort iD = FileTools.ReadUShort(sprFile);
                ushort idx = (ushort)(iD - 1000);                
                uint start = FileTools.ReadUInt(sprFile);
                ushort height = FileTools.ReadUShort(sprFile);
                ushort width = FileTools.ReadUShort(sprFile);
                ushort? next = 0;

                // use the offset start of the next frame to determine the end of this frame
                if (offset + 10 <= readLen - 2)
                {
                    sprFile.Seek(offset + 10, SeekOrigin.Begin);
                    next = (ushort)(FileTools.ReadUShort(sprFile));
                    next -= 1000;
                }
                else next = null;
                if (archive.Graphics.ContainsKey(idx)) continue;
                archive.Graphics.Add(idx, new()
                {
                    Header = new SC2SpriteHeader()
                    {
                        Height = height,
                        Width = width,
                        ID = iD,
                        Start = start,
                        Next = next
                    }
                });
            }
            int index = 0;
            // calculate image ending offset
            // separate loop so we can easily get the end byte of the following frame
            foreach (var value in archive.Graphics)
            {
                var tile = value.Value;
                tile.Header.ImageName = $"{ tile.Header.ID + 1000}";
                tile.Header.End = tile.Header.Next != null? archive.Graphics[tile.Header.Next.Value].Header.Start : (uint)readLen;
                if (tile.Header.End < tile.Header.Start) continue;
                tile.Header.Size = tile.Header.End - tile.Header.Start;
                tile.Header.Data = new byte[tile.Header.Size];
                sprFile.Seek(tile.Header.Start, SeekOrigin.Begin);
                sprFile.Read(tile.Header.Data, 0, tile.Header.Data.Length);
                tile.Header.Block = parseBlock(tile.Header);

                tile.Loaded = false;
                tile.Animated = isAnimatedImage(tile.Header.Block);
                tile.Frames = tile.Frames ?? getFrameCount(tile.Header.Block);
                tile.Rotate = tile.Rotate ?? new int[4] { value.Key, value.Key, value.Key, value.Key };
                //tile.hitbox = shape(tile.hitbox ?? tile.heightmap ?? tiles.data[256].heightmap);

                tile.Textures = new List<string>();

                for (int t = 0; t < tile.Frames; t++)
                {
                    tile.Textures.Add($"{ tile.Header.ID + 1000}_{t}");
                }

                index++;
            }

            return true;
        }

        int getFrameCount(SC2GraphicBlock Block) {
            List<int> frames = new();

            for (int y = 0; y < Block.Rows.Count; y++) {
                for (int x = 0; x < Block.Rows[y].Pixels.Length; x++) {
                    SC2PaletteColor color = Block.Rows[y].Pixels[x];
                    int frameCount = getPaletteFrameCount(color.Color);
                    frames.Add(frameCount);
                }
            }

            if (!frames.Any())
            {
                return 1;
            }
            else
            {
                int result = MathExtensions.lcm(frames.ToArray());
                return result;
            }
        }

        private int getPaletteFrameCount(byte color)
        {
            if (color >= 200 && color <= 211)
            {
                return 12;
            }

            if ((color >= 171 && color <= 194) || (color >= 212 && color <= 219))
            {
                return 8;
            }

            if ((color >= 195 && color <= 198) || (color >= 220 && color <= 231))
            {
                return 4;
            }

            return 1;
        }    

        private bool isAnimatedImage(SC2GraphicBlock Block) {
            for (int y = 0; y < Block.Count; y++)
            {
                for (int x = 0; x < Block.Rows[y].Pixels.Length; x++)
                {
                    if (isAnimatedIndex(Block.Rows[y].Pixels[x]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private bool isAnimatedIndex(SC2PaletteColor index) {
            if ((index.Color >= 171 && index.Color <= 198) ||
                (index.Color >= 200 && index.Color <= 219) ||
                (index.Color >= 224 && index.Color <= 231)) {
                return true;
            } else {
                return false;
            }
        }

        private SC2GraphicBlock parseBlock(SC2SpriteHeader header)
        {
            SC2GraphicBlock block = new();
            int offset = 0x00;

            while (offset < header.Size)
            {                
                byte length = header.Data[offset];
                SPRChunkMode mode = (SPRChunkMode)header.Data[offset + 0x01];

                offset += 0x02;

                byte[]? data = header.Data[offset..(offset + length)];
                if (data == null)
                    ; // WHAT
                var pixels = parseRow(data);

                block.Rows.Add(new()
                {
                    Length = length,
                    Pixels = pixels.ToArray()
                });

                // check "more" flag
                if (mode == SPRChunkMode.EOF)
                {
                    break;
                }

                offset += length;
            }
            block.Count = (byte)block.Rows.Count;
            return block;
        }

        private List<SC2PaletteColor> parseRow(byte[] buffer)
        {            
            List<SC2PaletteColor> pixels = new();
            int offset = 0x00;

            if (buffer.Length == 0x00)
            {
                return pixels;
            }

            // loop through the row chunks
            while (offset < buffer.Length - 0x01)
            {
                int count = buffer[offset + 0x00];
                byte[] pixelData;
                int extra = 0;
                int padding = 0;
                int length = 0;
                int header = 0;

                // special case for multi-chunk rows, drop first byte if zero
                if (count == 0x00 && offset > 0x00)
                {
                    offset++;
                }

                SPRPixelMode mode = (SPRPixelMode)buffer[offset + 0x01];

                if (mode == SPRPixelMode.NONE || mode == SPRPixelMode.TRANSPARENT)
                {
                    padding = buffer[(offset + 0x00)]; // padding pixels from the left edge
                    length = buffer[(offset + 0x02)]; // pixels in the row to draw
                    extra = buffer[(offset + 0x03)]; // extra bit / flag

                    if (length == 0x00 && extra == 0x00)
                    {
                        header = 0x06;
                        length = buffer[(offset + 0x04)];
                        extra = buffer[(offset + 0x05)];
                        int start = offset + header, end = offset + header + length;
                        pixelData = buffer[start..end];
                    }
                    else
                    {
                        header = 0x04;
                        pixelData = buffer[(offset + header) .. (offset + header + length)];
                    }
                }
                else if (mode == SPRPixelMode.PALETTE)
                {
                    header = 0x02;
                    length = buffer[(offset + 0x00)];
                    pixelData = buffer[(offset + header)..(offset + header + length)];
                }
                else
                {
                    return pixels;
                }

                // byte offset for the next loop
                offset += header + length;

                // save padding pixels (transparent) as -1
                for (int i = 0; i < padding; i++)
                {
                    pixels.Add(new()
                    {
                        Transparent = true
                    });
                }

                // save pixel data afterwards
                if (pixelData != default && pixelData.Any())
                {
                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        pixels.Add(new()
                        {
                            Transparent = false,
                            Color = pixelData[i]
                        });
                    }
                }
            }

            return pixels;
        }
    }
}
