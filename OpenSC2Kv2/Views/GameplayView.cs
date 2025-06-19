using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenSC2Kv2.API;
using OpenSC2Kv2.API.Graphics;
using OpenSC2Kv2.API.Graphics.Win95;
using OpenSC2Kv2.API.World;
using OpenSC2Kv2.Game.Engine;
using OpenSC2Kv2.Game.Managers;
using OpenSC2Kv2.Game.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Views
{
    internal class GameplayView : GameView
    {
        private double animationSpeed = 3.5;

        private readonly SC2World currentCity;
        private SPRExtractor graphicsExtractor;
        private SC2SpriteArchive archive;

        private WorldRenderProvider renderer;
        private double currentFrame { get => renderer.CurrentFrame; set => renderer.CurrentFrame = value; }

        private IEnumerable<SC2GraphicResource> resources => archive.Graphics.Values;

        public LoadingStatusToken CurrentStatus => renderer.TextureLoadingProgress;

        public GameplayView(SC2World CurrentCity)
        {
            currentCity = CurrentCity;
        }

        public override void Initialize()
        {
            if (currentCity == null)
                throw new ArgumentNullException($"currentCity is null!");
            var largeDatPath = SC2Path.GetSpecialPath(SC2Path.SpecialPath.LargeDatFilePath);
            graphicsExtractor = new(new Uri(largeDatPath));
        }

        public override Task<bool> LoadContent(Action<LoadingStatusToken> callback)
        {
            return Task.Run(delegate
            {                
                if (graphicsExtractor == null)
                    throw new ArgumentNullException("SC2 SPR Extractor is not set!");
                var result = graphicsExtractor.ExtractAll();
                if (result?.Value == null)
                    throw new ArgumentNullException($"Extractor load unsuccessful. Errors: {string.Join(',', result?.Errors ?? new string[0])}");
                archive = result.Value;                

                var tManager = ManagerRegistry.Get<ContentManager>();
                int index = -1;
                foreach (var spr in resources)
                {                    
                    index++;
                    for (int frame = 0; frame < spr.Frames; frame++)
                    {
                        callback?.Invoke(new LoadingStatusToken
                        {
                            Description = $"SPRLOAD: {spr.Header.ImageName} ({frame} / {spr.Frames})",
                            Percentage = index / (double)resources.Count()
                        });
                        
                        tManager.Add(spr.Textures[frame], null);
                    }
                }

                renderer = new(currentCity, archive);

                return true;
            });
        }        

        public override void UpdateOne(GameTime Time, params object[] args)
        {
            currentFrame += animationSpeed * TimeSpan.FromMilliseconds(25).TotalSeconds;
        }

        public override void Draw(in SpriteBatch batch)
        {
            renderer.Draw(batch);            
        }
    }
}
