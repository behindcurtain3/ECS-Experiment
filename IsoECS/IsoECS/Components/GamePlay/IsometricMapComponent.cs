using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Components.GamePlay
{
    public class IsometricMapComponent : Component
    {
        // the tiles
        public int[, ,] Terrain { get; set; }

        public RenderTarget2D Buffer { get; set; }
        public GraphicsDevice Graphics { get; set; }

        // Width/Height of the map in tiles
        public int TxWidth { get; set; }
        public int TxHeight { get; set; }

        // Width/Height of the tiles in pixels
        public int PxTileWidth { get; set; }
        public int PxTileHeight { get; set; }

        public float PxTileHalfHeight { get; set; }
        public float PxTileHalfWidth { get; set; }

        // name of the spritesheet containing the sprites for the map
        public string SpriteSheetName { get; set; }

        // used to callback after the map has been rendered
        //public delegate void RenderHandler(IsoMap map);
    }
}
