namespace Phoenix.Infrastructure.Wpf {
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;

    public class TwoListSynchronizer : IWeakEventListener {
        private static readonly IListItemConverter _defaultConverter = new DoNothingListItemConverter();
        private readonly IList _masterList;
        private readonly IListItemConverter _masterTargetConverter;
        private readonly IList _targetList;

        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter) {
            _masterList = masterList;
            _targetList = targetList;
            _masterTargetConverter = masterTargetConverter;
        }

        public TwoListSynchronizer(IList masterList, IList targetList) : this(masterList, targetList, _defaultConverter) {}

        #region IWeakEventListener Members
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
            HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);

            return true;
        }
        #endregion

        public void StartSynchronizing() {
            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);

            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);

            if (!TargetAndMasterCollectionsAreEqual())
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
        }

        public void StopSynchronizing() {
            StopListeningForChangeEvents(_masterList);
            StopListeningForChangeEvents(_targetList);
        }

        protected void ListenForChangeEvents(IList list) {
            if (list is INotifyCollectionChanged)
                CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
        }

        protected void StopListeningForChangeEvents(IList list) {
            if (list is INotifyCollectionChanged)
                CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
        }

        private static void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter) {
            var itemCount = e.NewItems.Count;

            for (var i = 0; i < itemCount; i++) {
                var insertionPoint = e.NewStartingIndex + i;

                if (insertionPoint > list.Count)
                    list.Add(converter(e.NewItems[i]));
                else
                    list.Insert(insertionPoint, converter(e.NewItems[i]));
            }
        }

        private object ConvertFromMasterToTarget(object masterListItem) {
            return _masterTargetConverter == null ? masterListItem : _masterTargetConverter.Convert(masterListItem);
        }

        private object ConvertFromTargetToMaster(object targetListItem) {
            return _masterTargetConverter == null ? targetListItem : _masterTargetConverter.ConvertBack(targetListItem);
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var sourceList = sender as IList;

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    PerformActionOnAllLists(AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    PerformActionOnAllLists(MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PerformActionOnAllLists(RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    PerformActionOnAllLists(ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateListsFromSource(sender as IList);
                    break;
                default:
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter) {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(ChangeListAction action, IList sourceList,
                                             NotifyCollectionChangedEventArgs collectionChangedArgs) {
            if (sourceList == _masterList)
                PerformActionOnList(_targetList, action, collectionChangedArgs, ConvertFromMasterToTarget);
            else
                PerformActionOnList(_masterList, action, collectionChangedArgs, ConvertFromTargetToMaster);
        }

        private void PerformActionOnList(IList list, ChangeListAction action,
                                         NotifyCollectionChangedEventArgs collectionChangedArgs,
                                         Converter<object, object> converter) {
            StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            ListenForChangeEvents(list);
        }

        private static void RemoveItems(IList list, NotifyCollectionChangedEventArgs e,
                                        Converter<object, object> converter) {
            var itemCount = e.OldItems.Count;

            for (var i = 0; i < itemCount; i++)
                list.RemoveAt(e.OldStartingIndex);
        }

        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter) {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter) {
            StopListeningForChangeEvents(targetList);

            targetList.Clear();

            foreach (var o in sourceList)
                targetList.Add(converter(o));

            ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual() {
            return
                _masterList.Cast<object>().SequenceEqual(
                    _targetList.Cast<object>().Select(item => ConvertFromTargetToMaster(item)));
        }

        private void UpdateListsFromSource(IList sourceList) {
            if (sourceList == _masterList)
                SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            else
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
        }

        #region Nested type: ChangeListAction
        private delegate void ChangeListAction(
            IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);
        #endregion
    }
}