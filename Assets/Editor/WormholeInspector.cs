using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wormhole))]
public class WormholeInspector : Editor
{
    public Wormhole Target => (Wormhole)target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            GUI.enabled = !Application.isPlaying;

            Target.In =  (Portal)EditorGUILayout.ObjectField("Portal In:",  Target.In,  typeof(Portal), true);
            Target.Out = (Portal)EditorGUILayout.ObjectField("Portal Out:", Target.Out, typeof(Portal), true);

            GUI.enabled = true;
        }
        EditorGUILayout.EndVertical();
    }
}
