namespace Phoenix.Infrastructure.Extensions {
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    public static class TreeViewExtensions {
        public static IList<TreeViewItem> ActualItems(this TreeView treeView) {
            if (treeView != null && treeView.Items.Count > 0) {
                var items = new List<TreeViewItem>();
                foreach (var item in treeView.Items.Cast<TreeViewItem>()) {
                    items.Add(item);
                    items.AddRange(IterateChilds(item));
                }
                return items;
            }
            return null;
        }

        private static IEnumerable<TreeViewItem> IterateChilds(ItemsControl parent) {
            var items = new List<TreeViewItem>();
            foreach (var item in parent.Items.Cast<TreeViewItem>()) {
                items.Add(item);
                items.AddRange(IterateChilds(item));
            }
            return items;
        }

        public static int ActualCount(this TreeView treeView) {
            if (treeView != null && treeView.Items.Count > 0) {
                var count = -1;
                foreach (var item in treeView.Items.Cast<TreeViewItem>()) {
                    count++;
                    count += CountChilds(item);
                }
                return count;
            }
            return -1;
        }

        private static int CountChilds(ItemsControl parent) {
            var count = 0;
            foreach (var item in parent.Items.Cast<TreeViewItem>()) {
                count++;
                count += CountChilds(item);
            }
            return count;
        }

        public static int ActualIndex(this TreeView treeView) {
            if (treeView != null && treeView.Items.Count > 0) {
                var index = -1;
                var selectedItem = treeView.SelectedItem as TreeViewItem;
                if (selectedItem == null)
                    return -1;
                foreach (var item in treeView.Items.Cast<TreeViewItem>()) {
                    index++;
                    bool founded;
                    if (item == selectedItem)
                        return index;
                    index += IndexChilds(item, selectedItem, out founded);
                    if (founded)
                        return index;
                }
                return index;
            }
            return -1;
        }

        private static int IndexChilds(ItemsControl parent, TreeViewItem selectedItem, out bool founded) {
            var index = 0;
            foreach (var item in parent.Items.Cast<TreeViewItem>()) {
                index++;
                if (item == selectedItem) {
                    founded = true;
                    return index;
                }
                index += IndexChilds(item, selectedItem, out founded);
                if (founded)
                    return index;
            }
            founded = false;
            return index;
        }
    }
}