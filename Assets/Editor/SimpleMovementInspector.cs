using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleMovement))]
class SimpleMovementInspector : Editor
{
    private new SimpleMovement target => (SimpleMovement)base.target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (new EditorGUILayout.VerticalScope())
        {
            if (GUILayout.Button("Reset values"))
            {
                target.ResetEditorValues();
            }

            GUI.enabled = false;
            EditorGUILayout.Vector3Field("Position min:", target.MinPosition);
            EditorGUILayout.Vector3Field("Position max:", target.MaxPosition);
            GUI.enabled = true;
        }
    }
}
