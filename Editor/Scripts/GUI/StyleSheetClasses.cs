#if UNITY_EDITOR
namespace MegaPint.Editor.Scripts.GUI
{

/// <summary> Lookup table for stylesheet classes </summary>
internal static class StyleSheetClasses
{
    public static class Background
    {
        public static class Color
        {
            private static readonly string s_color = $"{s_background}-color";
            public static readonly string Primary = $"{s_color}--primary";
            public static readonly string Secondary = $"{s_color}--secondary";
            public static readonly string Tertiary = $"{s_color}--tertiary";
            public static readonly string Quaternary = $"{s_color}--quaternary";
            public static readonly string Identity = $"{s_color}--identity";
            public static readonly string Separator = $"{s_color}--separator";
        }

        private static readonly string s_background = $"{ClassBase}background";
    }

    public static class Border
    {
        public static class Color
        {
            private static readonly string s_color = $"{s_border}-color";
            public static readonly string Identity = $"{s_color}--identity";
            public static readonly string Black = $"{s_color}--black";
            public static readonly string Primary = $"{s_color}--primary";
            public static readonly string Secondary = $"{s_color}--secondary";
            public static readonly string Tertiary = $"{s_color}--tertiary";
            public static readonly string Separator = $"{s_color}--separator";
        }

        private static readonly string s_border = $"{ClassBase}border";
    }

    public static class Code
    {
        public static class Color
        {
            private static readonly string s_color = $"{s_code}-color";
            public static readonly string Basic = $"{s_color}--basic";
            public static readonly string Class = $"{s_color}--class";
            public static readonly string Method = $"{s_color}--method";
            public static readonly string Comment = $"{s_color}--comment";
            public static readonly string String = $"{s_color}--string";
        }

        private static readonly string s_code = $"{ClassBase}code";
    }

    public static class Image
    {
        public static class Tint
        {
            private static readonly string s_tint = $"{s_image}-tint";
            public static readonly string Identity = $"{s_tint}--identity";
            public static readonly string TextSecondary = $"{s_tint}--text-secondary";
            public static readonly string ButtonImage = $"{s_tint}--button-image";
        }

        private static readonly string s_image = $"{ClassBase}image";
    }

    public static class Text
    {
        public static class Color
        {
            private static readonly string s_color = $"{s_text}-color";
            public static readonly string Link = $"{s_color}--link";
            public static readonly string Info = $"{s_color}--info";
            public static readonly string Identity = $"{s_color}--identity";
            public static readonly string Primary = $"{s_color}--primary";
            public static readonly string Secondary = $"{s_color}--secondary";
            public static readonly string Red = $"{s_color}--red";
            public static readonly string Orange = $"{s_color}--orange";
            public static readonly string Green = $"{s_color}--green";
            public static readonly string ButtonActive = $"{s_color}--buttonActive";
        }

        private static readonly string s_text = $"{ClassBase}text";
    }

    public static class Theme
    {
        private static readonly string s_theme = $"{ClassBase}theme";
        public static readonly string Dark = $"{s_theme}--dark";
        public static readonly string Light = $"{s_theme}--light";

        public static string Current => SaveValues.BasePackage.IsDarkTheme ? Dark : Light;
    }

    private const string ClassBase = "mp_";

    public static string Button => $"{ClassBase}button";

    public static string SearchField => $"{ClassBase}searchField";

    public static string CursorColor => $"{ClassBase}cursorColor";

    public static string Dropdown => $"{ClassBase}dropdown";

    public static string LinkText => $"{ClassBase}link";

    public static string Foldout => $"{ClassBase}foldout";

    public static string LinkCursor => "link-cursor";

    public static string Tooltip => $"{ClassBase}tooltip-custom";
}

}
#endif
