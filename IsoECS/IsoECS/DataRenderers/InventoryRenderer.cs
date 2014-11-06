using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.GamePlay;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class InventoryRenderer : DataRenderer<Inventory, Table>
    {
        public InventoryRenderer(Inventory data, GameWorld world)
            : base(data, world)
        {
            Control = new Table(Manager)
            {
                Anchor = Anchors.All,
                RowHeight = 24
            };
            Control.Init();

            Data.AmountChanged += new Inventory.InventoryEventHandler(Data_AmountChanged);
        }

        public override Table GetControl(Control parent)
        {
            UpdateEntireTable();
            
            return base.GetControl(parent);
        }

        public override void Shutdown()
        {
            Data.AmountChanged -= Data_AmountChanged;

            base.Shutdown();
        }

        private void Data_AmountChanged(Inventory sender, InventoryEventArgs e)
        {
            UpdateEntireTable();
        }

        private void UpdateEntireTable()
        {
            Control.Clear();

            if (Data.Items.Count == 0)
            {
                Control.AddColumn("Inventory Empty");
            }
            else
            {
                // setup the columns
                Control.AddColumns(new string[] { "Item", "Amount", "Status" });
                Control.Columns[2].ToolTip = new ToolTip(Manager)
                {
                    Text = "Is the item available for use by other\nbuildings or reserved for this building?"
                };

                foreach (InventoryData item in Data.Items.Values)
                {
                    TableRow currentRow = Control.AddRow();
                    currentRow.Tag = item;

                    string name = ((Item)World.Prototypes[item.Item]).Name;
                    Control.AddAt(0, Control.RowsCount - 1, name);
                    Control.AddAt(1, Control.RowsCount - 1, item.Amount.ToString());

                    string status = (item.Output) ? "Available" : (item.Input) ? "Reserved" : "";

                    Control.AddAt(2, Control.RowsCount - 1, status);
                }
            }
        }
    }
}
