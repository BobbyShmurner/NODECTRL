using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager _Singleton;
    public static GameObject[] NodePool;

    [SerializeField] GameObject nodeObject;
    [SerializeField] int poolCount;

    private void Awake()
    {
        if (_Singleton != null) Destroy(gameObject);
        _Singleton = this;

        NodePool = new GameObject[poolCount];

        GameObject tmpNode;
        for (int i = 0; i < poolCount; i++)
        {
            tmpNode = Instantiate(nodeObject/*, transform*/);
            //tmpNode.transform.name = "Node (Unused)";
            tmpNode.SetActive(false);
            NodePool[i] = tmpNode;
        }

        //UpdateName();
    }

    public static GameObject GetNode()
    {
        GameObject nodeFetched;
        for (int i = 0; i < NodePool.Length; i++)
        {
            if (!NodePool[i].activeInHierarchy)
            {
                nodeFetched = NodePool[i];

                nodeFetched.SetActive(true);
                //nodeFetched.transform.parent = null;
                //nodeFetched.name = "Node";

                //UpdateName();

                return nodeFetched;
            }
        }

        Debug.LogError("Failed to fetch a Node from the NodePool, All Nodes are in use!");
        return null;
    }

    public static void ReleaseNode(GameObject node)
    {
        node.SetActive(false);
        //node.transform.parent = _Singleton.transform;
        //node.transform.name = "Node (Unused)";

        //UpdateName();
    }

    static void UpdateName()
    {
        _Singleton.name = $"Node Manager (Free Nodes: {_Singleton.transform.childCount})";
    }
}
