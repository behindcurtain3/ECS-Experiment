using System.Collections.Generic;
using System.IO;
using IsoECS.DataStructures.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace IsoECS.DataStructures
{
    public sealed class Textures
    {
        private static readonly Textures _instance = new Textures();

        public static Textures Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, Rectangle> _sources;
        private GraphicsDevice _graphics;
        public GraphicsDevice Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        public ContentManager Content { get; set; }

        public int Count { get; set; }

        private Textures()
        {
            Count = 0;
            _textures = new Dictionary<string, Texture2D>();
            _sources = new Dictionary<string, Rectangle>();
        }

        public void AddTexture(string sourceName, string sourcePath, string name, Rectangle source)
        {
            // If we haven't loaded the texture do so
            if (!_textures.ContainsKey(sourceName))
            {
                if (File.Exists("Content\\" + sourcePath + ".png"))
                {
                    using (Stream stream = TitleContainer.OpenStream("Content\\" + sourcePath + ".png"))
                    {
                        Texture2D tex = Texture2D.FromStream(Graphics, stream);
                        Count++;

                        if (tex != null)
                            _textures.Add(sourceName, tex);
                    }
                }
                else
                {
                    Texture2D tex = Content.Load<Texture2D>(sourcePath);
                    if (tex != null)
                        _textures.Add(sourceName, tex);
                }
            }

            if (!_sources.ContainsKey(name))
                _sources.Add(name, source);
        }

        public Texture2D Get(string path)
        {
            return _textures[path];
        }

        public Rectangle GetSource(string name)
        {
            return _sources[name];
        }

        public void Clear()
        {
            _textures.Clear();
        }

        public void LoadFromJson(string path, bool config = false)
        {
            string json = File.ReadAllText(path);

            if (config)
            {
                JsonListConfig jsonConfig = JsonConvert.DeserializeObject<JsonListConfig>(json);
                foreach (string str in jsonConfig.List)
                {
                    LoadFromJson(str);
                }
            }
            else
            {
                JsonTextureLoader textureLoader = JsonConvert.DeserializeObject<JsonTextureLoader>(json);
                foreach (JsonTexture tex in textureLoader.Sources)
                {
                    // Add the new textures in
                    AddTexture(textureLoader.Name, textureLoader.Texture, tex.ID, tex.Source);
                }
            }
        }
    }
}
