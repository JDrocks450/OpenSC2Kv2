using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.IFF
{
    internal class XZONSegment : ISegmentItem
    {
        public SC2ZoneDescriptor ZoneInformation { get; set; }
    }

    public enum SC2CornerFlag
    {
        /// <summary>
        /// This is the KEY TILE.
        /// </summary>
        TOP,
        /// <summary>
        /// Bottom tile
        /// </summary>
        BOTTOM,
        /// <summary>
        /// Left tile
        /// </summary>
        LEFT,
        /// <summary>
        /// Right tile
        /// </summary>
        RIGHT,
        /// <summary>
        /// No flags have been set
        /// </summary>
        NONE,
    }

    /// <summary>
    /// Describes <see cref="XZONSegment"/> information such as Corner flags, Zone information, etc.
    /// </summary>
    public class SC2ZoneDescriptor
    {
        /// <summary>
        /// Dictates which corner of a building this tile represents. 
        /// <para>For 1x1 tiles, this should be ignored.</para>
        /// </summary>
        public SC2CornerFlag CornerFlag { get; set; } = SC2CornerFlag.NONE;
        /// <summary>
        /// Metadata about this zone's graphical properties
        /// </summary>
        public SC2ZoneGraphicDescription? Zone { get; set; } = null;
        public override string ToString()
        {
            return $"XZON: CORNER: {CornerFlag}\nZone?: {Zone?.ToString() ?? "null"}";
        }
    }
    /// <summary>
    /// Contains information on which graphic tile links to what zone type
    /// </summary>
    public class SC2ZoneGraphicDescription
    {
        /// <summary>
        /// The texture's ID.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The description of this zone.
        /// </summary>
        public string Name { get; set; }
        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}";
        }
    }

    /// <summary>
    /// Represents the different tile zone types SC2 recognizes
    /// </summary>
    public enum SC2CityZoneTypes
    {
        EMPTY,
        /// <summary>
        /// Light residential
        /// </summary>
        L_RES,
        /// <summary>
        /// Dense residential
        /// </summary>
        D_RES,
        /// <summary>
        /// Light commercial
        /// </summary>
        L_COM,
        /// <summary>
        /// Dense commercial
        /// </summary>
        D_COM,
        /// <summary>
        /// Light Industry
        /// </summary>
        L_IND,
        /// <summary>
        /// Dense Industry
        /// </summary>
        D_IND,
        MIL,
        AIRPORT,
        SEAPORT
    }

    internal class XZONSegmentHandler : SegmentHandler
    {
        private static Dictionary<SC2CityZoneTypes, SC2ZoneGraphicDescription> xzonMap = new()
        {
            { SC2CityZoneTypes.EMPTY, null },
            { (SC2CityZoneTypes)0x01, new() { ID = 291, Name = "lightResidential" } },
            { (SC2CityZoneTypes)0x02, new() { ID = 292, Name = "denseResidential" } },
            { (SC2CityZoneTypes)0x03, new() { ID = 293, Name = "lightCommercial" } },
            { (SC2CityZoneTypes)0x04, new() { ID = 294, Name = "denseCommercial" } },
            { (SC2CityZoneTypes)0x05, new() { ID = 295, Name = "lightIndustrial" } },
            { (SC2CityZoneTypes)0x06, new() { ID = 296, Name = "denseIndustrial" } },
            { (SC2CityZoneTypes)0x07, new() { ID = 297, Name = "military" } },
            { (SC2CityZoneTypes)0x08, new() { ID = 298, Name = "airport" } },
            { (SC2CityZoneTypes)0x09, new() { ID = 299, Name = "seaport" } },
        };

        public override IEnumerable<ISegmentItem> Process(SC2Segment Segment)
        {
            foreach (var Byte in Segment.GetContent()) {                
                SC2ZoneDescriptor zone = new SC2ZoneDescriptor();
                XZONSegment xzon = new()
                {
                    ZoneInformation = zone
                };

                // indicates the tile is a key / corner tile
                // for a building larger than 1x1 tile
                if ((Byte & 0b00010000) != 0) zone.CornerFlag = SC2CornerFlag.TOP;
                if ((Byte & 0b00100000) != 0) zone.CornerFlag = SC2CornerFlag.RIGHT;
                if ((Byte & 0b01000000) != 0) zone.CornerFlag = SC2CornerFlag.BOTTOM;
                if ((Byte & 0b10000000) != 0) zone.CornerFlag = SC2CornerFlag.LEFT;

                // indicates the tile has no key / corners set
                if ((Byte & 0b11110000) == 0) zone.CornerFlag = SC2CornerFlag.NONE;

                // tile zone id and type
                zone.Zone = xzonMap[(SC2CityZoneTypes)(Byte & 0b00001111)];

                yield return xzon;
            }
        }        
    }
}
