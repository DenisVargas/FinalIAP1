using System;
using System.Collections.Generic;
using UnityEngine;
using IA.PathFinding;

public class Traveller : MonoBehaviour
{
    [SerializeField] Node _startPoint = null;
    [SerializeField] Node _targetPoint = null;
    IEnumerable<Node> _currentPath;

    // Start is called before the first frame update
    void Start()
    {
        //_currentPath = ThetaStar.getPath<Node>(_startPoint, isTarget, getNodeConnections, GetHeurístic, hasValidConnection);
        //_currentPath = AStar.getPath(_startPoint, isTarget, getNodeConnections, GetHeurístic);

        int count = 0;
        //foreach (var node in _currentPath)
        //{
        //    count++;
        //    print(node.gameObject.name);
        //}
        print($"Se imprimieron {count} elementos");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isTarget(Node reference)
    {
        return reference == _targetPoint;
    }
    bool hasValidConnection(Node A, Node B)
    {
        return A.Connections.Contains(B) && B.Connections.Contains(A);
    }
    float GetHeurístic(Node reference)
    {
        return Vector3.Distance(reference.transform.position, _targetPoint.transform.position);
    }
    IEnumerable<Tuple<Node, float>> getNodeConnections(Node reference)
    {
        List<Tuple<Node, float>> NodeConnections = new List<Tuple<Node, float>>();

        foreach (var connection in reference.Connections)
        {
            var tuple = Tuple.Create(connection, Vector3.Distance(reference.transform.position, connection.transform.position));
            NodeConnections.Add(tuple);
        }

        return NodeConnections;
    }
}
