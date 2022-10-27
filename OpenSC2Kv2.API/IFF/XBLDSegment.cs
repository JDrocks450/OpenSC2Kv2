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
    internal class XBLDSegment : ISegmentItem
    {
        public SC2BuildingDescriptor BuildingDetermination { get; internal set; }
    }

    public class SC2BuildingDescriptor
    {
        public byte DescriptorID { get; internal set; }
        public SC2BuildingTypes Type { get; internal set; }
        public ushort TryGetGraphicID()
        {
            return (ushort)(1000 + DescriptorID);
        }

        public override string ToString()
        {
            return $"XBLD:\nDESC: {DescriptorID}\n BLDID: {Enum.GetName(Type) ?? Type.ToString()}\nSUBTYPE?: {DescriptorID}";
        }
    }

    internal class XBLDSegmentHandler : SegmentHandler
    {
        public override IEnumerable<ISegmentItem> Process(SC2Segment Segment)
        {
            for (int i = 0; i < Segment.Content.Length; i++)
            {
                var xbld = new SC2BuildingDescriptor();
                try
                {
                    var Byte = Segment.Content[i];

                    if (Byte > 0)
                    {
                        xbld.DescriptorID = Byte;
                        xbld.Type = (SC2BuildingTypes)Byte;                        
                    }
                }
                catch
                {

                }

                yield return new XBLDSegment() { BuildingDetermination = xbld };
            }
        }
    }
}
