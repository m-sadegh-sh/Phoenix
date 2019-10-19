namespace Phoenix.Infrastructure.Wpf.Behaviours {
    public interface IExecutionStrategy {
        CommandBehaviorBinding Behavior { get; set; }

        void Execute(object parameter);
    }
}