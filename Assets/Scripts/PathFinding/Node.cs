using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IHeapItem<Node>
{
    public int gridX;   // X position in the node array
    public int gridY;   // Y position in the node array

    public bool walkable;       // tell the program if the node is not being obstructed and then walkable
    public Vector3 position;    // World position of the node

    public Node parent;     // For the A* algoritm, will store what node it previously came from so it can trace the shortest path

    public int gCost;           // The cost of moving to the next node
    public int hCost;           // Distance from end node
    public int movementPenalty; // movement penalty of this node

    int heapIndex;

    public int FCost    // gCost + hCost
    {
        get { return gCost + hCost; }
    }
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }


    public Node (bool walkable, Vector3 position, int gridX, int gridY, int penalty)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = penalty;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
