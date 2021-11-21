using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.U2D;

public class NodeGroup : MonoBehaviour
{
    [SerializeField] SpriteAtlas spriteAtlas;
    [SerializeField] Material energyMaterial;

    [Header("Node Settings")]
    [SerializeField] Color energyColor = Color.red;
    [SerializeField] Vector2Int nodeGroupSize;
    [SerializeField] float energyCount = 100f;
    [SerializeField] [Range(0.0f, 1.0f)] float energyLevel = 1.0f;


    static public Sprite[] sprites;
    static public float ppu;

    SpriteMask mask;
    Transform nodesEmptyTrans; // This is just for orginisation purporses

    bool[,] nodes;
    float maxEnergyCount;
    Vector2 unitScale; // This is the size of the NodeGroup in Unity units;

    private void Awake()
    {
        if (sprites == null)
        {
            sprites = new Sprite[256];
            spriteAtlas.GetSprites(sprites);
            System.Array.Sort(sprites, (x, y) => string.Compare(x.name, y.name));

            ppu = sprites[0].pixelsPerUnit;
        }

        mask = GetComponentInChildren<SpriteMask>();
        nodesEmptyTrans = transform.GetChild(0); // Child 0 Should be the empty 'Nodes' GO
    }

    void Start()
    {
        GenerateNodes();
    }

    private void Update()
    {
        GenerateNodes();
        UpdateEnergyLevel();
    }

    public void GenerateNodes()
    {
        /*Stopwatch sw = Stopwatch.StartNew();

        long newObjectTicks = 0;
        long nodePropTicks = 0;
        long nodeTextureTicks = 0;
        long nodeDestruction = 0;*/

        // Release Existing Nodes
        for (int i = 0; i < nodesEmptyTrans.childCount;)
        {
            // sw.Restart();

            NodeManager.ReleaseNode(nodesEmptyTrans.GetChild(0).gameObject);

            // sw.Stop();
            // nodeDestruction += sw.ElapsedTicks;
        }

        maxEnergyCount = energyCount / energyLevel;
        unitScale = new Vector2(nodeGroupSize.x / (ppu * 0.2f), nodeGroupSize.y / (ppu * 0.2f));

        // This just populates the node group for testing
        nodes = new bool[nodeGroupSize.x, nodeGroupSize.y];
        for (int x = 0; x < nodeGroupSize.x; x++)
        {
            for (int y = 0; y < nodeGroupSize.y; y++)
            {
                nodes[x, y] = Random.Range(0, 5) != 0;
            }
        }

        // Set Material Color
        energyMaterial.SetColor("_Color", energyColor);

        // Create Nodes
        for (int x = 0; x < nodeGroupSize.x; x++)
        {
            for (int y = 0; y < nodeGroupSize.y; y++)
            {
                if (!nodes[x, y]) continue;

                // sw.Restart();

                GameObject newNode = NodeManager.GetNode();
                newNode.transform.parent = nodesEmptyTrans;
                newNode.name = $"Node ({x}, {y})";

                /*sw.Stop();
                newObjectTicks += sw.ElapsedTicks;
                sw.Restart();*/

                Vector2 pos = new Vector2(0.5f, 0.5f);
                pos.x = x * 1 / (ppu * 0.2f);
                pos.y = y * 1 / (ppu * 0.2f);

                newNode.transform.localPosition = new Vector2(pos.x - ((nodeGroupSize.x - 1) / (ppu * 0.2f)) * 0.5f, pos.y - ((nodeGroupSize.y - 1) / (ppu * 0.2f)) * 0.5f);

                /*sw.Stop();
                nodePropTicks += sw.ElapsedTicks;
                sw.Restart();*/

                newNode.GetComponent<Node>().nodeGroup = this;
                newNode.GetComponent<Node>().SetVars(x, y, nodeGroupSize, ref nodes, ref energyMaterial);

                /*sw.Stop();
                nodeTextureTicks += sw.ElapsedTicks;*/
            }
        }

        // UnityEngine.Debug.Log($"Node Instantiation: Avg: {newObjectTicks / (nodeGroupSize.x * nodeGroupSize.y)} ticks, Total: {newObjectTicks}");
        // UnityEngine.Debug.Log($"Texture Instantiation: Avg: {nodeTextureTicks / (nodeGroupSize.x * nodeGroupSize.y)} ticks, Total: {nodeTextureTicks}");
        //UnityEngine.Debug.Log($"Property Instantiation: Avg: {nodePropTicks / (nodeGroupSize.x * nodeGroupSize.y)} ticks, Total: {nodePropTicks}");
        //UnityEngine.Debug.Log($"Node Destruction: Avg: {nodeDestruction / (nodeGroupSize.x * nodeGroupSize.y)} ticks, Total: {nodeDestruction}");

        // Setup Mask and Energy Count
        UpdateEnergyLevel();
    }

    void UpdateEnergyLevel()
    {
        energyCount = maxEnergyCount * energyLevel;

        mask.transform.localScale = new Vector3(unitScale.x, unitScale.y * energyLevel, 1);
        mask.transform.localPosition = new Vector3(0, unitScale.y * 0.5f * energyLevel - unitScale.y * 0.5f, 0);
    }
}