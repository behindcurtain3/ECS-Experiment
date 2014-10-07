using System.Collections.Generic;
using System.IO;
using IsoECS.DataStructures.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace IsoECS.DataStructures
{
    public struct TextureInfo
    {
        public Rectangle Source;
        public Vector2 Origin;
    }

    public sealed class Textures
    {
        private static readonly Textures _instance = new Textures();

        public static Textures Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, Dictionary<string, TextureInfo>> _textureDb;
        private Dictionary<string, Texture2D> _textures;
        
        //private Dictionary<string, Rectangle> _sources;
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
            _textureDb = new Dictionary<string, Dictionary<string, TextureInfo>>();
            _textures = new Dictionary<string, Texture2D>();
            //_sources = new Dictionary<string, Rectangle>();
        }

        public void AddTexture(string spriteSheetName, string texturePath, string sourceID, Rectangle sourceRect, Vector2 sourceOrigin)
        {
            // If we haven't loaded the texture do so
            if (!_textures.ContainsKey(spriteSheetName))
            {
                if (File.Exists("Content\\" + texturePath + ".png"))
                {
                    using (Stream stream = TitleContainer.OpenStream("Content\\" + texturePath + ".png"))
                    {
                        Texture2D tex = Texture2D.FromStream(Graphics, stream);
                        Count++;

                        if (tex != null)
                            _textures.Add(spriteSheetName, tex);
                    }
                }
                else
                {
                    Texture2D tex = Content.Load<Texture2D>(texturePath);
                    if (tex != null)
                        _textures.Add(spriteSheetName, tex);
                }
            }

            // setup the info for this source
            TextureInfo info = new TextureInfo();
            info.Source = sourceRect;
            info.Origin = sourceOrigin;

            if (!_textureDb.ContainsKey(spriteSheetName))
            {
                _textureDb.Add(spriteSheetName, new Dictionary<string, TextureInfo>());
            }

            if (!_textureDb[spriteSheetName].ContainsKey(sourceID))
                _textureDb[spriteSheetName].Add(sourceID, info);
        }

        public Texture2D Get(string path)
        {
            return _textures[path];
        }

        public Rectangle GetSource(string spirteSheetName, string sourceID)
        {
            return _textureDb[spirteSheetName][sourceID].Source;
        }

        public Vector2 GetOrigin(string spriteSheetName, string sourceID)
        {
            return _textureDb[spriteSheetName][sourceID].Origin;
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
                    AddTexture(textureLoader.Name, textureLoader.Texture, tex.ID, tex.Source, tex.Origin);
                }
            }
        }
    }
}
