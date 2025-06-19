using Microsoft.Xna.Framework;
using OpenSC2Kv2.Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Managers
{
    public sealed class CameraManager : IManager
    {
        private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();
        internal Camera Default => cameras.Values.First();

        public CameraManager()
        {
            Create(); // creates the root camera
        }

        internal Camera Get(string key)
        {
            if (cameras.TryGetValue(key, out var camera))
                return camera;
            else
                return null;            
        }

        internal Camera Create(string name = "root") => Add(new Camera() { Name = name });

        internal Camera Add(Camera cam)
        {
            if (Get(cam.Name) == null)
                cameras.Add(cam.Name, cam);
            return cam;
        }

        internal void Refresh(GameTime gameTime)
        {
            foreach (var cam in cameras.Values)
                cam.Update(gameTime);
        }
    }
}
