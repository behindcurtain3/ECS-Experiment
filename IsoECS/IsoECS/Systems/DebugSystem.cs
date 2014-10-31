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
        private List<Entity> brains = new List<Entity>();

        public void Update(int dt)
        {
            foreach (Entity e in brains)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();
                CitizenComponent citizen = e.Get<CitizenComponent>();

                foreach (IGameDrawable d in drawable.Get("Text"))
                {
                    d.Visible = true;
                    if (d is DrawableText)
                    {
                        string text = "";

                        Behavior brain = citizen.Brain;
                        while (brain != null)
                        {
                            text += brain.GetType().Name + System.Environment.NewLine;
                            brain = brain.Child;
                        }

                        ((DrawableText)d).Text = text;
                    }
                }
            }
        }

        public void Init()
        {
            brains.AddRange(EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); }));

            EntityManager.Instance.EntityAdded += new EntityManager.EntityEventHandler(Instance_EntityAdded);
            EntityManager.Instance.EntityRemoved += new EntityManager.EntityEventHandler(Instance_EntityRemoved);
        }

        void Instance_EntityRemoved(Entity e)
        {
            brains.Remove(e);
        }

        void Instance_EntityAdded(Entity e)
        {
            if (e.HasComponent<CitizenComponent>())
                brains.Add(e);
        }

        public void Shutdown()
        {
            EntityManager.Instance.EntityRemoved -= Instance_EntityRemoved;
            EntityManager.Instance.EntityAdded -= Instance_EntityAdded;

            foreach (Entity e in brains)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();

                foreach (IGameDrawable d in drawable.Get("Text"))
                {
                    d.Visible = false;
                }
            }

            brains.Clear();
        }
    }
}
