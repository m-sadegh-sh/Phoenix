namespace Phoenix.Domain.Items {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.RepositoryItems;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class ItemsService : ServiceBase<Item> {
        public static ItemsService Instanse {
            get { return new ItemsService(); }
        }

        public static bool Exist(string name) {
            return ItemsRepository.Instanse.Get(item => string.Compare(item.Name, name, true) == 0) != null;
        }

        public static bool Exist(string name, Item current) {
            return
                ItemsRepository.Instanse.GetAll(
                    item => string.Compare(item.Name, name, true) == 0 && item.ItemID != current.ItemID).Count > 0;
        }

        public static bool Insert(Item item, int initialCount = 0) {
            try {
                if (Get(item.ItemID) == null) {
                    ItemsRepository.Instanse.InsertOrUpdateAndSubmit(item);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Items,
                                                            Details = Log.ItemDetailer(item, ActionType.Created)
                                                        });
                    if (initialCount > 0)
                        RepositoryItemsService.Instanse.Insert(new RepositoryItem
                                                               {ItemID = item.ItemID, Count = initialCount});
                    return true;
                }
                item.LastModifiedOn = DateTime.Now;
                ItemsRepository.Instanse.InsertOrUpdateAndSubmit(item);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Items,
                                                        Details = Log.ItemDetailer(item, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static bool ChangeNotifiable(Item item, bool notifiable) {
            try {
                if (item != null) {
                    item.Notifiable = notifiable;
                    ItemsRepository.Instanse.InsertOrUpdateAndSubmit(item);
                }
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static Item Get(Guid itemID) {
            return ItemsRepository.Instanse.Get(new Item {ItemID = itemID});
        }

        public override IList<Item> GetAll() {
            return (from items in ItemsRepository.Instanse.GetAll()
                    join categories in CategoriesService.Instanse.GetAll() on items.CategoryID equals
                        categories.CategoryID into temp
                    from tempItems in temp.DefaultIfEmpty()
                    select
                        new Item {
                                     Notifiable = items.Notifiable,
                                     CategoryID = items.CategoryID,
                                     CreatedOn = items.CreatedOn,
                                     Description = items.Description,
                                     LastModifiedOn = items.LastModifiedOn,
                                     Name = items.Name,
                                     ItemID = items.ItemID,
                                     StringCategoryID = tempItems != null ? tempItems.Name : SharedResources.Unknown,
                                     LowestCount = items.LowestCount,
                                     CurrentCount =
                                         RepositoryItemsService.Instanse.GetAll(items.ItemID).Sum(
                                             repositoryItem => repositoryItem.Count)
                                 }).OrderBy(item => item.Name).ToList();
        }

        public override bool Remove(Item item) {
            try {
                item = Get(item.ItemID);
                if (item != null) {
                    foreach (var reopItem in
                        RepositoryItemsRepository.Instanse.GetAll(repoMat => repoMat.ItemID == item.ItemID))
                        RepositoryItemsService.Remove(reopItem.ItemID);

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Items,
                                                            Details = Log.ItemDetailer(item, ActionType.Removed)
                                                        });
                    ItemsRepository.Instanse.DeleteAndSubmit(item);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public IList<Item> GetAllContains(string name) {
            return GetAll().Where(item => item.Name.Contains(name)).OrderBy(item => item.Name).ToList();
        }

        public static DateTime? GetMinCreatedOn() {
            var items = ItemsRepository.Instanse.GetAll();
            return items.Count > 0 ? (DateTime?) items.Min(item => item.CreatedOn) : null;
        }

        public static DateTime? GetMaxCreatedOn() {
            var items = ItemsRepository.Instanse.GetAll();
            return items.Count > 0 ? (DateTime?) items.Max(item => item.CreatedOn) : null;
        }

        public IEnumerable<Item> GetAllLowItemsAmount() {
            return GetAll().Where(item => item.LowestCount > item.CurrentCount && item.Notifiable).ToList();
        }

        public static int CountOfNotNotifiable() {
            return ItemsRepository.Instanse.GetAll(item => !item.Notifiable).Count;
        }

        public IList<Item> GetAll(Guid? categoryID, Guid? itemID, DateTime? createdOnLowerBound,
                                  DateTime? createdOnUpperBound, bool createdOnOutside, int? lowestAmountLowerBound,
                                  int? lowestAmountUpperBound, bool lowestAmountOutside) {
            var items = GetAll();
            if (categoryID.HasValue && categoryID != Guid.Empty)
                items = items.Where(item => item.CategoryID == categoryID).ToList();
            if (itemID.HasValue && itemID.Value != Guid.Empty)
                items = items.Where(item => item.ItemID == itemID).ToList();
            if (createdOnLowerBound.HasValue) {
                createdOnLowerBound = createdOnLowerBound.Value.AddDays(-1);
                items =
                    items.Where(
                        item =>
                        createdOnOutside ? item.CreatedOn <= createdOnLowerBound : item.CreatedOn >= createdOnLowerBound)
                        .ToList();
            }
            if (createdOnUpperBound.HasValue) {
                createdOnUpperBound = createdOnUpperBound.Value.AddDays(1);
                items =
                    items.Where(
                        item =>
                        createdOnOutside ? item.CreatedOn >= createdOnUpperBound : item.CreatedOn <= createdOnUpperBound)
                        .ToList();
            }
            if (lowestAmountLowerBound.HasValue) {
                items =
                    items.Where(
                        item =>
                        lowestAmountOutside
                            ? item.LowestCount <= lowestAmountLowerBound
                            : item.LowestCount >= lowestAmountLowerBound).ToList();
            }
            if (lowestAmountUpperBound.HasValue) {
                items =
                    items.Where(
                        item =>
                        lowestAmountOutside
                            ? item.LowestCount >= lowestAmountUpperBound
                            : item.LowestCount <= lowestAmountUpperBound).ToList();
            }

            return items.ToList();
        }

        public static int GetMaxLowestAmount() {
            var items = ItemsRepository.Instanse.GetAll();
            return items.Count > 0 ? items.Max(item => item.LowestCount) : 0;
        }

        public static int GetMinLowestAmount() {
            var items = ItemsRepository.Instanse.GetAll();
            return items.Count > 0 ? items.Min(item => item.LowestCount) : 0;
        }

        public override bool Insert(Item entity) {
            return false;
        }

        public override IList<Item> Search(Func<Item, bool> func) {
            return GetAll().Where(func).ToList();
        }
    }
}