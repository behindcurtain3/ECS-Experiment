using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;

namespace IsoECS.Behaviors
{
    public class GoToBehavior : Behavior
    {
        public int TargetID { get; set; }
        public Path GeneratedPath { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            if (GeneratedPath == null)
            {
                FindPathBehavior fpb = new FindPathBehavior()
                {
                    MoveToNearbyRoad = true,
                    TargetID = this.TargetID
                };
                state.Push(fpb);
            }
            else
            {
                FollowPath(state, GeneratedPath, 1.5f);
            }
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            if (finished is FindPathBehavior)
            {
                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    FollowPath(state, ((FindPathBehavior)finished).GeneratedPath, 1.5f);
                }
                else
                {
                    // TODO: quit job if not able to find a path to it
                    Status = BehaviorStatus.FAILURE;
                }
            }
            else if (finished is FollowPathBehavior)
            {
                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    // Fadeout
                    state.Push(new FadeBehavior() { FadeIn = false });
                    self.Get<CitizenComponent>().InsideID = TargetID;
                }
                else
                {
                    // failed to follow path to home
                    // TOOD: possibly fail out to decide to do something else?
                    Status = BehaviorStatus.RUNNING;
                }
            }
            else
            {
                Status = finished.Status;
            }
        }

        private void FollowPath(Stack<Behavior> state, Path p, float speed)
        {
            // follow path
            FollowPathBehavior fpb = new FollowPathBehavior()
            {
                PathToFollow = p,
                Speed = speed
            };
            state.Push(fpb);
            state.Push(new FadeBehavior() { FadeIn = true });
        }
    }
}
