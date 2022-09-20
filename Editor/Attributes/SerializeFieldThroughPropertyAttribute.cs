namespace HandcraftedGames.Common
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(SerializeFieldThroughPropertyAttribute), true)]
    public class SerializeFieldThroughPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            var attrib = attribute as SerializeFieldThroughPropertyAttribute;

            if(attrib.DrawItself)
            {
                position.height = (position.height - EditorGUIUtility.standardVerticalSpacing) / 2.0f;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }

            var target = property.serializedObject.targetObject;

            var parent = property.GetDirectParent();

            var properties = parent.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);

            PropertyInfo foundProperty = null;

            foreach(var p in properties)
            {
                if(p.Name == attrib.PropertyName)
                {
                    foundProperty = p;
                    break;
                }
            }

            if(foundProperty == null)
            {
                EditorGUI.LabelField(position, "Property not found: " + attrib.PropertyName);
                return;
            }

            if(attrib.DrawItself)
            {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            }
            if(foundProperty.GetSetMethod(true) == null)
                EditorGUI.BeginDisabledGroup(true);

            if(DoItForType<int>(foundProperty, parent, property, oldValue => EditorGUI.IntField(position, attrib.PropertyName, oldValue))) {}
            else if(DoItForType<float>(foundProperty, parent, property, oldValue => EditorGUI.FloatField(position, attrib.PropertyName, oldValue))) {}
            else if(DoItForType<string>(foundProperty, parent, property, oldValue => EditorGUI.TextField(position, attrib.PropertyName, oldValue))) {}
            else
                EditorGUI.LabelField(position, "Property type not supported: " + foundProperty.PropertyType);

            if(foundProperty.GetSetMethod(true) == null)
                EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attrib = attribute as SerializeFieldThroughPropertyAttribute;
            if(attrib.DrawItself)
                return EditorGUI.GetPropertyHeight(property, label) * 2.0f + EditorGUIUtility.standardVerticalSpacing;
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private bool DoItForType<T>(PropertyInfo foundProperty,
                                    System.Object parent,
                                    SerializedProperty property,
                                    System.Func<T, T> propertyDrawer)
        {
            if(foundProperty.PropertyType != typeof(T))
                return false;

            var oldValue = (T)foundProperty.GetValue(parent);

            var newValue = propertyDrawer(oldValue);

            if(!oldValue.Equals(newValue))
            {
                Undo.RecordObject(property.serializedObject.targetObject, $"Set {foundProperty.Name}");
                foundProperty.SetValue(parent, newValue);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.ApplyModifiedProperties();
            }
            return true;
        }
    }

}