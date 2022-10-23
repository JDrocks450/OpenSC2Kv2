using OpenSC2Kv2.API.IFF;

namespace OpenSC2Kv2.API
{
    /// <summary>
    /// Represents a chunk of data in a segment
    /// </summary>
    public class SC2SegChunk
    {
        public enum SC2SegChunkType
        {
            ARRAY_SIZE,
            REPEAT_SIZE
        }
        /// <summary>
        /// The type of chunk
        /// </summary>
        public SC2SegChunkType Type { get; set; }
        /// <summary>
        /// The length of data, uncompressed.
        /// </summary>
        public byte Length { get; set; }
        /// <summary>
        /// The byte to repeat, if Type is <see cref="SC2SegChunkType.REPEAT_SIZE"/>
        /// </summary>
        public byte RepeatValue { get; set; }
        public static SC2SegChunkType GetTypeByMagicByte(byte Magic, out byte Length)
        {
            var returnValue = Magic < 128 ? SC2SegChunkType.ARRAY_SIZE : SC2SegChunkType.REPEAT_SIZE;
            Length = 0;
            switch (returnValue)
            {
                case SC2SegChunkType.ARRAY_SIZE: Length = Magic; break;
                case SC2SegChunkType.REPEAT_SIZE: Length = (byte)(Magic - 127); break;
            }
            return returnValue;
        }
        /// <summary>
        /// If Type is <see cref="SC2SegChunkType.ARRAY_SIZE"/>, this is the content.
        /// </summary>
        public byte[]? ArrayContent { get; set; } = null;
        public byte[] GetContent()
        {
            switch (Type)
            {
                case SC2SegChunkType.ARRAY_SIZE: return ArrayContent ?? new byte[Length];
                case SC2SegChunkType.REPEAT_SIZE:
                    var retVal = new byte[Length];
                    Array.Fill(retVal, RepeatValue);
                    return retVal;
            };
            return new byte[Length];
        }
    }

    public class SC2CityHeader
    {
        public string Name { get; set; } = "NewCity";
        public int Width { get; set; }
        public int Height { get; set; }
        public byte Rotation { get; set; }        
        public int WaterLevel { get; set; }
    }

    /// <summary>
    /// Represents a processed *.SC2 file.
    /// <para><see cref="IFFParser"/></para>
    /// </summary>
    public class SC2File
    {
        public SC2CityHeader CityInformation { get; set; } = new();
        public string Header { get; internal set; } = "NONE";
        public string Schema { get; internal set; } = "NONE";
        public int ContentLength { get; internal set; }
        public IEnumerable<SC2Segment> Segments => segments;
        
        private List<SC2Segment> segments = new();

        public IEnumerable<SC2Segment> GetSegmentsByType(SC2SegmentTypes Type) => segments.Where(s => s.SC2Type == Type);

        public SC2Segment? GetSegmentByType(SC2SegmentTypes Type) => GetSegmentsByType(Type).FirstOrDefault();
        /// <summary>
        /// Searches for all segments that match the given segment type, grabs the processed segment data, and casts it to the given type parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="ISegmentItem"/> type to cast to.</typeparam>
        /// <param name="SegmentType">The <see cref="SC2SegmentTypes"/> segment to look for.</param>
        /// <returns></returns>
        public IEnumerable<T>? GetSegmentItemData<T>(SC2SegmentTypes SegmentType) where T : ISegmentItem =>
            GetSegmentByType(SegmentType).ProcessedItems.Cast<T>();

        public bool AddSegment(SC2Segment Segment)
        {
            segments.Add(Segment);
            return true;
        }
    }
}
