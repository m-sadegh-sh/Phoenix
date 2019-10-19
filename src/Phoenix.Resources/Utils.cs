namespace Phoenix.Resources {
    using System;

    public static class Utils {
        public static string GetLocalizedString(Type type, string resourceKey) {
            string resourceValue = null;
            switch (type.Name) {
                case "Category":
                    resourceValue = CategoriesResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "LabProp":
                    resourceValue = LabPropsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "Lab":
                    resourceValue = LabsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "Log":
                    resourceValue = LogsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "Material":
                    resourceValue = MaterialsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "Prop":
                    resourceValue = PropsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "PropStatusChange":
                    resourceValue = PropStatusResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "RepositoryMaterial":
                    resourceValue =
                        RepositoryMaterialsAndItemsResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "Role":
                    resourceValue = RolesResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
                case "User":
                    resourceValue = UsersResources.ResourceManager.GetObject(resourceKey) as string;
                    break;
            }
            if (resourceValue == null)
                throw new Exception();
            return resourceValue;
        }
    }
}