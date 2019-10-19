namespace LMS.Infrastructure.Extensions {
    using System.Windows.Forms;

    public static class ComboBoxExtensions {
        public static void Configure(this ComboBox comboBox, int maxDropDownItems = 20) {
            if(comboBox != null) {
                comboBox.IntegralHeight = false;
                comboBox.MaxDropDownItems = maxDropDownItems;
            }
        }

        public static void Focus(this ComboBox comboBox, bool selectText) {
            if(comboBox != null) {
                if(!comboBox.Focused)
                    comboBox.Focus();
                if(selectText && comboBox.DropDownStyle != ComboBoxStyle.DropDownList && !string.IsNullOrEmpty(comboBox.Text))
                    comboBox.SelectAll();
            }
        }

        public static void FocusWithDropDown(this ComboBox comboBox) {
            if(comboBox != null) {
                if(!comboBox.Focused)
                    comboBox.Focus();
                comboBox.DroppedDown = true;
            }
        }
    }
}