namespace Phoenix.Infrastructure.Wpf.Behaviours {
    using System;

    public class CommandExecutionStrategy : IExecutionStrategy {
        #region IExecutionStrategy Members
        public CommandBehaviorBinding Behavior { get; set; }

        public void Execute(object parameter) {
            if (Behavior == null)
                throw new InvalidOperationException("Behavior property cannot be null when executing a strategy");

            if (Behavior.Command.CanExecute(Behavior.CommandParameter))
                Behavior.Command.Execute(Behavior.CommandParameter);
        }
        #endregion
    }
}