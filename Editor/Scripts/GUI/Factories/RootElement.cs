using Editor.Scripts.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.GUI.Factories
{

public class RootElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory <RootElement, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        #region Public Methods

        public override void Init(
            VisualElement element,
            IUxmlAttributes attributes,
            CreationContext context)
        {
            base.Init(element, attributes, context);
            
            ApplyTheme(element);
            
            GUIUtility.onForceRepaint += () => ApplyTheme(element);
        }

        #endregion

        private static void ApplyTheme(VisualElement element)
        {
            var darkMode = SaveValues.BasePackage.EditorTheme switch {
                               0 => EditorGUIUtility.isProSkin,
                               1 => true,
                               var _ => false
                           };
            
            element.ClearClassList();
            
            element.AddToClassList("mp");
            element.AddToClassList(darkMode ? "mp_theme--dark" : "mp_theme--light");
        }
    }
}

}
