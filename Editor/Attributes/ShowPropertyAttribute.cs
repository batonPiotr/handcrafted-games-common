namespace HandcraftedGames.Common
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    // [CustomEditor(typeof(System.Object), true)]
    // public class ShowPropertiesEditorLol : Editor
    // {

    // }

    // [CustomEditor(typeof(MonoBehaviour), true)]
    // [CanEditMultipleObjects()]
    public class ShowPropertiesEditor : Editor
    {
        protected bool SuppressDefaultInspector = false;
        private class ShownPropertyInfo
        {
            public ShowPropertyAttribute Attrib;
            public System.Reflection.PropertyInfo PropertyInfo;
            public GUIContent Label;
        }

        private List<ShownPropertyInfo> _shownFields;
        protected virtual void OnEnable()
        {
            var tp = this.target.GetType();
            var fields = tp.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            foreach (var f in fields)
            {
                var attribs = f.GetCustomAttributes(typeof(ShowPropertyAttribute), false) as ShowPropertyAttribute[];
                if (attribs != null && attribs.Length > 0)
                {
                    if (_shownFields == null) _shownFields = new List<ShownPropertyInfo>();
                    var attrib = attribs[0];
                    _shownFields.Add(new ShownPropertyInfo()
                    {
                        Attrib = attrib,
                        PropertyInfo = f,
                        Label = new GUIContent(attrib.Label ?? f.Name, attrib.Tooltip)
                    });
                }
            }

            // _constantlyRepaint = tp.GetCustomAttributes(typeof(ConstantlyRepaintEditorAttribute), false).FirstOrDefault() as ConstantlyRepaintEditorAttribute;
        }

        public override void OnInspectorGUI()
        {
            if(!SuppressDefaultInspector)
                DrawDefaultInspector();
            if (_shownFields != null)
            {
                foreach(var info in _shownFields)
                {
                    info.PropertyInfo.SetValue(this.target, EditorGUILayout.IntField(info.Label, (int)info.PropertyInfo.GetValue(target)));
                }
                // GUILayout.Label("Runtime Values", EditorStyles.boldLabel);

                // foreach (var info in _shownFields)
                // {
                //     // var cache = SPGUI.DisableIf(info.Attrib.Readonly);

                //     var value = DynamicUtil.GetValue(this.target, info.MemberInfo);
                //     EditorGUI.BeginChangeCheck();
                //     value = SPEditorGUILayout.DefaultPropertyField(info.Label, value, DynamicUtil.GetReturnType(info.MemberInfo));
                //     if (EditorGUI.EndChangeCheck())
                //     {
                //         DynamicUtil.SetValue(this.target, info.MemberInfo, value);
                //     }

                //     // cache.Reset();
                // }
            }
        }
    }

    // [CustomPropertyDrawer(typeof(ShowPropertyAttribute))]
    // public class ShowPropertyDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position,
    //                                SerializedProperty property,
    //                                GUIContent label)
    //     {
    //         // GUI.enabled = false;
    //         var val = property.GetValue();
    //         EditorGUI.BeginChangeCheck();
    //         // base.OnGUI(position, property, label);
    //         EditorGUI.PropertyField(position, property, label, true);
    //         if(EditorGUI.EndChangeCheck())
    //         {
    //             property.serializedObject.UpdateIfRequiredOrScript();
    //             this.Log("LOOL");
    //             var val2 = property.GetValue();
    //             if(val != val2)
    //                 this.Log("Change! From: " + val + " to: " + val2);
    //         }
    //         // GUI.enabled = true;
    //         // EditorGUI.PropertyField()
    //     }
    // }
}