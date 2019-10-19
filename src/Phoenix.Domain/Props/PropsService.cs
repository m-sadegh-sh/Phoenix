namespace Phoenix.Domain.Props {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class PropsService : ServiceBase<Prop> {
        public static PropsService Instanse {
            get { return new PropsService(); }
        }

        public static bool Exist(int propNo) {
            return PropsRepository.Instanse.Get(prop => prop.PropNo == propNo) != null;
        }

        public static bool Exist(int propNo, Prop originProp) {
            return
                PropsRepository.Instanse.GetAll(prop => prop.PropNo == propNo).Where(
                    prop => prop.PropID != originProp.PropID).ToList().Count > 0;
        }

        public static bool Exist(string serialNo) {
            return PropsRepository.Instanse.Get(prop => string.Compare(prop.SerialNo, serialNo, true) == 0) != null;
        }

        public static bool Exist(string serialNo, Prop originProp) {
            return
                PropsRepository.Instanse.GetAll(prop => string.Compare(prop.SerialNo, serialNo, true) == 0).Where(
                    prop => prop.PropID != originProp.PropID).ToList().Count > 0;
        }

        public override bool Insert(Prop prop) {
            try {
                if (prop.PropID == Guid.Empty) {
                    PropsRepository.Instanse.InsertOrUpdateAndSubmit(prop);
                    PropStatusChangesService.Instanse.Insert(new PropStatusChange
                                                             {PropID = prop.PropID, Type = (short) ReportType.Free});
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Props,
                                                            Details = Log.PropDetailer(prop, ActionType.Created)
                                                        });
                    return true;
                }
                prop.LastModifiedOn = DateTime.Now;
                PropsRepository.Instanse.InsertOrUpdateAndSubmit(prop);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Props,
                                                        Details = Log.PropDetailer(prop, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<Prop> Search(Func<Prop, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public static bool ChangeNotifiable(Guid propID, bool notifiable) {
            var prop = Get(propID);
            if (prop != null) {
                prop.Notifiable = notifiable;
                PropsRepository.Instanse.InsertOrUpdateAndSubmit(prop);
                return true;
            }
            return false;
        }

        public static Prop Get(Guid propID) {
            return PropsRepository.Instanse.Get(new Prop {PropID = propID});
        }

        public override IList<Prop> GetAll() {
            return (from propStatusChanges in PropStatusChangesService.Instanse.GetAll()
                    join props in PropsRepository.Instanse.GetAll() on propStatusChanges.PropID equals props.PropID
                    join labProps in LabPropsRepository.Instanse.GetAll() on props.PropID equals labProps.PropID into
                        labPropsTemp
                    from labPropTemp in labPropsTemp.DefaultIfEmpty(new LabProp())
                    join labs in LabsRepository.Instanse.GetAll() on labPropTemp.LabID equals labs.LabID into labsTemp
                    from labTemp in labsTemp.DefaultIfEmpty()
                    join categories in CategoriesService.Instanse.GetAll() on props.CategoryID equals
                        categories.CategoryID into temp
                    from tempItems in temp.DefaultIfEmpty()
                    where propStatusChanges.IsAlive
                    select
                        new Prop {
                                     StringLabID = labTemp != null ? labTemp.Name : SharedResources.Unknown,
                                     Notifiable = props.Notifiable,
                                     CategoryID = props.CategoryID,
                                     CreatedOn = props.CreatedOn,
                                     Description = props.Description,
                                     LastModifiedOn = props.LastModifiedOn,
                                     Name = props.Name,
                                     PropID = props.PropID,
                                     PropNo = props.PropNo,
                                     PurchasingDate = props.PurchasingDate,
                                     SerialNo = props.SerialNo,
                                     Status = propStatusChanges.Type,
                                     StringCategoryID = tempItems != null ? tempItems.Name : SharedResources.Unknown,
                                     WarrantyExpirationDate = props.WarrantyExpirationDate
                                 }).OrderBy(prop => prop.Name).ToList();
        }

        public IList<Prop> GetAllWarrantyExpired() {
            return
                GetAll().Where(
                    prop =>
                    (prop.WarrantyExpirationDate.HasValue && prop.WarrantyExpirationDate.Value <= DateTime.Now) &&
                    prop.Notifiable).ToList();
        }

        public IList<Prop> GetAllLoanExpired() {
            return (from propStatusChanges in PropStatusChangesService.Instanse.GetAll()
                    join props in GetAll() on propStatusChanges.PropID equals props.PropID
                    join categories in CategoriesService.Instanse.GetAll() on props.CategoryID equals
                        categories.CategoryID
                    where
                        propStatusChanges.IsAlive && propStatusChanges.Type == (short) ReportType.Borrowed &&
                        propStatusChanges.ResolveDate <= DateTime.Now && props.Notifiable
                    select
                        new Prop {
                                     Notifiable = props.Notifiable,
                                     CategoryID = props.CategoryID,
                                     CreatedOn = props.CreatedOn,
                                     Description = props.Description,
                                     LastModifiedOn = props.LastModifiedOn,
                                     Name = props.Name,
                                     PropID = props.PropID,
                                     PropNo = props.PropNo,
                                     PurchasingDate = props.PurchasingDate,
                                     SerialNo = props.SerialNo,
                                     Status = propStatusChanges.Type,
                                     StringCategoryID = categories.Name,
                                     WarrantyExpirationDate = props.WarrantyExpirationDate
                                 }).OrderByDescending(
                                     prop => prop.LastModifiedOn).ToList();
        }

        public IList<Prop> GetAllRepairExpired() {
            return (from propStatusChanges in PropStatusChangesService.Instanse.GetAll()
                    join props in GetAll() on propStatusChanges.PropID equals props.PropID
                    join categories in CategoriesService.Instanse.GetAll() on props.CategoryID equals
                        categories.CategoryID
                    where
                        propStatusChanges.IsAlive && propStatusChanges.Type == (short) ReportType.Corrupted &&
                        propStatusChanges.ResolveDate <= DateTime.Now && props.Notifiable
                    select
                        new Prop {
                                     Notifiable = props.Notifiable,
                                     CategoryID = props.CategoryID,
                                     CreatedOn = props.CreatedOn,
                                     Description = props.Description,
                                     LastModifiedOn = props.LastModifiedOn,
                                     Name = props.Name,
                                     PropID = props.PropID,
                                     PropNo = props.PropNo,
                                     PurchasingDate = props.PurchasingDate,
                                     SerialNo = props.SerialNo,
                                     Status = propStatusChanges.Type,
                                     StringCategoryID = categories.Name,
                                     WarrantyExpirationDate = props.WarrantyExpirationDate
                                 }).OrderByDescending(
                                     prop => prop.LastModifiedOn).ToList();
        }

        private IEnumerable<Prop> GetAllUnallocated() {
            var labProps = LabPropsRepository.Instanse.GetAll().Select(labProp => labProp.PropID).Distinct();
            return GetAll().Where(prop => !labProps.Contains(prop.PropID)).ToList();
        }

        public static IEnumerable<int?> GetAllPropNos() {
            return
                PropsRepository.Instanse.GetAll().Select(prop => prop.PropNo).Distinct().Where(prop => prop != null).
                    OrderBy(propNo => propNo).ToList();
        }

        public static bool ReferencedToOther(Guid propID, out string labName) {
            var labProp = LabPropsRepository.Instanse.GetAll(lab => lab.PropID == propID).FirstOrDefault();
            if (labProp != null) {
                labName = LabsRepository.Instanse.Get(lab => lab.LabID == labProp.LabID).Name;
                return true;
            }
            labName = null;
            return false;
        }

        public override bool Remove(Prop prop) {
            try {
                prop = Get(prop.PropID);
                if (prop != null) {
                    foreach (var labProp in LabPropsRepository.Instanse.GetAll(lprop => lprop.PropID == prop.PropID))
                        LabPropsService.Instanse.Remove(labProp);

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Props,
                                                            Details = Log.PropDetailer(prop, ActionType.Removed)
                                                        });
                    PropsRepository.Instanse.DeleteAndSubmit(prop);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static DateTime? GetMinCreatedOn() {
            var props = PropsRepository.Instanse.GetAll();
            return props.Count > 0 ? (DateTime?) props.Min(prop => prop.CreatedOn) : null;
        }

        public static DateTime? GetMaxCreatedOn() {
            var props = PropsRepository.Instanse.GetAll();
            return props.Count > 0 ? (DateTime?) props.Max(prop => prop.CreatedOn) : null;
        }

        public IList<Prop> GetAllOfLab(Guid? labID) {
            if (!labID.HasValue || labID == Guid.Empty)
                return GetAll();
            return (from props in GetAll()
                    join labProps in LabPropsRepository.Instanse.GetAll(labProp => labProp.LabID == labID) on
                        props.PropID equals labProps.PropID
                    join labs in LabsRepository.Instanse.GetAll() on labProps.LabID equals labs.LabID
                    select
                        new Prop {
                                     Notifiable = props.Notifiable,
                                     CategoryID = props.CategoryID,
                                     CreatedOn = props.CreatedOn,
                                     Description = props.Description,
                                     LastModifiedOn = props.LastModifiedOn,
                                     Name = props.Name,
                                     PropID = props.PropID,
                                     PropNo = props.PropNo,
                                     PurchasingDate = props.PurchasingDate,
                                     SerialNo = props.SerialNo,
                                     Status = props.Status,
                                     StringCategoryID = props.StringCategoryID,
                                     WarrantyExpirationDate = props.WarrantyExpirationDate
                                 }).OrderByDescending(
                                     prop => prop.LastModifiedOn).ToList();
        }

        public IList<Prop> GetAll(Guid labID, Guid categoryID, string propName, bool showAll) {
            if (!showAll) {
                var res = GetAllUnallocated().Where(prop => prop.Status == (short) ReportType.Free);
                if (categoryID != Guid.Empty)
                    res = res.Where(prop => prop.CategoryID == categoryID);
                if (!string.IsNullOrWhiteSpace(propName))
                    res = res.Where(prop => prop.Name.Contains(propName));
                return res.ToList();
            }
            var props = GetAllOfLab(labID);
            if (categoryID != Guid.Empty)
                props = props.Where(prop => prop.CategoryID == categoryID).ToList();
            if (!string.IsNullOrWhiteSpace(propName))
                props = props.Where(prop => prop.Name.Contains(propName)).ToList();
            return props.ToList();
        }

        public IList<Prop> GetAll(Guid? categoryID, Guid? propID, DateTime? createdOnLowerBound,
                                  DateTime? createdOnUpperBound, bool createdOnOutside, int? propNoLowerBound,
                                  int? propNoUpperBound, bool propNoOutside, ReportType reportType = ReportType.All) {
            var props = GetAll();
            if (categoryID.HasValue && categoryID != Guid.Empty)
                props = props.Where(prop => prop.CategoryID == categoryID).ToList();
            if (propID.HasValue && propID.Value != Guid.Empty)
                props = props.Where(prop => prop.PropID == propID).ToList();
            if (createdOnLowerBound.HasValue) {
                createdOnLowerBound = createdOnLowerBound.Value.AddDays(-1);
                props = createdOnOutside
                            ? props.Where(prop => prop.CreatedOn <= createdOnLowerBound).ToList()
                            : props.Where(prop => prop.CreatedOn >= createdOnLowerBound).ToList();
            }
            if (createdOnUpperBound.HasValue) {
                createdOnUpperBound = createdOnUpperBound.Value.AddDays(1);
                props = createdOnOutside
                            ? props.Where(prop => prop.CreatedOn >= createdOnUpperBound).ToList()
                            : props.Where(prop => prop.CreatedOn <= createdOnUpperBound).ToList();
            }
            if (propNoLowerBound.HasValue) {
                props = propNoOutside
                            ? props.Where(prop => prop.PropNo <= propNoLowerBound).ToList()
                            : props.Where(prop => prop.PropNo >= propNoLowerBound).ToList();
            }
            if (propNoUpperBound.HasValue) {
                props = propNoOutside
                            ? props.Where(prop => prop.PropNo >= propNoUpperBound).ToList()
                            : props.Where(prop => prop.PropNo <= propNoUpperBound).ToList();
            }
            if (reportType != ReportType.All)
                props = props.Where(prop => prop.Status == (short) reportType).ToList();
            return props.ToList();
        }

        public static int CountOfNotNotifiable() {
            return PropsRepository.Instanse.GetAll(prop => !prop.Notifiable).Count;
        }

        public static IEnumerable<Guid> GetAllNotNotifiables() {
            return PropsRepository.Instanse.GetAll(prop => !prop.Notifiable).Select(prop => prop.PropID).ToList();
        }

        public IList<Prop> GetAllContains(string name) {
            return GetAll().Where(prop => prop.Name.Contains(name)).ToList();
        }
    }
}