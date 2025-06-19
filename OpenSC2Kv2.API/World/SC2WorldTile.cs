using OpenSC2Kv2.API.IFF;

namespace OpenSC2Kv2.API.World
{
    /// <summary>
    /// Represents a definition to override physical properties of a terrain tile.
    /// </summary>
    [Serializable]
    public class OverridePhysicalPropertiesDefinition
    {
        /// <summary>
        /// This is the target TerrainID of this definition.
        /// <para>The engine will automatically apply this definition to all <see cref="SC2WorldTile"/> 
        /// instances loaded through standard means using the <see cref="SC2WorldTile.TerrainID"/> property.</para>
        /// </summary>
        public int TargetTerrainID { get; set; }
        /// <summary>
        /// The spacial offset of this particular tile type (in pixels)
        /// <para>This is added to the default spacial calculation of all world tiles.</para>
        /// </summary>
        public int ManualOffsetX { get; set; } = 0;
        /// <summary>
        /// The spacial offset of this particular tile type (in pixels)
        /// <para>This is added to the default spacial calculation of all world tiles.</para>
        /// </summary>
        public int ManualOffsetY { get; set; } = 0;   
    }

    /// <summary>
    /// Represents all of the data attached to a tile in the SimCity 2000 World
    /// <para>See <see cref="SC2World"/></para>
    /// </summary>
    public class SC2WorldTile
    {
        public const int TILE_WIDTH = 32, TILE_HEIGHT = 17, LAYER_OFFSET = 12;
        public const byte MAX_ALTITUDE = 32;
        private SC2TerrainDescriptor _tDef;

        /// <summary>
        /// The current coordinate of this tile.
        /// <para>(0,0) is the TOP-LEFT of the grid</para>
        /// </summary>
        public GridCoordinate Coordinate { get; set; }
        public int TileWidth { get; set; } = TILE_WIDTH;
        public int TileHeight { get; set; } = TILE_HEIGHT;
        public int TerrainID => TerrainDescription.TerrainID;        

        /// <summary>
        /// If there is an override definition for this Terrain type, this will be set to match the set values.
        /// </summary>
        public OverridePhysicalPropertiesDefinition? OverrideDefinition { get; set; }
        /// <summary>
        /// See <see cref="Coordinate"/>
        /// </summary>
        public int Column => Coordinate.Column;
        /// <summary>
        /// See <see cref="Coordinate"/>
        /// </summary>
        public int Row => Coordinate.Row;
        public ushort Z => Altitude;
        /// <summary>
        /// Altitude data from the <see cref="ALTMSegment"/>        
        /// </summary>
        public ushort Altitude { get; set; }
        /// <summary>
        /// Basic Water Coverage data from the <see cref="ALTMSegment"/>
        /// <para>It is recommended to use information from the <see cref="TerrainDescription"/> instead.</para>
        /// </summary>
        public bool IsWaterCovered { get; set; }
        /// <summary>
        /// Contains information from the <see cref="XTERSegment"/> of the level save data.
        /// <para>Describes how the terrain tile should look.</para>
        /// </summary>
        public SC2TerrainDescriptor TerrainDescription
        {
            get => _tDef; 
            set
            {
                _tDef = value;
                RequestUpdateOverrides();
            }
        }
        private async void RequestUpdateOverrides()
        {
            if (!SC2WorldOverrides.Ready)
                return;
            SC2WorldOverrides.SetOverrides(this);
        }
        /// <summary>
        /// The descriptor describing how to draw a building tile on this tile, if one is present.
        /// <para>Use this in conjunction with XZON data to create buildings.</para>
        /// </summary>
        public SC2BuildingDescriptor BuildingDescription { get; internal set; }
        /// <summary>
        /// The descriptor aiding the ability to draw buildings correctly.
        /// </summary>
        public SC2ZoneDescriptor ZoneDescription { get; internal set; }

        public enum SC2WorldViewPerspectives
        {
            /// <summary>
            /// The classic viewpoint of SimCity 2000
            /// </summary>
            ISO,
            /// <summary>
            /// Experimental viewpoint from the horizon
            /// </summary>
            HORIZON,
            /// <summary>
            /// Experimental viewpoint from top-down.
            /// </summary>
            TOPDOWN
        }

        /// <summary>
        /// Calculates the world-position of this tile in the world using the <see cref="Row"/> and <see cref="Column"/>
        /// <para>Any camera/matrix calculations are NOT factored into this calculation. This assumes the camera is at default position.</para>
        /// </summary>
        /// <returns></returns>
        public Point2D GetWorldPos(SC2WorldViewPerspectives Perspective = SC2WorldViewPerspectives.ISO)
        {
            var wCol = Coordinate.Column;
            var wRow = Coordinate.Row;
            double scalar = .5;
            switch (Perspective)
            {
                case SC2WorldViewPerspectives.HORIZON: scalar = 0; break;
                case SC2WorldViewPerspectives.TOPDOWN: scalar = 1; break;
            }
            int offsetX = OverrideDefinition?.ManualOffsetX ?? 0;
            int offsetY = OverrideDefinition?.ManualOffsetY ?? 0;
            if (TerrainDescription != null && TerrainDescription.Watered)
                offsetY = 0;
            var basePosition = new Point2D(((wCol - wRow) * (TILE_WIDTH / 2)) + offsetX,
                ((wRow + wCol) * (int)(TILE_HEIGHT * scalar)) + offsetY);                
            basePosition.Y -= Z * LAYER_OFFSET;
            return basePosition;
        }
        public Point2D WorldPosition => GetWorldPos();
        /// <summary>
        /// Creates a new SC2WorldTile at the given space.
        /// </summary>
        /// <param name="coordinate">The coordinate to use</param>
        public SC2WorldTile(GridCoordinate coordinate)
        {
            Coordinate = coordinate;            
        }
        /// <summary>
        /// Creates a new SC2WorldTile at the given space.
        /// </summary>        
        public SC2WorldTile(int Col, int Row) : this(new GridCoordinate(Row, Col))
        {

        }
        public override string ToString()
        {
            return $"SC2TILE:\n{TerrainDescription}\nALT: {Altitude}\nX: {Column}\nY: {Row}\nWATER?: {IsWaterCovered}";
        }

        public bool IsMultitile()
        {
            int type = (int)BuildingDescription.Type;
            return ((type >= 0x8C && type <= 0xDA) && type != 0xC6 && type != 0xC7);
        }
    }
}
