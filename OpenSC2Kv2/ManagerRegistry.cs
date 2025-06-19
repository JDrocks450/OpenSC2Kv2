using Microsoft.Xna.Framework;
using OpenSC2Kv2.Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game
{
    internal interface IManager
    {

    }

    /// <summary>
    /// Lives for the lifetime of the program -- provides a directory of all registered <see cref="IManager"/> instances
    /// </summary>
    internal static class ManagerRegistry
    {
        private static HashSet<IManager> all = new HashSet<IManager>();

        /// <summary>
        /// Register a new manager. If a manager of this type has already been registered, returns <see langword="false"/>
        /// </summary>
        /// <typeparam name="T">The type of manager</typeparam>
        /// <param name="Manager">The manager to add</param>
        /// <returns></returns>
        public static bool Register<T>(T Manager) where T : IManager => all.Add(Manager);

        public static T? Get<T>() where T : IManager => all.OfType<T>().FirstOrDefault();

        public static bool TryGet<T>(out T? value) where T : IManager
        {
            value = Get<T>();
            return value != null;
        }

        public static bool Deregister<T>() where T : IManager
        {
            if (!TryGet<T>(out var value))
                return false;
            return all.Remove(value);
        }
    }
}
