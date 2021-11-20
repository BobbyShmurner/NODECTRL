using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class NodeGroup : MonoBehaviour
{
    [SerializeField] SpriteAtlas spriteAtlas;
    [SerializeField] GameObject node;

    [Header("Node Settings")]
    [SerializeField] Color energyColor = Color.red;
    [SerializeField] Vector2Int nodeGroupSize;
    [SerializeField] float energyCount;
    [SerializeField] [Range(0.0f, 1.0f)] float energyLevel;

    static public Sprite[] sprites;
    static public float ppu;

    bool[,] nodes;
    float maxEnergyCount;

    private void Awake()
    {
        if (sprites == null)
        {
            sprites = new Sprite[256];
            spriteAtlas.GetSprites(sprites);
            System.Array.Sort(sprites, (x, y) => string.Compare(x.name, y.name));

            ppu = sprites[0].pixelsPerUnit;
        }
    }

    void Start()
    {
        GenerateNodes();
    }

    public void GenerateNodes()
    {
        // Destroy Existing Nodes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Sets the Max Energy. This is used for calculating the percentage full
        maxEnergyCount = energyCount;

        // This just populates the node group for testing
        nodes = new bool[nodeGroupSize.x, nodeGroupSize.y];
        for (int x = 0; x < nodeGroupSize.x; x++)
        {
            for (int y = 0; y < nodeGroupSize.y; y++)
            {
                nodes[x, y] = !(Random.Range(0, 5) == 0);
            }
        }

        // Create Nodes
        for (int x = 0; x < nodeGroupSize.x; x++)
        {
            for (int y = 0; y < nodeGroupSize.y; y++)
            {
                if (!nodes[x, y]) continue;

                GameObject newNode = Instantiate(node, transform);
                newNode.name = $"Node ({x}, {y})";

                Vector2 pos = new Vector2(0.5f, 0.5f);
                pos.x = x * 1 / (ppu * 0.2f);
                pos.y = y * 1 / (ppu * 0.2f);

                newNode.transform.localPosition = new Vector2(pos.x - ((nodeGroupSize.x - 1) / (ppu * 0.2f)) / 2f, pos.y - ((nodeGroupSize.y - 1) / (ppu * 0.2f)) / 2f);

                newNode.GetComponent<Node>().nodeGroup = this;
                newNode.GetComponent<Node>().SetNodeTexture(x, y, nodeGroupSize, ref nodes);

                newNode.transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Color", energyColor);
            }
        }
    }
}

// (0,0) = (-1.5, -1.5)
// (1,1) = ( 0.0,  0.0)
// (2,2) = ( 1.5,  1.5)

// ((x / 2) * width) - (width / 2)