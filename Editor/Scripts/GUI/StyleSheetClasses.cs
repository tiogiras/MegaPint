namespace Editor.Scripts.GUI
{

public static class StyleSheetClasses
{
    private const string ClassBase = "mp_";

    public static string Button => $"{ClassBase}button";
    public static string SearchField => $"{ClassBase}searchField";
    public static string CursorColor => $"{ClassBase}cursorColor";

    public static string Dropdown => $"{ClassBase}dropdown";

    public static class Background
    {
        private static readonly string s_background = $"{ClassBase}background";
        
        public static class Color
        {
            private static readonly string s_color = $"{s_background}-color";
            
            public static readonly string Primary = $"{s_color}--primary";
            public static readonly string Secondary = $"{s_color}--secondary";
            public static readonly string Tertiary = $"{s_color}--tertiary";
            public static readonly string Identity = $"{s_color}--identity";
            public static readonly string Separator = $"{s_color}--separator";
        }
    }

    public static class Border
    {
        private static readonly string s_border = $"{ClassBase}border";

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
    }

    public static class Text
    {
        private static readonly string s_text = $"{ClassBase}text";
        
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
        }
    }
}

}
