using System.Text;

namespace OpenSC2Kv2.API.Graphics.Win95
{
    public struct SC2PaletteColor
    {
        public bool Transparent;
        public byte Color;
        public override string ToString()
        {
            return $"SC2Color: {(Transparent ? "Transparent" : Color)}";
        }
    }
    public class SC2GraphicData
    {
        public byte Length;
        public SC2PaletteColor[] Pixels;
        public override string ToString()
        {
            return $"SC2XGFXDAT: Length {Length}";
        }
    }
    public class SC2GraphicBlock
    {
        /// <summary>
        /// Contextual based on the ChunkMode
        /// </summary>
        public byte Count { get; set; }
        public List<SC2GraphicData> Rows { get; set; } = new();
        public override string ToString()
        {
            return $"SC2XBLOCK: Count {Count}";
        }
    }
    public enum SPRChunkMode : byte
    {
        EMPTY = 0,
        /// <summary>
        /// Length is the length of this row's data before the next row.
        /// </summary>
        NEW_ROW = 1,
        EOF = 2,
        /// <summary>
        /// Transparent cells
        /// </summary>
        SKIP = 3,
        /// <summary>
        /// Actual image data
        /// </summary>
        DATA = 4
    }
    public enum SPRPixelMode : byte
    {
        NONE = 0,
        TRANSPARENT = 3,
        PALETTE = 4
    }
    public class SC2GraphicResource
    {
        public SC2SpriteHeader? Header { get; set; }
        public bool Loaded { get; internal set; }
        public bool Animated { get; internal set; }
        public int? Frames { get; internal set; }
        public ushort? Width => Header?.Width;
        public ushort? Height => Header?.Height;

        public int[]? Rotate { get; internal set; }
        public List<string> Textures { get; internal set; }

        private string GetInfo(object type)
        {
            StringBuilder builder = new();
            foreach (var prop in type.GetType().GetProperties())
            {
                var value = prop.GetValue(type);
                if (prop.PropertyType == typeof(SC2SpriteHeader))
                    builder.AppendLine($"{prop.Name}: \n {GetInfo(value)}");
                else
                    builder.AppendLine($"{prop.Name}: {value}");
            }
            return builder.ToString();
        }

        public string ScrapeInformation()
        {
            return GetInfo(this);
        }

        public override string ToString()
        {
            return $"SC2GFXRES: Loaded {Loaded}, Animated {Animated}, Frames {Frames ?? 0}";
        }
    }
}