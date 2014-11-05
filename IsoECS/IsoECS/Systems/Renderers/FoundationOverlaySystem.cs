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
            if (layer.Equals("Foundation") || layer.Equals("Foreground") || layer.Equals("Text"))
            {
                // override the drawing of the foundation layer
                DrawOverlay(layer, spriteBatch, spriteFont, camera, layerData);
            }
            else
            {
                base.DrawLayer(layer, spriteBatch, spriteFont, camera, layerData);
            }
        }

        protected virtual void DrawOverlay(string layer, SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> layerData)
        {
            // if the drawable is a building draw the foundation placeholder, otherwise just draw it
            if(layer.Equals("Foundation"))
            {
                foreach (var space in EntityManager.Instance.Foundations.SpaceTaken)
                {
                    Entity e = EntityManager.Instance.GetEntity(space.Value);
                    DrawData dd = layerData.Find(delegate(DrawData d) { return d.Position.BelongsTo.ID == e.ID; });

                    if (e == null)
                        continue;

                    if (IsEntityValid(e))
                    {
                        Vector2 p = EntityManager.Instance.Map.GetPositionFromIndex(space.Key.X, space.Key.Y);
                        p.X -= cameraPosition.X;
                        p.Y -= cameraPosition.Y;

                        spriteBatch.Draw(Textures.Instance.Get(spriteSheet), p, Textures.Instance.GetSource(spriteSheet, source), Color.White);
                    }
                    else
                    {
                        if(dd.Drawable != null)
                            Draw(spriteBatch, spriteFont, dd, camera);
                    }
                }
            }
            else
            {
                foreach (DrawData dd in layerData)
                {
                    if (!IsEntityValid(dd.Position.BelongsTo))
                    {
                        Draw(spriteBatch, spriteFont, dd, camera);
                    }
                }
            }

        }

        protected bool IsEntityValid(Entity e)
        {
            return e.HasComponent<FoundationComponent>() && !e.HasComponent<RoadComponent>();
        }
    }
}
