using System;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

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
    }
}
