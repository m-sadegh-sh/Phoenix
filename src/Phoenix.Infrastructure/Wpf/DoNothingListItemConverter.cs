namespace Phoenix.Infrastructure.Wpf {
    public class DoNothingListItemConverter : IListItemConverter {
        #region IListItemConverter Members
        public object Convert(object masterListItem) {
            return masterListItem;
        }

        public object ConvertBack(object targetListItem) {
            return targetListItem;
        }
        #endregion
    }
}