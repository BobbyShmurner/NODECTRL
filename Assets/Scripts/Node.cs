using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[System.Flags]
enum NodeSurrounding
{
    None = 0,

    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,

    TopLeft = 16,
    TopRight = 32,
    BottomLeft = 64,
    BottomRight = 128,

    All = 255,
    UseMeIfYouWantToBreakTheGame = 256
}

public class Node : MonoBehaviour
{
    [HideInInspector] public NodeGroup nodeGroup;

    [SerializeField] SpriteRenderer spriteRenderer;

    public void SetNodeTexture(int nodeX, int nodeY, Vector2Int nodeGroupSize, ref bool[,] nodes)
    {
        // Set Enum Flags
        NodeSurrounding surroundingNodes = NodeSurrounding.None;

        if (nodeGroupSize.x != 1 || nodeGroupSize.y != 1) // If they both equal one, theres no need to check all of this
        {
            if (nodeY < nodeGroupSize.y - 1 && nodes[nodeX, nodeY + 1]) surroundingNodes = surroundingNodes | NodeSurrounding.Up;
            if (nodeY > 0 && nodes[nodeX, nodeY - 1]) surroundingNodes = surroundingNodes | NodeSurrounding.Down;
            if (nodeX > 0 && nodes[nodeX - 1, nodeY]) surroundingNodes = surroundingNodes | NodeSurrounding.Left;
            if (nodeX < nodeGroupSize.x - 1 && nodes[nodeX + 1, nodeY]) surroundingNodes = surroundingNodes | NodeSurrounding.Right;
            if (nodeX > 0 && nodeY < nodeGroupSize.y - 1 && nodes[nodeX - 1, nodeY + 1]) surroundingNodes = surroundingNodes | NodeSurrounding.TopLeft;
            if (nodeX < nodeGroupSize.x - 1 && nodeY < nodeGroupSize.y - 1 && nodes[nodeX + 1, nodeY + 1]) surroundingNodes = surroundingNodes | NodeSurrounding.TopRight;
            if (nodeX > 0 && nodeY > 0 && nodes[nodeX - 1, nodeY - 1]) surroundingNodes = surroundingNodes | NodeSurrounding.BottomLeft;
            if (nodeX < nodeGroupSize.x - 1 && nodeY > 0 && nodes[nodeX + 1, nodeY - 1]) surroundingNodes = surroundingNodes | NodeSurrounding.BottomRight;
        }

        spriteRenderer.sprite = NodeGroup.sprites[(int)surroundingNodes];
    }
}
