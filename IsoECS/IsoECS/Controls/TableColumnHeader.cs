using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TomShane.Neoforce.Controls
{
    public class TableColumnHeader : Label
    {
        #region //// Construstors //////

        ////////////////////////////////////////////////////////////////////////////       
        public TableColumnHeader(Manager manager): base(manager)
        {
            CanFocus = false;
            Passive = false;
            Resizable = true;
            ResizerSize = 3;
            ResizeEdge = Anchors.Right;
            Width = 96;
            Height = 20;
            MinimumWidth = 48;
            Alignment = Alignment.MiddleCenter;
        }
        ////////////////////////////////////////////////////////////////////////////    
    
        #endregion

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            // Draw the table column background
            SkinLayer l1 = Skin.Layers["Control"];
            Color cl = l1.States.Enabled.Color;

            //renderer.DrawLayer(l1, rect, cl, 0);
            base.DrawControl(renderer, rect, gameTime);
        }
    }
}
