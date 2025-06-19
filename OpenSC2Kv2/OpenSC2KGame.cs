using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenSC2Kv2.API;
using OpenSC2Kv2.Game.Engine;
using OpenSC2Kv2.Game.Managers;
using OpenSC2Kv2.Game.Views;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game
{
    public class OpenSC2KGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ViewManager viewManager;
        private ContentManager textureManager;
        private CameraManager cameraManager;        

        public OpenSC2KGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            GameResources.Init(GraphicsDevice);

            viewManager = new();
            textureManager = new(Content);
            cameraManager = new();

            _ = ManagerRegistry.Register(viewManager);
            _ = ManagerRegistry.Register(textureManager);
            _ = ManagerRegistry.Register(cameraManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _ = LoadLarge();
        }

        protected async Task LoadLarge()
        {
            IFFParser parser = new IFFParser(new System.Uri(@"E:\Games\SC2K\GAME\CITIES\TOKYO.SC2"));
            await parser.ParseAsync();

            var currentView = new GameplayView(parser.LoadedWorld);
            currentView.Initialize();
            await currentView.LoadContent(null);

            viewManager.Add("game", currentView);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (viewManager.CurrentView != null)
            {
                cameraManager.Refresh(gameTime);
                viewManager.CurrentView.UpdateOne(gameTime);
                if (cameraManager.Default.Zoom > 2)
                    GameResources.Sampler = SamplerState.PointClamp;
                else GameResources.Sampler = SamplerState.LinearClamp;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(48,16,0));

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, GameResources.Sampler, null, null, null, cameraManager.Default.Transformation);
            
            viewManager.CurrentView?.Draw(_spriteBatch);                    

            _spriteBatch.End();
            ///UI!!!
            _spriteBatch.Begin();
            if (viewManager.CurrentView != null)
            {
                double percentage = (viewManager.CurrentView as GameplayView).CurrentStatus.Percentage;
                _spriteBatch.Draw(GameResources.BaseTexture, new Rectangle(40, 40, 10 + 200, 30), Color.DarkGreen);
                _spriteBatch.Draw(GameResources.BaseTexture, new Rectangle(40, 40, 10 + (int)(200 *
                    percentage), 30), percentage >= 1 ? Color.Green : Color.Red);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}