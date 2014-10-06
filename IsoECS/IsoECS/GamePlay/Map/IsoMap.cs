using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.GamePlay.Map
{
    public class IsoMap
    {
        // the tiles
        public int[, ,] Terrain { get; private set; }

        public RenderTarget2D Buffer { get; set; }
        public GraphicsDevice Graphics { get; set; }

        // Width/Height of the map in tiles
        public int TxWidth { get; private set; }
        public int TxHeight { get; private set; }

        // Width/Height of the tiles in pixels
        public int PxTileWidth { get; private set; }
        public int PxTileHeight { get; private set; }

        public int PxTileHalfHeight { get; private set; }
        public int PxTileHalfWidth { get; private set; }

        // name of the spritesheet containing the sprites for the map
        public string SpriteSheetName { get; private set; }

        // used to callback after the map has been rendered
        public delegate void RenderHandler(IsoMap map);

        public IsoMap(GraphicsDevice graphics)
        {
            Graphics = graphics;
            Buffer = new RenderTarget2D(Graphics, Graphics.Viewport.Width, Graphics.Viewport.Height);
        }

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

        /// <summary>
        /// Renders the map to a texture buffer
        /// </summary>
        /// <param name="spriteBatch">The game spritebatch to use when rendering</param>
        /// <param name="xOffset">An X offset to draw the map at, used for camera</param>
        /// <param name="yOffset">A Y offset to draw the map at, used for camera</param>
        /// <param name="handler">Function to call when the map has finished rendering</param>
        public void RenderToTexture(SpriteBatch spriteBatch, float xOffset, float yOffset, RenderHandler handler = null)
        {
            Vector2 position;
            Rectangle destination;
            Rectangle source;

            // Set the render target to the internal buffer
            Graphics.SetRenderTarget(Buffer);
            // Clear the buffer
            Graphics.Clear(Color.White);

            // begin drawing
            spriteBatch.Begin();

            // loop through the map and render each tile
            // TODO: implement z-levels?
            for (int z = 0; z < 1; z++)
            {
                for (int y = 0; y < TxHeight; y++)
                {
                    for (int x = 0; x < TxWidth; x++)
                    {
                        // get the position of this tile
                        position = GetIsometricPosition(z, y, x);

                        // update the position by the camera offsets
                        position.X -= xOffset;
                        position.Y -= yOffset;

                        // set the destination rectangle
                        destination = new Rectangle((int)position.X, (int)position.Y, PxTileWidth, PxTileHeight);

                        // get the source rectangle
                        source = Textures.Instance.GetSource("grass");

                        // draw the tile
                        spriteBatch.Draw(Textures.Instance.Get(SpriteSheetName), destination, source, Color.White);
                    }
                }
            }

            // finish drawing
            spriteBatch.End();
            Graphics.SetRenderTarget(null);

            // update the callback if valid
            if (handler != null)
                handler(this);
        }

        /// <summary>
        /// Given a position in the map coordinates return the corresponding isometric position
        /// </summary>
        /// <param name="z">The z index</param>
        /// <param name="y">The y index</param>
        /// <param name="x">The x index</param>
        /// <returns>A Vector2 containing the isometric position of the index's passed in</returns>
        public Vector2 GetIsometricPosition(int z, int y, int x)
        {
            return new Vector2((x - y) * PxTileHalfWidth, (x + y) * PxTileHalfHeight - (z * PxTileHeight));
        }
    }
}
