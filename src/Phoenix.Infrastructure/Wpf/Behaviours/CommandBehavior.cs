namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class CommandBehavior {
        private static readonly DependencyProperty _behaviorProperty = DependencyProperty.RegisterAttached("Behavior",
                                                                                                           typeof (
                                                                                                               CommandBehaviorBinding
                                                                                                               ),
                                                                                                           typeof (
                                                                                                               CommandBehavior
                                                                                                               ),
                                                                                                           new FrameworkPropertyMetadata
                                                                                                               ((
                                                                                                                CommandBehaviorBinding
                                                                                                                ) null));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command",
                                                                                                        typeof (ICommand
                                                                                                            ),
                                                                                                        typeof (
                                                                                                            CommandBehavior
                                                                                                            ),
                                                                                                        new FrameworkPropertyMetadata
                                                                                                            (null,
                                                                                                             OnCommandChanged));

        public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached("Action",
                                                                                                       typeof (
                                                                                                           Action
                                                                                                           <object>),
                                                                                                       typeof (
                                                                                                           CommandBehavior
                                                                                                           ),
                                                                                                       new FrameworkPropertyMetadata
                                                                                                           (null,
                                                                                                            OnActionChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (CommandBehavior),
                                                new FrameworkPropertyMetadata(null, OnCommandParameterChanged));

        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event",
                                                                                                      typeof (string),
                                                                                                      typeof (
                                                                                                          CommandBehavior
                                                                                                          ),
                                                                                                      new FrameworkPropertyMetadata
                                                                                                          (String.Empty,
                                                                                                           OnEventChanged));

        private static CommandBehaviorBinding GetBehavior(DependencyObject d) {
            return (CommandBehaviorBinding) d.GetValue(_behaviorProperty);
        }

        private static void SetBehavior(DependencyObject d, CommandBehaviorBinding value) {
            d.SetValue(_behaviorProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d) {
            return (ICommand) d.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject d, ICommand value) {
            d.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var binding = FetchOrCreateBinding(d);
            binding.Command = (ICommand) e.NewValue;
        }

        public static Action<object> GetAction(DependencyObject d) {
            return (Action<object>) d.GetValue(ActionProperty);
        }

        public static void SetAction(DependencyObject d, Action<object> value) {
            d.SetValue(ActionProperty, value);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var binding = FetchOrCreateBinding(d);
            binding.Action = (Action<object>) e.NewValue;
        }

        public static object GetCommandParameter(DependencyObject d) {
            return d.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject d, object value) {
            d.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var binding = FetchOrCreateBinding(d);
            binding.CommandParameter = e.NewValue;
        }

        public static string GetEvent(DependencyObject d) {
            return (string) d.GetValue(EventProperty);
        }

        public static void SetEvent(DependencyObject d, string value) {
            d.SetValue(EventProperty, value);
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var binding = FetchOrCreateBinding(d);
            if (binding.Event != null && binding.Owner != null)
                binding.Dispose();
            binding.BindEvent(d, e.NewValue.ToString());
        }

        private static CommandBehaviorBinding FetchOrCreateBinding(DependencyObject d) {
            var binding = GetBehavior(d);
            if (binding == null) {
                binding = new CommandBehaviorBinding();
                SetBehavior(d, binding);
            }
            return binding;
        }
    }
}