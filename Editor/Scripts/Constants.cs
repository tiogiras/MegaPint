#if UNITY_EDITOR
using System.IO;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial lookup table for constants containing BasePackage values  </summary>
internal static partial class Constants
{
    public static class BasePackage
    {
        public static class Images
        {
            public static readonly string PackageImages = Path.Combine(s_imagesPath, "Packages");
            private static readonly string s_iconsPath = Path.Combine(s_imagesPath, "Icons");
            public static readonly string CopyToClipboard = Path.Combine(s_iconsPath, "Copy To Clipboard");
        }

        public static class Links
        {
            public const string Shortcuts = "Edit/Shortcuts...";

            public static readonly string BaseWindow = Utility.CombineMenuItemPath(
                ContextMenu.MenuItemMegaPint,
                "Open");

            public static readonly string PackageManager = Utility.CombineMenuItemPath(
                ContextMenu.MenuItemMegaPint,
                "PackageManager");
        }

        public static class UserInterface
        {
            private static readonly string s_windowsPath = Path.Combine(s_userInterfacePath, "Windows");
            public static readonly string PackageManager = Path.Combine(s_windowsPath, "Package Manager");
            public static readonly string PackageManagerPackageItem = Path.Combine(PackageManager, "Package Item");
            public static readonly string PackageManagerVariationItem = Path.Combine(PackageManager, "Variation Item");
            public static readonly string PackageManagerSampleItem = Path.Combine(PackageManager, "Sample Item");

            public static readonly string PackageManagerDependencyItem = Path.Combine(
                PackageManager,
                "Dependency Item");

            public static readonly string Gallery = Path.Combine(PackageManager, "Gallery");
            public static readonly string SplashScreen = Path.Combine(s_windowsPath, "Splash Screen");
            public static readonly string Tooltip = Path.Combine(s_windowsPath, "Tooltip");
            public static readonly string InfoContent = Path.Combine(s_userInterfacePath, "Info Content");
            public static readonly string SettingsContent = Path.Combine(s_userInterfacePath, "Settings Content");

            public static readonly string BaseWindow = Path.Combine(s_windowsPath, "Base Window");
            public static readonly string BaseWindowInfoItem = Path.Combine(BaseWindow, "Info Item");
            public static readonly string BaseWindowPackageItem = Path.Combine(BaseWindow, "Package Item");
            private static readonly string s_devMode = Path.Combine(s_windowsPath, "Development Mode");
            public static readonly string DevModeCenter = Path.Combine(s_devMode, "Center");
            public static readonly string DevModeToggle = Path.Combine(s_devMode, "Toggle");
            public static readonly string InterfaceOverview = Path.Combine(s_devMode, "Interface Overview");

            public static readonly string FirstSteps = Path.Combine(s_windowsPath, "First Steps");
        }

        private const string ResourcesPath = "MegaPint";
        private static readonly string s_userInterfacePath = Path.Combine(ResourcesPath, "User Interface");
        private static readonly string s_imagesPath = Path.Combine(ResourcesPath, "Images");
    }
}

}
#endif
