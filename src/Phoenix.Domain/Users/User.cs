namespace Phoenix.Domain.Users {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Users")]
    public sealed class User {
        private string _description;

        public User() {
            CreatedOn = LastModifiedOn = DateTime.Now;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid UserID { get; set; }

        [PhoenixDisplayName(typeof (User), "UserName")]
        [Column(CanBeNull = false)]
        public string UserName { get; set; }

        [PhoenixDisplayName(typeof (User), "Password")]
        [Column(CanBeNull = false)]
        public string Password { get; set; }

        [PhoenixDisplayName(typeof (User), "Name")]
        [Column]
        public string Name { get; set; }

        [Column]
        public Guid RoleID { get; set; }

        [PhoenixDisplayName(typeof (User), "Role")]
        public string StringRoleID { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [PhoenixDisplayName(typeof (User), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn { get; set; }

        [PhoenixDisplayName(typeof (User), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [Column]
        public bool LockedOut { get; set; }

        [PhoenixDisplayName(typeof (User), "LockedOut2")]
        public string StringLockedOut {
            get { return LockedOut ? SharedResources.Yes : SharedResources.No; }
        }

        [PhoenixDisplayName(typeof (User), "Description")]
        [Column]
        public string Description {
            get { return _description; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        public string GetScreenName() {
            return Name ?? UserName;
        }
    }
}