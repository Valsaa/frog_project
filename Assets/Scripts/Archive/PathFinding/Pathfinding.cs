using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    private Grid grid;    

    void Awake()
    {
        grid = GetComponent<Grid>();
    }	
    

    public void FindPath(PathRequest request, OnPathDelegate callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
                
        bool pathSuccess = false;

        Node startNode = grid.GetNearestNode(request.pathStart);
        Node targetNode = grid.GetNearestNode(request.pathEnd);
        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openList = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedList = new HashSet<Node>();
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // Start by adding the original position to the openList
                // And get the node with the lowest F score and the lowest H cost
                Node currentNode = openList.RemoveFirst();

                // Add the current node to the closedList
                closedList.Add(currentNode);

                // If we added the destination to the closedList, we've found a path
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found : " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighborNode in grid.GetNeighboringNodes(currentNode))
                {
                    // If this adjacent node is a wall or is already in the closedList ignore it
                    if (!neighborNode.walkable || closedList.Contains(neighborNode))
                    {
                        continue;   // Go to the next adjacent node
                    }

                    // Test if using the current G score make the parent F score lower, and test if its not in the openList
                    int moveCost = currentNode.gCost + grid.GetDistance(currentNode, neighborNode) + neighborNode.movementPenalty;
                    if (moveCost < neighborNode.gCost || !openList.Contains(neighborNode))
                    {
                        // Compute its score, set the parent
                        neighborNode.gCost = moveCost;
                        neighborNode.hCost = grid.GetDistance(neighborNode, targetNode);
                        neighborNode.parent = currentNode;

                        // Add it to the openList
                        if (!openList.Contains(neighborNode))
                            openList.Add(neighborNode);
                        else
                            openList.UpdateItem(neighborNode);
                    }
                }
            }
        }

        request.pipelineState = PathState.Error;
        if (pathSuccess)
        {
            request.path = RetracePath(startNode, targetNode);
            pathSuccess = request.path.Length > 0;

            if (pathSuccess)
                request.pipelineState = PathState.Calculated;
        }
        callback(request);
    }
    
    private Vector3[] RetracePath(Node startingNode, Node endNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startingNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(finalPath);
        Array.Reverse(waypoints);
        return waypoints;
    }
    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        waypoints.Add(path[0].position);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
                waypoints.Add(path[i].position);

            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

}

