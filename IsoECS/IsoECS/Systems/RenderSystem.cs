using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin();

            // TODO: render any background
            // TODO: render the map
            // TODO: render any sprites
            // sort the drawables by layer
            drawables.Sort(delegate(Entity a, Entity b)
            {
                return b.Get<DrawableComponent>().Layer.CompareTo(a.Get<DrawableComponent>().Layer);
            });
            foreach (Entity e in drawables)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();

                // if this entity is not visible continue to next
                if (!drawable.Visible) continue;

                if(drawable.Destination.Equals(Rectangle.Empty))
                    spriteBatch.Draw(drawable.Texture, e.Get<PositionComponent>().Position, drawable.Source, drawable.Color);
                else
                    spriteBatch.Draw(drawable.Texture, drawable.Destination, drawable.Source, drawable.Color);
            }

            // Render the text entities
            string text;
            Vector2 position;
            Color color;
            
            foreach (Entity e in drawableText)
            {
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
