using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenSC2Kv2.API.Graphics;
using OpenSC2Kv2.API.Graphics.Win95;
using OpenSC2Kv2.API.IFF;
using OpenSC2Kv2.API.World;
using OpenSC2Kv2.Game.Engine;
using OpenSC2Kv2.Game.Engine.Extensions;
using OpenSC2Kv2.Game.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Rendering
{
    internal class WorldRenderProvider : IManager
    {
        private SC2SpriteArchive archive;
        internal SC2World World;

        private SC2Palette palette;

        private ConcurrentDictionary<int, Texture2D> sprMap = new();
        private ConcurrentDictionary<int, SC2GraphicResource> resourceMap = new();

        private int waterFrame = 0, waterChange = 1;

        /// <summary>
        /// This is a reference to the current loading status.
        /// <para>It fluctuates depending on what textures are currently being streamed from SPR archive.</para>
        /// </summary>
        public LoadingStatusToken TextureLoadingProgress { get; private set; }
        int totalLoaded, totalRequested;

        private IEnumerable<SC2GraphicResource> resources => archive.Graphics.Values;

        private ConcurrentDictionary<string,bool> lazyLoadingQueue = new();
        
        public enum WorldRenderMode
        {
            NormalMode,
            DrawOrderHeatmap,
            Normal_ShowOccupiedTiles,
            PerlinNoise,
            LandscapeRenderer,
        }
        public WorldRenderMode WorldRenderingMode { get; set; }
        public double CurrentFrame { get; internal set; }

        const int TILE_WIDTH = SC2WorldTile.TILE_WIDTH, TILE_HEIGHT = SC2WorldTile.TILE_HEIGHT;

        private Vector2 StarOffset;
        private List<Point> StarPositions = new List<Point>();
        private float pxSinceLastStarUpdate = 90f;

        internal WorldRenderProvider(SC2World World, SC2SpriteArchive archive)
        {
            palette = SC2Palette.Default;
            this.World = World;
            this.archive = archive;
        }

        internal void Draw(SpriteBatch batch)
        {
            //DRAW LANDSCAPE
            if (WorldRenderingMode > WorldRenderMode.LandscapeRenderer)
                WorldRenderingMode = 0;
            switch (WorldRenderingMode)
            {
                case WorldRenderMode.PerlinNoise:
                    //Debug_DrawPerlinNoise(batch, World, WorldCam);
                    return;
                case WorldRenderMode.DrawOrderHeatmap:
                    //Debug_DrawOrderHeatmap(batch, World);
                    return;
                case WorldRenderMode.Normal_ShowOccupiedTiles:
                case WorldRenderMode.NormalMode:
                    NormalModeDraw(batch, World);
                    break;
                case WorldRenderMode.LandscapeRenderer:
                    //LandscapeDraw(batch, World, WorldCam);
                    break;
            }
        }

        /*
        private void LandscapeDraw(SpriteBatch batch, SC2World World, Camera camera)
        {
            var cameraZoom = camera.Zoom;
            Color drawColor = Color.Black;
            double[,] map = World.PerlinNoise;
            float lower = (float)map.OfType<double>().Min(), upper = (float)map.OfType<double>().Max();
            //var values = (WorldTile.TileTypes[])Enum.GetValues(typeof(WorldTile.TileTypes));\
            var values = new WorldTile.TileTypes[]
            {
                WorldTile.TileTypes.DeepOcean,
                WorldTile.TileTypes.Ocean,
                WorldTile.TileTypes.Dirt,
                WorldTile.TileTypes.Grass,
                WorldTile.TileTypes.DeepGrass
            };

            for (int x = 0; x < World.SizeX; x++)
            {
                for (int y = 0; y < World.SizeY; y++)
                {
                    float current = (float)map[x, y];
                    current -= lower;
                    current /= (upper - lower);
                    int amount = values.Count();
                    float percentage = (float)amount * current;
                    amount = (int)percentage;
                    float blendAmount = percentage - amount;
                    WorldTile.TileTypes type = (WorldTile.TileTypes)amount;
                    var dest = type != WorldTile.TileTypes.Snow ? type + 1 : type = WorldTile.TileTypes.Snow;
                    drawColor = Color.Lerp(World.GetTileColor(type), World.GetTileColor(dest), blendAmount);
                    Debug_DrawOrderHeatmapTile(World.WorldData[x, y], camera, content, batch, drawColor);
                }
            }
        }

        private void Debug_DrawPerlinNoise(SpriteBatch batch, AntWorld World, Camera camera)
        {
            var cameraZoom = ProviderManager.Root.Get<CameraProvider>().Default.Zoom;
            var content = _contentProvider;
            var worldProvider = ProviderManager.Root.Get<WorldProvider<AntWorld>>();
            Color drawColor = Color.Black;
            double[,] map = World.PerlinNoise;
            float lower = (float)map.OfType<double>().Min(), upper = (float)map.OfType<double>().Max();

            for (int x = 0; x < World.SizeX; x++)
            {
                for (int y = 0; y < World.SizeY; y++)
                {
                    float current = (float)map[x, y];
                    current -= lower;
                    current /= (upper - lower);
                    drawColor = Color.Lerp(Color.Black, Color.White, current);
                    Debug_DrawOrderHeatmapTile(World.WorldData[x, y], camera, content, batch, drawColor);
                }
            }
        }
        */
        private void NormalModeDraw(SpriteBatch batch, SC2World World)
        {
            DrawWorldTiles(batch, World);
        }

        private void Sky_DoTick(GameTime time)
        {
            var pxSec = (float)time.ElapsedGameTime.TotalSeconds * 5f; // 1px / sec
            pxSinceLastStarUpdate += pxSec;
            StarOffset.Y += pxSec;
            Point StarSize = new Point(5);
            int scrWidth = GameResources.CurrentViewport.Width;
            int starsRow = (int)(scrWidth * .01f);
            if (pxSinceLastStarUpdate > 80)
            {
                var rand = GameResources.Rand;
                for (int i = 0; i < starsRow; i++)
                {
                    int x = rand.Next(-StarSize.X, scrWidth + StarSize.X);
                    int y = -StarSize.Y - (int)StarOffset.Y - rand.Next(0, 40);
                    StarPositions.Add(new Point(x, y));
                }
                pxSinceLastStarUpdate = 0;
            }
        }
        /*
        public void DrawSky(SpriteBatch batch)
        {
            var content = ProviderManager.Root.Get<ContentProvider>();
            var grad = content.GenerateGradient("FARM_SMALL_VGRAD",
                Color.Transparent, Color.White,
                new Rectangle(0, 0, 1, 200),
                ContentProvider.GradientDirections.Vertical);
            var star = GameResources.GetTexture("Objects/star");
            var camera = ProviderManager.Root.Get<CameraProvider>().Default;
            Point StarSize = new Point(5);
            batch.Draw(grad, new Rectangle(0, 100, GameResources.CurrentViewport.Width, GameResources.CurrentViewport.Height), Color.Blue);
            foreach (Point loc in StarPositions)
            {
                batch.Draw(star, new Rectangle(loc + StarOffset.ToPoint(), StarSize), Color.White * .75f);
            }
        }*/

        private void DrawWorldTiles(SpriteBatch batch, SC2World World)
        {
            var cameraZoom = ManagerRegistry.Get<CameraManager>().Default.Zoom;
            var content = ManagerRegistry.Get<ContentManager>();
            var camera = ManagerRegistry.Get<CameraManager>().Default;
            int drawCalls = 0, loops = 0;
            foreach (var tile in World.WorldTiles)
            {
                if (DrawTile(tile, content, camera, batch))
                {
                    DrawTileBuildings(tile, content, camera, batch);
                    drawCalls+=2;
                }
            }
        }

        private Task LazyLoad(int tileID, SC2GraphicResource resource, int Frame)
        {
            return Task.Run(delegate
            {
                string texName = resource.Textures[Frame];
                var tmanager = ManagerRegistry.Get<ContentManager>();
                using (var bmp = SPRRender.Render(resource, Frame, palette))
                {
                    if (bmp == null) return;
                    using (var mstream = new MemoryStream())
                    {                        
                        bmp.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);

                        var texture = Texture2D.FromStream(GameResources.Device, mstream);
                        tmanager.Add(texName, texture);
                        if (!sprMap.ContainsKey(tileID))
                            for (int tries = 0; tries <25; tries++)
                            {
                                if (sprMap.TryAdd(tileID, texture))
                                    break;
                                Task.Delay(10);
                            }                    
                    }
                }
                totalLoaded++;
                lazyLoadingQueue.TryUpdate(texName, true, false);
                if (lazyLoadingQueue.Values.All(x => true))
                    ;// lazyLoadingQueue.Clear(); // All requested textures have been streamed for this moment
                if (totalLoaded > totalRequested)
                    totalLoaded = totalRequested;
                TextureLoadingProgress = new()
                {
                    Description = $"{texName} ({totalLoaded})",
                    Percentage = (double)totalLoaded / totalRequested
                };
            });
        }

        private bool DrawTileBuildings(SC2WorldTile tile, ContentManager content, Camera camera, SpriteBatch batch)
        {
            if (!camera.VisibleArea.Intersects(tile.Area()))
                return false;

            bool multitile = tile.IsMultitile();
            if (multitile && tile.ZoneDescription.CornerFlag is not SC2CornerFlag.BOTTOM)
                return false;

            int currentFrame = 0;
            var cameraZoom = camera.Zoom;
            if (tile.TerrainDescription == null) return false;
            ushort sprID = tile.BuildingDescription.TryGetGraphicID();
            if (!resourceMap.TryGetValue(sprID, out var resource))
            {
                if (!archive.TryGetGraphicByID((ushort)(sprID), out resource)) return false;
                _ = resourceMap.TryAdd(sprID, resource);
            }
            if (resource.Frames > 1)
                currentFrame = (int)CurrentFrame % (resource.Frames ?? 0);
            if (!sprMap.TryGetValue(sprID, out var texture) || currentFrame != 0)
            {
                if (!content.TryGet(resource.Textures[currentFrame], out texture) || texture == null)
                {
                    if (lazyLoadingQueue.ContainsKey(resource.Textures[currentFrame])) return false;
                    if (!lazyLoadingQueue.TryAdd(resource.Textures[currentFrame], false)) return false;
                    totalRequested++;
                    TextureLoadingProgress = new()
                    {
                        Description = $"{resource.Textures[currentFrame]} ({totalLoaded})",
                        Percentage = (double)totalLoaded / totalRequested
                    };
                    LazyLoad(sprID, resource, currentFrame);
                    return false;
                }
            }
            var graphic = texture;
            var location = tile.WorldPosition;                         

            if (texture.Width > SC2WorldTile.TILE_WIDTH)
            {
                var diff = texture.Width - SC2WorldTile.TILE_WIDTH;
                location.X -= (int)(diff / 2);
            }
            location.Y -= (int)texture.Height;
            location.Y += tile.TileHeight;

            batch.Draw(graphic, new Rectangle(location.X, location.Y, texture.Width, texture.Height), Color.White);
            return true;
        }

        private bool DrawTile(SC2WorldTile tile, ContentManager content, Camera camera, SpriteBatch batch)
        {
            if (!camera.VisibleArea.Intersects(tile.Area()))            
                return false;            
            int currentFrame = 0;
            var cameraZoom = camera.Zoom;
            if (tile.TerrainDescription == null) return false;
                int sprID = tile.TerrainDescription.TerrainID;
                if (tile.TerrainDescription.Watered) sprID = tile.TerrainDescription.WaterID.Value;
            if (!resourceMap.TryGetValue(sprID, out var resource))
            {
                if (!archive.TryGetGraphicByID((ushort)(sprID + 1000), out resource)) return false;
                _ = resourceMap.TryAdd(sprID, resource);
            }
            if (resource.Frames > 1)
            {
                int maxFrame = 3;
                var waterf = (int)(CurrentFrame / 3) % (maxFrame);
                var occilator = Math.Abs(waterf - waterFrame);
                int changedValue = waterFrame + (occilator * waterChange);
                if (changedValue < 0 || changedValue > maxFrame)
                    waterChange *= -1;
                waterFrame += (occilator * waterChange);
                currentFrame = waterFrame;
            }
            if (camera.Zoom < .75)
                currentFrame = 0;
            if (!sprMap.TryGetValue(sprID, out var texture) || currentFrame != 0)
            {                
                if (!content.TryGet(resource.Textures[currentFrame], out texture) || texture == null)
                {
                    if (lazyLoadingQueue.ContainsKey(resource.Textures[currentFrame])) return false;
                    if (!lazyLoadingQueue.TryAdd(resource.Textures[currentFrame], false)) return false;
                    totalRequested++;
                    TextureLoadingProgress = new()
                    {
                        Description = $"{resource.Textures[currentFrame]} ({totalLoaded})",
                        Percentage = (double)totalLoaded / totalRequested
                    };
                    LazyLoad(sprID, resource, currentFrame);
                    return false;
                }
            }
            var graphic = texture;
            Color blend = Color.White;
            if (graphic == null)
                return false;
            var grad = content.GenerateGradient("SC2_SMALL_VGRAD",
                Color.Transparent, Color.White,
                new Rectangle(0, 0, 1, 200),
                ContentManager.GradientDirections.Vertical);
            var location = tile.WorldPosition;
            var tileLocation = new Point(location.X, location.Y);
            var brown = new Color(48, 24, 0);
            if (false)
            {
                if (tile.Column + 1 > World.Height)
                {
                    var highlight = brown;
                    batch.Draw(grad,
                        new Rectangle(tileLocation + new Point(0, tile.Area().Height), new Point(tile.Area().Width / 2, 5000)), null,
                        highlight, 0f, new Vector2(), SpriteEffects.FlipVertically, 1);
                }
                var world = World;
                if (tile.Row + 1 >= world.Height)
                {
                    var highlight = brown;
                    batch.Draw(grad,
                        new Rectangle(tileLocation + new Point(tile.Area().Width / 2,
                        tile.Area().Height), new Point(tile.Area().Width / 2, 2000)), null,
                        highlight, 0f, new Vector2(), SpriteEffects.FlipVertically, 1);
                }
            }                    
            batch.Draw(graphic, new Rectangle(location.X, location.Y, texture.Width, texture.Height), blend);
            tile.TileWidth = resource.Width.Value;
            tile.TileHeight = resource.Height.Value;
            return true;
        }

        public void Refresh(GameTime time)
        {
            Sky_DoTick(time);
        }
    }
}
