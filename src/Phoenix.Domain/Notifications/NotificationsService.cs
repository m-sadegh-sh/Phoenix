namespace Phoenix.Domain.Notifications {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Items;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure;
    using Phoenix.Resources;

    public static class NotificationsService {
        public static bool HaveNofication {
            get { return Count > 0; }
        }

        public static int Count {
            get {
                return PropsService.Instanse.GetAllWarrantyExpired().Count +
                       PropsService.Instanse.GetAllLoanExpired().Count +
                       MaterialsService.Instanse.GetAllLowMaterialsAmount().Count +
                       PropsService.Instanse.GetAllRepairExpired().Count;
            }
        }

        public static bool HaveHidedNofication {
            get {
                return PropsService.CountOfNotNotifiable() > 0 || MaterialsService.CountOfNotNotifiable() > 0 ||
                       ItemsService.CountOfNotNotifiable() > 0;
            }
        }

        public static IList<Notification> GetAll() {
            var notifications =
                PropsService.Instanse.GetAllLoanExpired().Select(
                    prop =>
                    new Notification {NotifyType = NotifyType.Prop, OriginalObject = prop, Title = FormatBorrowed(prop)})
                    .ToList();
            notifications.AddRange(
                PropsService.Instanse.GetAllRepairExpired().Select(
                    prop =>
                    new Notification {NotifyType = NotifyType.Prop, OriginalObject = prop, Title = FormatRepaire(prop)}));
            notifications.AddRange(
                PropsService.Instanse.GetAllWarrantyExpired().Select(
                    prop =>
                    new Notification {NotifyType = NotifyType.Prop, OriginalObject = prop, Title = FormatExpired(prop)}));
            notifications.AddRange(
                MaterialsService.Instanse.GetAllLowMaterialsAmount().Select(
                    material =>
                    new Notification
                    {NotifyType = NotifyType.Material, OriginalObject = material, Title = Format(material)}));
            notifications.AddRange(
                ItemsService.Instanse.GetAllLowItemsAmount().Select(
                    item => new Notification {NotifyType = NotifyType.Item, OriginalObject = item, Title = Format(item)}));
            return notifications;
        }

        private static string Format(Prop prop, string format) {
            return string.Format(format, prop.Name, prop.PropNo, prop.SerialNo);
        }

        private static string FormatExpired(Prop prop) {
            return Format(prop, NotificationsResources.ExpiredPropMessageFormat);
        }

        private static string FormatRepaire(Prop prop) {
            return Format(prop, NotificationsResources.RepairePropMessageFormat);
        }

        private static string FormatBorrowed(Prop prop) {
            return Format(prop, NotificationsResources.BorrowedPropMessageFormat);
        }

        private static string Format(Item item) {
            return string.Format(NotificationsResources.ItemMessageFormat, item.Name);
        }

        private static string Format(Material material) {
            return string.Format(NotificationsResources.MaterialMessageFormat, material.Name);
        }

        public static bool ChangeNotifiable(Notification notification) {
            try {
                if (notification.NotifyType == NotifyType.Prop)
                    return PropsService.ChangeNotifiable(((Prop) notification.OriginalObject).PropID, false);
                if (notification.NotifyType == NotifyType.Item)
                    return ItemsService.ChangeNotifiable(((Item) notification.OriginalObject), false);
                if (notification.NotifyType == NotifyType.Material)
                    return MaterialsService.ChangeNotifiable(((Material) notification.OriginalObject).MaterialID, false);
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }
    }
}