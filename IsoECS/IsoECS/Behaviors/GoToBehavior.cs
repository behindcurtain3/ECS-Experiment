using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;

namespace IsoECS.Behaviors
{
    public class GoToBehavior : Behavior
    {
        public int TargetID { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            FindPathBehavior fpb = new FindPathBehavior()
            {
                MoveToNearbyRoad = true,
                TargetID = this.TargetID
            };
            state.Push(fpb);
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            if (finished is FindPathBehavior)
            {
                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    // follow path
                    FollowPathBehavior fpb = new FollowPathBehavior()
                    {
                        PathToFollow = ((FindPathBehavior)finished).GeneratedPath,
                        Speed = 1.5f
                    };
                    state.Push(fpb);
                    state.Push(new FadeBehavior() { FadeIn = true });
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

    }
}
