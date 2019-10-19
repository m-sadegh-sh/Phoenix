namespace Phoenix.Domain.Categories {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class CategoriesService : ServiceBase<Category> {
        public static CategoriesService Instanse {
            get { return new CategoriesService(); }
        }

        public override IList<Category> Search(Func<Category, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<Category> GetAll() {
            return CategoriesRepository.Instanse.GetAll().OrderBy(category => category.Name).ToList();
        }

        public override bool Insert(Category category) {
            try {
                if (category.CategoryID == Guid.Empty) {
                    CategoriesRepository.Instanse.InsertOrUpdateAndSubmit(category);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Categories,
                                                            Details = Log.CategoryDetailer(category, ActionType.Created)
                                                        });
                    return true;
                }
                category.LastModifiedOn = DateTime.Now;
                CategoriesRepository.Instanse.InsertOrUpdateAndSubmit(category);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Categories,
                                                        Details = Log.CategoryDetailer(category, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        private static Category Get(Guid categoryID) {
            return CategoriesRepository.Instanse.Get(new Category {CategoryID = categoryID});
        }

        public IList<Category> GetAllContains(string name) {
            return GetAll().Where(category => category.Name.Contains(name)).OrderBy(category => category.Name).ToList();
        }

        public static bool ReferencedToOther(Category category, out int count) {
            return
                (count =
                 PropsRepository.Instanse.GetAll(prop => prop.CategoryID == category.CategoryID).Count +
                 MaterialsRepository.Instanse.GetAll(material => material.CategoryID == category.CategoryID).Count) > 0;
        }

        public override bool Remove(Category category) {
            try {
                category = Get(category.CategoryID);
                if (category != null) {
                    foreach (var referencedProp in
                        PropsRepository.Instanse.GetAll(prop => prop.CategoryID == category.CategoryID)) {
                        referencedProp.CategoryID = null;
                        PropsService.Instanse.Insert(referencedProp);
                    }
                    foreach (var referencedMaterial in
                        MaterialsRepository.Instanse.GetAll(material => material.CategoryID == category.CategoryID)) {
                        referencedMaterial.CategoryID = null;
                        MaterialsService.Insert(referencedMaterial);
                    }

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Categories,
                                                            Details = Log.CategoryDetailer(category, ActionType.Removed)
                                                        });
                    CategoriesRepository.Instanse.DeleteAndSubmit(category);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static bool Exist(string name) {
            return CategoriesRepository.Instanse.Get(category => string.Compare(category.Name, name, true) == 0) != null;
        }

        public static bool Exist(string name, Category current) {
            var x = CategoriesRepository.Instanse.GetAll(category => string.Compare(category.Name, name, true) == 0);
            var y = x.Where(category => string.Compare(category.Name, current.Name, true) != 0).ToList();
            return y.Count > 0;
        }

        public IList<Category> GetAllAndAppendDefault() {
            var all = GetAll();
            return all.Count != 0
                       ? new[] {new Category {Name = CategoriesResources.All}}.Concat(all).ToList()
                       : new Category[] {}.ToList();
        }
    }
}