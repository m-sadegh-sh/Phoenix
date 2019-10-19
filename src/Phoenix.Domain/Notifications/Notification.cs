namespace Phoenix.Domain.Notifications {
    public class Notification {
        public string Title { get; set; }

        public NotifyType NotifyType { get; set; }

        public object OriginalObject { get; set; }
    }
}