#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.GUI.Factories.Structure
{
    /// <summary>
    ///     Uxml factory to create a <see cref="VisualElement" /> that displays the background image in a selected aspect
    ///     ratio
    /// </summary>
    [Preserve]
    [UxmlElement]
    internal partial class AspectRatioPanel : VisualElement
    {
        public AspectRatioPanel()
        {
            style.position = Position.Relative;
            style.left = StyleKeyword.Auto;
            style.top = StyleKeyword.Auto;
            style.right = StyleKeyword.Auto;
            style.bottom = StyleKeyword.Auto;

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
        }

        [UxmlAttribute("aspect-ratio-x")] 
        public int AspectRatioX { get; set; } = 16;

        [UxmlAttribute("aspect-ratio-y")] 
        public int AspectRatioY { get; set; } = 9;

        [UxmlAttribute("scale")] 
        private float Scale { get; set; } = 1f;

        #region Public Methods

        public void FitToParent()
        {
            if (parent == null)
                return;

            var parentW = parent.resolvedStyle.width;
            var parentH = parent.resolvedStyle.height;

            if (float.IsNaN(parentW) || float.IsNaN(parentH))
                return;

            if (AspectRatioX <= 0.0f || AspectRatioY <= 0.0f)
            {
                style.width = parentW;
                style.height = parentH;

                return;
            }

            var ratio = Mathf.Min(parentW / AspectRatioX, parentH / AspectRatioY);
            var targetW = Mathf.Floor(AspectRatioX * ratio);
            var targetH = Mathf.Floor(AspectRatioY * ratio);
            style.width = targetW * Scale;
            style.height = targetH * Scale;
        }

        #endregion

        #region Private Methods

        private void OnAttachToPanelEvent(AttachToPanelEvent e)
        {
            parent?.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            FitToParent();
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent e)
        {
            FitToParent();
        }

        #endregion
    }
}
#endif