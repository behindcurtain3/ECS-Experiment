using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TomShane.Neoforce.Controls
{
    public class TableRow : Control
    {
        public delegate void TableEventHandler(object sender, EventArgs e);
        public event TableEventHandler ItemAdded;
        public event TableEventHandler ItemRemoved;

        private Control[] cells;

        public bool Alternate { get; set; }
        public bool Header { get; set; }
        public Control this[int index] { get { return cells[index]; } }

        public TableRow(Manager manager, int columns)
            : base(manager)
        {
            Passive = false;
            Resizable = false;
            Movable = false;
            Alternate = false;
            Header = false;
            StayOnBack = true;

            SetColumns(columns);
        }

        public override void Init()
        {
            base.Init();

            Skin = Manager.Skin.Controls["Table.Row"];
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            // Draw the table column background
            SkinLayer l1 = Skin.Layers["Control"];
            Color cl = l1.States.Enabled.Color;

            int index = Header ? 2 : Alternate ? 1 : 0;

            renderer.DrawLayer(l1, rect, cl, index);
        }

        public void AddToRow(Control c)
        {
            int index = 0;
            while (cells[index] == null && index <= cells.Length)
                index++;

            if(index < cells.Length)
                AddToRowAt(index, c);
        }

        public void AddToRowAt(int index, Control c)
        {
            OverwriteAt(index, c);
        }

        public void OverwriteAt(int index, Control c)
        {
            if (!ValidIndex(index))
                return;

            if (cells[index] != null)
            {
                Remove(cells[index]);

                if (ItemRemoved != null)
                    ItemRemoved.Invoke(this, new EventArgs());
            }

            cells[index] = c;
            Add(c);

            if (ItemAdded != null)
                ItemAdded.Invoke(this, new EventArgs());
        }

        public void SetColumns(int columns)
        {
            if (columns <= 0)
                return;

            Control[] currentCells = cells;
            cells = new Control[columns];

            for (int i = 0; i < cells.Length; i++)
            {
                if (currentCells != null && i < currentCells.Length)
                {
                    cells[i] = currentCells[i];
                }
            }

            // remove any cells that are no longer in a column
            if (currentCells != null && currentCells.Length > cells.Length)
            {
                for (int i = cells.Length; i < currentCells.Length; i++)
                {
                    if (currentCells[i] != null)
                        Remove(currentCells[i]);
                }
            }
        }

        private bool ValidIndex(int index)
        {
            return index > 0 && index < cells.Length;
        }
    }
}
