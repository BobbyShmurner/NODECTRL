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
    [SerializeField] SpriteRenderer bgSpriteRenderer;

    public void SetNodeTexture(int nodeX, int nodeY, Vector2Int nodeGroupSize, ref bool[,] nodes, ref Material bgMat)
    {
        // Set Enum Flags
        NodeSurrounding surroundingNodes = NodeSurrounding.None;

        if (nodeGroupSize.x != 1 || nodeGroupSize.y != 1) // If they both equal one, theres no need to check all of this
        {
            bool xLess = nodeX < nodeGroupSize.x - 1;
            bool xGreat = nodeX > 0;
            bool yLess = nodeY < nodeGroupSize.y - 1;
            bool yGreat = nodeY > 0;

            if (yLess   &&              nodes[nodeX, nodeY + 1])     surroundingNodes |= NodeSurrounding.Up;
            if (yGreat  &&              nodes[nodeX, nodeY - 1])     surroundingNodes |= NodeSurrounding.Down;
            if (xGreat  &&              nodes[nodeX - 1, nodeY])     surroundingNodes |= NodeSurrounding.Left;
            if (xLess   &&              nodes[nodeX + 1, nodeY])     surroundingNodes |= NodeSurrounding.Right;
            if (xGreat  &&  yLess    && nodes[nodeX - 1, nodeY + 1]) surroundingNodes |= NodeSurrounding.TopLeft;
            if (xLess   &&  yLess    && nodes[nodeX + 1, nodeY + 1]) surroundingNodes |= NodeSurrounding.TopRight;
            if (xGreat  &&  yGreat   && nodes[nodeX - 1, nodeY - 1]) surroundingNodes |= NodeSurrounding.BottomLeft;
            if (xLess   &&  yGreat   && nodes[nodeX + 1, nodeY - 1]) surroundingNodes |= NodeSurrounding.BottomRight;
        }

        spriteRenderer.sprite = NodeGroup.sprites[(int)surroundingNodes];
        bgSpriteRenderer.material = bgMat;
    }
}
