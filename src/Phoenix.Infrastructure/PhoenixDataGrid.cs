namespace Phoenix.Infrastructure {
    using System.Windows.Controls;

    public class PhoenixDataGrid : DataGrid {
        public PhoenixDataGrid() {
            AutoGenerateColumns = false;
            IsReadOnly = true;
        }
    }
}