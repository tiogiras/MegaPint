#if UNITY_EDITOR
using System;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Settings;
using MegaPint.Editor.Scripts.Windows;
using MegaPint.Editor.Scripts.Windows.DevMode;
using UnityEditor;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class used to store MenuItems </summary>
internal static partial class ContextMenu
{
    public static class BasePackage
    {
        private static readonly MenuItemSignature s_openBaseWindowSignature = new()
        {
            package = PackageKey.Undefined, signature = "Open"
        };

        private static readonly MenuItemSignature s_openBaseWindowSignatureLink = new()
        {
            package = PackageKey.Undefined, signature = "Open (Link)"
        };

        private static readonly MenuItemSignature s_openPackageManagerSignature = new()
        {
            package = PackageKey.Undefined, signature = "Package Manager"
        };

        private static readonly MenuItemSignature s_openDevCenterSignature = new()
        {
            package = PackageKey.Undefined, signature = "Dev Center"
        };

        private static readonly MenuItemSignature s_openDevModeToggleSignature = new()
        {
            package = PackageKey.Undefined, signature = "Dev Mode Toggle"
        };

        private static readonly MenuItemSignature s_openInterfaceOverviewSignature = new()
        {
            package = PackageKey.Undefined, signature = "Interface Overview"
        };

        #region Public Methods

        [MenuItem(MenuItemMegaPint + "/Open", false, 0)]
        public static void OpenBaseWindow()
        {
            BaseWindow.openingLink = "";
            TryOpen <BaseWindow>(false, s_openBaseWindowSignature);
        }

        /// <summary> Open the base window with a link </summary>
        /// <param name="link"> Link to refer to content inside the base window </param>
        public static void OpenBaseWindowPerLink(string link)
        {
            BaseWindow.openingLink = link;
            TryOpen <BaseWindow>(false, s_openBaseWindowSignatureLink);
        }

        /// <summary> Open the dev center </summary>
        public static void OpenDevCenter()
        {
            TryOpen <Center>(false, s_openDevCenterSignature);
        }

        /// <summary> Open the dev mode toggle </summary>
        public static void OpenDevModeToggle()
        {
            TryOpen <Toggle>(true, s_openDevModeToggleSignature);
        }

        /// <summary> Open the interface overview </summary>
        public static void OpenInterfaceOverview()
        {
            TryOpen <InterfaceOverview>(false, s_openInterfaceOverviewSignature);
        }

        [MenuItem(MenuItemMegaPint + "/PackageManager", false, 11)]
        public static void OpenPackageManager()
        {
            TryOpen <Windows.PackageManager>(false, s_openPackageManagerSignature, "Package Manager");
        }

        #endregion
    }

    public struct MenuItemSignature
    {
        public PackageKey package;
        public string signature;
    }

    public const string MenuItemMegaPint = "MegaPint";
    public const string MenuItemPackages = MenuItemMegaPint + "/Packages";

    public static Action <MenuItemSignature> onMenuItemInvoked;

    #region Public Methods

    /// <summary> Try opening an editor window derived from the <see cref="EditorWindowBase" /> </summary>
    /// <param name="utility"> If the window should be an utility window </param>
    /// <param name="menuItemSignature"> Signature to identify this menuItem </param>
    /// <param name="title"> Title of the window </param>
    /// <typeparam name="T"> Type of the wanted window </typeparam>
    /// <returns> Editor window of the selected parameters </returns>
    public static EditorWindowBase TryOpen <T>(bool utility, MenuItemSignature menuItemSignature, string title = "")
        where T : EditorWindowBase
    {
        if (typeof(T) == typeof(FirstSteps))
            return EditorWindow.GetWindow <T>(true, title).ShowWindow();

        if (!MegaPintSettings.Exists())
            return EditorWindow.GetWindow <FirstSteps>(true, title).ShowWindow();

        onMenuItemInvoked?.Invoke(menuItemSignature);

        return EditorWindow.GetWindow <T>(utility, title).ShowWindow();
    }

    #endregion
}

}
#endif
