using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IsoECS.Components.GamePlay;

namespace IsoECS.Util
{
    public class Isometric
    {
        public static Vector2 GetIsometricPosition(IsometricMapComponent map, int z, int y, int x)
        {
            return new Vector2(
                x * map.PxTileHalfWidth + y * -map.PxTileHeight,
                x * map.PxTileHalfHeight + y * map.PxTileHalfHeight
                );

            /// 3d implementation
            /// return new Vector2((x - y) * map.PxTileHalfWidth, (x + y) * map.PxTileHalfHeight - (z * map.PxTileHeight));
        }

        public static Point GetPointAtScreenCoords(IsometricMapComponent map, int x, int y)
        {
            /*
            Vector2 v = new Vector2();

            v.X = (x + 2 * y) / map.PxTileWidth;
            v.Y = (2 * y - x) / map.PxTileWidth;

            return v;
            */
            //x += _camera.X;
            //y += _camera.Y + (_camera.Level * (_tileHeight + _floorHeight)) - _tileHeight;
            
            Point p = new Point();
            int x1 = (int)(x - map.PxTileHalfWidth);
            int y1 = y * -2;

            double xr = Math.Cos(Math.PI / 4) * x1 - Math.Sin(Math.PI / 4) * y1;
            double yr = Math.Sin(Math.PI / 4) * x1 + Math.Cos(Math.PI / 4) * y1;

            double diag = (map.PxTileHeight) * Math.Sqrt(2);
            p.X = (int)(xr / diag);
            p.Y = (int)(yr * -1 / diag);

            return p;
        }

        public static bool ValidIndex(IsometricMapComponent map, int x, int y)
        {
            return (x >= 0 && x < map.TxWidth && y >= 0 && y < map.TxHeight);
        }

        public static Vector2 MoveTowards(Vector2 position, float speed, Vector2 target)
        {
            double tx = target.X - position.X;
            double ty = target.Y - position.Y;
            double length = Math.Sqrt(tx * tx + ty * ty);

            if (length > speed)
            {
                // move towards the goal
                position.X = (float)(position.X + speed * tx / length);
                position.Y = (float)(position.Y + speed * ty / length);
            }
            else
            {
                // already there
                position.X = target.X;
                position.Y = target.Y;
            }

            return position;
        }
    }
}
