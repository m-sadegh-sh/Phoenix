namespace Phoenix.Infrastructure {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Image = System.Windows.Controls.Image;

    public sealed class AnimatedImage : Image {
        public static readonly DependencyProperty AnimatedBitmapProperty = DependencyProperty.Register(
            "AnimatedBitmap", typeof (Bitmap), typeof (AnimatedImage),
            new FrameworkPropertyMetadata(null, OnAnimatedBitmapChanged));

        public static readonly RoutedEvent AnimatedBitmapChangedEvent =
            EventManager.RegisterRoutedEvent("AnimatedBitmapChanged", RoutingStrategy.Bubble,
                                             typeof (RoutedPropertyChangedEventHandler<Bitmap>), typeof (AnimatedImage));

        private List<BitmapSource> _bitmapSources;
        private List<int> _delays;
        private int _nCurrentFrame;
        private Timer _timer;
        //void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (IsVisible)
        //        StartAnimate();
        //    else
        //        StopAnimate();
        //}
        static AnimatedImage() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (AnimatedImage),
                                                     new FrameworkPropertyMetadata(typeof (AnimatedImage)));
        }

        public bool IsAnimating { get; private set; }

        public Bitmap AnimatedBitmap {
            get { return (Bitmap) GetValue(AnimatedBitmapProperty); }
            set {
                StopAnimate();
                SetValue(AnimatedBitmapProperty, value);
            }
        }

        private static void OnAnimatedBitmapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var control = (AnimatedImage) obj;

            control.UpdateAnimatedBitmap();

            var e = new RoutedPropertyChangedEventArgs<Bitmap>((Bitmap) args.OldValue, (Bitmap) args.NewValue,
                                                               AnimatedBitmapChangedEvent);
            control.OnAnimatedBitmapChanged(e);
        }

        public event RoutedPropertyChangedEventHandler<Bitmap> AnimatedBitmapChanged {
            add { AddHandler(AnimatedBitmapChangedEvent, value); }
            remove { RemoveHandler(AnimatedBitmapChangedEvent, value); }
        }

        public void LoadSmile(Bitmap bitmap) {
            AnimatedBitmap = bitmap;
        }

        private void OnAnimatedBitmapChanged(RoutedPropertyChangedEventArgs<Bitmap> args) {
            try {
                RaiseEvent(args);
            } catch {}
        }

        private void UpdateAnimatedBitmap() {
            try {
                var nTimeFrames = GetFramesCount();
                _nCurrentFrame = 0;
                if (nTimeFrames > 0) {
                    var stream = new MemoryStream();
                    AnimatedBitmap.Save(stream, ImageFormat.Gif);
                    stream.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    ParseGif(buffer);
                    _bitmapSources = new List<BitmapSource>(nTimeFrames);
                    stream.Dispose();
                    FillBitmapSources(nTimeFrames);
                    _timer = new Timer(OnFrameChanged, null, -1, -1);
                    StartAnimate();
                } else {
                    var bitmap = new Bitmap(AnimatedBitmap);
                    _bitmapSources = new List<BitmapSource>(1) {CreateBitmapSourceFromBitmap(bitmap)};
                    Source = _bitmapSources[0];
                }
            } catch {}
        }

        private void ParseGif(byte[] buffer) {
            var parseGif = new ParseGif();
            _delays = parseGif.ParseGifDataStream(buffer, 0);
        }

        private void FillBitmapSources(int nTimeFrames) {
            for (var i = 0; i < nTimeFrames; i++) {
                AnimatedBitmap.SelectActiveFrame(FrameDimension.Time, i);
                var bitmap = new Bitmap(AnimatedBitmap);
                bitmap.MakeTransparent();
                _bitmapSources.Add(CreateBitmapSourceFromBitmap(bitmap));
                _bitmapSources[i].Freeze();
            }
        }

        private int GetFramesCount() {
            try {
                return AnimatedBitmap.GetFrameCount(FrameDimension.Time);
            } catch (Exception) {
                return 1;
            }
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap) {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var hBitmap = bitmap.GetHbitmap();

            try {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                                             BitmapSizeOptions.FromEmptyOptions());
            } finally {
                DeleteObject(hBitmap);
            }
        }

        private void OnFrameChanged(object obj) {
            try {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new VoidDelegate(ChangeSource));
            } catch {}
        }

        private void ChangeSource() {
            try {
                _timer.Change(_delays[_nCurrentFrame]*10, 0);
                Source = _bitmapSources[_nCurrentFrame++];
                _nCurrentFrame = _nCurrentFrame%_bitmapSources.Count;
            } catch {}
        }

        public void StopAnimate() {
            try {
                if (IsAnimating) {
                    _timer.Change(-1, -1);
                    IsAnimating = false;
                }
            } catch {}
        }

        public void StartAnimate() {
            try {
                if (!IsAnimating) {
                    _timer.Change(0, 0);
                    IsAnimating = true;
                }
            } catch {}
        }

        public void Dispose() {
            try {
                _timer.Change(-1, -1);
                _timer.Dispose();
                _bitmapSources.Clear();
                Source = null;
                GC.Collect();
                GC.SuppressFinalize(this);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            } catch {}
        }
    }
}