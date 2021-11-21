using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(NodeGroup))]
[CanEditMultipleObjects]
public class NodeGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NodeGroup targetGroup = (NodeGroup)target;

        // Show default inspector property editor
        DrawDefaultInspector();

        if (!Application.isPlaying) return;

        if (GUILayout.Button("Regenerate Nodes")) targetGroup.GenerateNodes();
    }
}