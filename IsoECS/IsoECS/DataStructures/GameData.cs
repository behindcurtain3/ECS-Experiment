using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;
using System.IO;
using Newtonsoft.Json;
using IsoECS.DataStructures.Json;
using Newtonsoft.Json.Linq;

namespace IsoECS.DataStructures
{
    public sealed class GameData
    {
        private static readonly GameData _instance = new GameData();

        public static GameData Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, Recipe> _recipes;
        private Dictionary<string, Item> _items;

        private GameData()
        {
            _recipes = new Dictionary<string, Recipe>();
            _items = new Dictionary<string, Item>();
        }

        public void LoadRecipesFromJson(string path, bool config = false)
        {
            string json = File.ReadAllText(path);

            if (config)
            {
                JsonListConfig jsonConfig = JsonConvert.DeserializeObject<JsonListConfig>(json);
                foreach (string str in jsonConfig.List)
                {
                    LoadRecipesFromJson(str);
                }
            }
            else
            {
                JObject file = JObject.Parse(json);
                foreach (JObject o in file["Recipes"].ToObject<IEnumerable<JObject>>())
                {
                    Recipe r = JsonConvert.DeserializeObject<Recipe>(o.ToString());

                    _recipes.Add(r.UniqueID, r);
                }
            }
        }

        public void LoadItemsFromJson(string path, bool config = false)
        {
            string json = File.ReadAllText(path);

            if (config)
            {
                JsonListConfig jsonConfig = JsonConvert.DeserializeObject<JsonListConfig>(json);
                foreach (string str in jsonConfig.List)
                {
                    LoadItemsFromJson(str);
                }
            }
            else
            {
                JObject file = JObject.Parse(json);
                foreach (JObject o in file["Items"].ToObject<IEnumerable<JObject>>())
                {
                    Item i = JsonConvert.DeserializeObject<Item>(o.ToString());

                    _items.Add(i.UniqueID, i);
                }
            }
        }
    }
}
