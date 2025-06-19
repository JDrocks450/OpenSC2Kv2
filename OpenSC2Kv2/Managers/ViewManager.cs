using OpenSC2Kv2.Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Managers
{
    internal class ViewManager : IManager
    {
        private Dictionary<string, GameView> views = new();
        /// <summary>
        /// Sets the current view.
        /// <para>The current view is whichever view is marked as the main screen the game is currently on.</para>
        /// </summary>
        public string? CurrentViewName { get; set; } = null;
        /// <summary>
        /// This is the current view
        /// <para>It is the main view the game should be rendering at the given time.</para>
        /// </summary>
        public GameView? CurrentView
        {
            get
            {
                if (views.Any())
                {
                    if (CurrentViewName == null) return views.Values.FirstOrDefault();
                    if (views.TryGetValue(CurrentViewName, out var view))
                        return view;
                }
                return null;
            }
        }

        internal ViewManager()
        {

        }

        internal bool Add<T>(string Name, T View) where T : GameView
        {
            if (views.ContainsKey(Name)) return false;
            views.Add(Name, View);
            return true;
        }
    }
}
