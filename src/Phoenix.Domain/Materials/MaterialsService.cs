namespace Phoenix.Domain.Materials {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class MaterialsService : ServiceBase<Material> {
        public static MaterialsService Instanse {
            get { return new MaterialsService(); }
        }

        public static bool Exist(string name) {
            return MaterialsRepository.Instanse.Get(material => string.Compare(material.Name, name, true) == 0) != null;
        }

        public static bool Exist(string name, Material current) {
            return
                MaterialsRepository.Instanse.GetAll(
                    material =>
                    string.Compare(material.Name, name, true) == 0 && material.MaterialID != current.MaterialID).Count >
                0;
        }

        public static bool Insert(Material material, int initialAmount = 0) {
            try {
                if (Get(material.MaterialID) == null) {
                    MaterialsRepository.Instanse.InsertOrUpdateAndSubmit(material);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Materials,
                                                            Details = Log.MaterialDetailer(material, ActionType.Created)
                                                        });
                    if (initialAmount > 0) {
                        RepositoryMaterialsService.Instanse.Insert(new RepositoryMaterial {
                                                                                              MaterialID = material.MaterialID,
                                                                                              Amount = initialAmount
                                                                                          });
                    }
                    return true;
                }
                material.LastModifiedOn = DateTime.Now;
                MaterialsRepository.Instanse.InsertOrUpdateAndSubmit(material);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Materials,
                                                        Details = Log.MaterialDetailer(material, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static bool ChangeNotifiable(Guid materialID, bool notifiable) {
            var material = Get(materialID);
            if (material != null) {
                material.Notifiable = notifiable;
                MaterialsRepository.Instanse.InsertOrUpdateAndSubmit(material);
                return true;
            }
            return false;
        }

        public static Material Get(Guid materialID) {
            return MaterialsRepository.Instanse.Get(new Material {MaterialID = materialID});
        }

        public override IList<Material> Search(Func<Material, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<Material> GetAll() {
            return (from materials in MaterialsRepository.Instanse.GetAll()
                    join categories in CategoriesService.Instanse.GetAll() on materials.CategoryID equals
                        categories.CategoryID into temp
                    from tempItems in temp.DefaultIfEmpty()
                    select
                        new Material {
                                         Notifiable = materials.Notifiable,
                                         CategoryID = materials.CategoryID,
                                         CreatedOn = materials.CreatedOn,
                                         Description = materials.Description,
                                         LastModifiedOn = materials.LastModifiedOn,
                                         Name = materials.Name,
                                         MaterialID = materials.MaterialID,
                                         StringCategoryID = tempItems != null ? tempItems.Name : SharedResources.Unknown,
                                         LowestAmount = materials.LowestAmount,
                                         CurrentAmount =
                                             RepositoryMaterialsService.Instanse.GetAll(materials.MaterialID).Sum(
                                                 repositoryMaterial => repositoryMaterial.Amount),
                                         Unit = materials.Unit
                                     }).OrderBy(material => material.Name).ToList();
        }

        public override bool Remove(Material material) {
            try {
                material = Get(material.MaterialID);
                if (material != null) {
                    foreach (var reopMaterial in
                        RepositoryMaterialsRepository.Instanse.GetAll(
                            repoMat => repoMat.MaterialID == material.MaterialID))
                        RepositoryMaterialsService.Remove(reopMaterial.MaterialID);

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Materials,
                                                            Details = Log.MaterialDetailer(material, ActionType.Removed)
                                                        });
                    MaterialsRepository.Instanse.DeleteAndSubmit(material);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override bool Insert(Material entity) {
            return false;
        }

        public IList<Material> GetAllContains(string name) {
            return GetAll().Where(material => material.Name.Contains(name)).OrderBy(material => material.Name).ToList();
        }

        public static DateTime? GetMinCreatedOn() {
            var materials = MaterialsRepository.Instanse.GetAll();
            return materials.Count > 0 ? (DateTime?) materials.Min(material => material.CreatedOn) : null;
        }

        public static DateTime? GetMaxCreatedOn() {
            var materials = MaterialsRepository.Instanse.GetAll();
            return materials.Count > 0 ? (DateTime?) materials.Max(material => material.CreatedOn) : null;
        }

        public IList<Material> GetAllLowMaterialsAmount() {
            return
                GetAll().Where(material => material.LowestAmount > material.CurrentAmount && material.Notifiable).ToList
                    ();
        }

        public static int CountOfNotNotifiable() {
            return MaterialsRepository.Instanse.GetAll(material => !material.Notifiable).Count;
        }

        public static IEnumerable<Guid> GetAllNotNotifiables() {
            return
                MaterialsRepository.Instanse.GetAll(material => !material.Notifiable).Select(
                    material => material.MaterialID).ToList();
        }

        public IList<Material> GetAll(Guid? categoryID, Guid? materialID, DateTime? createdOnLowerBound,
                                      DateTime? createdOnUpperBound, bool createdOnOutside, int? lowestAmountLowerBound,
                                      int? lowestAmountUpperBound, bool lowestAmountOutside) {
            var materials = GetAll();
            if (categoryID.HasValue && categoryID != Guid.Empty)
                materials = materials.Where(material => material.CategoryID == categoryID).ToList();
            if (materialID.HasValue && materialID.Value != Guid.Empty)
                materials = materials.Where(material => material.MaterialID == materialID).ToList();
            if (createdOnLowerBound.HasValue) {
                createdOnLowerBound = createdOnLowerBound.Value.AddDays(-1);
                materials =
                    materials.Where(
                        material =>
                        createdOnOutside
                            ? material.CreatedOn <= createdOnLowerBound
                            : material.CreatedOn >= createdOnLowerBound).ToList();
            }
            if (createdOnUpperBound.HasValue) {
                createdOnUpperBound = createdOnUpperBound.Value.AddDays(1);
                materials =
                    materials.Where(
                        material =>
                        createdOnOutside
                            ? material.CreatedOn >= createdOnUpperBound
                            : material.CreatedOn <= createdOnUpperBound).ToList();
            }
            if (lowestAmountLowerBound.HasValue) {
                materials =
                    materials.Where(
                        material =>
                        lowestAmountOutside
                            ? material.LowestAmount <= lowestAmountLowerBound
                            : material.LowestAmount >= lowestAmountLowerBound).ToList();
            }
            if (lowestAmountUpperBound.HasValue) {
                materials =
                    materials.Where(
                        material =>
                        lowestAmountOutside
                            ? material.LowestAmount >= lowestAmountUpperBound
                            : material.LowestAmount <= lowestAmountUpperBound).ToList();
            }

            return materials.ToList();
        }

        public static int GetMaxLowestAmount() {
            var materials = MaterialsRepository.Instanse.GetAll();
            return materials.Count > 0 ? materials.Max(material => material.LowestAmount) : 0;
        }

        public static int GetMinLowestAmount() {
            var materials = MaterialsRepository.Instanse.GetAll();
            return materials.Count > 0 ? materials.Min(material => material.LowestAmount) : 0;
        }
    }
}