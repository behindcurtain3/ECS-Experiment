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

            Point Center = Isometric.GetPointAtScreenCoords(map, map.Graphics.Viewport.Width / 2 + (int)camera.X, map.Graphics.Viewport.Height / 2 + (int)camera.Y);

            int goFirstA = (Center.X + Center.Y) - (map.Graphics.Viewport.Height / map.PxTileHeight) - 2;
            int goLastA = (Center.X + Center.Y) + (map.Graphics.Viewport.Height / map.PxTileHeight) + 3;
            int goFirstB = (Center.X - Center.Y) - (map.Graphics.Viewport.Width / map.PxTileWidth) - 2;
            int goLastB = (Center.X - Center.Y) + (map.Graphics.Viewport.Width / map.PxTileWidth) + 3;
            int x, y;
            for (int z = 0; z < 1; z++)
            {
                for (int a = goFirstA; a < goLastA; a++)
                {
                    for (int b = goFirstB; b < goLastB; b++)
                    {
                        if ((b & 1) != (a & 1)) continue;
                        x = (a + b) / 2;
                        y = (a - b) / 2;

                        if (!Isometric.ValidIndex(map, x, y))
                            continue;

                        // get the position of this tile
                        position = Isometric.GetIsometricPosition(map, z, y, x);

                        // update the position by the camera offsets
                        position.X -= camera.X;
                        position.Y -= camera.Y;

                        // set the destination rectangle
                        destination = new Rectangle((int)position.X, (int)position.Y, map.PxTileWidth, map.PxTileHeight);

                        // get the source rectangle
                        // TODO: this should be dynamic based on the value at Terrain[z, y, x]
                        source = Textures.Instance.GetSource(map.SpriteSheetName, "grass");

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
                Textures.Instance.UpdateTexture("internal_map_texture", (Texture2D)map.Buffer, "internal_map_source", new TextureInfo()
                {
                    Source = new Rectangle(0, 0, map.Buffer.Width, map.Buffer.Height),
                    Origin = Vector2.Zero
                });
            }
        }
    }
}
