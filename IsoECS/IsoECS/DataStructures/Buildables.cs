using System.Collections.Generic;
using System.IO;
using IsoECS.DataStructures.Json;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IsoECS.DataStructures
{
    public struct BuildableInfo
    {
        public string Category;
        public string Name;
        public string Description;
        public string SpriteSheetName;
        public string SourceID;

        public string ConstructSpriteSheetName;
        public string ConstructSourceID;

        public bool DragBuildEnabled;
        public JObject[] Components;
    }

    public sealed class Buildables
    {
        private static readonly Buildables _instance = new Buildables();

        public static Buildables Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, List<BuildableInfo>> _buildables;

        private Buildables()
        {
            _buildables = new Dictionary<string, List<BuildableInfo>>();
        }

        public void AddBuildable(BuildableInfo buildable)
        {
            if (!_buildables.ContainsKey(buildable.Category))
                _buildables.Add(buildable.Category, new List<BuildableInfo>());

            _buildables[buildable.Category].Add(buildable);
        }

        public BuildableInfo Get(string name, int index)
        {
            return _buildables[name][index];
        }

        public List<BuildableInfo> GetCategory(string name)
        {
            return _buildables[name];
        }

        public List<string> GetCategories()
        {
            return new List<string>(_buildables.Keys);
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
                JsonBuildableLoader loader = JsonConvert.DeserializeObject<JsonBuildableLoader>(json);
                foreach (BuildableInfo info in loader.Buildables)
                {
                    AddBuildable(info);
                }
            }
        }
    }
}
