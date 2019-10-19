namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;

    public class CommandBehaviorBinding : IDisposable {
        private Action<object> _action;
        private ICommand _command;
        private bool _disposed;
        private IExecutionStrategy _strategy;

        public DependencyObject Owner { get; private set; }

        public string EventName { get; private set; }

        public EventInfo Event { get; private set; }

        public Delegate EventHandler { get; private set; }

        public object CommandParameter { get; set; }

        public ICommand Command {
            get { return _command; }
            set {
                _command = value;
                _strategy = new CommandExecutionStrategy {Behavior = this};
            }
        }

        public Action<object> Action {
            get { return _action; }
            set {
                _action = value;
                _strategy = new ActionExecutionStrategy {Behavior = this};
            }
        }

        #region IDisposable Members
        public void Dispose() {
            if (!_disposed) {
                Event.RemoveEventHandler(Owner, EventHandler);
                _disposed = true;
            }
        }
        #endregion

        public void BindEvent(DependencyObject owner, string eventName) {
            EventName = eventName;
            Owner = owner;
            Event = Owner.GetType().GetEvent(EventName, BindingFlags.Public | BindingFlags.Instance);
            if (Event == null)
                throw new InvalidOperationException(String.Format("Could not resolve event name {0}", EventName));

            EventHandler = EventHandlerGenerator.CreateDelegate(Event.EventHandlerType,
                                                                typeof (CommandBehaviorBinding).GetMethod("Execute",
                                                                                                          BindingFlags.
                                                                                                              Public |
                                                                                                          BindingFlags.
                                                                                                              Instance),
                                                                this);
            Event.AddEventHandler(Owner, EventHandler);
        }

        public void Execute() {
            _strategy.Execute(CommandParameter);
        }
    }
}