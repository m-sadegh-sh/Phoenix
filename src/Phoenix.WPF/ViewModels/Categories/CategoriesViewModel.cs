namespace Phoenix.WPF.ViewModels.Categories {
    using System.Linq;
    using System.Windows;

    using Phoenix.Domain.Categories;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;

    internal class CategoriesViewModel : ViewModelBase<Category, CategoriesWindow, CategoriesService> {
        public CategoriesViewModel() {
            SearchCondition = category => category.Name.Contains(SearchQuery);
        }

        protected override bool CanInsert {
            get {
                return !IsInDesignMode &&
                       (base.CanInsert &&
                        (Status == ModelStatus.OnEdit ? AppContext.CanUpdateCategories : AppContext.CanInsertCategories));
            }
        }

        protected override bool CanDelete {
            get { return !IsInDesignMode && (base.CanDelete && AppContext.CanDeleteCategories); }
        }

        protected override bool CanSelect {
            get { return !IsInDesignMode && (base.CanSelect && AppContext.CanUpdateCategories); }
        }

        protected override void Insert() {
            if (IsValid()) {
                Category category = null;
                if (Status == ModelStatus.OnEdit)
                    category = CurrentItem;

                if (category == null)
                    category = new Category();

                category.Name = CurrentItem.Name;
                category.Description = CurrentItem.Description;

                if (ModelService.Insert(category)) {
                    Global.SubmissionSuceeded(ModelWindow);
                    Load();
                } else
                    Global.SubmissionFailed(ModelWindow);
            }
        }

        protected override void Delete() {
            Frozen = true;
            if (Global.DeleteQuestion(null)) {
                var failed = false;
                var removedCount = 0;
                var shallowCopy = SelectedItems.ToList();
                var progressWindow = new ProgressWindow(SelectedItems.Count);
                progressWindow.Show(ModelWindow);
                foreach (var item in shallowCopy) {
                    int count;
                    if (CategoriesService.ReferencedToOther(item, out count)) {
                        if (
                            Global.ReferenceFound(ModelWindow,
                                                  string.Format(CategoriesResources.Referenced, item.Name, count)) ==
                            MessageBoxResult.Yes) {
                            if (!ModelService.Remove(item)) {
                                failed = true;
                                Global.DeletionFailed(ModelWindow);
                            } else
                                removedCount++;
                        }
                    } else if (!ModelService.Remove(item)) {
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

        protected override bool IsValid() {
            if (Status == ModelStatus.Clear) {
                if (CategoriesService.Exist(CurrentItem.Name)) {
                    Global.ValidationFailed(ModelWindow, CategoriesResources.NameDuplicate);
                    return false;
                }
            } else if (CategoriesService.Exist(CurrentItem.Name, TempItem)) {
                Global.ValidationFailed(ModelWindow, CategoriesResources.NameDuplicate);
                return false;
            }

            return true;
        }
    }
}