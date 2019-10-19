namespace Phoenix.Domain.Props {
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Props")]
    public sealed class Prop {
        private string _description;
        private string _stringCategoryID;
        private string _stringLabID;

        public Prop() {
            CreatedOn = LastModifiedOn = DateTime.Now;
            Notifiable = true;
        }

        [Browsable(false)]
        public string StringLabID {
            get { return _stringLabID ?? SharedResources.Unknown; }
            set { _stringLabID = value; }
        }

        [Browsable(false)]
        [Column]
        public bool Notifiable { get; set; }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid PropID { get; set; }

        [PhoenixDisplayName(typeof (Prop), "Name")]
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [PhoenixDisplayName(typeof (Prop), "PropNo")]
        [Column]
        public int? PropNo { get; set; }

        [PhoenixDisplayName(typeof (Prop), "PropNo")]
        public string StringPropNo {
            get { return PropNo.HasValue ? PropNo.ToString() : SharedResources.Unknown; }
        }

        [PhoenixDisplayName(typeof (Prop), "SerialNo")]
        [Column]
        public string SerialNo { get; set; }

        [PhoenixDisplayName(typeof (Prop), "SerialNo")]
        public string StringSerialNo {
            get { return string.IsNullOrWhiteSpace(SerialNo) ? SharedResources.Unknown : SerialNo; }
        }

        [Column]
        public DateTime? PurchasingDate { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (Prop), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (Prop), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (Prop), "PurchasingDate")]
        public string StringPurchasingDate {
            get { return PurchasingDate.HasValue ? PurchasingDate.Value.ToLocalized() : SharedResources.Unknown; }
        }

        [Column]
        public DateTime? WarrantyExpirationDate { get; set; }

        [PhoenixDisplayName(typeof (Prop), "WarrantyExpirationDate")]
        public string StringWarrantyExpirationDate {
            get {
                return WarrantyExpirationDate.HasValue
                           ? WarrantyExpirationDate.Value.ToLocalized()
                           : SharedResources.Unknown;
            }
        }

        [Column]
        public Guid? CategoryID { get; set; }

        [PhoenixDisplayName(typeof (Prop), "CategoryName")]
        public string StringCategoryID {
            get { return _stringCategoryID ?? SharedResources.Unknown; }
            set { _stringCategoryID = value; }
        }

        public short Status { get; set; }

        [PhoenixDisplayName(typeof (Prop), "Status")]
        public string StringStatus {
            get { return ((ReportType) Status).ToUIString(); }
        }

        [PhoenixDisplayName(typeof (Prop), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        [PhoenixDisplayName(typeof (Prop), "Description")]
        public string StringDescription {
            get { return string.IsNullOrWhiteSpace(Description) ? SharedResources.Unknown : Description; }
        }
    }
}