using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class InventoryRenderer : DataRenderer<Inventory, Table>
    {
        public InventoryRenderer(Inventory data, Manager manager)
            : base(data, manager)
        {
            Control = new Table(Manager)
            {
                Anchor = Anchors.All,
                RowHeight = 24
            };
            Control.Init();
        }

        public override Table GetControl(Control parent)
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

                    string name = GameData.Instance.GetItem(item.Item).Name;
                    Control.AddAt(0, Control.RowsCount - 1, name);
                    Control.AddAt(1, Control.RowsCount - 1, item.Amount.ToString());

                    string status = (item.Output) ? "Available" : (item.Input) ? "Reserved" : "";

                    Control.AddAt(2, Control.RowsCount - 1, status);
                }
            }
            return base.GetControl(parent);
        }
    }
}
