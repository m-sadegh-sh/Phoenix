namespace Phoenix.Domain.Labs {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class LabsService : ServiceBase<Lab> {
        public static LabsService Instanse {
            get { return new LabsService(); }
        }

        public override IList<Lab> Search(Func<Lab, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<Lab> GetAll() {
            return (from labs in LabsRepository.Instanse.GetAll()
                    join cats in CategoriesService.Instanse.GetAll() on labs.CategoryID equals cats.CategoryID into cats
                    from tempCats in cats.DefaultIfEmpty()
                    select
                        new Lab {
                                    CountOfProps = LabPropsService.Instanse.Search(labProp => labProp.LabID == labs.LabID).Count,
                                    CategoryID = labs.CategoryID,
                                    LabID = labs.LabID,
                                    CreatedOn = labs.CreatedOn,
                                    Description = labs.Description,
                                    LastModifiedOn = labs.LastModifiedOn,
                                    Name = labs.Name,
                                    PlaqueNo = labs.PlaqueNo,
                                    StringCategoryID = tempCats != null ? tempCats.Name : SharedResources.Unknown
                                }).ToList();
        }

        public IEnumerable<Lab> GetAll(Guid? categoryID) {
            return categoryID.HasValue ? GetAll().Where(lab => lab.CategoryID == categoryID).ToList() : GetAll();
        }

        public override bool Insert(Lab lab) {
            try {
                if (lab.LabID == Guid.Empty) {
                    LabsRepository.Instanse.InsertOrUpdateAndSubmit(lab);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Labs,
                                                            Details = Log.LabDetailer(lab, ActionType.Created)
                                                        });
                    return true;
                }
                lab.LastModifiedOn = DateTime.Now;
                LabsRepository.Instanse.InsertOrUpdateAndSubmit(lab);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Labs,
                                                        Details = Log.LabDetailer(lab, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static Lab Get(Guid labID) {
            return LabsRepository.Instanse.Get(new Lab {LabID = labID});
        }

        public static bool Exist(string labName) {
            return LabsRepository.Instanse.Get(lab => string.Compare(lab.Name, labName, false) == 0) != null;
        }

        public bool Exist(string labName, Lab current) {
            return
                GetAll().Where(lab => string.Compare(lab.Name, labName, false) == 0).Where(
                    lab => string.Compare(lab.Name, current.Name, false) != 0).ToList().Count > 0;
        }

        public bool ReferencedToOther(Guid labID) {
            return GetAll().Where(labProp => labProp.LabID == labID).ToList().Count > 0 ||
                   RepositoryMaterialsService.Instanse.GetAllOfLab(labID).Count > 0;
        }

        public override bool Remove(Lab lab) {
            try {
                lab = Get(lab.LabID);
                if (lab != null) {
                    foreach (var labProp in LabPropsRepository.Instanse.GetAll(category => category.LabID == lab.LabID))
                        LabPropsService.Instanse.Remove(labProp);
                    foreach (var repositoryMaterial in RepositoryMaterialsRepository.Instanse.GetAll())
                        RepositoryMaterialsService.Remove(repositoryMaterial.MaterialID);

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Labs,
                                                            Details = Log.LabDetailer(lab, ActionType.Removed)
                                                        });
                    LabsRepository.Instanse.DeleteAndSubmit(lab);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public IList<Lab> GetAllContains(string pattern) {
            return GetAll().Where(lab => lab.Name.Contains(pattern)).OrderBy(lab => lab.Name).ToList();
        }

        public IEnumerable<int> GetAllPlaqueNos() {
            return GetAll().Select(lab => lab.PlaqueNo).OrderBy(plaqueNo => plaqueNo).ToList();
        }

        public static DateTime? GetMinCreatedOn() {
            var labs = LabsRepository.Instanse.GetAll();
            return labs.Count > 0 ? (DateTime?) labs.Min(lab => lab.CreatedOn) : null;
        }

        public static DateTime? GetMaxCreatedOn() {
            var labs = LabsRepository.Instanse.GetAll();
            return labs.Count > 0 ? (DateTime?) labs.Max(lab => lab.CreatedOn) : null;
        }

        public IList<Lab> GetAllAndAppendDefault() {
            var all = GetAll();
            return all.Count != 0
                       ? new[] {new Lab {Name = LabsResources.All}}.Concat(all).ToList()
                       : new Lab[] {}.ToList();
        }

        public IList<Lab> GetAll(Guid? labID, int? plaqueNoLowerBound, int? plaqueNoUpperBound, bool plaqueNoOutside,
                                 DateTime? createdOnLowerBound, DateTime? createdOnUpperBound, bool createdOnOutside) {
            var labs = GetAll();
            if (labID.HasValue && labID.Value != Guid.Empty)
                labs = labs.Where(lab => lab.LabID == labID).ToList();
            if (plaqueNoLowerBound.HasValue) {
                labs =
                    labs.Where(
                        lab => plaqueNoOutside ? lab.PlaqueNo <= plaqueNoLowerBound : lab.PlaqueNo >= plaqueNoLowerBound)
                        .ToList();
            }
            if (plaqueNoUpperBound.HasValue) {
                labs =
                    labs.Where(
                        lab => plaqueNoOutside ? lab.PlaqueNo >= plaqueNoUpperBound : lab.PlaqueNo <= plaqueNoUpperBound)
                        .ToList();
            }
            if (createdOnLowerBound.HasValue) {
                createdOnLowerBound = createdOnLowerBound.Value.AddDays(-1);
                labs = createdOnOutside
                           ? labs.Where(lab => lab.CreatedOn <= createdOnLowerBound).ToList()
                           : labs.Where(lab => lab.CreatedOn >= createdOnLowerBound).ToList();
            }
            if (createdOnUpperBound.HasValue) {
                createdOnUpperBound = createdOnUpperBound.Value.AddDays(1);
                labs = createdOnOutside
                           ? labs.Where(lab => lab.CreatedOn >= createdOnUpperBound).ToList()
                           : labs.Where(lab => lab.CreatedOn <= createdOnUpperBound).ToList();
            }
            return labs.ToList();
        }
    }
}