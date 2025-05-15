/// 2025-05-14

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UIUnit), true)]
public class UIUnitEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        UIUnit uiUnit = (UIUnit)target;
        
        if (GUILayout.Button("Setting UI Type"))
        {
            SetUIType(uiUnit);
        }

        GUILayout.Space(10f);
        base.OnInspectorGUI();
        
        GUILayout.Space(10f);
    }
    
    private void SetUIType(UIUnit uiUnit)
    {
        string className = uiUnit.GetType().Name;
        string enumString = className.Replace("UI", "");

        if (System.Enum.TryParse(typeof(eUIType), enumString, out var type))
        {
            Undo.RecordObject(uiUnit, "Set UIType");
            uiUnit.UIType = (eUIType)type;
            EditorUtility.SetDirty(uiUnit);
        }
        else
        {
            Debug.LogError($"Check UIType and {className}!");
        }
    }
}
