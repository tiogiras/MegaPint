using System.Text;
using Editor.Scripts.DevModeUtil;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI
{

public static class StyleSheetValues
{
    private const string BaseStyleSheetPath = "MegaPint/User Interface/MegaPint";
    private const string AttributeStyleSheetPath = "MegaPint/user Interface/MegaPintAttributes";
    private const string UnityElementsStyleSheetPath = "MegaPint/user Interface/MegaPintUnityElements";

    private static StyleSheet s_baseStyleSheet;
    private static StyleSheet s_attributesStyleSheet;
    private static StyleSheet s_elementsStyleSheet;

    public static bool Initialized {get; private set;}

    public static Color LinkColor {get; private set;}

    public static Color InfoColor {get; private set;}
    public static Color CodeBasicColor {get; private set;}
    public static Color CodeClassColor {get; private set;}
    public static Color CodeMethodColor {get; private set;}
    public static Color CodeCommentColor {get; private set;}
    public static Color CodeStringColor {get; private set;}

    public static StyleSheet BaseStyleSheet => s_baseStyleSheet ??= Resources.Load<StyleSheet>(BaseStyleSheetPath);
    
    public static StyleSheet AttributesStyleSheet => s_attributesStyleSheet ??= Resources.Load<StyleSheet>(AttributeStyleSheetPath);
    
    public static StyleSheet ElementsStyleSheet => s_elementsStyleSheet ??= Resources.Load<StyleSheet>(UnityElementsStyleSheetPath);

    public static void Initialize(
        Color linkColor, 
        Color infoColor, 
        Color codeBasicColor, 
        Color codeClassColor, 
        Color codeMethodColor, 
        Color codeCommentColor, 
        Color codeStringColor)
    {
        if (Initialized)
            return;

        var log = new StringBuilder("Initializing StyleSheetValues\n");
        
        LinkColor = linkColor;
        log.AppendLine($"Link Color: {LinkColor}");
        
        InfoColor = infoColor;
        log.AppendLine($"Info Color: {InfoColor}");
        
        CodeBasicColor = codeBasicColor;
        log.AppendLine($"Code Basic Color: {CodeBasicColor}");
        
        CodeClassColor = codeClassColor;
        log.AppendLine($"Code Class Color: {CodeClassColor}");
        
        CodeMethodColor = codeMethodColor;
        log.AppendLine($"Code Method Color: {CodeMethodColor}");
        
        CodeCommentColor = codeCommentColor;
        log.AppendLine($"Code Comment Color: {CodeCommentColor}");
        
        CodeStringColor = codeStringColor;
        log.AppendLine($"Code String Color: {CodeStringColor}");

        Initialized = true;
        
        DevLog.Log(log.ToString());
    }

    public static void Reset()
    {
        DevLog.Log("Requesting new initialization of StyleSheetValues");
        
        Initialized = false;
    }
}

}
