using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.UI;
using UnityEngine;

public class RobotMovingController : MonoBehaviour
{
    private PathFindingAlgorithm PathFinding;
    public Transform target;
    private Rigidbody rig;
    private List<Node> path;

    public float moveSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        PathFinding = GetComponent<PathFindingAlgorithm>();
        rig = GetComponent<Rigidbody>();
        PathFinding.seeker = transform;
        PathFinding.target = target;
        var grid = GetComponentInParent<Grid>();
        PathFinding.grid = grid;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) > PathFinding.grid.nodeRadius+1)
        {
            Move();
        }
        else
        {
            rig.velocity=Vector3.zero;
        }
      
    }

    public void Move()
    {
        path = PathFinding.FindPath(transform.position, target.position);
        if (path != null && path.Count<0)
        {
            var dir = transform.position - path[0].worldPosition;
            var rotation = Quaternion.LookRotation((new Vector3(dir.x, 0, dir.z))*Time.deltaTime);
            transform.rotation = rotation;
            rig.AddForce(moveSpeed * -transform.forward * Time.deltaTime);
            if (rig.velocity.sqrMagnitude >= 50)
            {
                rig.velocity *= 0.8f;
            }
;        }
    }
}
