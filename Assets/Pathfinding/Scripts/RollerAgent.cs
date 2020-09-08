using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    //komponent odpowiedzialny za obliczenia fizyczne - parametry 
    private Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    //referencja do Transform - przechowującej pozycje, orientacje, skalę w świecie 3d celu typu GameObject
    public Transform Target;

    /// <summary>
    /// Resetowanie pozycji agenta
    /// </summary>
    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            //jezeli spadnie agent, to ped jest 0 i wroc na pozycje startową
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }
        // ustaw cel na nową losową pozycję
        Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }
    /// <summary>
    /// observacja środowiska i zapamiętywanie zaobserwowanych danych dla mózgu
    /// </summary>
    public override void CollectObservations()
    {
        //pozycja celu
        AddVectorObs(Target.position);
        //pozycja agenta
        AddVectorObs(this.transform.position);
        //wektor prędkości agenta
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }
    public float speed = 10;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        // Fell off platform
        if (this.transform.position.y < 0)
        {
            Done();
        }

    }
    /// <summary>
    /// metoda domanualnego testowania poprzezpodawanie pozycji z klawaitury
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

}
