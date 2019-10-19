namespace Phoenix.Domain.RepositoryMaterials {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public class RepositoryMaterialsService : ServiceBase<RepositoryMaterial> {
        private static RepositoryMaterialsService _instanse;
        private RepositoryMaterialsService() {}

        public static RepositoryMaterialsService Instanse {
            get { return _instanse ?? (_instanse = new RepositoryMaterialsService()); }
        }

        public override bool Insert(RepositoryMaterial repositoryMaterial) {
            try {
                RepositoryMaterialsRepository.Instanse.InsertAndSubmit(repositoryMaterial);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.RepositoryMaterials,
                                                        Details =
                                                            Log.RepositoryMaterialDetailer(repositoryMaterial, ActionType.Created)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<RepositoryMaterial> Search(Func<RepositoryMaterial, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public IEnumerable<string> GetAllTargetApplicants(bool insertAll) {
            var tahvils =
                GetAll().Select(repositoryMaterial => repositoryMaterial.TargetApplicant).Where(x => x != null).Distinct
                    ();
            return insertAll
                       ? tahvils.Concat(new[] {RepositoryMaterialsAndItemsResources.All}).ToList()
                       : tahvils.ToList();
        }

        public override IList<RepositoryMaterial> GetAll() {
            return (from users in UsersRepository.Instanse.GetAll()
                    join repositoryMaterials in RepositoryMaterialsRepository.Instanse.GetAll() on users.UserID equals
                        repositoryMaterials.RegisteredBy
                    join materials in MaterialsRepository.Instanse.GetAll() on repositoryMaterials.MaterialID equals
                        materials.MaterialID into temp1
                    from temp1Items in temp1.DefaultIfEmpty()
                    join labs in LabsRepository.Instanse.GetAll() on repositoryMaterials.LabID equals labs.LabID into
                        temp
                    from tempItems in temp.DefaultIfEmpty()
                    select
                        new RepositoryMaterial {
                                                   StringAmount =
                                                       string.Format(
                                                           repositoryMaterials.Amount > 0
                                                               ? RepositoryMaterialsAndItemsResources.Requested
                                                               : RepositoryMaterialsAndItemsResources.Returned,
                                                           Math.Abs(repositoryMaterials.Amount),
                                                           temp1Items != null
                                                               ? ((ComputingUnit) temp1Items.Unit).ToUIString(true)
                                                               : ComputingUnit.Count.ToUIString(true)),
                                                   StringMaterialID = temp1Items != null ? temp1Items.Name : SharedResources.Unknown,
                                                   StringLabID = tempItems != null ? tempItems.Name : SharedResources.Unknown,
                                                   MaterialID = repositoryMaterials.MaterialID,
                                                   Amount = repositoryMaterials.Amount,
                                                   LabID = repositoryMaterials.LabID,
                                                   RegisteredOn = repositoryMaterials.RegisteredOn,
                                                   TargetApplicant = repositoryMaterials.TargetApplicant,
                                                   RegisteredBy = repositoryMaterials.RegisteredBy,
                                                   StringRegisteredBy = users.UserName
                                               }).OrderByDescending(
                                                   repositoryMaterial => repositoryMaterial.RegisteredOn).ToList();
        }

        public IEnumerable<RepositoryMaterial> GetAll(Guid materialID) {
            return materialID == Guid.Empty
                       ? GetAll()
                       : GetAll().Where(repositoryMaterial => repositoryMaterial.MaterialID == materialID).ToList();
        }

        public IList<RepositoryMaterial> GetAllOfLab(Guid labID) {
            return labID == Guid.Empty
                       ? GetAll()
                       : GetAll().Where(repositoryMaterial => repositoryMaterial.LabID == labID).ToList();
        }

        public void ValidateAmount(Guid materialID, int amount, out int result) {
            var material = MaterialsService.Get(materialID);
            var all = GetAll(materialID);
            var repositoryAmount = all.Sum(m => m.Amount);
            if (repositoryAmount + amount >= material.LowestAmount)
                result = -2;
            else if (repositoryAmount + amount >= 0)
                result = -1;
            else
                result = repositoryAmount;
        }

        public IList<RepositoryMaterial> GetAll(Guid? materialID, DateTime? registeredOnLowerBound,
                                                DateTime? registeredOnUpperBound, bool registeredOnOutside,
                                                string tahvilGirande, int? amountLowerBound, int? amountUpperBound,
                                                bool amountOutside) {
            var repositoryMayerials = GetAll();
            if (materialID.HasValue && materialID.Value != Guid.Empty) {
                repositoryMayerials =
                    repositoryMayerials.Where(repositoryMayerial => repositoryMayerial.MaterialID == materialID).ToList();
            }
            if (registeredOnLowerBound.HasValue) {
                registeredOnLowerBound = registeredOnLowerBound.Value.AddDays(-1);
                repositoryMayerials =
                    repositoryMayerials.Where(
                        repositoryMayerial =>
                        registeredOnOutside
                            ? repositoryMayerial.RegisteredOn <= registeredOnLowerBound
                            : repositoryMayerial.RegisteredOn >= registeredOnLowerBound).ToList();
            }
            if (registeredOnUpperBound.HasValue) {
                registeredOnUpperBound = registeredOnUpperBound.Value.AddDays(1);
                repositoryMayerials =
                    repositoryMayerials.Where(
                        repositoryMayerial =>
                        registeredOnOutside
                            ? repositoryMayerial.RegisteredOn >= registeredOnUpperBound
                            : repositoryMayerial.RegisteredOn <= registeredOnUpperBound).ToList();
            }
            if (!string.IsNullOrEmpty(tahvilGirande)) {
                repositoryMayerials =
                    repositoryMayerials.Where(
                        repositoryMayerial => repositoryMayerial.TargetApplicant.Contains(tahvilGirande)).ToList();
            }
            if (amountLowerBound.HasValue) {
                repositoryMayerials =
                    repositoryMayerials.Where(
                        repositoryMayerial =>
                        amountOutside
                            ? repositoryMayerial.Amount <= amountLowerBound
                            : repositoryMayerial.Amount >= amountLowerBound).ToList();
            }
            if (amountUpperBound.HasValue) {
                repositoryMayerials =
                    repositoryMayerials.Where(
                        repositoryMayerial =>
                        amountOutside
                            ? repositoryMayerial.Amount >= amountUpperBound
                            : repositoryMayerial.Amount <= amountUpperBound).ToList();
            }
            return repositoryMayerials.ToList();
        }

        public static int GetMaxAmount() {
            var repositoryMaterials = RepositoryMaterialsRepository.Instanse.GetAll();
            return repositoryMaterials.Count > 0
                       ? repositoryMaterials.Max(repositoryMaterial => repositoryMaterial.Amount)
                       : 0;
        }

        public static int GetMinAmount() {
            var repositoryMaterials = RepositoryMaterialsRepository.Instanse.GetAll();
            return repositoryMaterials.Count > 0
                       ? repositoryMaterials.Min(repositoryMaterial => repositoryMaterial.Amount)
                       : 0;
        }

        public static DateTime? GetMaxRegisteredOn() {
            var repositoryMaterials = RepositoryMaterialsRepository.Instanse.GetAll();
            return repositoryMaterials.Count > 0
                       ? (DateTime?) repositoryMaterials.Max(repositoryMaterial => repositoryMaterial.RegisteredOn)
                       : null;
        }

        public static DateTime? GetMinRegisteredOn() {
            var repositoryMaterials = RepositoryMaterialsRepository.Instanse.GetAll();
            return repositoryMaterials.Count > 0
                       ? (DateTime?) repositoryMaterials.Min(repositoryMaterial => repositoryMaterial.RegisteredOn)
                       : null;
        }

        public IList<RepositoryMaterial> GetAll(int? filterType) {
            if (filterType.HasValue) {
                switch (filterType) {
                    case 0:
                        return
                            GetAll().Where(
                                repositoryMaterial =>
                                repositoryMaterial.RegisteredOn >= DateTime.Now.AddDays(-1) &&
                                repositoryMaterial.RegisteredOn <= DateTime.Now).ToList();
                    case 1:
                        return
                            GetAll().Where(
                                repositoryMaterial =>
                                repositoryMaterial.RegisteredOn >= DateTime.Now.AddDays(-2) &&
                                repositoryMaterial.RegisteredOn <= DateTime.Now).ToList();
                    case 2:
                        return
                            GetAll().Where(
                                repositoryMaterial =>
                                repositoryMaterial.RegisteredOn >= DateTime.Now.AddDays(-8) &&
                                repositoryMaterial.RegisteredOn <= DateTime.Now).ToList();
                    case 3:
                        return
                            GetAll().Where(
                                repositoryMaterial =>
                                repositoryMaterial.RegisteredOn >= DateTime.Now.AddDays(-32) &&
                                repositoryMaterial.RegisteredOn <= DateTime.Now).ToList();
                }
            }
            return GetAll().ToList();
        }

        public static void Remove(Guid materialID) {
            foreach (var material in RepositoryMaterialsRepository.Instanse.GetAll(rm => rm.MaterialID == materialID))
                RepositoryMaterialsRepository.Instanse.DeleteAndSubmit(material);
        }

        public override bool Remove(RepositoryMaterial repositoryMaterial) {
            try {
                RepositoryMaterialsRepository.Instanse.DeleteAndSubmit(repositoryMaterial);
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }
    }
}