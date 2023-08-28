#if UNITY_EDITOR
using Editor.Scripts.PackageManager;

namespace Editor.Scripts
{
    public static partial class DisplayContent
    {
        public static void DisplayRightPane(MegaPintPackagesData.PackageKey key)
        {
            switch (key)
            {
                case MegaPintPackagesData.PackageKey.AutoSave: AutoSave(); break;
                default: return;
            }
        }
        
        private static partial void AutoSave();
    }
}
#endif