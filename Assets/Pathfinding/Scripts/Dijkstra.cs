using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : PathFindingAlgorithm
{

    //dist = Node.gCost
    // previous[v] = Node.parent
    // 
    private Node[,] gridOfNodes;
    public override List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        gridOfNodes = grid.grid;
        path = new List<Node>(); //czyszczenie ściezki start -> meta
        Node startingNode = grid.NodeFromWorldPoint(seeker.position);
        if (startingNode.walkable)
        {
            List<Node> nodeList = new List<Node>();
            foreach (var node in gridOfNodes)
            {
                node.DijInfo.distance = Int32.MaxValue; //około nieskończoność
                node.DijInfo.parent = null;
                nodeList.Add(node);
            }

            Node actualNode;
            startingNode.DijInfo.distance = 0;
            while (nodeList.Count > 0)
            {
                nodeList = nodeList.OrderBy(node => node.DijInfo.distance).ToList(); // posortowana lista wierzchołków, po najmniejszym gCost
                nodeList = nodeList.Where(node => node.walkable == true).ToList();
                actualNode = nodeList[0];
                nodeList.Remove(actualNode);
                foreach (var neighbourNode in grid.GetNeighbours(actualNode))
                {
                    int temporaryCost = actualNode.DijInfo.distance + grid.GetDistance(actualNode, neighbourNode);
                    if (temporaryCost < neighbourNode.DijInfo.distance)
                    {
                        neighbourNode.DijInfo.distance = temporaryCost;
                        neighbourNode.parent = actualNode;
                    }
                }

            }


            path = grid.RetracePath(startingNode, grid.NodeFromWorldPoint(target.position));
            grid.pathToDrawDijkstra = path;
        }
        return path;
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    

}
