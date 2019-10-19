namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System.Windows;

    public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding> {
        public DependencyObject Owner { get; set; }
    }
}