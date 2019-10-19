namespace LMS.Infrastructure {
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class WatermarkTextBox : TextBox {
        public bool IsControlTextChange {
            get;
            set;
        }

        public string Watermark {
            get;
            set;
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            Anchor = AnchorStyles.Right;
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if(string.Compare(Text, Watermark) == 0) {
                ForeColor = SystemColors.ControlText;
                IsControlTextChange = true;
                Text = null;
                IsControlTextChange = false;
            }
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            if(string.IsNullOrWhiteSpace(Text)) {
                ForeColor = SystemColors.GrayText;
                IsControlTextChange = true;
                Text = Watermark;
                IsControlTextChange = false;
            }
        }
    }
}