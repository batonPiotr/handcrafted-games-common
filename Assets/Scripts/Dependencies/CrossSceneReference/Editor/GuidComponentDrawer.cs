#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GuidComponent))]
public class GuidComponentDrawer : Editor
{
    private GuidComponent guidComp;

    public override void OnInspectorGUI()
    {
        if (guidComp == null)
        {
            guidComp = (GuidComponent)target;
        }

        // Draw label
        GUI.enabled = false;
        EditorGUILayout.TextField("Guid:", guidComp.GetGuid().ToString());
        GUI.enabled = true;
    }
}
#endif