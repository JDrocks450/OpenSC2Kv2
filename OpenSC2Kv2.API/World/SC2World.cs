namespace OpenSC2Kv2.API.World
{
    /// <summary>
    /// Contains information on a SimCity 2000 world.
    /// </summary>
    public class SC2World
    {
        /// <summary>
        /// This is the name of the world.
        /// <para>Also known as: City Name.</para>
        /// </summary>
        public string WorldName { get; set; }

        /// <summary>
        /// The Width, in tiles, of this world.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The Height, in tiles, of this world.
        /// </summary>
        public int Height { get; set; }  
        /// <summary>
        /// The default water level for this world.
        /// </summary>
        public int WaterLevel { get; set; }
        /// <summary>
        /// The <see cref="SC2WorldTile"/>s making up the landscape.
        /// <para><see cref="SC2WorldTile"/>s contain information on Terrain, Buildings, Zoning, etc.</para>
        /// </summary>
        public HashSet<SC2WorldTile> WorldTiles { get; set; } = new();
    }
}
