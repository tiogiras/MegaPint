#if UNITY_EDITOR
using System.IO;

namespace MegaPint.Editor.Scripts
{

internal static partial class Constants
{
    public static class BasePackage
    {
        public static class Resources
        {
            private static readonly string s_resources = "MegaPint";
            
            public static readonly string UserInterfacePath = Path.Combine(s_resources, "User Interface");

            public static class UserInterface
            {
                public static readonly string WindowsPath = Path.Combine(UserInterfacePath, "Windows");
            }
            
            public static readonly string ImagesPath = Path.Combine(s_resources, "Images");
            
            public static class Images
            {
                public static readonly string IconsPath = Path.Combine(ImagesPath, "Icons");
            }
        }
    }
}

}
#endif
