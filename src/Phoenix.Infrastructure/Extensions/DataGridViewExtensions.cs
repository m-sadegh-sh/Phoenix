namespace LMS.Infrastructure.Extensions {
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public static class DataGridViewExtensions {
        public static void HideColumns(this DataGridView dataGridView, IEnumerable<string> excepts = null) {
            if(dataGridView != null) {
                var columns = dataGridView.Columns.Cast<DataGridViewColumn>();
                if(columns != null) {
                    if(excepts != null)
                        columns = columns.Where(col => !excepts.Contains(col.Name));
                    foreach(var col in columns.Where(col => !col.Name.StartsWith("String") && col is DataGridViewTextBoxColumn))
                        col.Visible = false;
                }
            }
        }

        public static void AutoSize(this DataGridView dataGridView, IEnumerable<string> fillables) {
            if(dataGridView != null) {
                dataGridView.SuspendLayout();
                var columns = dataGridView.Columns.Cast<DataGridViewColumn>().Where(col => col.Visible);
                if(columns != null) {
                    foreach(var col in columns)
                        col.AutoSize();
                    var sum = columns.Sum(col => col.Width);
                    if(sum < dataGridView.Width) {
                        foreach(var col in columns.Where(col => fillables.Contains(col.Name)))
                            col.AutoSize(DataGridViewAutoSizeColumnMode.Fill);
                    }
                }
                dataGridView.ResumeLayout(true);
            }
        }

        public static void Configure(this DataGridView dataGridView) {
            if(dataGridView != null) {
                dataGridView.SuspendLayout();
                dataGridView.Visible = false;
                dataGridView.ShowCellErrors = dataGridView.ShowEditingIcon = dataGridView.ShowRowErrors = false;
                dataGridView.ShowCellToolTips = true;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                dataGridView.BackgroundColor = SystemColors.Control;
                dataGridView.BorderStyle = BorderStyle.Fixed3D;
                dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.ReadOnly = true;
                dataGridView.SetAsReadOnly();
                dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
                dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.StandardTab = true;
                dataGridView.ResumeLayout(true);
            }
        }

        public static void SetAsReadOnly(this DataGridView dataGridView, int lowerBound = 0) {
            SetAsReadOnly(dataGridView, lowerBound, dataGridView.Columns.Count);
        }

        public static void SetAsReadOnly(this DataGridView dataGridView, int lowerBound, int upperBound) {
            if(dataGridView != null) {
                var readOnlyColumns = dataGridView.Columns.Cast<DataGridViewColumn>().Where(col => col.Visible && col.Index >= lowerBound && col.Index <= upperBound).ToList();
                if(readOnlyColumns.Count() > 0) {
                    foreach(var col in readOnlyColumns)
                        col.ReadOnly = true;
                }
                var normalColumns = dataGridView.Columns.Cast<DataGridViewColumn>().Except(readOnlyColumns).ToList();
                if(normalColumns.Count() > 0) {
                    foreach(var col in normalColumns) {
                        try {
                            col.ReadOnly = false;
                        } catch {}
                    }
                }
            }
        }

        public static bool NumberOfSelectedRowsIsGreaterThan(this DataGridView dataGridView, int unitialCount = 1) {
            if(dataGridView != null) {
                var count = 0;
                var rows = dataGridView.Rows.Cast<DataGridViewRow>().Where(row => row.Visible);
                if(rows != null)
                    count = rows.Count(row => row.Selected);
                return count > unitialCount;
            }
            return false;
        }
    }
}