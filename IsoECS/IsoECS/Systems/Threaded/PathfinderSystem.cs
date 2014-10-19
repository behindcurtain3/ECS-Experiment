﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using IsoECS.Util;
using IsoECS.Components.GamePlay;
using System.Threading;

namespace IsoECS.Systems.Threaded
{
    public class PathRequest
    {
        public int ID { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public List<Point> Ends { get; set; }
    }

    public class PathfinderSystem
    {
        private static int _counter = 0;
        public static Queue<PathRequest> PathsRequested { get; set; }
        public static Dictionary<int, Path> PathsCreated { get; set; }
        public IsometricMapComponent Map { get; set; }
        public CollisionMapComponent Collisions { get; set; }

        public PathfinderSystem()
        {
            PathsCreated = new Dictionary<int, Path>();
            PathsRequested = new Queue<PathRequest>();
        }

        public static int RequestPath(PathRequest pr)
        {
            pr.ID = ++_counter;

            lock(PathsRequested)
            {
                PathsRequested.Enqueue(pr);
            }

            return pr.ID;
        }

        public static Path GetPath(int id)
        {
            lock (PathsCreated)
            {
                if (PathsCreated.ContainsKey(id))
                {
                    // remove the path from the dictionary and return it
                    Path p = PathsCreated[id];
                    PathsCreated.Remove(id);
                    return p;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Run()
        {
            PathRequest pr;
            Path p;
            while (true)
            {
                
                // check if any paths are requested
                lock (PathsRequested)
                {
                    if (PathsRequested.Count == 0)
                        continue;

                    // get the first one off the queue if so
                    pr = PathsRequested.Dequeue();
                }

                // generate the path
                if (pr.Ends == null)
                {
                    p = Pathfinder.Generate(Collisions, Map, pr.Start, pr.End);
                }
                else
                {
                    p = Pathfinder.Generate(Collisions, Map, pr.Start, pr.Ends);
                }

                // add the path to the completion dictionary
                lock (PathsCreated)
                {
                    PathsCreated.Add(pr.ID, p);
                }

                Thread.Sleep(1);
            }
        }
    }
}
