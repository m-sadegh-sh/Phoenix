namespace Phoenix.Domain.Categories {
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    [Table(Name = "Phoenix.Categories")]
    public sealed class Category : INotifyPropertyChanged, ICloneable {
        private Guid _categoryID;
        private DateTime _createdOn;
        private string _description;
        private DateTime _lastModifiedOn;
        private string _name;

        public Category() {
            CreatedOn = LastModifiedOn = DateTime.Now;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid CategoryID {
            get { return _categoryID; }
            set {
                ValidationHelper.ValidateProperty(value, () => CategoryID);
                _categoryID = value;
                PropertyChanged.Raise(() => CategoryID);
            }
        }

        [PhoenixDisplayName(typeof (Category), "Name")]
        [LocalizableRequired(typeof (Category), "Name")]
        [LocalizableStringLength(typeof (Category), "Name", 1, 64)]
        [Column(CanBeNull = false)]
        public string Name {
            get { return _name; }
            set {
                ValidationHelper.ValidateProperty(value, () => Name);
                _name = value;
                PropertyChanged.Raise(() => Name);
            }
        }

        [Column]
        public DateTime CreatedOn {
            get { return _createdOn; }
            set {
                ValidationHelper.ValidateProperty(value, () => CreatedOn);
                _createdOn = value;
                PropertyChanged.Raise(() => CreatedOn);
            }
        }

        [PhoenixDisplayName(typeof (Category), "CreatedOn")]
        public string StringCreatedOn {
            get { return CreatedOn.ToLocalized(); }
        }

        [Column]
        public DateTime LastModifiedOn {
            get { return _lastModifiedOn; }
            set {
                ValidationHelper.ValidateProperty(value, () => LastModifiedOn);
                _lastModifiedOn = value;
                PropertyChanged.Raise(() => LastModifiedOn);
            }
        }

        [PhoenixDisplayName(typeof (Category), "LastModifiedOn")]
        public string StringLastModifiedOn {
            get { return LastModifiedOn.ToLocalized(); }
        }

        [PhoenixDisplayName(typeof (Category), "Description")]
        [Column]
        [LocalizableStringLength(typeof (Category), "Description", 0, 1024)]
        public string Description {
            get { return _description; }
            set {
                ValidationHelper.ValidateProperty(value, () => Description);
                _description = value;
                PropertyChanged.Raise(() => Description);
            }
        }

        [PhoenixDisplayName(typeof (Category), "Description")]
        public string StringDescription {
            get { return string.IsNullOrWhiteSpace(Description) ? SharedResources.Unknown : Description; }
        }

        #region ICloneable Members
        public object Clone() {
            return new Category {
                                    CategoryID = CategoryID,
                                    CreatedOn = CreatedOn,
                                    Description = Description,
                                    Name = Name,
                                    LastModifiedOn = LastModifiedOn
                                };
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}