using System;
using Microsoft.Xna.Framework;

namespace IsoECS.Util
{
    public class MathHelper
    {
        public static int Distance(Point a, Point b)
        {
            return (int)Math.Ceiling(Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)));
        }
    }
}
