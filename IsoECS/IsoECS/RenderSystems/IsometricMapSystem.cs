﻿using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.GamePlay;
using IsoECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TecsDotNet;

namespace IsoECS.RenderSystems
{
    public class IsometricMapSystem : RenderSystem
    {
        private Entity cameraEntity;
        private Entity mapEntity;
        private IsometricMapComponent map;

        private int verticalTiles;
        private int horizontalTiles;

        public override void Init()
        {
            base.Init();

            cameraEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            mapEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
            map = mapEntity.Get<IsometricMapComponent>();

            if (map.Graphics == null)
                map.Graphics = Graphics;

            if (map.Buffer == null)
                map.Buffer = new RenderTarget2D(map.Graphics, map.Graphics.Viewport.Width, map.Graphics.Viewport.Height);

            verticalTiles = (map.Graphics.Viewport.Height / map.PxTileHeight);
            horizontalTiles = (map.Graphics.Viewport.Width / map.PxTileWidth);
        }

        public override void Shutdown()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if (mapEntity == null || cameraEntity == null)
                return;

            PositionComponent camera = cameraEntity.Get<PositionComponent>();
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

            Point Center = map.GetIndexFromPosition((int)(map.Graphics.Viewport.Width * 0.5) + (int)camera.X, (int)(map.Graphics.Viewport.Height * 0.5) + (int)camera.Y);

            int goFirstA = (Center.X + Center.Y) - verticalTiles - 2;
            int goLastA = (Center.X + Center.Y) + verticalTiles + 3;
            int goFirstB = (Center.X - Center.Y) - horizontalTiles - 2;
            int goLastB = (Center.X - Center.Y) + horizontalTiles + 3;
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

                        if (!map.IsValidIndex(x, y))
                            continue;

                        // get the position of this tile
                        position = map.GetPositionFromIndex(x, y);

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
