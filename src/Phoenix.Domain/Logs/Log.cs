namespace Phoenix.Domain.Logs {
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    using GalaSoft.MvvmLight;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.Items;
    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Domain.Props;
    using Phoenix.Domain.RepositoryItems;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Domain.Roles;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;

    [Table(Name = "Phoenix.Logs")]
    public sealed class Log : ICloneable {
        public Log() {
            LoggedOn = DateTime.Now;
            if (!ViewModelBase.IsInDesignModeStatic) {
                PerformedBy = AppContext.Instanse.User != null
                                  ? AppContext.Instanse.User.UserID
                                  : UsersService.Instanse.GetSystemUser().UserID;
            }
        }

        [PhoenixDisplayName(typeof (Log), "Details")]
        [Column]
        public string Details { get; set; }

        [Browsable(false)]
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid LogID { get; set; }

        [Browsable(false)]
        [Column]
        public DateTime LoggedOn { get; set; }

        [PhoenixDisplayName(typeof (Log), "HostTable")]
        public string StringHostTable {
            get { return ((HostTable) HostTable).ToUIString(); }
        }

        [PhoenixDisplayName(typeof (Log), "LoggedOn")]
        public string StringLoggedOn {
            get { return LoggedOn.ToLocalized(); }
        }

        [Column]
        public short HostTable { get; set; }

        [Column]
        public Guid PerformedBy { get; set; }

        [PhoenixDisplayName(typeof (Log), "PerformedBy")]
        public string StringPerformedBy { get; set; }

        #region ICloneable Members
        public object Clone() {
            return new Log {
                               Details = Details,
                               HostTable = HostTable,
                               LoggedOn = LoggedOn,
                               LogID = LogID,
                               PerformedBy = PerformedBy
                           };
        }
        #endregion

        public static string CategoryDetailer(Category category, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("گروهی با نام {0} ایجاد شد.", category.Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات گروهه با نام {0} بروز شد.", category.Name);
                case ActionType.Removed:
                    return string.Format("گروهی با نام {0} حذف شد.", category.Name);
                default:
                    return null;
            }
        }

        public static string ExceptionDetailer(Exception exception) {
            return string.Format("پیغام خطا: {0}", exception.Message);
        }

        public static string LabPropDetailer(LabProp labProp, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("{0} به لیست اموال {1} اضافه شد.", PropsService.Get(labProp.PropID).Name,
                                         LabsService.Get(labProp.LabID).Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات اموال {0} بروز شد.", LabsService.Get(labProp.LabID).Name);
                case ActionType.Removed:
                    return string.Format("{0} از لیست اموال {1} حذف شد.", PropsService.Get(labProp.PropID).Name,
                                         LabsService.Get(labProp.LabID).Name);
                default:
                    return null;
            }
        }

        public static string LabDetailer(Lab lab, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("آزمایشگاهی با نام {0} ایجاد شد.", lab.Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات آزمایشگاهی با نام {0} بروز شد.", lab.Name);
                case ActionType.Removed:
                    return string.Format("آزمایشگاهی با نام {0} حذف شد.", lab.Name);
                default:
                    return null;
            }
        }

        public static string PropDetailer(Prop prop, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("اموالی با نام {0}، شماره اموال {1} و سریال {2} ایجاد شد.", prop.Name,
                                         prop.PropNo, prop.SerialNo);
                case ActionType.Modified:
                    return string.Format("اطلاعات اموالی با نام {0}، شماره اموال {1} و سریال {2} بروز شد.", prop.Name,
                                         prop.PropNo, prop.SerialNo);
                case ActionType.Removed:
                    return string.Format("اموالی با نام {0}، شماره اموال {1} و سریال {2} حذف شد.", prop.Name,
                                         prop.PropNo, prop.SerialNo);
                default:
                    return null;
            }
        }

        internal static string UserDetailer(User user, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("کاربری با نام {0} ایجاد شد.", user.UserName);
                case ActionType.Modified:
                    return string.Format("اطلاعات کاربری با نام {0} بروز شد.", user.UserName);
                case ActionType.Removed:
                    return string.Format("کاربری با نام {0} حذف شد.", user.UserName);
                default:
                    return null;
            }
        }

        internal static string RoleDetailer(Role role, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("نقشی با نام {0} ایجاد شد.", role.Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات نقشی با نام {0} بروز شد.", role.Name);
                case ActionType.Removed:
                    return string.Format("نقشی با نام {0} حذف شد.", role.Name);
                default:
                    return null;
            }
        }

        public static string LoginLockedDetailer(string userName, string password) {
            return string.Format("کاربر {0} با کلمه عبور {1}: اقدام به ورود به حساب کاربری مسدود شده.", userName,
                                 password);
        }

        public static string LoginFailedDetailer(string userName, string password) {
            return string.Format("نام کاربری: {0}، کلمه عبور {1}، حسابی یافت نشد.", userName, password);
        }

        public static string PropStatusChangeDetailer(PropStatusChange propStatusChange, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("برای اموالی با شناسه {0} تعیین وضعیت (پیشفرض سیستم: آزاد) انجام شد.",
                                         propStatusChange.PropID);
                case ActionType.Modified:
                    return string.Format("وضعیت تعیین شده ({1}) اموالی با شناسه {0} بروز شد.", propStatusChange.PropID,
                                         ((ReportType) propStatusChange.Type).ToUIString());
                case ActionType.Removed:
                    return string.Format("وضعیت ({1}) اموالی با شناسه {0} حذف شد.", propStatusChange.PropID,
                                         ((ReportType) propStatusChange.Type).ToUIString());
                default:
                    return null;
            }
        }

        public static string MaterialDetailer(Material material, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("ماده‌ای با نام {0} ایجاد شد.", material.Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات ماده‌ای با نام {0} بروز شد.", material.Name);
                case ActionType.Removed:
                    return string.Format("ماده‌ای با نام {0} حذف شد.", material.Name);
                default:
                    return null;
            }
        }

        public static string RepositoryMaterialDetailer(RepositoryMaterial repositoryMaterial, ActionType actionType) {
            var material = MaterialsService.Get(repositoryMaterial.MaterialID);
            if (material == null)
                return null;
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("به موجودی ماده‌ای با نام {0} {1} {2} اضافه شد.", material.Name,
                                         repositoryMaterial.Amount, material.StringUnit);
                case ActionType.Removed:
                    return string.Format("از موجودی ماده‌ای با نام {0} {1} {2} کم شد.", material.Name,
                                         repositoryMaterial.Amount, material.StringUnit);
                default:
                    return null;
            }
        }

        public static string LoggedInDetailer(string userName, string password) {
            return string.Format("کاربر {0} با کلمه عبور {1}: ورود به حساب کاربری.", userName, password);
        }

        internal static string RepositoryItemDetailer(RepositoryItem repositoryItem, ActionType actionType) {
            var item = ItemsService.Get(repositoryItem.ItemID);
            if (item == null)
                return null;
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("به موجودی اقلامی با نام {0} {1} عدد اضافه شد.", item.Name,
                                         repositoryItem.Count);
                case ActionType.Removed:
                    return string.Format("از موجودی اقلامی با نام {0} {1} عدد کم شد.", item.Name, repositoryItem.Count);
                default:
                    return null;
            }
        }

        internal static string ItemDetailer(Item item, ActionType actionType) {
            switch (actionType) {
                case ActionType.Created:
                    return string.Format("اقلامی با نام {0} ایجاد شد.", item.Name);
                case ActionType.Modified:
                    return string.Format("اطلاعات اقلامی با نام {0} بروز شد.", item.Name);
                case ActionType.Removed:
                    return string.Format("اقلامی با نام {0} حذف شد.", item.Name);
                default:
                    return null;
            }
        }
    }
}