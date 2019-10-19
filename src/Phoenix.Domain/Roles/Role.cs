namespace Phoenix.Domain.Roles {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Roles")]
    public sealed class Role {
        private string _description;

        public Role() {
            CreatedOn = LastModifiedOn = DateTime.Now;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid RoleID { get; set; }

        public string AccessibilityPercent {
            get { return ComputeAccessibilityPercent(); }
        }

        [PhoenixDisplayName(typeof (Role), "Name")]
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (Role), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (Role), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (Role), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        [Column]
        public bool CategoriesDisplay { get; set; }

        [Column]
        public bool CategoriesInsert { get; set; }

        [Column]
        public bool CategoriesUpdate { get; set; }

        [Column]
        public bool CategoriesDelete { get; set; }

        [Column]
        public bool PropsDisplay { get; set; }

        [Column]
        public bool PropsInsert { get; set; }

        [Column]
        public bool PropsUpdate { get; set; }

        [Column]
        public bool PropsDelete { get; set; }

        [Column]
        public bool PropsSearch { get; set; }

        [Column]
        public bool PropStatusDisplay { get; set; }

        [Column]
        public bool PropStatusUpdate { get; set; }

        [Column]
        public bool MaterialsDisplay { get; set; }

        [Column]
        public bool MaterialsInsert { get; set; }

        [Column]
        public bool MaterialsUpdate { get; set; }

        [Column]
        public bool MaterialsDelete { get; set; }

        [Column]
        public bool MaterialsSearch { get; set; }

        [Column]
        public bool ItemsSearch { get; set; }

        [Column]
        public bool RepositoryMaterialsAndItemsInsert { get; set; }

        [Column]
        public bool RepositoryMaterialsAndItemsDelete { get; set; }

        [Column]
        public bool RepositoryMaterialsSearch { get; set; }

        [Column]
        public bool RepositoryItemsSearch { get; set; }

        [Column]
        public bool LabsDisplay { get; set; }

        [Column]
        public bool LabsInsert { get; set; }

        [Column]
        public bool LabsUpdate { get; set; }

        [Column]
        public bool LabsDelete { get; set; }

        [Column]
        public bool LabsSearch { get; set; }

        [Column]
        public bool LabPropsDisplay { get; set; }

        [Column]
        public bool LabPropsInsert { get; set; }

        [Column]
        public bool LabPropsDelete { get; set; }

        [Column]
        public bool LabPropsSearch { get; set; }

        [Column]
        public bool UsersDisplay { get; set; }

        [Column]
        public bool UsersInsert { get; set; }

        [Column]
        public bool UsersUpdate { get; set; }

        [Column]
        public bool UsersDelete { get; set; }

        [Column]
        public bool SearchDisplay { get; set; }

        [Column]
        public bool RolesDisplay { get; set; }

        [Column]
        public bool RolesInsert { get; set; }

        [Column]
        public bool RolesUpdate { get; set; }

        [Column]
        public bool RolesDelete { get; set; }

        [Column]
        public bool LogsDisplay { get; set; }

        [Column]
        public bool LogsDelete { get; set; }

        [Column]
        public bool LogsSearch { get; set; }

        [Column]
        public bool BackupGenerate { get; set; }

        [Column]
        public bool RestorePerform { get; set; }

        [Column]
        public bool ItemsDisplay { get; set; }

        [Column]
        public bool ItemsDelete { get; set; }

        [Column]
        public bool ItemsInsert { get; set; }

        [Column]
        public bool ItemsUpdate { get; set; }

        private string ComputeAccessibilityPercent() {
            var score = 0.0;
            var maxScore = 0.0;

            if (CategoriesDisplay)
                score += 10;
            maxScore += 10;
            if (CategoriesInsert)
                score += 25;
            maxScore += 25;
            if (CategoriesUpdate)
                score += 20;
            maxScore += 20;
            if (CategoriesDelete)
                score += 30;
            maxScore += 30;

            if (PropsDisplay)
                score += 10;
            maxScore += 10;
            if (PropsInsert)
                score += 25;
            maxScore += 25;
            if (PropsUpdate)
                score += 20;
            maxScore += 20;
            if (PropsDelete)
                score += 30;
            maxScore += 30;

            if (PropStatusDisplay)
                score += 10;
            maxScore += 10;
            if (PropStatusUpdate)
                score += 20;
            maxScore += 20;

            if (MaterialsDisplay)
                score += 10;
            maxScore += 10;
            if (MaterialsInsert)
                score += 25;
            maxScore += 25;
            if (MaterialsUpdate)
                score += 20;
            maxScore += 20;
            if (MaterialsDelete)
                score += 30;
            maxScore += 30;

            if (ItemsDisplay)
                score += 10;
            maxScore += 10;
            if (ItemsInsert)
                score += 25;
            maxScore += 25;
            if (ItemsUpdate)
                score += 20;
            maxScore += 20;
            if (ItemsDelete)
                score += 30;
            maxScore += 30;

            if (RepositoryMaterialsAndItemsInsert)
                score += 25;
            maxScore += 25;
            if (RepositoryMaterialsAndItemsDelete)
                score += 30;
            maxScore += 30;

            if (LabsDisplay)
                score += 10;
            maxScore += 10;
            if (LabsInsert)
                score += 25;
            maxScore += 25;
            if (LabsUpdate)
                score += 20;
            maxScore += 20;
            if (LabsDelete)
                score += 30;
            maxScore += 30;

            if (LabPropsDisplay)
                score += 10;
            maxScore += 10;
            if (LabPropsInsert)
                score += 25;
            maxScore += 25;
            if (LabPropsDelete)
                score += 30;
            maxScore += 30;

            if (UsersDisplay)
                score += 25;
            maxScore += 25;
            if (UsersInsert)
                score += 40;
            maxScore += 40;
            if (UsersUpdate)
                score += 40;
            maxScore += 40;
            if (UsersDelete)
                score += 50;
            maxScore += 50;

            if (RolesDisplay)
                score += 10;
            maxScore += 10;
            if (RolesInsert)
                score += 25;
            maxScore += 25;
            if (RolesUpdate)
                score += 20;
            maxScore += 20;
            if (RolesDelete)
                score += 30;
            maxScore += 30;

            if (LogsDisplay)
                score += 40;
            maxScore += 40;
            if (LogsDelete)
                score += 50;
            maxScore += 50;

            if (SearchDisplay)
                score += 50;
            maxScore += 50;
            if (PropsSearch)
                score += 15;
            maxScore += 15;
            if (LabsSearch)
                score += 15;
            maxScore += 15;
            if (LogsSearch)
                score += 15;
            maxScore += 15;
            if (MaterialsSearch)
                score += 15;
            maxScore += 15;
            if (ItemsSearch)
                score += 15;
            maxScore += 15;
            if (RepositoryMaterialsSearch)
                score += 15;
            maxScore += 15;

            var result = (score/maxScore)*100;
            if (double.IsNaN(result) || result <= 0.0)
                return RolesResources.ZeroAccessibility;
            if (result >= 100.0)
                return RolesResources.FullAccessibility;
            return string.Format(RolesResources.AboutNPercent, (int) result);
        }
    }
}