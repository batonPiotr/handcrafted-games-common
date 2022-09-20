namespace HandcraftedGames.Common
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using System.Linq;

    public struct CompactReorderableListGeneratorModel<T>
    {
        public string AddTitle;
        public System.Func<T> Generate;

        /// <summary>
        /// Used to anchor some data when using predicate
        /// </summary>
        // public System.Object UserData;
    }

    public class CompactReorderableList<T>
    {
        private List<T> rawList;
        private SerializedObject serializedObject;
        private SerializedProperty serializedList;
        private ReorderableList reorderableList;
        private string Header;
        private IEnumerable<CompactReorderableListGeneratorModel<T>> model;
        private GenericMenu AddMenu;
        private bool RegenerateModel;

        public CompactReorderableList(
            SerializedObject serializedObject,
            SerializedProperty serializedList,
            string Header,
            IEnumerable<CompactReorderableListGeneratorModel<T>> model,
            bool RegenerateModel = false
        )
        {
            this.Header = Header;
            this.serializedObject = serializedObject;
            this.serializedList = serializedList;
            this.model = model;
            this.RegenerateModel = RegenerateModel;
            this.rawList = serializedList.GetValue() as List<T>;

            reorderableList = new ReorderableList(serializedObject, serializedList, true, true, true, true);
            reorderableList.drawElementCallback = DrawListItems;
            reorderableList.drawHeaderCallback = DrawHeader;
            reorderableList.elementHeightCallback = ElementHeight;
            reorderableList.onAddDropdownCallback = OnAddDropDownCallback;
            reorderableList.onRemoveCallback = l =>
            {
                if(l.selectedIndices.Count == 0)
                    return;
                var index = l.selectedIndices.First();
                rawList.RemoveAt(index);
                l.serializedProperty.DeleteArrayElementAtIndex(index);
                if(RegenerateModel)
                {
                    this.AddMenu = null;
                    GenerateMenuIfNeeded();
                }
            };
        }

        float ElementHeight(int index)
        {
            return EditorGUI.GetPropertyHeight(serializedList.GetArrayElementAtIndex(index));
        }

        void OnAddDropDownCallback(Rect buttonRect, ReorderableList list)
        {
            GenerateMenuIfNeeded();
            AddMenu.ShowAsContext();
        }

        private void GenerateMenuIfNeeded()
        {
            if(AddMenu == null)
            {
                AddMenu = new GenericMenu();
                foreach (var m in model)
                {
                    AddMenu.AddItem(new GUIContent(m.AddTitle), false, () =>
                    {
                        var instance = m.Generate();
                        var l = rawList;
                        l.Add(instance);
                        reorderableList.serializedProperty.SetValue(l);
                        if(RegenerateModel)
                        {
                            AddMenu = null;
                            GenerateMenuIfNeeded();
                        }
                    });
                }
            }
        }

        // Draws the elements on the list
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var indent = 15.0f;
            rect.x += indent;
            rect.width -= indent;

            var item = serializedList.GetArrayElementAtIndex(index);

            if(item.GetValue() == null)
                EditorGUI.LabelField(rect, "<NULL ITEM>");
            else
                EditorGUI.PropertyField(rect, item);
        }

        //Draws the header
        void DrawHeader(Rect rect)
        {
            string name = Header;
            EditorGUI.LabelField(rect, name);
        }

        public void DoLayoutList()
        {
            reorderableList.DoLayoutList();
        }
    }
}