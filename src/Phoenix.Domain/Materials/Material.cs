namespace Phoenix.Domain.Materials {
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Materials")]
    public sealed class Material {
        private string _description;
        private string _stringCategoryID;

        public Material() {
            CreatedOn = LastModifiedOn = DateTime.Now;
            Notifiable = true;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid MaterialID { get; set; }

        [Browsable(false)]
        [Column]
        public bool Notifiable { get; set; }

        [PhoenixDisplayName(typeof (Material), "Name")]
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column]
        public int LowestAmount { get; set; }

        [PhoenixDisplayName(typeof (Material), "LowestAmount")]
        public string StringLowestAmount {
            get { return LowestAmount + " " + StringUnit; }
        }

        public int CurrentAmount { get; set; }

        [PhoenixDisplayName(typeof (Material), "Formula")]
        [Column]
        public string Formula { get; set; }

        [PhoenixDisplayName(typeof (Material), "MolecularMass")]
        [Column]
        public string MolecularMass { get; set; }

        [PhoenixDisplayName(typeof (Material), "CurrentAmount")]
        public string StringCurrentAmount {
            get { return CurrentAmount + " " + StringUnit; }
        }

        [Column]
        public short Unit { get; set; }

        [Browsable(false)]
        [PhoenixDisplayName(typeof (Material), "ComputingUnit")]
        public string StringUnit {
            get { return ((ComputingUnit) Unit).ToUIString(true); }
        }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (Material), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (Material), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [Column]
        public Guid? CategoryID { get; set; }

        [PhoenixDisplayName(typeof (Material), "CategoryName")]
        public string StringCategoryID {
            get { return string.IsNullOrWhiteSpace(_stringCategoryID) ? SharedResources.Unknown : _stringCategoryID; }
            set { _stringCategoryID = value; }
        }

        [PhoenixDisplayName(typeof (Material), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        [PhoenixDisplayName(typeof (Material), "Description")]
        public string StringDescription {
            get { return string.IsNullOrWhiteSpace(Description) ? SharedResources.Unknown : Description; }
        }
    }
}