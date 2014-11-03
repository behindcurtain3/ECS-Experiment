using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;

namespace IsoECS.Behaviors
{
    public class GoToBehavior : Behavior
    {
        public int TargetID { get; set; }
        public bool FollowRoadsOnly { get; set; }
        public Path GeneratedPath { get; set; }

        public GoToBehavior()
            : base()
        {
            FollowRoadsOnly = true;
        }

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    if (Finished is FindPathBehavior)
                    {
                        if (status == BehaviorStatus.SUCCESS)
                        {
                            FollowPath(((FindPathBehavior)Finished).GeneratedPath, 1.5f);
                        }
                        else
                        {
                            // TODO: quit job if not able to find a path to it
                            return BehaviorStatus.FAIL;
                        }
                    }
                    else if (Finished is FollowPathBehavior)
                    {
                        if (status == BehaviorStatus.SUCCESS)
                        {
                            // Fadeout
                            AddChild(new FadeBehavior() { FadeIn = false });
                            self.Get<CitizenComponent>().InsideID = TargetID;
                        }
                        else
                        {
                            // failed to follow path to home
                            // TOOD: possibly fail out to decide to do something else?
                            return BehaviorStatus.WAIT;
                        }
                    }
                    else
                    {
                        return status;
                    }
                    break;

                case BehaviorStatus.RUN:
                    if (GeneratedPath == null)
                    {
                        FindPathBehavior fpb = new FindPathBehavior()
                        {
                            MoveToNearbyRoad = true,
                            TargetID = this.TargetID,
                            FollowRoadsOnly = FollowRoadsOnly
                        };
                        AddChild(fpb);
                    }
                    else
                    {
                        FollowPath(GeneratedPath, 1.5f);
                    }
                    break;
            }

            return BehaviorStatus.WAIT;
        }

        private void FollowPath(Path p, float speed)
        {
            // follow path
            FollowPathBehavior fpb = new FollowPathBehavior()
            {
                PathToFollow = p,
                Speed = speed
            };
            AddChild(fpb);
            fpb.AddChild(new FadeBehavior() { FadeIn = true });
        }
    }
}
