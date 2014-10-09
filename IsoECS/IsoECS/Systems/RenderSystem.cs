using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IsoECS.DataStructures;

namespace IsoECS.Systems
{
    public class RenderSystem : IRenderSystem
    {
        public GraphicsDevice Graphics { get; set; }
        public Color ClearColor { get; set; }

        public void Draw(List<Entity> entities, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Get the list of drawable text entities from the main list
            List<Entity> drawables = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); });
            List<Entity> drawableText = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableTextComponent>() && e.HasComponent<PositionComponent>(); });
            PositionComponent cameraPosition = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin();

            // TODO: render any background
            // sort the drawables by layer
            drawables.Sort(delegate(Entity a, Entity b)
            {
                return b.Get<DrawableComponent>().Layer.CompareTo(a.Get<DrawableComponent>().Layer);
            });
            foreach (Entity e in drawables)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();

                foreach (DrawableSprite sprite in drawable.Sprites)
                {
                    // if this entity is not visible continue to next
                    if (!sprite.Visible) continue;

                    // update the destination on the drawable
                    Rectangle source = Textures.Instance.GetSource(sprite.SpriteSheet, sprite.ID);
                    Rectangle destination;

                    if (sprite.Static)
                        destination = new Rectangle(0, 0, Graphics.Viewport.Width, Graphics.Viewport.Height);
                    else
                        destination = new Rectangle(
                                                (int)(e.Get<PositionComponent>().X - cameraPosition.X),
                                                (int)(e.Get<PositionComponent>().Y - cameraPosition.Y),
                                                source.Width,
                                                source.Height);

                    if (Graphics.Viewport.Bounds.Contains(destination) || Graphics.Viewport.Bounds.Intersects(destination))
                        spriteBatch.Draw(Textures.Instance.Get(sprite.SpriteSheet), destination, source, sprite.Color, sprite.Rotation, Textures.Instance.GetOrigin(sprite.SpriteSheet, sprite.ID), sprite.Effects, 0);
                }
            }

            // Render the text entities
            // TODO: update this
            string text;
            Vector2 position;
            Color color;
            
            foreach (Entity e in drawableText)
            {
                if (!e.Get<DrawableTextComponent>().Visible)
                    continue;

                // get the needed data
                text = e.Get<DrawableTextComponent>().Text;
                position = e.Get<PositionComponent>().Position;
                color = e.Get<DrawableTextComponent>().Color;
                
                // do the draw call
                if(!string.IsNullOrWhiteSpace(text))
                    spriteBatch.DrawString(spriteFont, text, position, color);
            }

            // end the scene
            spriteBatch.End();
        }
    }
}
