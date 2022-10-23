using OpenSC2Kv2.API.IFF;

namespace OpenSC2Kv2.API.World
{
    /// <summary>
    /// Represents all of the data attached to a tile in the SimCity 2000 World
    /// <para>See <see cref="SC2World"/></para>
    /// </summary>
    public class SC2WorldTile
    {
        public const int TILE_WIDTH = 32, TILE_HEIGHT = 17, LAYER_OFFSET = 12;
        public const byte MAX_ALTITUDE = 32;
        /// <summary>
        /// The current coordinate of this tile.
        /// <para>(0,0) is the TOP-LEFT of the grid</para>
        /// </summary>
        public GridCoordinate Coordinate { get; set; }
        public int TileWidth { get; set; } = TILE_WIDTH;
        public int TileHeight { get; set; } = TILE_HEIGHT;
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
        public SC2TerrainDescriptor TerrainDescription { get; set; }
        public SC2BuildingDescriptor BuildingDescription { get; internal set; }

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
            var basePosition = new Point2D((wCol - wRow) * (TILE_WIDTH / 2), ((wRow + wCol) * (int)(TILE_HEIGHT * scalar)));                
            basePosition.Y -= Z * LAYER_OFFSET;
            return basePosition;
        }
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
    }
}
