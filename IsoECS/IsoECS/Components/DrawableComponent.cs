using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using IsoECS.GamePlay;
using TecsDotNet;

namespace IsoECS.Components
{
    [Serializable]
    public class DrawableComponent : Component
    {
        // Stores all drawables
        // the string is a reference to the layer the drawables belong on
        public Dictionary<string, List<GameDrawable>> Drawables { get; set; }

        public DrawableComponent()
        {
            Drawables = new Dictionary<string, List<GameDrawable>>();
        }

        public void Add(string layer, GameDrawable drawable)
        {
            try
            {
                if (!Drawables.ContainsKey(layer))
                    Drawables.Add(layer, new List<GameDrawable>());

                if(!Drawables[layer].Contains(drawable))
                    Drawables[layer].Add(drawable);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void RemoveByPrototypeID(string id)
        {
            foreach (KeyValuePair<string, List<GameDrawable>> kvp in Drawables)
            {
                Drawables[kvp.Key].RemoveAll(delegate(GameDrawable d) { return d.PrototypeID.Equals(id); });
            }
        }

        public List<GameDrawable> Get(string layer)
        {
            if (!Drawables.ContainsKey(layer))
                Drawables.Add(layer, new List<GameDrawable>());

            return Drawables[layer];
        }
    }
}
