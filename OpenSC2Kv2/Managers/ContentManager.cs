using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using OpenSC2Kv2.Game.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenSC2Kv2.Game.Managers
{
    internal sealed class ContentManager : IManager
    {
        public enum GradientDirections
        {
            Horizontal,
            Vertical,
            HorizontalReverse,
            VerticalReverse,
        }
        private Microsoft.Xna.Framework.Content.ContentManager Manager;
        private Dictionary<string, SoundEffect> soundCache = new Dictionary<string, SoundEffect>();
        private Dictionary<string, Texture2D> textureCache = new();

        public ContentManager(Microsoft.Xna.Framework.Content.ContentManager manager)
        {
            Manager = manager;
        }

        private Rectangle Crop(Texture2D texture)
        {
            var foo = new Color[texture.Width * texture.Height];
            texture.GetData(0, null, foo, 0, foo.Length);
            int startLine = -1, endLine = -1, startIndex = -1, endIndex = -1;
            for (var line = 0; line < texture.Height; line++)
            {
                int colorIndex = -1;
                for (var index = 0; index < texture.Width; index++)
                {
                    var color = foo[line * texture.Width + index];
                    if (color != Color.Transparent)
                    {
                        if (startLine == -1)
                            startLine = line;
                        if (startIndex == -1 || startIndex > index)
                            startIndex = index;
                        colorIndex = index;
                    }
                }
                if (endIndex == -1 || colorIndex > endIndex)
                    endIndex = colorIndex;
                if (colorIndex != -1)
                    endLine = line;
            }
            return new Rectangle(startIndex, startLine, endIndex - startIndex, endLine - startLine);
        }

        public Texture2D Create(string name, int width, int height) => Add(name, new Texture2D(GameResources.Device, width, height));

        public Texture2D Add(string name, Texture2D texture)
        {
            if (textureCache.ContainsKey(name))
                DestroyTexture(name);
            textureCache.Add(name, texture);
            return texture;
        }

#if false
        private unsafe int DEBUG_GetCacheSize()
        {
            int amount = 0;
            foreach(var tuple in textureCache.Values)
            {
                //amount += sizeof(tuple.texture);
            }
            return amount;
        }
#endif

        public bool DestroyTexture(string Name)
        {
            if (!textureCache.ContainsKey(Name))
                return false;
            var texture = textureCache[Name];
            texture?.Dispose();
            textureCache.Remove(Name);
            return true;
        }

        public SoundEffect GetSoundEffect(string key)
        {
            if (soundCache.ContainsKey(key))
                return soundCache[key];

            var sound = GetContent<SoundEffect>(key);
            soundCache.Add(key, sound);
            return sound;
        }
        public Song GetSong(string key)
        {
            return GetContent<Song>(key);
        }

        public bool TryGet(string Name, out Texture2D texture)
        {
            texture = GetTexture(Name);
            return texture != null;
        }

        public Texture2D GetTexture(string key)
        {
            if (textureCache.TryGetValue(key, out var tuple))
                return tuple;
            else
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    var texture = GetContent<Texture2D>(key);
                    textureCache.Add(key, texture);
                    watch.Stop();
                    Debug.WriteLine("[GLACIER] TextureCache: Loaded " + key + " in " + watch.ElapsedMilliseconds + "ms");
                    Debug.WriteLine("[GLACIER] TextureCache: " + textureCache.Count + " textures in memory, GC Estimated: " + GC.GetTotalMemory(false));
                    return texture;
                }
                catch (Exception)
                {
                    Debug.WriteLine("[GLACIER]: Content was not found: " + key);
                    return null;
                }
        }

        /// <summary>
        /// For loading textures, consider <see cref="GetTexture(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetContent<T>(string key)
        {
            return Manager.Load<T>(key);
        }

        public void Refresh(GameTime time)
        {
            ;
        }

        /// <summary>
        /// Generates a gradient texture using the two supplied colors, filling the Size, 
        /// in the given direction, storing the texture reference with the Name.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="Size"></param>
        public Texture2D GenerateGradient(string Name, Color color1, Color color2, Rectangle Size, GradientDirections GradientDirection)
        {
            if (textureCache.TryGetValue(Name, out var tuple))
                return tuple;
            Texture2D newAllocTex = new Texture2D(GameResources.Device, Size.Width, Size.Height);
            Color[] sourceTexture = new Color[Size.Width * Size.Height];
            if (GradientDirection == GradientDirections.HorizontalReverse || GradientDirection == GradientDirections.VerticalReverse)
            {
                if (GradientDirection == GradientDirections.HorizontalReverse)
                    GradientDirection = GradientDirections.Horizontal;
                else GradientDirection = GradientDirections.Vertical;
                var foo = color1;
                color1 = color2;
                color2 = foo;
            }
            switch (GradientDirection)
            {
                case GradientDirections.Horizontal:
                    {
                        for (int x = 0; x < Size.Width; x++)
                        {
                            for (int y = 0; y < Size.Height; y++)
                            {
                                float amount = (float)x / Size.Width;
                                Color setColor = Color.Lerp(color1, color2, amount);
                                sourceTexture[y * Size.Width + x] = setColor;
                            }
                        }
                    }
                    break;
                case GradientDirections.Vertical:
                    {
                        for (int y = 0; y < Size.Height; y++)
                        {
                            for (int x = 0; x < Size.Width; x++)
                            {
                                float amount = (float)y / Size.Height;
                                Color setColor = Color.Lerp(color1, color2, amount);
                                sourceTexture[y * Size.Width + x] = setColor;
                            }
                        }
                    }
                    break;
            }
            newAllocTex.SetData(sourceTexture);
            Add(Name, newAllocTex);
            return newAllocTex;
        }
    }
}
