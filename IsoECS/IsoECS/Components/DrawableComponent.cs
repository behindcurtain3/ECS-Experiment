using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using IsoECS.GamePlay;

namespace IsoECS.Components
{
    [Serializable]
    public class DrawableComponent : Component
    {
        // Stores all drawables
        // the string is a reference to the layer the drawables belong on
        public Dictionary<string, List<IGameDrawable>> Drawables { get; set; }

        public DrawableComponent()
        {
            Drawables = new Dictionary<string, List<IGameDrawable>>();
        }

        public void Add(string layer, IGameDrawable drawable)
        {
            try
            {
                if (!Drawables.ContainsKey(layer))
                    Drawables.Add(layer, new List<IGameDrawable>());

                if(!Drawables[layer].Contains(drawable))
                    Drawables[layer].Add(drawable);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void RemoveByUniqueID(string uniqueID)
        {
            foreach (KeyValuePair<string, List<IGameDrawable>> kvp in Drawables)
            {
                Drawables[kvp.Key].RemoveAll(delegate(IGameDrawable d) { return d.UniqueID.Equals(uniqueID); });
            }
        }

        public List<IGameDrawable> Get(string layer)
        {
            if (!Drawables.ContainsKey(layer))
                Drawables.Add(layer, new List<IGameDrawable>());

            return Drawables[layer];
        }
    }
}
