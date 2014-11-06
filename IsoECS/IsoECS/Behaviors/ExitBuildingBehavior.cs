using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using TecsDotNet;

namespace IsoECS.Behaviors
{
    public class ExitBuildingBehavior : Behavior
    {
        private List<Point> _validExits = new List<Point>();
        private List<Path> _createdPaths = new List<Path>();

        public uint TargetID { get; set; }
        public uint ExitID { get; set; }
        public Path SelectedPath { get; set; }

        public override BehaviorStatus Update(Entity self, double dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    if (Finished is FindPathBehavior)
                    {
                        _validExits.RemoveAt(0);

                        if (status == BehaviorStatus.SUCCESS)
                        {
                            // add the generated path
                            _createdPaths.Add(((FindPathBehavior)Finished).GeneratedPath);
                        }

                        if (_validExits.Count > 0)
                        {
                            self.Get<PositionComponent>().Index = _validExits[0];
                            FindPathBehavior fpb = new FindPathBehavior()
                            {
                                TargetID = this.TargetID,
                                MoveToNearbyRoad = true
                            };
                            AddChild(fpb);
                        }
                        else
                        {
                            // paths have finished generating, pick the best one
                            Path selected = null;

                            foreach (Path p in _createdPaths)
                            {
                                if (selected == null)
                                {
                                    selected = p;
                                    continue;
                                }

                                // TODO: select the one with the least "length" not the least count
                                if (p.Length < selected.Length)
                                {
                                    selected = p;
                                }
                            }

                            if (selected == null)
                                return BehaviorStatus.FAIL;
                            else
                            {
                                SelectedPath = selected;
                                self.Get<PositionComponent>().Index = SelectedPath.Start;
                                return BehaviorStatus.SUCCESS;
                            }
                        }
                    }
                    break;

                case BehaviorStatus.RUN:
                    // get any valid exits
                    if (_validExits.Count == 0)
                    {
                        Entity target = World.Entities.Get(TargetID);
                        Entity exit = World.Entities.Get(ExitID);

                        // if invalid target fail out
                        if (target == null || exit == null)
                        {
                            return BehaviorStatus.FAIL;
                        }

                        // if the target or self has no position fail out
                        if (!target.HasComponent<PositionComponent>() || !exit.HasComponent<PositionComponent>())
                        {
                            return BehaviorStatus.FAIL;
                        }

                        _validExits = World.GetValidExitsFromFoundation(exit);

                        if (_validExits.Count == 0)
                        {
                            return BehaviorStatus.FAIL;
                        }
                        else
                        {
                            // set the position of self to the first point while it tests the path
                            self.Get<PositionComponent>().Index = _validExits[0];
                            FindPathBehavior fpb = new FindPathBehavior()
                            {
                                TargetID = this.TargetID,
                                MoveToNearbyRoad = true
                            };
                            AddChild(fpb);
                        }
                    }
                    break;
            }

            return BehaviorStatus.WAIT;
        }
    }
}
