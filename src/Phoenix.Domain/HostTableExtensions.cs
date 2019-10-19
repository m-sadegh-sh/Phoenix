namespace Phoenix.Domain {
    using Phoenix.Resources;

    public static class HostTableExtensions {
        public static string ToUIString(this HostTable hostTable) {
            switch (hostTable) {
                case HostTable.Sys:
                    return SharedResources.HostTableSys;
                case HostTable.Categories:
                    return SharedResources.HostTableCategories;
                case HostTable.Props:
                    return SharedResources.HostTableProps;
                case HostTable.Labs:
                    return SharedResources.HostTableLabs;
                case HostTable.LabProps:
                    return SharedResources.HostTableLabProps;
                case HostTable.Users:
                    return SharedResources.HostTableUsers;
                case HostTable.Roles:
                    return SharedResources.HostTableRoles;
                case HostTable.Login:
                    return SharedResources.HostTableLogin;
                case HostTable.PropStatusChanges:
                    return SharedResources.HostTablePropStatusChanges;
                case HostTable.Materials:
                    return SharedResources.HostTableMaterials;
                case HostTable.RepositoryMaterials:
                    return SharedResources.HostTableRepositoryMaterials;
                case HostTable.Items:
                    return SharedResources.HostTableItems;
                case HostTable.RepositoryItems:
                    return SharedResources.HostTableRepositoryItems;
                default:
                    return SharedResources.HostTableUnknown;
            }
        }
    }
}