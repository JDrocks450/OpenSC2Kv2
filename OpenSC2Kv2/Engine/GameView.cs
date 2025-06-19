using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Engine
{
    internal class LoadingStatusToken
    {
        public string Description { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Defines a general component in the GameEngine
    /// </summary>
    internal interface IGameEngineComponent : IGameComponent, IDisposable
    {        
        Task<bool> LoadContent(Action<LoadingStatusToken> loadingCallback);

        void UpdateOne(GameTime Time, params object[] args);

        void Draw(in SpriteBatch batch);
    }

    internal abstract class GameView : IGameEngineComponent
    {
        /// <summary>
        /// The area this view is meant to fill.
        /// <para>This represents the size of the render area, positioning the view is handled by the renderer.</para>
        /// </summary>
        internal Point SafeArea { get; set; }

        #region dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GameView()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public abstract Task<bool> LoadContent(Action<LoadingStatusToken> callback);
        public abstract void UpdateOne(GameTime Time, params object[] args);
        public abstract void Draw(in SpriteBatch batch);
        public abstract void Initialize();
        #endregion
    }
}
