using System.Collections.Generic;
using IsoECS.Behaviors;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using TecsDotNet;
using TecsDotNet.Managers;

namespace IsoECS.Systems
{
    public class DebugSystem : GameSystem
    {
        private List<Entity> brains = new List<Entity>();

        public override void Update(double dt)
        {
            foreach (Entity e in brains)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();
                CitizenComponent citizen = e.Get<CitizenComponent>();

                foreach (GameDrawable d in drawable.Get("Text"))
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

        public override void Init()
        {
            base.Init();
            brains.AddRange(World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); }));

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
        }

        private void Entities_EntityRemoved(object sender, EntityEventArgs e)
        {
            brains.Remove(e.Entity);
        }

        private void Entities_EntityAdded(object sender, EntityEventArgs e)
        {
            if (e.Entity.HasComponent<CitizenComponent>())
                brains.Add(e.Entity);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            World.Entities.EntityAdded -= Entities_EntityAdded;
            World.Entities.EntityRemoved -= Entities_EntityRemoved;

            foreach (Entity e in brains)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();

                foreach (GameDrawable d in drawable.Get("Text"))
                {
                    d.Visible = false;
                }
            }

            brains.Clear();
        }
    }
}
