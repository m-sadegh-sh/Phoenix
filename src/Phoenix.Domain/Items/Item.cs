namespace Phoenix.Domain.Items {
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    using Phoenix.Domain.Materials;
    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Items")]
    public sealed class Item {
        private string _description;
        private string _stringCategoryID;

        public Item() {
            CreatedOn = LastModifiedOn = DateTime.Now;
            Notifiable = true;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid ItemID { get; set; }

        [Browsable(false)]
        [Column]
        public bool Notifiable { get; set; }

        [PhoenixDisplayName(typeof (Item), "Name")]
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column]
        public int LowestCount { get; set; }

        [PhoenixDisplayName(typeof (Item), "LowestCount")]
        public string StringLowestCount {
            get { return LowestCount + " " + ComputingUnit.Count.ToUIString(true); }
        }

        public int CurrentCount { get; set; }

        [PhoenixDisplayName(typeof (Item), "CurrentCount")]
        public string StringCurrentCount {
            get { return CurrentCount + " " + ComputingUnit.Count.ToUIString(true); }
        }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (Item), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (Item), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [Column]
        public Guid? CategoryID { get; set; }

        [PhoenixDisplayName(typeof (Item), "CategoryName")]
        public string StringCategoryID {
            get { return string.IsNullOrWhiteSpace(_stringCategoryID) ? SharedResources.Unknown : _stringCategoryID; }
            set { _stringCategoryID = value; }
        }

        [PhoenixDisplayName(typeof (Item), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        [PhoenixDisplayName(typeof (Item), "Description")]
        public string StringDescription {
            get { return string.IsNullOrWhiteSpace(Description) ? SharedResources.Unknown : Description; }
        }
    }
}