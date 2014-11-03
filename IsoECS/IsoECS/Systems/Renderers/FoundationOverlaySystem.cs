using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework.Graphics;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using IsoECS.Entities;

namespace IsoECS.Systems.Renderers
{
    public class FoundationOverlaySystem : RenderSystem
    {
        private string spriteSheet = "foundation-overlay";
        private string source = "overlay";

        protected override void DrawLayer(string layer, SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> layerData)
        {
            if (layer.Equals("Foundation"))
            {
                // override the drawing of the foundation layer
                DrawFoundationOverlay(layer, spriteBatch, spriteFont, camera, layerData);
            }
            else if (layer.Equals("Foreground") || layer.Equals("Text"))
            {
                // override the drawing of the foundation layer
                DrawFoundationOverlay(layer, spriteBatch, spriteFont, camera, layerData);
            }
            else
                base.DrawLayer(layer, spriteBatch, spriteFont, camera, layerData);
        }

        protected virtual void DrawFoundationOverlay(string layer, SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> layerData)
        {
            // if the drawable is a building draw the foundation placeholder, otherwise just draw it
            foreach (DrawData dd in layerData)
            {
                if (!dd.Position.BelongsTo.HasComponent<FoundationComponent>() || dd.Position.BelongsTo.HasComponent<RoadComponent>())
                {
                    Draw(spriteBatch, spriteFont, dd, camera);
                }
                else
                {
                    // draw basic foundation overlay
                    if(layer.Equals("Foundation"))
                    {
                        FoundationComponent foundation = dd.Position.BelongsTo.Get<FoundationComponent>();

                        foreach (LocationValue lv in foundation.Plan)
                        {
                            Point index = new Point(lv.Offset.X, lv.Offset.Y);
                            index.X += dd.Position.Index.X;
                            index.Y += dd.Position.Index.Y;

                            Vector2 p = EntityManager.Instance.Map.GetPositionFromIndex(index.X, index.Y);
                            p.X -= cameraPosition.X;
                            p.Y -= cameraPosition.Y;

                            spriteBatch.Draw(Textures.Instance.Get(spriteSheet), p, Textures.Instance.GetSource(spriteSheet, source), dd.Drawable.Color);
                        }
                    }
                }
            }
        }
    }
}
