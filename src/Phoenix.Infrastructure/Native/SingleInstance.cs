namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class SingleInstance<TApplication> where TApplication : Application, ISingleInstanceApp {
        private const string Delimiter = ":";
        private const string ChannelNameSuffix = "SingeInstanceIPCChannel";
        private const string RemoteServiceName = "SingleInstanceApplicationService";
        private const string IpcProtocol = "ipc://";
        private static Mutex _singleInstanceMutex;
        private static IpcServerChannel _channel;

        public static IList<string> CommandLineArgs { get; private set; }

        public static bool InitializeAsFirstInstance(string uniqueName) {
            CommandLineArgs = GetCommandLineArgs(uniqueName);

            var applicationIdentifier = uniqueName + Environment.UserName;

            var channelName = String.Concat(applicationIdentifier, Delimiter, ChannelNameSuffix);

            bool firstInstance;
            _singleInstanceMutex = new Mutex(true, applicationIdentifier, out firstInstance);
            if (firstInstance)
                CreateRemoteService(channelName);
            else
                SignalFirstInstance(channelName, CommandLineArgs);

            return firstInstance;
        }

        public static void Cleanup() {
            if (_singleInstanceMutex != null) {
                _singleInstanceMutex.Close();
                _singleInstanceMutex = null;
            }

            if (_channel != null) {
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
            }
        }

        private static IList<string> GetCommandLineArgs(string uniqueApplicationName) {
            string[] args = null;
            if (AppDomain.CurrentDomain.ActivationContext == null)
                args = Environment.GetCommandLineArgs();
            else {
                var appFolderPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                 uniqueApplicationName);

                var cmdLinePath = Path.Combine(appFolderPath, "cmdline.txt");
                if (File.Exists(cmdLinePath)) {
                    try {
                        using (TextReader reader = new StreamReader(cmdLinePath, Encoding.Unicode))
                            args = NativeMethods.CommandLineToArgvW(reader.ReadToEnd());

                        File.Delete(cmdLinePath);
                    } catch (IOException) {}
                }
            }

            if (args == null)
                args = new string[] {};

            return new List<string>(args);
        }

        private static void CreateRemoteService(string channelName) {
            var serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Dictionary<string, string>();

            props["name"] = channelName;
            props["portName"] = channelName;
            props["exclusiveAddressUse"] = "false";

            _channel = new IpcServerChannel(props, serverProvider);

            ChannelServices.RegisterChannel(_channel, true);

            var remoteService = new IpcRemoteService();
            RemotingServices.Marshal(remoteService, RemoteServiceName);
        }

        private static void SignalFirstInstance(string channelName, IList<string> args) {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            var remotingServiceUrl = IpcProtocol + channelName + "/" + RemoteServiceName;

            var firstInstanceRemoteServiceReference =
                (IpcRemoteService) RemotingServices.Connect(typeof (IpcRemoteService), remotingServiceUrl);

            if (firstInstanceRemoteServiceReference != null)
                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
        }

        private static object ActivateFirstInstanceCallback(object arg) {
            var args = arg as IList<string>;
            ActivateFirstInstance(args);
            return null;
        }

        private static void ActivateFirstInstance(IList<string> args) {
            if (Application.Current == null)
                return;

            ((TApplication) Application.Current).SignalExternalCommandLineArgs(args);
        }

        #region Nested type: IpcRemoteService
        private class IpcRemoteService : MarshalByRefObject {
            public void InvokeFirstInstance(IList<string> args) {
                if (Application.Current != null) {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                               new DispatcherOperationCallback(
                                                                   ActivateFirstInstanceCallback), args);
                }
            }

            public override object InitializeLifetimeService() {
                return null;
            }
        }
        #endregion
    }
}