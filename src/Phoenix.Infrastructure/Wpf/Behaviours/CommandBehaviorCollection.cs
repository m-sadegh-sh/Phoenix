namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Collections.Specialized;
    using System.Windows;

    public class CommandBehaviorCollection {
        private static readonly DependencyPropertyKey BehaviorsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("BehaviorsInternal", typeof (BehaviorBindingCollection),
                                                        typeof (CommandBehaviorCollection),
                                                        new FrameworkPropertyMetadata((BehaviorBindingCollection) null));

        public static readonly DependencyProperty BehaviorsProperty = BehaviorsPropertyKey.DependencyProperty;

        public static BehaviorBindingCollection GetBehaviors(DependencyObject d) {
            if (d == null)
                throw new InvalidOperationException("The dependency object trying to attach to is set to null");

            var collection = d.GetValue(BehaviorsProperty) as BehaviorBindingCollection;
            if (collection == null) {
                collection = new BehaviorBindingCollection {Owner = d};
                SetBehaviors(d, collection);
            }
            return collection;
        }

        private static void SetBehaviors(DependencyObject d, BehaviorBindingCollection value) {
            d.SetValue(BehaviorsPropertyKey, value);
            INotifyCollectionChanged collection = value;
            collection.CollectionChanged += CollectionChanged;
        }

        private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var sourceCollection = (BehaviorBindingCollection) sender;
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null) {
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null) {
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null) {
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;
                    }

                    if (e.OldItems != null) {
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null) {
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}