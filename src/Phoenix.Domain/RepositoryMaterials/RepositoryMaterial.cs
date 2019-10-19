namespace Phoenix.Domain.RepositoryMaterials {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;

    [Table(Name = "Phoenix.RepositoryMaterials")]
    public sealed class RepositoryMaterial {
        public RepositoryMaterial() {
            RegisteredOn = DateTime.Now;
            RegisteredBy = AppContext.Instanse.User.UserID;
        }

        [Column(IsPrimaryKey = true)]
        public Guid MaterialID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "MaterialID")]
        public string StringMaterialID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "RegisteredBy")]
        public string StringRegisteredBy { get; set; }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "LabName")]
        public string StringLabID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "TargetApplicant")]
        [Column]
        public string TargetApplicant { get; set; }

        [Column(IsPrimaryKey = true)]
        public DateTime RegisteredOn { get; set; }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "RegisteredOn")]
        public string StringRegisteredOn {
            get { return RegisteredOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (RepositoryMaterial), "Amount")]
        [Column]
        public int Amount { get; set; }

        public string StringAmount { get; set; }

        [Column]
        public Guid RegisteredBy { get; set; }

        [Column]
        public Guid? LabID { get; set; }
    }
}