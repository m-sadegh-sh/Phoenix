namespace Phoenix.Domain.PropStatusChanges {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.PropStatusChanges")]
    public sealed class PropStatusChange {
        private string _description;

        public PropStatusChange() {
            ReportedOn = LastModifiedOn = DateTime.Now;
            IsAlive = true;
            ReportedBy = AppContext.Instanse.User.UserID;
        }

        [Column(IsPrimaryKey = true)]
        public Guid PropID { get; set; }

        [Column(IsPrimaryKey = true)]
        public DateTime ReportedOn { get; set; }

        [Column]
        public Guid ReportedBy { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "ReportedBy")]
        public string StringReportedBy { get; set; }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [Column]
        public short Type { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "Type")]
        public string StringType {
            get { return ((ReportType) Type).ToUIString(); }
        }

        [Column]
        public bool IsAlive { get; set; }

        [Column]
        public DateTime? ChangedOn { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "ChangedOn")]
        public string StringChangedOn {
            get { return ChangedOn.HasValue ? ChangedOn.Value.ToLocalized() : SharedResources.Unknown; }
        }

        [Column]
        public DateTime? ResolveDate { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "ResolveDate")]
        public string StringResolveDate {
            get { return ResolveDate.HasValue ? ResolveDate.Value.ToLocalized() : SharedResources.Unknown; }
        }

        [Column]
        public Guid? ClosedBy { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "ClosedBy")]
        public string StringClosedBy { get; set; }

        [Column]
        public DateTime? ClosedOn { get; set; }

        [PhoenixDisplayName(typeof (PropStatusChange), "ClosedOn")]
        public string StringClosedOn {
            get { return ClosedOn.HasValue ? ClosedOn.Value.ToLocalized() : SharedResources.Unknown; }
        }

        [PhoenixDisplayName(typeof (PropStatusChange), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }
    }
}