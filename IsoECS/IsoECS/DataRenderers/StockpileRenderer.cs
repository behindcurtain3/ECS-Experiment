using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.GamePlay;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class StockpileRenderer : DataRenderer<StockpileComponent, Table>
    {
        public StockpileRenderer(StockpileComponent stockpile, GameWorld world)
            : base(stockpile, world)
        {
        }

        public override Table GetControl(Control parent)
        {
            // Setup the stockpile table
            Control = new Table(Manager)
            {
                Name = "StockpileTable",
                Anchor = Anchors.All,
                RowHeight = 24
            };
            Control.Init();

            // get a list of all items in the game
            List<Item> items = World.Prototypes.GetAll<Item>();
            Control.AddColumns(new string[] { "Item", "Amount", "Minimum", "Maximum" });

            foreach (Item item in items)
            {
                TableRow currentRow = Control.AddRow();
                currentRow.Tag = item;

                Button btn = new Button(Manager)
                {
                    Name = string.Format("{0}-toggle", item.PrototypeID),
                    Text = item.Name,
                    Height = 20,
                    Width = 100,
                    CanFocus = false,
                    Tag = item
                };
                btn.Init();
                // Add event handler to listen for toggles on the button
                btn.Click += new EventHandler(ToggleStockPileItem);

                currentRow.AddToRowAt(0, btn, true);
                Control.AddAt(1, Control.RowsCount - 1, Data.Amount(item.PrototypeID).ToString());

                SetupMinimumMaximum(item, currentRow);

                Data.Get(item.PrototypeID).OnAmountChanged += new StockPileData.StockpileEventHandler(StockpileRenderer_OnAmountChanged);
            }
            
            return base.GetControl(parent);
        }

        public override void Shutdown()
        {
            Manager.Remove(Control);
        }

        private void ToggleStockPileItem(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if(btn == null)
                return;

            TableRow row = (TableRow)btn.Parent;
            Item item = (Item)btn.Tag;
            
            Data.ToggleAccepting(item.PrototypeID);

            SetupMinimumMaximum(item, row);
        }

        private void SetupMinimumMaximum(Item item, TableRow row)
        {
            // the spin boxes are on
            if (Data.IsAccepting(item.PrototypeID))
            {
                SpinBox spinner = GetSpinner(item, true);
                row.AddToRowAt(2, spinner);

                spinner = GetSpinner(item, false);
                row.AddToRowAt(3, spinner);
            }
            // the spinboxes are off
            else
            {
                row.AddToRowAt(2, new Label(Control.Manager)
                {
                    Text = "---",
                    Alignment = Alignment.MiddleCenter
                }, true);

                row.AddToRowAt(3, new Label(Control.Manager)
                {
                    Text = "---",
                    Alignment = Alignment.MiddleCenter
                }, true);
            }
        }

        private SpinBox GetSpinner(Item item, bool minimum = true)
        {
            SpinBox spinner = new SpinBox(Control.Manager, SpinBoxMode.Range)
            {
                Name = string.Format("{0}-{1}", item.PrototypeID, (minimum) ? "minimum" : "maximum"),
                Value = (minimum) ? Data.Minimum(item.PrototypeID) : Data.Maximum(item.PrototypeID),
                Width = 75,
                Rounding = 0,
                Minimum = 0,
                Maximum = 5000,
                Step = 1,
                DisplayFormat = "f",
                Tag = item
            };
            spinner.Init();
            // spinner is bugged if Text is set above so do it here
            spinner.Text = (minimum) ? Data.Minimum(item.PrototypeID).ToString() : Data.Maximum(item.PrototypeID).ToString();

            if(minimum)
                spinner.TextChanged += new EventHandler(Spinner_MinimumTextChanged);
            else
                spinner.TextChanged += new EventHandler(Spinner_MaximumTextChanged);

            return spinner;
        }

        private void Spinner_MinimumTextChanged(object sender, EventArgs e)
        {
            SpinBox spinner = (SpinBox)sender;
            Item item = (Item)spinner.Tag;

            Data.SetMinimum(item.PrototypeID, (int)spinner.Value);
        }

        private void Spinner_MaximumTextChanged(object sender, EventArgs e)
        {
            SpinBox spinner = (SpinBox)sender;
            Item item = (Item)spinner.Tag;

            Data.SetMaximum(item.PrototypeID, (int)spinner.Value);
        }

        private void StockpileRenderer_OnAmountChanged(StockPileData sender)
        {
            // go through each row until the item matches, then update the text
            foreach (TableRow row in Control.Rows)
            {
                Item item = (Item)row.Tag;

                if (item.PrototypeID.Equals(sender.Item))
                {
                    row.AddToRowAt(1, new Label(Control.Manager)
                    {
                        Text = sender.Amount.ToString(),
                        Alignment = Alignment.MiddleCenter
                    }, true);
                }
            }
        }
    }
}
