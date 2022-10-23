using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.IFF
{
    /// <summary>
    /// Represents information extracted from a XTERSegment.
    /// <para>XTER specifically has a segment item for EVERY tile, meaning there will be quite a number of these.</para>
    /// </summary>
    internal class XTERSegment : ISegmentItem
    {
        /// <summary>
        /// The descriptor for the given terrain on a world tile.
        /// </summary>
        public SC2TerrainDescriptor? TerrainDescriptor { get; set; } = default;
    }

    public class SC2TerrainDescriptor
    {
        public ushort TerrainDescriptorInstance { get; set; }
        /// <summary>
        /// The ID of the corrosponding TerrainTile graphic to use.
        /// </summary>
        public int TerrainID { get; set; }
        /// <summary>
        /// The ID of the corrosponding WaterTile graphic to use.
        /// </summary>
        public int? WaterID { get; set; }
        public bool Watered => WaterID != null;
        /// <summary>
        /// A description of the tile.
        /// </summary>
        public string TypeDescription { get; set; } = "Blank";
        public override string ToString()
        {
            return $"XTER:\nDESC: {TerrainDescriptorInstance}\n TERID: {TerrainID}\nWATID: {WaterID}, TYPE: {TypeDescription}";
        }
    }

    internal class XTERSegmentHandler : SegmentHandler
    {
        // terrain tile id is set in all cases so we know
        // what type of terrain tile to display when water
        // is hidden or heightmap is displayed
        public static Dictionary<int, SC2TerrainDescriptor> TerrainTypeMap = new()
        {
            // land
            { 0x00, new SC2TerrainDescriptor() { TerrainID = 256, WaterID = null, TypeDescription = "land" } },
            { 0x01, new SC2TerrainDescriptor() { TerrainID = 257, WaterID = null, TypeDescription = "land" } },
            { 0x02, new SC2TerrainDescriptor() { TerrainID = 258, WaterID = null, TypeDescription = "land" } },
            { 0x03, new SC2TerrainDescriptor() { TerrainID = 259, WaterID = null, TypeDescription = "land" } },
            { 0x04, new SC2TerrainDescriptor() { TerrainID = 260, WaterID = null, TypeDescription = "land" } },
            { 0x05, new SC2TerrainDescriptor() { TerrainID = 261, WaterID = null, TypeDescription = "land" } },
            { 0x06, new SC2TerrainDescriptor() { TerrainID = 262, WaterID = null, TypeDescription = "land" } },
            { 0x07, new SC2TerrainDescriptor() { TerrainID = 263, WaterID = null, TypeDescription = "land" } },
            { 0x08, new SC2TerrainDescriptor() { TerrainID = 264, WaterID = null, TypeDescription = "land" } },
            { 0x09, new SC2TerrainDescriptor() { TerrainID = 265, WaterID = null, TypeDescription = "land" } },
            { 0x0a, new SC2TerrainDescriptor() { TerrainID = 266, WaterID = null, TypeDescription = "land" } },
            { 0x0b, new SC2TerrainDescriptor() { TerrainID = 267, WaterID = null, TypeDescription = "land" } },
            { 0x0c, new SC2TerrainDescriptor() { TerrainID = 268, WaterID = null, TypeDescription = "land" } },
            { 0x0d, new SC2TerrainDescriptor() { TerrainID = 269, WaterID = null, TypeDescription = "land" } },

            // underwater
            { 0x10, new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 270, TypeDescription = "submerged" } },


            {
                0x11,
                new SC2TerrainDescriptor() { TerrainID = 257, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x12,
                new SC2TerrainDescriptor() { TerrainID = 258, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x13,
                new SC2TerrainDescriptor() { TerrainID = 259, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x14,
                new SC2TerrainDescriptor() { TerrainID = 260, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x15,
                new SC2TerrainDescriptor() { TerrainID = 261, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x16,
                new SC2TerrainDescriptor() { TerrainID = 262, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x17,
                new SC2TerrainDescriptor() { TerrainID = 263, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x18,
                new SC2TerrainDescriptor() { TerrainID = 264, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x19,
                new SC2TerrainDescriptor() { TerrainID = 265, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x1a,
                new SC2TerrainDescriptor() { TerrainID = 266, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x1b,
                new SC2TerrainDescriptor() { TerrainID = 267, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x1c,
                new SC2TerrainDescriptor() { TerrainID = 268, WaterID = 270, TypeDescription = "submerged" }
            },
            {
                0x1d,
                new SC2TerrainDescriptor() { TerrainID = 269, WaterID = 270, TypeDescription = "submerged" }
            },

            // coast
            {
                0x20,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 270, TypeDescription = "coast" }
            },
            {
                0x21,
                new SC2TerrainDescriptor() { TerrainID = 257, WaterID = 271, TypeDescription = "coast" }
            },
            {
                0x22,
                new SC2TerrainDescriptor() { TerrainID = 258, WaterID = 272, TypeDescription = "coast" }
            },
            {
                0x23,
                new SC2TerrainDescriptor() { TerrainID = 259, WaterID = 273, TypeDescription = "coast" }
            },
            {
                0x24,
                new SC2TerrainDescriptor() { TerrainID = 260, WaterID = 274, TypeDescription = "coast" }
            },
            {
                0x25,
                new SC2TerrainDescriptor() { TerrainID = 261, WaterID = 275, TypeDescription = "coast" }
            },
            {
                0x26,
                new SC2TerrainDescriptor() { TerrainID = 262, WaterID = 276, TypeDescription = "coast" }
            },
            {
                0x27,
                new SC2TerrainDescriptor() { TerrainID = 263, WaterID = 277, TypeDescription = "coast" }
            },
            {
                0x28,
                new SC2TerrainDescriptor() { TerrainID = 264, WaterID = 278, TypeDescription = "coast" }
            },
            {
                0x29,
                new SC2TerrainDescriptor() { TerrainID = 265, WaterID = 279, TypeDescription = "coast" }
            },
            {
                0x2a,
                new SC2TerrainDescriptor() { TerrainID = 266, WaterID = 280, TypeDescription = "coast" }
            },
            {
                0x2b,
                new SC2TerrainDescriptor() { TerrainID = 267, WaterID = 281, TypeDescription = "coast" }
            },
            {
                0x2c,
                new SC2TerrainDescriptor() { TerrainID = 268, WaterID = 282, TypeDescription = "coast" }
            },
            {
                0x2d,
                new SC2TerrainDescriptor() { TerrainID = 269, WaterID = 283, TypeDescription = "coast" }
            },

            // surface water
            {
                0x30,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 270, TypeDescription = "surface" }
            },
            {
                0x31,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 271, TypeDescription = "surface" }
            },
            {
                0x32,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 272, TypeDescription = "surface" }
            },
            {
                0x33,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 273, TypeDescription = "surface" }
            },
            {
                0x34,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 274, TypeDescription = "surface" }
            },
            {
                0x35,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 275, TypeDescription = "surface" }
            },
            {
                0x36,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 276, TypeDescription = "surface" }
            },
            {
                0x37,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 277, TypeDescription = "surface" }
            },
            {
                0x38,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 278, TypeDescription = "surface" }
            },
            {
                0x39,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 279, TypeDescription = "surface" }
            },
            {
                0x3a,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 280, TypeDescription = "surface" }
            },
            {
                0x3b,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 281, TypeDescription = "surface" }
            },
            {
                0x3c,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 282, TypeDescription = "surface" }
            },
            {
                0x3d,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 283, TypeDescription = "surface" }
            },

            // waterfall
            {
                0x3e,
                new SC2TerrainDescriptor() { TerrainID = 269, WaterID = 284, TypeDescription = "waterfall" }
            },

            // streams
            {
                0x40,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 285, TypeDescription = "surface" }
            },
            {
                0x41,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 286, TypeDescription = "surface" }
            },
            {
                0x42,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 287, TypeDescription = "surface" }
            },
            {
                0x43,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 288, TypeDescription = "surface" }
            },
            {
                0x44,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 289, TypeDescription = "surface" }
            },
            {
                0x45,
                new SC2TerrainDescriptor() { TerrainID = 256, WaterID = 290, TypeDescription = "surface" }
            },
        };

        public override IEnumerable<ISegmentItem> Process(SC2Segment Segment)
        {
            for(int i = 0; i < Segment.Content.Length; i++)
            {
                var xter = new SC2TerrainDescriptor();
                try
                {
                    var Byte = Segment.Content[i];

                    if (TerrainTypeMap.ContainsKey(Byte))
                    {
                        xter.TerrainDescriptorInstance = Byte;
                        xter.TerrainID = TerrainTypeMap[Byte].TerrainID;                        
                        xter.WaterID = TerrainTypeMap[Byte].WaterID;
                        xter.TypeDescription = TerrainTypeMap[Byte].TypeDescription;                        
                    }
                }
                catch
                {

                }
                
                yield return new XTERSegment() { TerrainDescriptor = xter };
            }
        }
    }
}
