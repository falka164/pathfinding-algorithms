using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArea : MonoBehaviour
{
    public GameObject WallPrefab;
    public List<GameObject> Walls;
    public RobotAgent Agent;
    public GameObject Target;
    public LayerMask RobotTargetLayerMask;
    private Grid grid;
    private AStarPathFinding aStarPath;
    public int WallCounter;
    public int range = 0;
    void Start()
    {
        Walls = new List<GameObject>();
        Agent = GetComponentInChildren<RobotAgent>();
        grid = GetComponent<Grid>();
        grid.WorldBottomLeft= transform.position - Vector3.right * grid.gridWorldSize.x / 2 - Vector3.forward * grid.gridWorldSize.y / 2;
        grid.CreateGrid();
        aStarPath = GetComponent<AStarPathFinding>();
        aStarPath.grid = grid;
        aStarPath.seeker = Agent.transform;
        aStarPath.target = Target.transform;
        grid.player = Agent.transform;
       
        SetTargetAtRandomPosition();
        initializeMap();
        aStarPath.enabled = false;
    }

    /// <summary>
    /// Tworzy nową mapę
    /// </summary>
    public void initializeMap()
    {
        int errors = 0;
        grid.updateAllNodes();
        for (int i = 0; i < WallCounter; i++)
        {
            if (errors > WallCounter)
            {
                return;
            }
            GameObject newWall = Instantiate(WallPrefab, transform);
            var newPosition = new Vector3(Random.RandomRange(-range, range), 0.5f,Random.RandomRange(-range,range))+transform.position;

            while (Physics.CheckBox(newPosition, new Vector3(0.5f,0.5f,0.5f)))
            {
                if (errors > WallCounter)   
                {
                    Destroy(newWall);
                    newWall = null;
                    break;
                }
                errors++;
                newPosition = new Vector3(Random.RandomRange(-range, range), 0.5f, Random.RandomRange(-range, range)) + transform.position;
                
            }

            if (newWall != null)
            {
                newWall.transform.position = newPosition;
                grid.updateAllNodes();
                if (checkIfPathExist())
                {
                    errors = 0;
                    Walls.Add(newWall);
                }
                else
                {
                    errors++;
                    if (newWall != null)
                        Destroy(newWall);
                    i--;
                }
            }
        }
        
    }

    public void changeSizes()
    {
        foreach (var wall in Walls)
        {
            wall.transform.localScale=new Vector3(1,4,1);
        }
    }
    /// <summary>
    /// Kasuje wszystkie ściany i ustawia agenta w startowej pozycji
    /// </summary>
    public void resetMap()
    {
        aStarPath.enabled = true;
        ClearWalls();
        Agent.transform.position = Agent.StartingPosition;
        SetTargetAtRandomPosition();
        initializeMap();
        aStarPath.enabled = false;
        changeSizes();

    }
    /// <summary>
    /// Sprawdza czy istnieje ścieżka do celu - Astar
    /// </summary>
    /// <returns></returns>
    public bool checkIfPathExist()
    {
        grid.updateAllNodes();
        var path = aStarPath.FindPath(Agent.transform.position, Target.transform.position);
        if (path.Count>0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// usuwanie GameObejct walll z mapy
    /// </summary>
    public void ClearWalls()
    {
        foreach (var wall in Walls)
        {
            Destroy(wall);
            
        }
        Walls=new List<GameObject>();
    }

    public void SetTargetAtRandomPosition()
    {
        Target.transform.position = new Vector3(Random.RandomRange(-range, range), 1, Random.RandomRange(-range, range))+transform.position;
    }
}
