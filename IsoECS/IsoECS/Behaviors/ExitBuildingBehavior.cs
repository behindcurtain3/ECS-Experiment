using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using IsoECS.DataStructures;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.Util;

namespace IsoECS.Behaviors
{
    public class ExitBuildingBehavior : Behavior
    {
        private List<Point> _validExits = new List<Point>();
        private List<Path> _createdPaths = new List<Path>();

        public int TargetID { get; set; }
        public int ExitID { get; set; }
        public Path SelectedPath { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            // get any valid exits
            if (_validExits.Count == 0)
            {
                Entity target = em.Entities.Find(delegate(Entity e) { return e.ID == TargetID; });
                Entity exit = em.Entities.Find(delegate(Entity e) { return e.ID == ExitID; });

                // if invalid target fail out
                if (target == null || exit == null)
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }

                // if the target or self has no position fail out
                if (!target.HasComponent<PositionComponent>() || !exit.HasComponent<PositionComponent>())
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }

                PositionComponent ePosition = exit.Get<PositionComponent>();
                FoundationComponent foundation = exit.Get<FoundationComponent>();
                
                foreach (LocationValue plot in foundation.Plan)
                {
                    // check the ortho points around the plot position
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            if (Math.Abs(x) == Math.Abs(y))
                                continue;

                            Point p = new Point(ePosition.Index.X + plot.Offset.X + x, ePosition.Index.Y + plot.Offset.Y + y);

                            if (!Isometric.ValidIndex(em.Map, p.X, p.Y))
                                continue;

                            if ((!em.Collisions.Map.ContainsKey(p) || em.Collisions.Map[p] != -1) && !_validExits.Contains(p))
                                _validExits.Add(p);
                        }
                    }
                }

                if (_validExits.Count == 0)
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
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
                    state.Push(fpb);
                }
            }
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(em, self, finished, state);

            if (finished is FindPathBehavior)
            {
                _validExits.RemoveAt(0);

                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    // add the generated path
                    _createdPaths.Add(((FindPathBehavior)finished).GeneratedPath);
                }

                if (_validExits.Count > 0)
                {
                    self.Get<PositionComponent>().Index = _validExits[0];
                    FindPathBehavior fpb = new FindPathBehavior()
                    {
                        TargetID = this.TargetID,
                        MoveToNearbyRoad = true
                    };
                    state.Push(fpb);
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
                        Status = BehaviorStatus.FAILURE;
                    else
                    {
                        SelectedPath = selected;
                        Status = BehaviorStatus.SUCCESS;
                        self.Get<PositionComponent>().Index = SelectedPath.Start;
                    }
                }
            }
        }
    }
}
