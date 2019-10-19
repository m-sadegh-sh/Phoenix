namespace Phoenix.Infrastructure.Extensions {
    using System.Collections.Generic;
    using System.Windows.Controls;

    public static class DataGridExtensions {
        public static void Fill(this DataGrid dataGrid, int index) {
            if (dataGrid != null) {
                var columns = dataGrid.Columns as IList<DataGridColumn>;
                if (columns != null) {
                    for (var i = 0; i < columns.Count; i++) {
                        if (i == index) {
                            columns[i].Width = new DataGridLength(100, DataGridLengthUnitType.Star);
                            break;
                        }
                    }
                }
            }
        }

        public static void FillFirst(this DataGrid dataGrid) {
            dataGrid.Fill(0);
        }

        public static void FillLast(this DataGrid dataGrid) {
            dataGrid.Fill(dataGrid.Columns.Count - 1);
        }

        public static void FocusAndSelect(this DataGrid dataGrid) {
            if (dataGrid != null) {
                if (!dataGrid.IsFocused)
                    dataGrid.Focus();
                if (dataGrid.Items.Count > 0)
                    dataGrid.SelectedIndex = 0;
            }
        }
    }
}