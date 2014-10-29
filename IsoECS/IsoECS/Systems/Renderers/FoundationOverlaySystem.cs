using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Graphics;
using IsoECS.Components;
using IsoECS.Components.GamePlay;

namespace IsoECS.Systems.Renderers
{
    public class FoundationOverlaySystem : RenderSystem
    {
        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            List<Entity> drawables = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); });
            PositionComponent cameraPosition = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

            // The idea is to draw as normal with the exception of the foundation layer
            // any non-roads on the foundation should instead draw a generic foundation sprite
            // covering their foundation and block the drawing of foregrounds drawing
            // on those locations

            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // TODO: render any background
            // TODO: this should be cached and only updated when drawbles are added, removed or changed
            _background.Clear();
            _foundation.Clear();
            _foreground.Clear();
            _text.Clear();

            // TODO: separate the drawables out into their layers?
            // Draw order (back to front):
            // 1. Background
            // 2. Foundation
            // 3. Foreground
            // 4. Text
            foreach (Entity e in drawables)
            {
                AddToDrawables(_background, e.Get<DrawableComponent>().Get("Background"), e);

                if (e.HasComponent<RoadComponent>())
                {
                    AddToDrawables(_foundation, e.Get<DrawableComponent>().Get("Foundation"), e);
                }
                else if(e.HasComponent<FoundationComponent>())
                {
                    // draw in the custom foundation sprite
                }

                // only draw the upper layers
                if (!e.HasComponent<FoundationComponent>() || e.HasComponent<RoadComponent>())
                {
                    AddToDrawables(_foreground, e.Get<DrawableComponent>().Get("Foreground"), e);
                    AddToDrawables(_text, e.Get<DrawableComponent>().Get("Text"), e);
                }
            }
        }
    }
}
