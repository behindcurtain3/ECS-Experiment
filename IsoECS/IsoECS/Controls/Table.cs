using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TomShane.Neoforce.Controls
{
    public class Table : Control
    {
        private List<TableColumnHeader> columnHeaders = new List<TableColumnHeader>();
        private List<TableRow> rows = new List<TableRow>();
        private TableRow headerRow;
        private int columnHeaderHeight = 24;
        private int rowHeight = 22;
        private bool autoSizeColumns = true;

        public bool AutoSizeColumns 
        {
            get { return autoSizeColumns; }
            set
            {
                if (autoSizeColumns != value)
                {
                    SetAutoSizeColumns(value);
                }
            }
        }

        public int ColumnsCount { get { return columnHeaders.Count; } }
        public TableColumnHeader[] Columns
        {
            get { return columnHeaders.ToArray(); }
        }
        public int RowsCount { get { return rows.Count; } }
        public TableRow[] Rows
        {
            get { return rows.ToArray(); }
        }

        public int RowHeight
        {
            get { return rowHeight; }
            set { rowHeight = value; }
        }

        public int HeaderHeight
        {
            get { return columnHeaderHeight; }
            set { columnHeaderHeight = value; }
        }

        public Table(Manager manager)
            : base(manager)
        {
            AutoSizeColumns = true;
            ClientMargins = new Margins()
            {
                Left = 1,
                Top = 1,
                Right = 1,
                Bottom = 2
            };

            headerRow = new TableRow(Manager, 0)
            {
                Left = ClientMargins.Left,
                Top = ClientMargins.Top,
                Width = ClientWidth - ClientMargins.Horizontal,
                Height = columnHeaderHeight,
                Header = true
            };
            headerRow.Init();
            Add(headerRow);
        }

        public override void Init()
        {
            base.Init();

            Resize += new ResizeEventHandler(Table_Resize);
        }

        public void SetTableSize(int columns, int rows, string[] columnNames = null)
        {
            if (columns <= 0 || rows <= 0)
                throw new ArgumentOutOfRangeException();

            // clear the table
            Clear();

            // add the new columns
            for (int c = 0; c < columns; c++)
            {
                string columnName = (columnNames == null || c >= columnNames.Length) 
                                        ? string.Format("Column #{0}", columnHeaders.Count + 1) 
                                        : columnNames[c];

                AddColumn(columnName);
            }

            // add the new rows
            AddRows(rows);
        }

        public void AddColumn(string name, int width = 96)
        {
            TableColumnHeader header = new TableColumnHeader(Manager)
            {
                Text = name,
                Height = columnHeaderHeight,
                Width = width,
                Resizable = !AutoSizeColumns
            };
            header.Skin = new SkinControl(Manager.Skin.Controls["Table.Header"]);
            if(header.Resizable)
                header.Resize += new ResizeEventHandler(Column_Resized);
            header.Init();

            columnHeaders.Add(header);

            ColumnsChanged();
            PositionColumns();

            Add(header);
        }

        public TableRow AddRow()
        {
            TableRow row = new TableRow(Manager, columnHeaders.Count)
            {
                Height = rowHeight,
                Alternate = (rows.Count % 2 != 0)
            };
            row.Init();
            row.ItemAdded += new TableRow.TableEventHandler(Row_ItemAdded);
            rows.Add(row);
            Add(row);

            PositionColumns();

            return row;
        }

        public void AddColumns(string[] names)
        {
            foreach (string name in names)
                AddColumn(name);
        }

        public void AddRows(int count)
        {
            while (count > 0)
            {
                AddRow();
                count--;
            }
        }

        public void Clear()
        {
            foreach (TableColumnHeader header in columnHeaders)
                Remove(header);

            foreach (TableRow row in rows)
                Remove(row);
        }

        public void AddAt(int column, int row, Control c, bool fitted = false)
        {
            if (row >= RowsCount || row < 0)
                throw new IndexOutOfRangeException("The requested row is out of range");

            if (column >= ColumnsCount || column < 0)
                throw new IndexOutOfRangeException("The requested column is out of range");

            rows[row].AddToRowAt(column, c, fitted);

            PositionColumns();
        }

        public void AddAt(int column, int row, string text)
        {
            Label lbl = new Label(Manager)
            {
                Text = text,
                Alignment = Alignment.MiddleCenter
            };

            AddAt(column, row, lbl, true);
        }

        private void Column_Resized(object sender, ResizeEventArgs e)
        {
            // reposition the columns
            if(sender != null)
                PositionColumns();
        }

        private void PositionColumns()
        {
            if(columnHeaders.Count == 0)
                return;

            for (int i = 0; i < columnHeaders.Count; i++)
            {
                columnHeaders[i].Top = ClientMargins.Top;
                if (AutoSizeColumns)
                {
                    columnHeaders[i].Width = (ClientWidth - ClientMargins.Horizontal) / columnHeaders.Count;
                }

                if (i == 0)
                {
                    // first element is positioned relative to parent
                    columnHeaders[i].Left = ClientMargins.Left;
                }
                else
                {
                    // all other elements position to the right of the previous one
                    columnHeaders[i].Left = columnHeaders[i - 1].Left + columnHeaders[i - 1].Width;
                }
            }

            // make sure the last column is sized to fill in the width properly
            // do this because the integer division rounding sometimes leads to
            // the last column being slightly off
            if (AutoSizeColumns && columnHeaders.Count > 1)
            {
                int desiredTotalWidth = ClientWidth - ClientMargins.Horizontal;
                int remainingWidth = desiredTotalWidth - columnHeaders[columnHeaders.Count - 1].Left;

                columnHeaders[columnHeaders.Count - 1].Width = remainingWidth;
            }

            for (int i = 0; i < RowsCount; i++)
            {
                rows[i].Left = ClientMargins.Left;
                rows[i].Width = ClientWidth - ClientMargins.Horizontal;
                rows[i].Top = i * rows[i].Height + columnHeaderHeight + ClientMargins.Top;

                // update the columns on each row
                for (int j = 0; j < ColumnsCount; j++)
                {
                    int top = rows[i].Top + rows[i].ClientMargins.Top;
                    int left = columnHeaders[j].Left;
                    int width = columnHeaders[j].Width;

                    if (rows[i][j] != null)
                    {
                        rows[i][j].Top = (rows[i].ClientHeight / 2) - (rows[i][j].Height / 2);
                        if (rows[i].Fitted[j])
                        {
                            rows[i][j].Left = left;
                            rows[i][j].Width = width;
                        }
                        else
                        {
                            rows[i][j].Left = left + (width / 2) - (rows[i][j].Width / 2);
                        }
                    }
                }
            }

            // update header row
            headerRow.Left = ClientMargins.Left;
            headerRow.Width = ClientWidth - ClientMargins.Horizontal;
            headerRow.Top = ClientMargins.Top;
        }

        private void SetAutoSizeColumns(bool value)
        {
            autoSizeColumns = value;

            foreach(TableColumnHeader header in columnHeaders)
            {
                header.Resizable = !AutoSizeColumns;

                if (header.Resizable)
                    header.Resize += new ResizeEventHandler(Column_Resized);
                else
                    header.Resize -= Column_Resized;
            }
        }

        private void ColumnsChanged()
        {
            foreach (TableRow row in rows)
                row.SetColumns(columnHeaders.Count);

            headerRow.SetColumns(columnHeaders.Count);
        }

        private void Table_Resize(object sender, ResizeEventArgs e)
        {
            PositionColumns();
        }

        private void Row_ItemAdded(object sender, EventArgs e)
        {
            PositionColumns();
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);

            PositionColumns();
        }
    }
}
