namespace Phoenix.Domain.RepositoryItems {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;

    [Table(Name = "Phoenix.RepositoryItems")]
    public sealed class RepositoryItem {
        public RepositoryItem() {
            RegisteredOn = DateTime.Now;
            RegisteredBy = AppContext.Instanse.User.UserID;
        }

        [Column(IsPrimaryKey = true)]
        public Guid ItemID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryItem), "ItemID")]
        public string StringItemID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryItem), "RegisteredBy")]
        public string StringRegisteredBy { get; set; }

        [PhoenixDisplayName(typeof (RepositoryItem), "LabName")]
        public string StringLabID { get; set; }

        [PhoenixDisplayName(typeof (RepositoryItem), "TargetApplicant")]
        [Column]
        public string TargetApplicant { get; set; }

        [Column(IsPrimaryKey = true)]
        public DateTime RegisteredOn { get; set; }

        [PhoenixDisplayName(typeof (RepositoryItem), "RegisteredOn")]
        public string StringRegisteredOn {
            get { return RegisteredOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (RepositoryItem), "Count")]
        [Column]
        public int Count { get; set; }

        public string StringCount { get; set; }

        [Column]
        public Guid RegisteredBy { get; set; }

        [Column]
        public Guid? LabID { get; set; }
    }
}