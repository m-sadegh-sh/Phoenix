namespace Phoenix.Infrastructure {
    using System;
    using System.IO;
    using System.Text;

    public static class Logger {
        public static void Write(Exception exception) {
            var message = new StringBuilder();

            do {
                message.AppendLine();
                message.AppendLine();
                message.AppendLine("Logged on: " + DateTime.Now);
                message.AppendLine("Message: " + exception.Message);
                message.AppendLine("StackTrace: " + exception.StackTrace);
                message.AppendLine("TargetSite: " + exception.TargetSite);
                message.AppendLine("Source: " + exception.Source);

                exception = exception.InnerException;
            } while (exception != null);

            using (
                var fs = File.Open(AppDomain.CurrentDomain.BaseDirectory + "_exceptions.log.phoenix", FileMode.Append,
                                   FileAccess.Write)) {
                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}