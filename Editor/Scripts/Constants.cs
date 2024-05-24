#if UNITY_EDITOR
using System.IO;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial lookup table for constants containing BasePackage values  </summary>
internal static class Constants
{
    public static class BasePackage
    {
        public static class Resources
        {
            public static class Images
            {
                public static readonly string IconsPath = Path.Combine(ImagesPath, "Icons");
            }

            public static class UserInterface
            {
                public static class Windows
                {
                    public static readonly string DevelopmentModePath = Path.Combine(WindowsPath, "Development Mode");
                    public static readonly string PackageManagerPath = Path.Combine(WindowsPath, "Package Manager");
                }

                public static readonly string WindowsPath = Path.Combine(UserInterfacePath, "Windows");
            }

            private const string ResourcesPath = "MegaPint";
            public static readonly string UserInterfacePath = Path.Combine(ResourcesPath, "User Interface");

            public static readonly string ImagesPath = Path.Combine(ResourcesPath, "Images");
        }
    }
}

}
#endif
