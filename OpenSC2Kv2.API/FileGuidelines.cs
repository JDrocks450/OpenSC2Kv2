using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API
{
    internal static class FileGuidelines
    {
        internal static string FileHeader = "FORM";
        internal static string FileSchema = "SCDH";
        internal static Dictionary<SC2SegmentTypes, int> LengthMap =
            new()
            {
                { SC2SegmentTypes.MISC, 4800 },

                { SC2SegmentTypes.ALTM, 32768 },
                { SC2SegmentTypes.XTER, 16384 },
                { SC2SegmentTypes.XBLD, 16384 },
                { SC2SegmentTypes.XZON, 16384 },
                { SC2SegmentTypes.XUND, 16384 },
                {
                    SC2SegmentTypes.XTXT,
                    16384
                },
                {
                    SC2SegmentTypes.XLAB,
                    6400
                },
                {
                    SC2SegmentTypes.XMIC,
                    1200
                },
                {
                    SC2SegmentTypes.XTHG,
                    480
                },
                {
                    SC2SegmentTypes.XBIT,
                    16384
                },
                {
                    SC2SegmentTypes.XTRF,
                    4096
                },
                {
                    SC2SegmentTypes.XPLT,
                    4096
                },
                {
                    SC2SegmentTypes.XVAL,
                    4096
                },
                {
                    SC2SegmentTypes.XCRM,
                    4096
                },
                {
                    SC2SegmentTypes.XPLC,
                    1024
                },
                {
                    SC2SegmentTypes.XFIR,
                    1024
                },
                {
                    SC2SegmentTypes.XPOP,
                    1024
                },
                {
                    SC2SegmentTypes.XROG,
                    1024
                },
                {
                    SC2SegmentTypes.XGRP,
                    3328
                },
                { SC2SegmentTypes.CNAM, 32 },
            };
    }
}
