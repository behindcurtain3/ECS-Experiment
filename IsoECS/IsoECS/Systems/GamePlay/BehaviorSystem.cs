using System.Collections.Generic;
using IsoECS.Behaviors;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Systems.GamePlay
{
    public class BehaviorSystem : ISystem
    {
        public void Init(EntityManager em)
        {
        }

        public void Shutdown(EntityManager em)
        {
        }

        public void Update(EntityManager em, int dt)
        {
            List<Entity> brains = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });

            foreach (Entity e in brains)
            {
                CitizenComponent citizen = e.Get<CitizenComponent>();

                // TODO: add a "default" behavior if the citizen has none
                if (citizen.Behaviors.Count == 0)
                {
                    DefaultBehavior db = new DefaultBehavior();
                    db.Init(em, e);
                    citizen.Behaviors.Push(db);
                    continue;
                }

                // check if the current behavior is still running
                while (citizen.Behaviors.Peek().Status != BehaviorStatus.RUNNING)
                {
                    // if it isn't running pop it off the stack and report its status to the next behavior on the stack
                    Behavior completedBehavior = citizen.Behaviors.Pop();

                    // let the "higher" behavior know the sub behavior finished
                    citizen.Behaviors.Peek().OnSubFinished(em, e, completedBehavior, citizen.Behaviors);
                }

                // update the current behavior
                citizen.Behaviors.Peek().Update(em, e, citizen.Behaviors, dt);
            }
        }
    }
}
