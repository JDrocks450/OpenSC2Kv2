using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API
{
    /// <summary>
    /// Assists in locating files in the SC2000 game installation folder
    /// <para/>Ensure <see cref="Setup"/> has been called with the SC2000 base game folder to use this service
    /// </summary>
    public static class SC2Path
    {
        public static DirectoryInfo BaseGamePath { get; set; }

        public static bool Ready => BaseGamePath != null;

        /// <summary>
        /// Sets up the service for first use in the lifetime of this application by setting the <see cref="BaseGamePath"/> property to be whatever is specified.
        /// <para/>Should be to the base directory for installed SC2K files. Is sometimes called "SC2K"
        /// </summary>
        public static void Setup(string BaseGameDirectory)
        {
            BaseGamePath = new(BaseGameDirectory);
            if (!Validate())
            {
                BaseGamePath = null;
                throw new InvalidDataException("The Directory specified is either: Not a directory, doesn't exist, or doesn't contain SIMCITY.EXE." +
                " If your SimCity game is called something different, you can make a file and call it SIMCITY.EXE to override this failsafe.");
            }
        }
        private static bool Validate() => BaseGamePath != null && (BaseGamePath?.Exists ?? false) && BaseGamePath?.EnumerateFiles()?.FirstOrDefault(x => x.Name.ToUpper() == "SIMCITY.EXE") != null;

        public static string Combine(params string[] Paths)
        {
            if (!Ready) throw new InvalidOperationException("You are trying to use the SC2Path service without calling Ready() first.");
            return Path.Combine(BaseGamePath.FullName, System.IO.Path.Combine(Paths));
        }

        /// <summary>
        /// Gets the specified <see cref="SpecialPath"/> in relation to the current <see cref="BaseGamePath"/>
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string GetSpecialPath(SpecialPath Path)
        {
            switch (Path)
            {
                case SpecialPath.BaseGameDirectory: break;
                case SpecialPath.GameDirectory: return Combine("GAME");
                case SpecialPath.CitiesDirectory: return Combine(GetSpecialPath(SpecialPath.GameDirectory), "CITIES");
                case SpecialPath.GameDataDirectory: return Combine(GetSpecialPath(SpecialPath.GameDirectory), "DATA");
                case SpecialPath.LargeDatFilePath: return Combine(GetSpecialPath(SpecialPath.GameDataDirectory), "LARGE.DAT");
                case SpecialPath.SmallMedFilePath: return Combine(GetSpecialPath(SpecialPath.GameDataDirectory), "SMALLMED.DAT");
                case SpecialPath.GameBitmapsDirectory: return Combine(GetSpecialPath(SpecialPath.GameDirectory), "BITMAPS");
                case SpecialPath.PalMstrFilePath: return Combine(GetSpecialPath(SpecialPath.GameBitmapsDirectory), "PAL_MSTR.BMP");
            }
            return Combine();
        }
        /// <summary>
        /// Gets the specified <see cref="SpecialPath"/> in relation to the current <see cref="BaseGamePath"/>
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string GetSpecialPath(SpecialPath Path, params string[] Paths) => System.IO.Path.Combine(GetSpecialPath(Path), System.IO.Path.Combine(Paths));

        /// <summary>
        /// Common file paths for SimCity 2000
        /// </summary>
        public enum SpecialPath
        {
            /// <summary>
            /// The path to the base game
            /// </summary>
            BaseGameDirectory,
            /// <summary>
            /// The path to the cities folder
            /// </summary>
            CitiesDirectory,
            /// <summary>
            /// The path to the Game folder
            /// </summary>
            GameDirectory,
            /// <summary>
            /// The path to the Game//Data folder
            /// </summary>
            GameDataDirectory,
            /// <summary>
            /// The path to LARGE.DAT
            /// </summary>
            LargeDatFilePath,
            /// <summary>
            /// The path to SMALLMED.DAT
            /// </summary>
            SmallMedFilePath,
            /// <summary>
            /// The path to the palette file
            /// </summary>
            PalMstrFilePath,
            /// <summary>
            /// The path to the Bitmaps folder
            /// </summary>
            GameBitmapsDirectory,
        }
    }
    

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
