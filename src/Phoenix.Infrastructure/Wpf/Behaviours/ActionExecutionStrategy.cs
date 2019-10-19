namespace Phoenix.Infrastructure.Wpf.Behaviours {
    public class ActionExecutionStrategy : IExecutionStrategy {
        #region IExecutionStrategy Members
        public CommandBehaviorBinding Behavior { get; set; }

        public void Execute(object parameter) {
            Behavior.Action(parameter);
        }
        #endregion
    }
}