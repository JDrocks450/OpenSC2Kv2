using OpenSC2Kv2.API.IFF;
using OpenSC2Kv2.API.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API
{
    public class ActionResult<T>
    {
        public ActionResult(List<string> errors, T value)
        {
            Errors = errors;
            Value = value;
        }

        public IEnumerable<string> Errors { get; }
        public T? Value { get; }
    }

    /// <summary>
    /// Parses an IFF (*.SC2 in this application) to a code object.
    /// </summary>
    public class IFFParser
    {
        public Uri SelectedResource
        {
            get;
        }

        public bool ParseStarted { get; set; } = false;
        public bool ParseSuccessful { get; set; } = false;
        public SC2File ParsedFile { get; private set; }
        public SC2World LoadedWorld { get; private set; }

        /// <summary>
        /// Takes a path to a resource containing the *.SC2 file.
        /// </summary>
        /// <param name="ResourcePath"></param>
        public IFFParser(Uri ResourcePath)
        {
            SelectedResource = ResourcePath;
        }

        /// <summary>
        /// Begins parsing the document
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult<SC2File>> ParseAsync()
        {
            if (SelectedResource == null) return default;
            if (!SelectedResource.IsAbsoluteUri) return default;
            if (!File.Exists(SelectedResource.LocalPath)) return default;

            ParseStarted = true;

            List<string> errors = new();

            SC2File sc2File = new SC2File();

            using(var iffFile = File.OpenRead(SelectedResource.LocalPath))
            {
                GetHeader(in iffFile, ref errors, ref sc2File);
                while(GetNextSegment(in iffFile, ref errors, ref sc2File))
                {

                }
            }

            sc2File.CityInformation = new()
            {
                WaterLevel = 6,
                Width = 128,
                Height = 128,
                Name = sc2File.GetSegmentByType(SC2SegmentTypes.CNAM)?.Text ?? "NewCity",                
            };

            SC2World world = await SC2WorldGenerator.GenerateWorld(sc2File);            

            foreach(var segment in sc2File.Segments)
            {
                if (segment.IsProcessed) continue;
                var segmentItems = SegmentHandler.TryProcess(segment);
                segment.ProcessedItems = segmentItems;
            }

            SC2WorldGenerator.ApplyData(world, sc2File.GetSegmentItemData<ALTMSegment>(SC2SegmentTypes.ALTM));
            SC2WorldGenerator.ApplyData(world, sc2File.GetSegmentItemData<XTERSegment>(SC2SegmentTypes.XTER));
            SC2WorldGenerator.ApplyData(world, sc2File.GetSegmentItemData<XBLDSegment>(SC2SegmentTypes.XBLD));
            SC2WorldGenerator.ApplyData(world, sc2File.GetSegmentItemData<XZONSegment>(SC2SegmentTypes.XZON));

            LoadedWorld = world;
            ParsedFile = sc2File;

            ParseSuccessful = true;

            return new ActionResult<SC2File>(errors,sc2File);
                
        }

        private bool GetHeader(in FileStream iffFile, ref List<string> errors, ref SC2File sc2File)
        {
            string fileHeader = FileTools.ReadString(iffFile, 4);
            int fileLength = FileTools.ReadInt(iffFile);
            string fileSchema = FileTools.ReadString(iffFile, 4);

            //VERIFY
            if (fileHeader != FileGuidelines.FileHeader)
                errors.Add("This is not a recognized file format. Header: " + fileHeader);
            else sc2File.Header = fileHeader;
            if (fileSchema != FileGuidelines.FileSchema)
                errors.Add("This is not a SC2 formatted IFF. Schema: " + fileSchema);
            else sc2File.Schema = fileSchema;
            sc2File.ContentLength = fileLength;
            return !errors.Any();
        }

        private bool GetNextSegment(in FileStream iffFile, ref List<string> errors, ref SC2File sc2File)
        {
            if (iffFile.Position >= iffFile.Length) return false;
            long offset = iffFile.Position;
            string type = FileTools.ReadString(iffFile, 4);
            int length = FileTools.ReadInt(iffFile);
            SC2Segment newSegment = new()
            {
                Type = type,
                Offset = offset,
                Length = length
            };
            if (type == "ALTM")
            { // ALTM is always stored uncompressed, there is no need to run the uncompressor here.
                byte[] data = new byte[length];
                iffFile.Read(data, 0, length);
                newSegment.OverrideContent(data);
            }
            else
            {
                for (int i = 0; i < length;)
                {
                    iffFile.Position = offset + 8 + i;
                    //GET CHUNK **
                    byte magic = (byte)iffFile.ReadByte();
                    i++;
                    if (magic == 0 || magic == 128) continue;
                    var cnktype = SC2SegChunk.GetTypeByMagicByte(magic, out byte chkLen);
                    var chunk = new SC2SegChunk()
                    {
                        Type = cnktype,
                        Length = chkLen
                    };
                    switch (cnktype)
                    {
                        case SC2SegChunk.SC2SegChunkType.ARRAY_SIZE:
                            chunk.ArrayContent = new byte[chkLen];
                            iffFile.Read(chunk.ArrayContent, 0, chkLen);
                            i += chkLen;
                            break;
                        case SC2SegChunk.SC2SegChunkType.REPEAT_SIZE:
                            chunk.RepeatValue = (byte)iffFile.ReadByte();
                            i++;
                            break;
                    }
                    newSegment.AddChunk(chunk);

                }
            }
            sc2File.AddSegment(newSegment);
            var c = newSegment.Content;
            iffFile.Seek(offset + length + 8, SeekOrigin.Begin);
            return true;
        }
    }
}
