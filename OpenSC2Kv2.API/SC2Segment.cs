using OpenSC2Kv2.API.IFF;
using System.Text;

namespace OpenSC2Kv2.API
{   
    public class SC2Segment
    {
        /// <summary>
        /// The type of segment it is
        /// </summary>
        public string Type
        {
            get; set;
        }
        /// <summary>
        /// The compressed length of the content of this segment
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// The offset of this segment, with header included
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// The offset where the content starts
        /// </summary>
        public long ContentOffset => Offset + 8;
        public string Text => GetString();
        public byte[] Content
        {
            get
            {
                if (_content == null)
                    _content = GetContent();
                return _content;
            }
        }
        /// <summary>
        /// Overrides the content of this segment to be the provided value.
        /// <para><see cref="Chunks"/> is no longer the source of this data if this is manually set.</para>
        /// <para>To reverse this change, supply <see langword="null"/> and make sure <see cref="Chunks"/> are loaded, then, evaluate the property again.</para>
        /// </summary>
        /// <param name="Array"></param>
        internal void OverrideContent(byte[] Array) =>        
            _content = Array;        
        /// <summary>
        /// All of the chunks added to this segment.
        /// </summary>
        public IEnumerable<SC2SegChunk> Chunks => Chunks;

        public bool IsProcessed => ProcessedItems != null;
        /// <summary>
        /// This is specifically for OpenSC2K.
        /// <para>The <see cref="IFFParser"/> can process segments into API-friendly code blocks.
        /// This property is available if the SC2 file was processed using <see cref="IFFParser"/> and
        /// is a supported form of data.
        /// </para>
        /// </summary>
        public IEnumerable<ISegmentItem>? ProcessedItems { get; set; } = null;
        public SC2SegmentTypes SC2Type => Enum.Parse<SC2SegmentTypes>(Type.ToUpper());

        private List<SC2SegChunk> chunks = new();
        private byte[]? _content;

        public bool AddChunk(SC2SegChunk Chunk)
        {
            chunks.Add(Chunk);
            return true;
        }
        public byte[] GetContent()
        {
            if (_content != null)
                return _content;
            using (var stream = new MemoryStream())
            {
                foreach (var chunk in chunks)
                    stream.WriteAsync(new ReadOnlyMemory<byte>(chunk.GetContent()));
                return stream.ToArray();
            }
        }
        /// <summary>
        /// Attempts to read the content of this segment as a string. (ASCII)
        /// </summary>
        /// <returns></returns>
        public string GetString() => Encoding.ASCII.GetString(GetContent());
        public override string ToString()
        {
            return $"SC2SEG: U:{Content.Length} C:{Length} CHUNKS:{Chunks.Count()} " +
                $"RUN: {Chunks.Where(x => x.Type == SC2SegChunk.SC2SegChunkType.REPEAT_SIZE).Count()} " +
                $"UNC: {Chunks.Where(x => x.Type == SC2SegChunk.SC2SegChunkType.ARRAY_SIZE).Count()}";
        }
    }
}
