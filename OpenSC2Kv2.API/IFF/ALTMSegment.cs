using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.IFF
{

    /// <summary>
    /// Represents a ALTM segment
    /// </summary>
    public class ALTMSegment : ISegmentItem
    {
        public int Index { get; set; }
        public int TunnelLevels { get; internal set; }
        public int TunnelParam { get; internal set; }
        public ushort Altitude { get; internal set; }
        public int WaterLevel { get; internal set; }
        public bool IsWaterCovered { get; internal set; }
    }

    /// <summary>
    /// Processes an <see cref="ALTMSegment"/>
    /// </summary>
    public class ALTMSegmentHandler : SegmentHandler
    {
        private static ushort GetUInt16(SC2Segment segment, int offset) =>
            BitConverter.ToUInt16(segment.Content[(offset * 2)..((offset * 2) + 2)].Reverse().ToArray());
        public override IEnumerable<ALTMSegment> Process(SC2Segment Segment)
        {
            for (int i = 0; i < (Segment.Content.Length / 2); i++)
            {
                var bits = GetUInt16(Segment, i);
                ALTMSegment altm = new()
                {
                    Index = i
                };

                var binary = "0b_" + Convert.ToString(bits, 2).PadLeft(16,'0');

                // how many levels below altitude should we display a grey
                // block for a tunnel?
                altm.TunnelLevels = ((bits >> 8) & 0b11111100) >> 2;

                // related to tunnel? appears to be set to 1 for
                // hydroelectric dams and nearby surface water tiles
                altm.TunnelParam = ((bits >> 8) & 0b00000011);

                // level / altitude
                altm.Altitude = (ushort)(bits & (0b0000000000011111));

                // not always accurate (rely on XTER value instead)
                altm.WaterLevel = (bits & 0b0000000001100000) >> 5;

                // not always accurate (rely on XTER value instead)
                int waterFlag = (bits & 0b0000000010000000) >> 7;
                if (waterFlag == 0)
                    altm.IsWaterCovered = false;
                else altm.IsWaterCovered = true;

                yield return altm;
            }
        }
    }
}
