using OpenSC2Kv2.API.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.IFF
{
    /// <summary>
    /// Generates world data from a given <see cref="SC2File"/>
    /// </summary>
    internal class SC2WorldGenerator
    {
        public static SC2World GenerateWorld(SC2File File)
        {
            var world = new SC2World()
            {
                WorldName = File.CityInformation.Name,
                Width = File.CityInformation.Width,
                Height = File.CityInformation.Height,
                WaterLevel = File.CityInformation.WaterLevel,
            };
            int x = 0, y = 0;
            for (int i = 0; i < (world.Height * world.Width); i++)
            {
                world.WorldTiles.Add(new SC2WorldTile(x, y));

                if (y == world.Height - 1)
                {
                    y = 0;
                    x += 1;
                }
                else
                {
                    y += 1;
                }
            }            

            return world;
        }

        /// <summary>
        /// Applies extracted <see cref="ALTMSegment"/> data to the given <see cref="SC2World"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ALTMData"></param>
        internal static void ApplyData(SC2World world, IEnumerable<ALTMSegment> ALTMData)
        {
            int index = 0;
            var data = ALTMData.ToList();
            foreach(var tile in world.WorldTiles)
            {
                if (data[index].Index != index)
                {
                    //TYIKES!
                }
                tile.Altitude = data[index].Altitude;
                tile.IsWaterCovered = data[index].IsWaterCovered;
                index++;
            }
        }
        /// <summary>
        /// Applies extracted <see cref="XTERSegment"/> data to the given <see cref="SC2World"/>
        /// </summary>
        internal static void ApplyData(SC2World world, IEnumerable<XTERSegment> XTERData)
        {
            int index = 0;
            var data = XTERData.ToList();
            foreach (var tile in world.WorldTiles)
            {
                if (index >= data.Count) continue;
                tile.TerrainDescription = data[index].TerrainDescriptor;
                if (tile.TerrainDescription.Watered)
                {
                    tile.IsWaterCovered = true;
                    if (tile.Altitude < (ushort)world.WaterLevel)
                        tile.Altitude = (ushort)world.WaterLevel;
                }
                var descriptor = tile.TerrainDescription.TerrainDescriptorInstance;
                if (descriptor > 0 && descriptor < 0x10)
                {
                    //if (descriptor != 3 && descriptor != 12 && descriptor != 4 && descriptor != 10 && descriptor != 8)
                        tile.Altitude += 1;
                }
                index++;
            }
        }
        /// <summary>
        /// Applies extracted <see cref="XBLDSegment"/> data to the given <see cref="SC2World"/>
        /// </summary>
        internal static void ApplyData(SC2World world, IEnumerable<XBLDSegment> XTERData)
        {
            int index = 0;
            var data = XTERData.ToList();
            foreach (var tile in world.WorldTiles)
            {
                if (index >= data.Count) continue;
                tile.BuildingDescription = data[index].BuildingDetermination;
                index++;
            }
        }

        public void PopulateWorldTiles(SC2File File)
        {
        }
    }
}
