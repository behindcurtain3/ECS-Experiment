using System;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using IsoECS.GamePlay.Map;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class IsometricMapComponent : Component
    {   
        [JsonIgnore]
        public RenderTarget2D Buffer { get; set; }
        [JsonIgnore]
        public GraphicsDevice Graphics { get; set; }

        // name of the spritesheet containing the sprites for the map
        public string SpriteSheetName { get; set; }

        // Width/Height of the map in tiles
        public int TxWidth { get; set; }
        public int TxHeight { get; set; }

        // Width/Height of the tiles in pixels
        public int PxTileWidth { get; set; }
        public int PxTileHeight { get; set; }

        public float PxTileHalfHeight { get; set; }
        public float PxTileHalfWidth { get; set; }

        // the tiles
        public int[, ,] Terrain { get; set; }

        public void CreateMap(string spriteSheetName, int txWidth, int txHeight, int pxTileWidth, int pxTileHeight)
        {
            SpriteSheetName = spriteSheetName;

            TxWidth = txWidth;
            TxHeight = txHeight;

            PxTileWidth = pxTileWidth;
            PxTileHeight = pxTileHeight;

            PxTileHalfWidth = PxTileWidth / 2;
            PxTileHalfHeight = PxTileHeight / 2;

            // Create the tile data structure
            Terrain = new int[1, txHeight, txWidth];

            // fill in the array
            for (int y = 0; y < TxHeight; y++)
            {
                for (int x = 0; x < TxWidth; x++)
                {
                    Terrain[0, y, x] = (int)Tiles.Grass;
                }
            }
        }

        public Vector2 GetPositionFromIndex(int x, int y)
        {
            return new Vector2(
                x * PxTileHalfWidth + y * -PxTileHeight,
                x * PxTileHalfHeight + y * PxTileHalfHeight
                );

            /// 3d implementation
            /// return new Vector2((x - y) * map.PxTileHalfWidth, (x + y) * map.PxTileHalfHeight - (z * map.PxTileHeight));
        }

        public Point GetIndexFromPosition(int x, int y)
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
            int x1 = (int)(x - PxTileHalfWidth);
            int y1 = y * -2;

            double xr = Math.Cos(Math.PI / 4) * x1 - Math.Sin(Math.PI / 4) * y1;
            double yr = Math.Sin(Math.PI / 4) * x1 + Math.Cos(Math.PI / 4) * y1;

            double diag = (PxTileHeight) * Math.Sqrt(2);
            p.X = (int)(xr / diag);
            p.Y = (int)(yr * -1 / diag);

            return p;
        }

        public bool IsValidIndex(Point p)
        {
            return IsValidIndex(p.X, p.Y);
        }

        public bool IsValidIndex(int x, int y)
        {
            return (x >= 0 && x < TxWidth && y >= 0 && y < TxHeight);
        }

        public Vector2 MoveTowards(Vector2 position, float speed, Vector2 target)
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
