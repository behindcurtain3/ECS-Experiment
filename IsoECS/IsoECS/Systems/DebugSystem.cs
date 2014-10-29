using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.Behaviors;

namespace IsoECS.Systems
{
    public class DebugSystem : ISystem
    {
        public void Update(int dt)
        {
            List<Entity> citizens = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });

            foreach (Entity e in citizens)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();
                CitizenComponent citizen = e.Get<CitizenComponent>();

                foreach (IGameDrawable d in drawable.Get("Text"))
                {
                    if (d is DrawableText)
                    {
                        string text = "";

                        foreach(Behavior b in citizen.Behaviors)
                        {
                            text += b.GetType().ToString() + System.Environment.NewLine;
                        }

                        ((DrawableText)d).Text = text;
                    }
                }
            }
        }

        public void Init()
        {
        }

        public void Shutdown()
        {
        }
    }
}
