using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Components.GamePlay;

namespace IsoECS.RenderSystems
{
    public class DesireabilityOverlayRenderer : FoundationOverlaySystem
    {
        private string spriteSheet = "overlay-marker";
        private string sourceBottom = "marker-bottom";
        private string sourceMiddle = "marker-middle";
        private string sourceTop = "marker-top";
        /*
        protected override void DrawOverlay(string layer, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Graphics.SpriteFont spriteFont, Components.PositionComponent camera, List<DrawData> layerData)
        {
            base.DrawOverlay(layer, spriteBatch, spriteFont, camera, layerData);

            if (layer.Equals("Foundation"))
            {

            }
        }
        */
    }
}
