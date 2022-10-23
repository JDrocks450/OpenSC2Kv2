using OpenSC2Kv2.API.Graphics.Win95;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.Graphics
{
    public class SPRRender
    {
        public Bitmap? Render(SC2GraphicResource Resource, int Frame, SC2Palette? Palette = default)
        {
            if (Palette == null)
                Palette = SC2Palette.Default;
            var tile = Resource;
            // skip tiles that were already flagged as loaded
            if (tile.Loaded)
            {
                return null;
            }

            Bitmap image = new Bitmap(tile.Width.Value, tile.Height.Value);

            // loop on every frame            
            for (int ty = 0; ty < tile.Header.Block.Rows.Count; ty++)
            {
                for (int tx = 0; tx < tile.Header.Block.Rows[ty].Pixels.Length; tx++)
                {
                    // palette index value
                    SC2PaletteColor index = tile.Header.Block.Rows[ty].Pixels[tx];

                    // set color and canvas x/y index
                    //bitmap.setPixel(x + tx, y + ty, palette.getColor(index, f));
                    image.SetPixel(tx, ty, Palette.GetColor(index, Frame));
                }
            }            
            return image;
        }

        public void ExportSheet(SC2SpriteArchive archive)
        {
            int x = 1;
            int y = 1;
            int maxWidth = 16;
            int maxHeight = 8;
            int rowMaxY = 0;
            int size = 4096;
            Dictionary<string, dynamic> json = new();
            int padding = 1;
            Bitmap image = new Bitmap(size, size);

            var tiles = archive.Graphics;
            var palette = SC2Palette.Default;

            // looping 128 times here to sort tiles by size
            // this shuffles the smaller tiles to the front of the tilemap
            for (int loop = 0; loop < 128; loop++)
            {
                foreach(var tile in tiles.Values)
                {                    
                    // skip tiles that were already flagged as loaded
                    if (tile.Loaded)
                    {
                        continue;
                    }

                    // skip anything that exceeds the current maximum
                    if (tile.Width > maxWidth || tile.Height > maxHeight)
                    {
                        continue;
                    }

                    // loop on every frame
                    for (int f = 0; f < tile.Frames; f++)
                    {
                        // max tile height in this row
                        if (tile.Height > rowMaxY)
                        {
                            rowMaxY = tile.Height.Value;
                        }

                        // exceeds tilemap width, start a new row
                        if (x + tile.Width > size)
                        {
                            x = 1;
                            y += rowMaxY + padding;
                            rowMaxY = 0;
                        }                        

                        for (int ty = 0; ty < tile.Header.Block.Count; ty++)
                        {
                            for (int tx = 0; tx < tile.Header.Block.Rows[ty].Pixels.Length; tx++)
                            {
                                // palette index value
                                SC2PaletteColor index = tile.Header.Block.Rows[ty].Pixels[tx];

                                // set color and canvas x/y index
                                //bitmap.setPixel(x + tx, y + ty, palette.getColor(index, f));
                                image.SetPixel(x + tx, y + ty, palette.GetColor(index, f));
                            }
                        }

                        // add tilemap data
                        json.Add($"{ tile.Header.ImageName}_{f}",
                            new
                            {
                                frame = new { X = x, Y = y, W = tile.Width, H = tile.Height },
                                Rotated = false,
                                Trimmed = false,
                                SpriteSourceSize = new { x = 0, y = 0, w = tile.Width, h = tile.Height },
                                SourceSize = new { W = tile.Width, H = tile.Height },
                            });

                        // move drawing position + padding
                        x += tile.Height ?? 0 + padding;

                        // flag tile as loaded if the frame count matches the current frame
                        // or if the tile has no frames
                        if (tile.Frames == f + 1 || tile.Frames == 1)
                        {
                            tile.Loaded = true;
                        }
                    }
                }

                // increase tile size next loop
                maxWidth = maxWidth + 4;
                maxHeight = maxHeight + 4;
            }

            using (var fs = File.CreateText("export/tilemap.json"))
                fs.Write(JsonSerializer.Serialize(json));

            // save tilemap image
            image.Save("export/tilemap.bmp");
        }
    }
}
