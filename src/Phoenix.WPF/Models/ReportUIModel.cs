namespace Phoenix.WPF.Models {
    using System.ComponentModel;
    using System.Windows.Documents;

    using Phoenix.Infrastructure.Extensions;

    public class ReportUIModel : INotifyPropertyChanged {
        private IDocumentPaginatorSource _document;
        private bool _isBusyIndicatorHidden;

        public IDocumentPaginatorSource Document {
            get { return _document; }
            set {
                if (_document == value)
                    return;
                _document = value;
                PropertyChanged.Raise(() => Document);
            }
        }

        public bool IsBusyIndicatorHidden {
            get { return _isBusyIndicatorHidden; }
            set {
                _isBusyIndicatorHidden = value;
                PropertyChanged.Raise(() => IsBusyIndicatorHidden);
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}