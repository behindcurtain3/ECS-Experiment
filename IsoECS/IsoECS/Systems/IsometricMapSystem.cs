using System.Collections.Generic;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Util;

namespace IsoECS.Systems
{
    public class IsometricMapSystem : IRenderSystem
    {

        public void Draw(List<Entity> entities, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });

            if (mapEntity == null || cameraEntity == null)
                return;

            PositionComponent camera = cameraEntity.Get<PositionComponent>();
            IsometricMapComponent map = mapEntity.Get<IsometricMapComponent>();
            DrawableComponent mapDrawable = mapEntity.Get<DrawableComponent>();

            Vector2 position;
            Rectangle destination;
            Rectangle source;

            // Set the render target to the internal buffer
            map.Graphics.SetRenderTarget(map.Buffer);
            // Clear the buffer
            map.Graphics.Clear(Color.Transparent);

            // begin drawing
            spriteBatch.Begin();

            // loop through the map and render each tile
            // TODO: implement z-levels?
            for (int z = 0; z < 1; z++)
            {
                for (int y = 0; y < map.TxHeight; y++)
                {
                    for (int x = 0; x < map.TxWidth; x++)
                    {
                        // get the position of this tile
                        position = Isometric.GetIsometricPosition(map, z, y, x);

                        // update the position by the camera offsets
                        position.X -= camera.X;
                        position.Y -= camera.Y;

                        // set the destination rectangle
                        destination = new Rectangle((int)position.X, (int)position.Y, map.PxTileWidth, map.PxTileHeight);

                        // get the source rectangle
                        // TODO: this should be dynamic based on the value at Terrain[z, y, x]
                        source = Textures.Instance.GetSource("grass");

                        // draw the tile
                        spriteBatch.Draw(Textures.Instance.Get(map.SpriteSheetName), destination, source, Color.White);
                    }
                }
            }

            // finish drawing
            spriteBatch.End();
            map.Graphics.SetRenderTarget(null);

            if (mapDrawable != null)
            {
                mapDrawable.Layer = 99;
                mapDrawable.Texture = (Texture2D)map.Buffer;
                mapDrawable.Source = new Rectangle(0, 0, map.Buffer.Width, map.Buffer.Height);
                mapDrawable.Destination = new Rectangle(0, 0, map.Graphics.Viewport.Width, map.Graphics.Viewport.Height);
            }
        }
    }
}
