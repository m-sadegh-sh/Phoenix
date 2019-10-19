namespace Phoenix.WPF.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using Phoenix.Domain;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public abstract class ViewModelBase<TEntity, TWindow, TService> : ViewModelBase, INotifyPropertyChanged
        where TEntity : class, ICloneable where TWindow : WindowBase where TService : ServiceBase<TEntity> {
        private readonly ObservableCollection<TEntity> _modelItems;
        private readonly ObservableCollection<TEntity> _selectedItems;
        private TEntity _currentItem;
        private bool _frozen;
        private Func<TEntity, bool> _searchCondition;
        private string _searchQuery;

        protected ViewModelBase() {
            ModelService = Activator.CreateInstance<TService>();
            _modelItems = new ObservableCollection<TEntity>();
            _selectedItems = new ObservableCollection<TEntity>();
            ExitCommand = new RelayCommand(Exit, () => CanExit);
            InsertCommand = new RelayCommand(Insert, () => CanInsert);
            ResetCommand = new RelayCommand(Reset, () => CanReset);
            DeleteCommand = new RelayCommand(Delete, () => CanDelete);
            SearchCommand = new RelayCommand(Search, () => CanSearch);
            SelectCommand = new RelayCommand(Select, () => CanSelect);
            UpgradeCommand = new RelayCommand(Upgrade, () => CanUpgrade);
            Load();
        }

        public ModelStatus Status { get; set; }

        protected TWindow ModelWindow {
            get { return Utils.GetWindow<TWindow>(); }
        }

        public TService ModelService { get; private set; }

        protected AppContext AppContext {
            get { return AppContext.Instanse; }
        }

        public RelayCommand ExitCommand { get; private set; }

        private bool CanExit {
            get { return ModelWindow != null; }
        }

        public bool Frozen {
            get { return _frozen; }
            set {
                _frozen = value;
                PropertyChanged.Raise(() => Frozen);
            }
        }

        public string SearchQuery {
            get { return _searchQuery; }
            set {
                _searchQuery = value;
                PropertyChanged.Raise(() => SearchQuery);
                SearchCommand.Execute(null);
            }
        }

        public Visibility FrozenVisibility {
            get { return Frozen ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility HasResultsVisibility {
            get { return ModelItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility NoResultsVisibility {
            get { return ModelItems.Count != 0 ? Visibility.Collapsed : Visibility.Visible; }
        }

        public ObservableCollection<TEntity> ModelItems {
            get { return _modelItems; }
        }

        public ObservableCollection<TEntity> SelectedItems {
            get { return _selectedItems; }
        }

        public TEntity CurrentItem {
            get { return _currentItem; }
            set {
                _currentItem = value;
                PropertyChanged.Raise(() => CurrentItem);
            }
        }

        protected TEntity TempItem { get; set; }

        public RelayCommand ResetCommand { get; private set; }

        public RelayCommand InsertCommand { get; private set; }

        public RelayCommand DeleteCommand { get; private set; }

        public RelayCommand UpgradeCommand { get; private set; }

        public RelayCommand SelectCommand { get; private set; }

        public RelayCommand SearchCommand { get; private set; }

        protected virtual bool CanInsert {
            get { return !ValidationHelper.GetErrors(CurrentItem).Any(); }
        }

        protected virtual bool CanReset {
            get { return Status == ModelStatus.OnEdit; }
        }

        protected virtual bool CanDelete {
            get { return SelectedItems.Count > 0 && Status != ModelStatus.OnEdit; }
        }

        protected virtual bool CanUpgrade {
            get { return false; }
        }

        protected virtual bool CanSelect {
            get { return ModelItems.Count > 0; }
        }

        protected virtual bool CanSearch {
            get { return true; }
        }

        protected virtual Func<TEntity, bool> SearchCondition {
            get { return _searchCondition; }
            set {
                _searchCondition = value;
                PropertyChanged.Raise(() => SearchCondition);
            }
        }

        #region INotifyPropertyChanged Members
        public new event PropertyChangedEventHandler PropertyChanged;
        #endregion

        protected virtual bool IsValid() {
            return true;
        }

        private void Exit() {
            ModelWindow.Close();
        }

        protected virtual void Insert() {}

        private void Select() {
            var origin = SelectedItems.FirstOrDefault();
            if (origin != null) {
                CurrentItem = origin.Clone() as TEntity;
                TempItem = origin.Clone() as TEntity;
                Status = ModelStatus.OnEdit;
            }
        }

        protected virtual void Upgrade() {}

        protected virtual void Delete() {
            Frozen = true;
            if (Global.DeleteQuestion(null)) {
                var failed = false;
                var removedCount = 0;
                var shallowCopy = SelectedItems.ToList();
                var progressWindow = new ProgressWindow(SelectedItems.Count);
                progressWindow.Show(ModelWindow);
                foreach (var item in shallowCopy) {
                    if (!ModelService.Remove(item)) {
                        failed = true;
                        Global.DeletionFailed(null);
                    } else {
                        removedCount++;
                        SelectedItems.Remove(item);
                        ModelItems.Remove(item);
                    }
                    progressWindow.IncreaseProgress();
                }
                progressWindow.Close();
                Reset();

                if (removedCount > 0) {
                    if (failed)
                        Global.DeletionSuceededWithSomeFailures(null);
                    else if (removedCount > 0)
                        Global.DeletionSuceeded(null);
                }
            }
            Frozen = false;
        }

        protected virtual void Search() {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                Load();
            else
                Load(ModelService.Search(SearchCondition));
        }

        protected virtual void Load(IList<TEntity> items = null) {
            Frozen = true;
            ModelItems.Clear();
            if (!IsInDesignModeStatic) {
                if (items != null)
                    items.ToList().ForEach(ModelItems.Add);
                else
                    ModelService.GetAll().ToList().ForEach(ModelItems.Add);
                Reset();
            }
            Frozen = false;
        }

        protected virtual void Reset() {
            CurrentItem = Activator.CreateInstance<TEntity>();
            TempItem = null;
            PropertyChanged.Raise(() => HasResultsVisibility);
            PropertyChanged.Raise(() => NoResultsVisibility);
            Status = ModelStatus.Clear;
        }
        }
}