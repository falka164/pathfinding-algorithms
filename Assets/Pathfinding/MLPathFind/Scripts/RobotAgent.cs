using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class RobotAgent : Agent
{
    public Vector3 StartingPosition;
    private Rigidbody rBody;
    public Transform Target;
    private RobotArea arena;
    public float speed = 10;
    private bool useVectorObs;
    private RayPerception3D robotRay;
    private Vector3 controlSignal;
    private float bestDistance;
    public override void AgentReset()
    {
        base.AgentReset();
     
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = StartingPosition;
            if(arena!=null)
                arena.resetMap();
            bestDistance = Vector3.Distance(transform.position, Target.position);
    }

    public override void AgentOnDone()
    {
        base.AgentOnDone();
        //W tym miejscu agent powinien resetować mape
      

    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rBody = GetComponent<Rigidbody>();
        robotRay = GetComponent<RayPerception3D>();
        StartingPosition = transform.position;
        arena = GetComponentInParent<RobotArea>();
        bestDistance = Vector3.Distance(transform.position, Target.position);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vectorAction"></param>
    /// <param name="textAction"></param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        AddReward(-1f / agentParameters.maxStep);
        var newDistance = Vector3.Distance(transform.position, Target.position);
        //if (newDistance<bestDistance)
        //{
        //    bestDistance = newDistance;
        //    AddReward(1/agentParameters.maxStep);
        //}
        if (rBody.velocity.sqrMagnitude > 25f) // slow it down
        {
            rBody.velocity *= 0.95f;
        }
        if (this.transform.position.y < -2)
        {
            Done();
        }
        MoveAgent(vectorAction);
    }

    public override void CollectObservations()
    {
        base.CollectObservations();
        //odległośc od celu
        AddVectorObs(Vector3.Distance(transform.position, Target.position));
        //prędkośc w osi x,z
        var localVelocity = transform.InverseTransformDirection(rBody.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        //tut powinny observacje z czujnika rayperception 3d
        var rayDistance = 35f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "Wall", "Target"};
            AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
            AddVectorObs(robotRay.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
       // AddVectorObs(bestDistance);
    //    AddVectorObs(transform.position);
     //   AddVectorObs(Target.position);
    }
    /// <summary>
    /// Sprawdzenie czy nastąpiła kolizja z celem, dodanie nagrody
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            SetReward(2);
            Done();
        }
        //else if (collision.gameObject.CompareTag("Wall"))
        //{
        //    AddReward((-0.00001f)); // kara za uderzenie w coś.
        //}
    }

    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var action = Mathf.FloorToInt(act[0]);
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime*150f);
        rBody.AddForce(dirToGo* speed, ForceMode.VelocityChange);
    }
}
