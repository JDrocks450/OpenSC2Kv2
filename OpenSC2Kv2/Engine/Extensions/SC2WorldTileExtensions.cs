using Microsoft.Xna.Framework;
using OpenSC2Kv2.API.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Engine.Extensions
{
    internal static class SC2WorldTileExtensions
    {
        internal static Rectangle Area(this SC2WorldTile tile)
        {
            var position = tile.WorldPosition;
            return new Rectangle(position.X, position.Y, SC2WorldTile.TILE_WIDTH, SC2WorldTile.TILE_HEIGHT);
        }
    }
}
