using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Managers
{
    internal class TextureManager : IManager
    {
        private readonly ContentManager manager;
        private Dictionary<string, Texture2D> textures = new();

        public TextureManager(GraphicsDevice Device, ContentManager Manager)
        {
            this.Device = Device;
            manager = Manager;
        }

        public GraphicsDevice Device { get; }

        internal bool Add(string Name, Texture2D Texture)
        {
            if (textures.ContainsKey(Name))
            {
                textures[Name] = Texture;
                return true;
            }
            textures.Add(Name, Texture);
            return true;
        }

        internal bool TryGet(string Name, out Texture2D Texture) => textures.TryGetValue(Name, out Texture);
    }
}
