namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public static class MultiSelectorBehaviours {
        public static readonly DependencyProperty SynchronizedSelectedItems =
            DependencyProperty.RegisterAttached("SynchronizedSelectedItems", typeof (IList),
                                                typeof (MultiSelectorBehaviours),
                                                new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        private static readonly DependencyProperty _synchronizationManagerProperty =
            DependencyProperty.RegisterAttached("SynchronizationManager", typeof (SynchronizationManager),
                                                typeof (MultiSelectorBehaviours), new PropertyMetadata(null));

        public static IList GetSynchronizedSelectedItems(DependencyObject dependencyObject) {
            return (IList) dependencyObject.GetValue(SynchronizedSelectedItems);
        }

        public static void SetSynchronizedSelectedItems(DependencyObject dependencyObject, IList value) {
            dependencyObject.SetValue(SynchronizedSelectedItems, value);
        }

        private static SynchronizationManager GetSynchronizationManager(DependencyObject dependencyObject) {
            return (SynchronizationManager) dependencyObject.GetValue(_synchronizationManagerProperty);
        }

        private static void SetSynchronizationManager(DependencyObject dependencyObject, SynchronizationManager value) {
            dependencyObject.SetValue(_synchronizationManagerProperty, value);
        }

        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject,
                                                               DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != null) {
                var synchronizer = GetSynchronizationManager(dependencyObject);
                synchronizer.StopSynchronizing();

                SetSynchronizationManager(dependencyObject, null);
            }

            var list = e.NewValue as IList;
            var selector = dependencyObject as Selector;

            if (list != null && selector != null) {
                var synchronizer = GetSynchronizationManager(dependencyObject);
                if (synchronizer == null) {
                    synchronizer = new SynchronizationManager(selector);
                    SetSynchronizationManager(dependencyObject, synchronizer);
                }

                synchronizer.StartSynchronizingList();
            }
        }
    }
}