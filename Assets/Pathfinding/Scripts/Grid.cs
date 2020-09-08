using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Grid : MonoBehaviour // klasa jest odpowiedzialna za komuterową reprezentację grafu
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize; 
    public float nodeRadius;
    public List<Node> pathToDrawAstar;
    public List<Node> pathToDrawDijkstra;
    public Node[,] grid; //2 wymiarowa reprezentacja wierzchołków
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public bool isLearning = true;
    public Vector3 WorldBottomLeft;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void Update()
    {
        if(grid!=null)
            updateAllNodes();
    }
    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            { 
                grid[x, y] = new Node(false,getWorldPoint(worldBottomLeft,x,y), x, y);
                grid[x, y].walkable = checkIfNodeIsWalkabe(grid[x, y].worldPosition);
            }
        }
    }

    private bool checkIfNodeIsWalkabe(Vector3 worldPosition)
    {
        bool walkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask)); //check if node is walkable
        return walkable;
    }

    public void updateAllNodes()
    {
        if(grid != null)
            foreach (var node in grid)
            {
                updateNodeIsWalkabe(node);
            }
    }
    public void updateNodeIsWalkabe(Node node)
    {
        node.walkable = !(Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask)); //check if node is walkable
    }

    private Vector3 getWorldPoint(Vector3 worldBottomLeft, int xPosition, int yPosition)
    {
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (xPosition * nodeDiameter + nodeRadius) + Vector3.forward * (yPosition * nodeDiameter + nodeRadius);
        return worldPoint;
    }

    public List<Node> GetNeighbours(Node node) 
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y==0) continue;//jeżeli x,y są równe pozycji wierzchołka to pomiń 


                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX <gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }

                
            }

        }
        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }

    }
    public List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;

        }

        path.Reverse();

        return path;

    }
    public void OnDrawGizmos()
    {
        if (!isLearning)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(player.position);

                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (pathToDrawAstar != null)
                    {
                        if (pathToDrawAstar.Contains(n))
                        {
                            Gizmos.color = Color.blue;
                        }
                    }

                    if (pathToDrawDijkstra != null)
                    {
                        if (pathToDrawDijkstra.Contains(n))
                        {
                            Gizmos.color = Color.green;
                        }
                    }

                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }

                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }

            }
        }

    }
}
