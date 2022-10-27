using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.World
{
    /// <summary>
    /// Handles loading <see cref="OverridePhysicalPropertiesDefinition"/> instances from file and applying them to <see cref="SC2WorldTile"/>s
    /// </summary>
    public static class SC2WorldOverrides
    {       
        /// <summary>
        /// The path to use to find the Tile Overrides file.
        /// <para>Default: tileoverrides.json</para>
        /// </summary>
        public static string OverridesDictionaryPath { get; set; } = "tileoverrides.json";
        private static ManualResetEvent fileInvoke = new ManualResetEvent(true);
        private static List<OverridePhysicalPropertiesDefinition> Definitions { get; set; } = new();
        public static bool Ready { get; private set; }
        private static bool readingFile = false;

        public static void AddOverride(SC2WorldTile Tile, OverridePhysicalPropertiesDefinition definition)
        {
            definition.TargetTerrainID = Tile.TerrainID;
            var id = Tile.TerrainID;
            var def = Definitions?.FirstOrDefault(x => x.TargetTerrainID == id);            
            if (def != default) Definitions.Remove(def);
            Definitions.Add(definition);
        }

        /// <summary>
        /// Flushes the current cached definition dictionary and reloads from file.
        /// <para>See: <see cref=""/></para>
        /// </summary>
        public static Task<bool> EnsureUpdated()
        {
            return Task.Run(async delegate
            {
                if (!File.Exists(OverridesDictionaryPath)) return false;
                bool shortcircuit = readingFile;
                fileInvoke.WaitOne();
                if (shortcircuit) return true;
                string text = null;
                fileInvoke.Reset();
                readingFile = true;
                using (var fs = File.OpenText(OverridesDictionaryPath))
                {
                    text = await fs.ReadToEndAsync();
                }
                readingFile = false;
                fileInvoke.Set();
                if (text == null) return false;

                try
                {
                    Definitions.Clear();
                    var list = JsonSerializer.Deserialize<OverridePhysicalPropertiesDefinition[]>(text);
                    Definitions.AddRange(list);

                    Ready = true;

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Applies the overrides for this tile, if there are any to set.
        /// </summary>
        /// <param name="Tile"></param>
        public static void SetOverrides(SC2WorldTile Tile)
        {
            var id = Tile.TerrainID;
            var def = Definitions?.FirstOrDefault(x => x.TargetTerrainID == id);
            if (def == default) return;
            Tile.OverrideDefinition = def;            
        }

        public static void SaveChanges()
        {
            var json = JsonSerializer.Serialize(Definitions.ToArray());
            File.WriteAllText(OverridesDictionaryPath, json);
        }
    }
}
