using Syncfusion.GridHelperClasses;
using Syncfusion.Windows.Forms.Grid;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GridControlDemo
{
    public partial class Form1 : Form
    {
        private int ParentGridScrollBarValue;
        public Form1()
        {
            InitializeComponent();

            // Set up main grid
            gridControl1.RowCount = 30;
            gridControl1.ColCount = 6;

            for (int row = 1; row <= gridControl1.RowCount; row++)
            {
                for (int col = 1; col <= gridControl1.ColCount; col++)
                {
                    gridControl1[row, col].Text = $"{row}/{col}";
                }
            }

            RegisterCellModel.GridCellType(gridControl1, CustomCellTypes.GridinCell);

            GridControl embeddedGrid;

            this.gridControl1[3, 2].CellType = CustomCellTypes.GridinCell.ToString();
            this.gridControl1.CoveredRanges.Add(GridRangeInfo.Cells(3, 2, 7, 4));

            embeddedGrid = new CellEmbeddedGrid(this.gridControl1);
            embeddedGrid.BackColor = Color.LightBlue;
            embeddedGrid.RowCount = 10;
            embeddedGrid.ColCount = 4;
            embeddedGrid[1, 1].Text = "Grid";
            this.gridControl1[3, 2].Control = embeddedGrid;

            gridControl1.VScrollPixel = true;
            embeddedGrid.VScrollPixel = true;

            //Events subscription
            gridControl1.CellMouseHover += OnCellMouseHover;
            gridControl1.VScrollPixelPosChanging += OnVScrollPixelPosChanging;
            embeddedGrid.MouseHover += EmbeddedGrid_MouseHover;
        }

        //Events customization
        private void EmbeddedGrid_MouseHover(object sender, EventArgs e)
        {
            ParentGridScrollBarValue = gridControl1.VScrollBar.Value;
        }

        private void OnVScrollPixelPosChanging(object sender, GridScrollPositionChangingEventArgs e)
        {
            GridControl grid = sender as GridControl;
            Point clientPt = grid.PointToClient(Control.MousePosition);
            if (grid.PointToRowCol(clientPt, out int row, out int col, -1))
            {
                var covered = grid.CoveredRanges?.FindRange(row, col);
                if (covered != null && grid.CoveredRanges.Count > 0) { row = covered.Top; col = covered.Left; }
                GridControl target = grid[row, col].Control as GridControl;
                if (target != null && target.IsHandleCreated && target.Visible && target.VScroll)
                {
                    gridControl1.VScrollBar.Value = ParentGridScrollBarValue;
                    e.Cancel = true;
                }
            }
        }

        private void OnCellMouseHover(object sender, GridCellMouseEventArgs e)
        {
            GridControl grid = sender as GridControl;
            if (grid[e.RowIndex, e.ColIndex].Control is CellEmbeddedGrid)
                grid.CurrentCell.MoveTo(e.RowIndex, e.ColIndex);
        }
    }
}
