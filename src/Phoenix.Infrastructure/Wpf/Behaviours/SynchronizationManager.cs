namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Collections;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class SynchronizationManager {
        private readonly Selector _multiSelector;
        private TwoListSynchronizer _synchronizer;

        internal SynchronizationManager(Selector selector) {
            _multiSelector = selector;
        }

        public void StartSynchronizingList() {
            var list = MultiSelectorBehaviours.GetSynchronizedSelectedItems(_multiSelector);

            if (list != null) {
                _synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(_multiSelector), list);
                _synchronizer.StartSynchronizing();
            }
        }

        public void StopSynchronizing() {
            _synchronizer.StopSynchronizing();
        }

        private static IList GetSelectedItemsCollection(Selector selector) {
            if (selector is MultiSelector)
                return (selector as MultiSelector).SelectedItems;
            if (selector is ListBox)
                return (selector as ListBox).SelectedItems;
            throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
        }
    }
}