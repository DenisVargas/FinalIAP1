using IA.PathFinding;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Triangulization : MonoBehaviour
{
    [SerializeField] GameObject GraphObject;
    [SerializeField] int _closerNodeToSearch = 4;
    [SerializeField] int _connectionExpandLimit = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnDrawGizmos()
    {
        if (GraphObject == null) return;

        List<Node> closer = getCloserNodes();
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        for (int i = 0; i < _closerNodeToSearch; i++)
        {
            //Gizmos.DrawWireSphere(closer[i].transform.position, 1);
            Gizmos.DrawLine(transform.position, closer[i].transform.position);
        }
        Gizmos.color = Color.red;
        if (closer.Count > 4)
        {
            for (int i = _closerNodeToSearch - 1; i < closer.Count; i++)
            {
                Gizmos.DrawWireSphere(closer[i].transform.position, 1);
                //Gizmos.DrawLine(transform.position, closer[i].transform.position);
            }
        }
        print(closer.Count);

        //List<Node> closer = getCloserNodes();
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, closer[0].transform.position);

        //Gizmos.color = Color.cyan;
        //for (int i = 1; i < closer.Count; i++)
        //{
        //    Gizmos.DrawLine(transform.position, closer[i].transform.position);
        //}

        //Gizmos.color = Color.red;
        //foreach (var item in tryGetCloserTriangles())
        //{
        //    Gizmos.DrawLine(transform.position, item.transform.position);
        //}

        //foreach (var triangle in tryGetCloserTriangles())
        //{
        //    DrawTriangle(triangle);
        //}
    }

    struct triangle
    {
        public Vector3 A, B, C;
    }
    struct Line
    {
        public Vector3 A, B;
    }

    void DrawTriangle(triangle tri)
    {
        Gizmos.DrawLine(tri.A, tri.B);
        Gizmos.DrawLine(tri.B, tri.C);
        Gizmos.DrawLine(tri.C, tri.A);
    }

    private List<Node> tryGetCloserTriangles()
    {
        var nodesGame = GraphObject.GetComponentsInChildren<Node>()
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

        Node CloserNode = nodesGame.ElementAt(0);
        var connectedToCloser = nodesGame.Skip(1)
                               .Where(x => Connected(CloserNode, x))
                               .Take(4);

        return connectedToCloser.ToList();
    }

    bool Connected (Node A, Node B)
    {
        return A.Connections.Contains(B) && B.Connections.Contains(A);
    }
    ///// <summary>
    ///// Retorna las conecciones comunes que hay entre 2 nodos. Se presupone que el primer nodo esta conectado al segundo nodo.
    ///// </summary>
    ///// <param name="reference1">Nodo de referencia A.</param>
    ///// <param name="reference2">Node de referencia B.</param>
    ///// <param name="exclude">Nodo que queremos excluir de la comparación.</param>
    //List<Node> getCommonConnections(Node reference1, Node reference2, Node exclude)
    //{
    //    List<Node> commonConections = new List<Node>();
    //    foreach (var con in reference1.Connections)
    //    {
    //        if (con == exclude) continue;
    //        if (Connected(con, reference2))
    //            commonConections.Add(con);
    //    }
    //    return commonConections;
    //}

    private List<Node> getCloserNodes()
    {
        //Tomo 3 nodos más cercanos independientemente de sus conecciones.
        var nodesGame = GraphObject.GetComponentsInChildren<Node>()
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

        var closerOnes = nodesGame.Take(_closerNodeToSearch);

        HashSet<Node> filter = new HashSet<Node>();
        foreach (var node in closerOnes)
            filter.Add(node);

        var expandCloserOnes = closerOnes.SelectMany(x => x.Connections.Take(_connectionExpandLimit)
                                                                       .Where(cx =>
                                                                       {
                                                                           bool addToList = !filter.Contains(cx);
                                                                           if (addToList)
                                                                               filter.Add(cx);
                                                                           return addToList;
                                                                       }
                                                     ));

        //IEnumerable<Node> expandCloserOnes = Enumerable.Empty<Node>();
        //foreach (var node in closerOnes)
        //{
        //    var selected = node.Connections
        //        .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
        //        .Take(_connectionExpandLimit)
        //        .Where(x =>
        //        {
        //            bool addToList = !filter.Contains(x);
        //            if (addToList)
        //                filter.Add(x);
        //            return addToList;
        //        });
        //}

        List<Node> nodes = new List<Node>(closerOnes);
        nodes.AddRange(expandCloserOnes);

        //Tomo 3 nodos que esten conectados solo al primer nodo.
        //var nodesGame = GraphObject.GetComponentsInChildren<Node>()
        //    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

        //Node CloserNode = nodesGame.ElementAt(0);
        //var connectedToCloser = nodesGame.Skip(1)
        //                       .Where(x => Connected(CloserNode, x))
        //                       .Take(3);

        //List<Node> nodes = new List<Node>() { CloserNode };
        //nodes.AddRange(connectedToCloser);

        return nodes;
    }
}
