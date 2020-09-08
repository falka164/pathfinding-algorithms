using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : PathFindingAlgorithm
{
    private void Update()
    {
        FindPath(seeker.position, target.position);
    }
    public override List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count >0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i< openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                    if(openSet[i].hCost < currentNode.hCost)// aktualny wierzchołek w OPEN z najniższym kosztem f_cost
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                path = grid.RetracePath(startNode, targetNode);
                grid.pathToDrawAstar = path;
                return path;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCostToNeighbour = currentNode.gCost + grid.GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = grid.GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return new List<Node>();
    }


    

    
}

