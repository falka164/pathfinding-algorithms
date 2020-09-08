using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFindingAlgorithm :MonoBehaviour
{
    [HideInInspector]
    public Transform seeker;
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public List<Node> path;
    public abstract List<Node> FindPath(Vector3 startPos, Vector3 targetPos);
}
