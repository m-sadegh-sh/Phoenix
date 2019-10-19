namespace Phoenix.Domain.Labs {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Labs")]
    public sealed class Lab {
        private string _description;
        private string _stringCategoryID;

        public Lab() {
            CreatedOn = LastModifiedOn = DateTime.Now;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid LabID { get; set; }

        [PhoenixDisplayName(typeof (Lab), "CategoryName")]
        public string StringCategoryID {
            get { return _stringCategoryID ?? SharedResources.Unknown; }
            set { _stringCategoryID = value; }
        }

        [Column]
        public Guid? CategoryID { get; set; }

        [PhoenixDisplayName(typeof (Lab), "Name")]
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [PhoenixDisplayName(typeof (Lab), "PlaqueNo")]
        [Column(AutoSync = AutoSync.Always, IsDbGenerated = true)]
        public int PlaqueNo { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (Lab), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (Lab), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (Lab), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        [PhoenixDisplayName(typeof (Lab), "Description")]
        public string StringDescription {
            get { return string.IsNullOrWhiteSpace(Description) ? SharedResources.Unknown : Description; }
        }

        public int CountOfProps { get; set; }
    }
}