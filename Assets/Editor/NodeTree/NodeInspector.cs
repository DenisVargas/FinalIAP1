using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using IA.PathFinding;

[CustomEditor(typeof(Node)), CanEditMultipleObjects]
public class NodeInspector : Editor
{
    Node inspectedNode;
    List<Node> inspectedNodes;

    private void OnEnable()
    {
        inspectedNode = target as Node;
        if (targets.Length > 1)
        {
            inspectedNodes = new List<Node>();
            foreach (var ins in targets)
                inspectedNodes.Add((Node)ins);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Custom Node Editor");
        if (GUILayout.Button("Connect Selection to this"))
        {
            foreach (var go in Selection.gameObjects)
            {
                var node = go.GetComponent<Node>();
                if (node != null && !node.Connections.Contains(inspectedNode))
                {
                    node.Connections.Add(inspectedNode);
                    inspectedNode.Connections.Add(node);
                }
            }
        }
        if (GUILayout.Button("Clear Connections"))
        {
            foreach (var node in inspectedNode.Connections)
            {
                if (node)
                    node.Connections.Remove(inspectedNode);
            }
            inspectedNode.Connections.Clear();
        }

        if (inspectedNodes != null && inspectedNodes.Count == 2)
        {
            if (GUILayout.Button("Subdivide"))
            {
                Node A = inspectedNodes[0];
                Node B = inspectedNodes[1];

                //Desconectamos.
                if (A.Connections.Contains(B))
                    A.Connections.Remove(B);
                if (B.Connections.Contains(A))
                    B.Connections.Remove(A);

                //Sacamos la posicion intermedia entre ambos.
                Vector3 pos = Vector3.Lerp(A.transform.position, B.transform.position, 0.5f);
                //Creamos un nuevo gameObject con el componente Nodo.
                GameObject empty = new GameObject("New Node");
                empty.transform.position = pos;
                Node newNode = empty.AddComponent<Node>();
                newNode.Connections.Add(A);
                newNode.Connections.Add(B);

                A.Connections.Add(newNode);
                B.Connections.Add(newNode);
            }
        }
    }
}
