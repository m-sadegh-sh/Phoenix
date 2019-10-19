namespace Phoenix.Domain.RepositoryItems {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Items;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class RepositoryItemsService : ServiceBase<RepositoryItem> {
        public static RepositoryItemsService Instanse {
            get { return new RepositoryItemsService(); }
        }

        public override bool Insert(RepositoryItem repositoryItem) {
            try {
                RepositoryItemsRepository.Instanse.InsertAndSubmit(repositoryItem);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.RepositoryItems,
                                                        Details = Log.RepositoryItemDetailer(repositoryItem, ActionType.Created)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<RepositoryItem> Search(Func<RepositoryItem, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public IEnumerable<string> GetAllTargetApplicants(bool insertAll) {
            var tahvils =
                GetAll().Select(repositoryItem => repositoryItem.TargetApplicant).Where(x => x != null).Distinct();
            return insertAll
                       ? tahvils.Concat(new[] {RepositoryMaterialsAndItemsResources.All}).ToList()
                       : tahvils.ToList();
        }

        public override IList<RepositoryItem> GetAll() {
            return (from users in UsersRepository.Instanse.GetAll()
                    join repositoryItems in RepositoryItemsRepository.Instanse.GetAll() on users.UserID equals
                        repositoryItems.RegisteredBy
                    join items in ItemsRepository.Instanse.GetAll() on repositoryItems.ItemID equals items.ItemID into
                        temp1
                    from temp1Items in temp1.DefaultIfEmpty()
                    join labs in LabsRepository.Instanse.GetAll() on repositoryItems.LabID equals labs.LabID into temp
                    from tempItems in temp.DefaultIfEmpty()
                    select
                        new RepositoryItem {
                                               StringCount =
                                                   string.Format(
                                                       repositoryItems.Count > 0
                                                           ? RepositoryMaterialsAndItemsResources.Requested
                                                           : RepositoryMaterialsAndItemsResources.Returned, Math.Abs(repositoryItems.Count),
                                                       ComputingUnit.Count.ToUIString(true)),
                                               StringItemID = temp1Items != null ? temp1Items.Name : SharedResources.Unknown,
                                               StringLabID = tempItems != null ? tempItems.Name : SharedResources.Unknown,
                                               ItemID = repositoryItems.ItemID,
                                               Count = repositoryItems.Count,
                                               LabID = repositoryItems.LabID,
                                               RegisteredOn = repositoryItems.RegisteredOn,
                                               TargetApplicant = repositoryItems.TargetApplicant,
                                               RegisteredBy = repositoryItems.RegisteredBy,
                                               StringRegisteredBy = users.UserName
                                           }).OrderByDescending(
                                               repositoryItem => repositoryItem.RegisteredOn).ToList();
        }

        public IEnumerable<RepositoryItem> GetAll(Guid itemID) {
            return itemID == Guid.Empty
                       ? GetAll()
                       : GetAll().Where(repositoryItem => repositoryItem.ItemID == itemID).ToList();
        }

        public void ValidateAmount(Guid itemID, int amount, out int result) {
            var item = ItemsService.Get(itemID);
            var all = GetAll(itemID);
            var repositoryAmount = all.Sum(m => m.Count);
            if (repositoryAmount + amount >= item.LowestCount)
                result = -2;
            else if (repositoryAmount + amount >= 0)
                result = -1;
            else
                result = repositoryAmount;
        }

        public IList<RepositoryItem> GetAll(Guid? itemID, DateTime? registeredOnLowerBound,
                                            DateTime? registeredOnUpperBound, bool registeredOnOutside,
                                            string tahvilGirande, int? amountLowerBound, int? amountUpperBound,
                                            bool amountOutside) {
            var repositoryMaterials = GetAll();
            if (itemID.HasValue && itemID.Value != Guid.Empty)
                repositoryMaterials =
                    repositoryMaterials.Where(repositoryMayerial => repositoryMayerial.ItemID == itemID).ToList();
            if (registeredOnLowerBound.HasValue) {
                registeredOnLowerBound = registeredOnLowerBound.Value.AddDays(-1);
                repositoryMaterials =
                    repositoryMaterials.Where(
                        repositoryMayerial =>
                        registeredOnOutside
                            ? repositoryMayerial.RegisteredOn <= registeredOnLowerBound
                            : repositoryMayerial.RegisteredOn >= registeredOnLowerBound).ToList();
            }
            if (registeredOnUpperBound.HasValue) {
                registeredOnUpperBound = registeredOnUpperBound.Value.AddDays(1);
                repositoryMaterials =
                    repositoryMaterials.Where(
                        repositoryMayerial =>
                        registeredOnOutside
                            ? repositoryMayerial.RegisteredOn >= registeredOnUpperBound
                            : repositoryMayerial.RegisteredOn <= registeredOnUpperBound).ToList();
            }
            if (!string.IsNullOrEmpty(tahvilGirande) && string.Compare(tahvilGirande, "еге") != 0) {
                repositoryMaterials =
                    repositoryMaterials.Where(
                        repositoryMayerial => repositoryMayerial.TargetApplicant.Contains(tahvilGirande)).ToList();
            }
            if (amountLowerBound.HasValue) {
                repositoryMaterials =
                    repositoryMaterials.Where(
                        repositoryMayerial =>
                        amountOutside
                            ? repositoryMayerial.Count <= amountLowerBound
                            : repositoryMayerial.Count >= amountLowerBound).ToList();
            }
            if (amountUpperBound.HasValue) {
                repositoryMaterials =
                    repositoryMaterials.Where(
                        repositoryMayerial =>
                        amountOutside
                            ? repositoryMayerial.Count >= amountUpperBound
                            : repositoryMayerial.Count <= amountUpperBound).ToList();
            }
            return repositoryMaterials.ToList();
        }

        public static int GetMaxAmount() {
            var repositoryItems = RepositoryItemsRepository.Instanse.GetAll();
            return repositoryItems.Count > 0 ? repositoryItems.Max(repositoryItem => repositoryItem.Count) : 0;
        }

        public static int GetMinAmount() {
            var repositoryItems = RepositoryItemsRepository.Instanse.GetAll();
            return repositoryItems.Count > 0 ? repositoryItems.Min(repositoryItem => repositoryItem.Count) : 0;
        }

        public static DateTime? GetMaxRegisteredOn() {
            var repositoryItems = RepositoryItemsRepository.Instanse.GetAll();
            return repositoryItems.Count > 0
                       ? (DateTime?) repositoryItems.Max(repositoryItem => repositoryItem.RegisteredOn)
                       : null;
        }

        public static DateTime? GetMinRegisteredOn() {
            var repositoryItems = RepositoryItemsRepository.Instanse.GetAll();
            return repositoryItems.Count > 0
                       ? (DateTime?) repositoryItems.Min(repositoryItem => repositoryItem.RegisteredOn)
                       : null;
        }

        public IList<RepositoryItem> GetAll(int? filterType) {
            if (filterType.HasValue) {
                switch (filterType) {
                    case 0:
                        return
                            GetAll().Where(
                                repositoryItem =>
                                repositoryItem.RegisteredOn >= DateTime.Now.AddDays(-1) &&
                                repositoryItem.RegisteredOn <= DateTime.Now).ToList();
                    case 1:
                        return
                            GetAll().Where(
                                repositoryItem =>
                                repositoryItem.RegisteredOn >= DateTime.Now.AddDays(-2) &&
                                repositoryItem.RegisteredOn <= DateTime.Now).ToList();
                    case 2:
                        return
                            GetAll().Where(
                                repositoryItem =>
                                repositoryItem.RegisteredOn >= DateTime.Now.AddDays(-8) &&
                                repositoryItem.RegisteredOn <= DateTime.Now).ToList();
                    case 3:
                        return
                            GetAll().Where(
                                repositoryItem =>
                                repositoryItem.RegisteredOn >= DateTime.Now.AddDays(-32) &&
                                repositoryItem.RegisteredOn <= DateTime.Now).ToList();
                }
            }
            return GetAll().ToList();
        }

        public static void Remove(Guid itemID) {
            foreach (var item in RepositoryItemsRepository.Instanse.GetAll(rm => rm.ItemID == itemID))
                RepositoryItemsRepository.Instanse.DeleteAndSubmit(item);
        }

        public override bool Remove(RepositoryItem repositoryItem) {
            try {
                RepositoryItemsRepository.Instanse.DeleteAndSubmit(repositoryItem);
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }
    }
}