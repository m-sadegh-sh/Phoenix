namespace Phoenix.Infrastructure.Extensions {
    using System.Windows.Controls;

    public static class DataGridColumnExtensions {
        public static void AutoSize(this DataGridColumn dataGridColumn) {
            if (dataGridColumn != null)
                dataGridColumn.Width = DataGridLength.Auto;
        }
    }
}