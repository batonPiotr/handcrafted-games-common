namespace HandcraftedGames.Common
{
    using UnityEditor;
    using UnityEngine;
    public class AutoRect
    {
        private Rect rect;
        float indentOffset = 15.0f;
        public AutoRect(Rect rect)
        {
            this.rect = rect;
        }

        public Rect NextSingleLine()
        {
            var r = rect;
            r.height = EditorGUIUtility.singleLineHeight;
            var offset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            rect.y += offset;
            rect.height -= offset;
            return r;
        }

        public Rect NextProperty(SerializedProperty property)
        {
            var r = rect;

            var height = EditorGUI.GetPropertyHeight(property, property.isExpanded);
            r.height = height;

            var offset = height + EditorGUIUtility.standardVerticalSpacing;
            rect.y += offset;
            rect.height -= offset;
            return r;
        }

        public void IncreaseIndentation()
        {
            rect.x += indentOffset;
            rect.width -= indentOffset;
        }

        public void DecreaseIndentation()
        {
            rect.x -= indentOffset;
            rect.width += indentOffset;
        }
    }
}