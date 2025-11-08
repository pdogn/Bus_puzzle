#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(SaveData))]
public class SaveDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveData saver = (SaveData)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Save Options", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save (Overwrite)", GUILayout.Height(30)))
        {
            saver.SaveOverwrite();
        }

        if (GUILayout.Button("Save (New)", GUILayout.Height(30)))
        {
            saver.SaveNew();
        }

        GUILayout.EndHorizontal();
    }
}
#endif
