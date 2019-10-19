namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class BehaviorBinding : Freezable {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
                                                                                                typeof (ICommand),
                                                                                                typeof (BehaviorBinding),
                                                                                                new FrameworkPropertyMetadata
                                                                                                    (null,
                                                                                                     OnCommandChanged));

        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action",
                                                                                               typeof (Action<object>),
                                                                                               typeof (BehaviorBinding),
                                                                                               new FrameworkPropertyMetadata
                                                                                                   (null,
                                                                                                    OnActionChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (BehaviorBinding),
                                        new FrameworkPropertyMetadata(null, OnCommandParameterChanged));

        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof (string),
                                                                                              typeof (BehaviorBinding),
                                                                                              new FrameworkPropertyMetadata
                                                                                                  (null, OnEventChanged));

        private CommandBehaviorBinding _behavior;
        private DependencyObject _owner;

        internal CommandBehaviorBinding Behavior {
            get { return _behavior ?? (_behavior = new CommandBehaviorBinding()); }
        }

        public DependencyObject Owner {
            get { return _owner; }
            set {
                _owner = value;
                ResetEventBinding();
            }
        }

        public ICommand Command {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Action<object> Action {
            get { return (Action<object>) GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public object CommandParameter {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public string Event {
            get { return (string) GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((BehaviorBinding) d).OnCommandChanged(e);
        }

        protected virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e) {
            Behavior.Command = Command;
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((BehaviorBinding) d).OnActionChanged(e);
        }

        protected virtual void OnActionChanged(DependencyPropertyChangedEventArgs e) {
            Behavior.Action = Action;
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((BehaviorBinding) d).OnCommandParameterChanged(e);
        }

        protected virtual void OnCommandParameterChanged(DependencyPropertyChangedEventArgs e) {
            Behavior.CommandParameter = CommandParameter;
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((BehaviorBinding) d).OnEventChanged(e);
        }

        protected virtual void OnEventChanged(DependencyPropertyChangedEventArgs e) {
            ResetEventBinding();
        }

        private static void OwnerReset(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((BehaviorBinding) d).ResetEventBinding();
        }

        private void ResetEventBinding() {
            if (Owner != null) {
                if (Behavior.Event != null && Behavior.Owner != null)
                    Behavior.Dispose();

                Behavior.BindEvent(Owner, Event);
            }
        }

        protected override Freezable CreateInstanceCore() {
            return null;
        }
    }
}