using System.Collections.Generic;
using IsoECS.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TecsDotNet;
using TecsDotNet.Json;
using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures.Json.Converters
{
    public class CustomEntityLoader : JsonEntityLoader
    {
        public override void ComponentLoaded(Entity e, Component c, JObject o)
        {
            // Do custom handling of the component
            switch (c.GetType().Name)
            {
                case "DrawableComponent":
                    DrawableComponent drawable = (DrawableComponent)c;
                    drawable.Drawables.Clear();
                    if (o["Sources"] != null)
                    {
                        IEnumerable<string> sources = o["Sources"].Values<string>();
                        foreach (string str in sources)
                        {
                            GameDrawable d = (GameDrawable)Library[str];

                            drawable.Add(d.Layer, d);
                        }

                    }
                    if (o["Drawables"] != null)
                    {
                        DrawableComponent dd = JsonConvert.DeserializeObject<DrawableComponent>(o.ToString(), new DrawableConverter());

                        foreach (KeyValuePair<string, List<GameDrawable>> kvp in dd.Drawables)
                        {
                            foreach (GameDrawable d in kvp.Value)
                            {
                                drawable.Add(kvp.Key, d);
                            }
                        }
                    }
                    break;

                case "FoundationComponent":
                    FoundationComponent floor = (FoundationComponent)c;
                    switch (floor.PlanType)
                    {
                        case "Fill":
                            Point start = floor.Plan[0].Offset;
                            Point end = floor.Plan[1].Offset;

                            floor.Plan.Clear(); // clear the plan, the for loops will fill it
                            for (int xx = start.X; xx <= end.X; xx++)
                            {
                                for (int yy = start.Y; yy <= end.Y; yy++)
                                {
                                    floor.Plan.Add(new LocationValue() { Offset = new Point(xx, yy) });
                                }
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
