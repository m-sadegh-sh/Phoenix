namespace Phoenix.WPF.ChildWindows {
    using System;
    using System.Drawing;
    using System.Media;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using Phoenix.WPF.CustomControls;

    public partial class MessageWindow : WindowBase {
        public MessageWindow(string message, MessageBoxButton buttons, MessageBoxImage image)
            : base(false, false, false) {
            InitializeComponent();
            tbMessage.Text = message;
            switch (buttons) {
                case MessageBoxButton.OKCancel:
                    btnOK.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnOK.IsDefault = true;
                    btnCancel.IsCancel = true;
                    break;
                case MessageBoxButton.YesNo:
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    btnYes.IsDefault = true;
                    btnNo.IsCancel = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnYes.IsDefault = true;
                    btnCancel.IsCancel = true;
                    break;
                case MessageBoxButton.OK:
                    btnOK.Visibility = Visibility.Visible;
                    btnOK.IsDefault = btnOK.IsCancel = true;
                    break;
            }
            Icon icon = null;
            switch (image) {
                case MessageBoxImage.Asterisk:
                    icon = SystemIcons.Asterisk;
                    SystemSounds.Asterisk.Play();
                    break;
                case MessageBoxImage.Hand:
                    icon = SystemIcons.Error;
                    SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Exclamation:
                    icon = SystemIcons.Exclamation;
                    SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxImage.Question:
                    icon = SystemIcons.Question;
                    SystemSounds.Question.Play();
                    break;
            }
            if (icon != null) {
                var bitmap = icon.ToBitmap();
                imgIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                                                       BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public MessageBoxResult MessageWindowResult { get; private set; }

        private void BtnCancelClick(object sender, RoutedEventArgs e) {
            MessageWindowResult = MessageBoxResult.Cancel;
            Close();
        }

        private void BtnNoClick(object sender, RoutedEventArgs e) {
            MessageWindowResult = MessageBoxResult.No;
            Close();
        }

        private void BtnYesClick(object sender, RoutedEventArgs e) {
            MessageWindowResult = MessageBoxResult.Yes;
            Close();
        }

        private void BtnOKClick(object sender, RoutedEventArgs e) {
            MessageWindowResult = MessageBoxResult.OK;
            Close();
        }
    }
}