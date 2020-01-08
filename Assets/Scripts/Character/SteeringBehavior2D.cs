using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior { SEEK = 0, FLEE, PURSUIT, EVADE, WANDER, FOLLOWPATH, ARRIVE };

public class SteeringBehavior2D
{
    // Variables
    private bool[] behavior = new bool[7];
    private Agent agent;

    private Vector2 seekTarget;

    private Vector2 arriveTarget;
    private float slowingRadius = 0;

    public SteeringBehavior2D(Agent agent)
    {
        this.agent = agent;
        for (int i = 0; i < behavior.Length; i++)
        {
            behavior[i] = false;
        }
    }



    /*
     *  PUBLIC FUNCTION
     */
    public bool On(Behavior onBehavior)
    {
        return behavior[(int)onBehavior];
    }

    public void Seek_On(Vector2 seekTarget)
    {
        this.seekTarget = seekTarget;
        behavior[(int)Behavior.SEEK] = true;
    }
    public void Seek_Off()
    {
        this.seekTarget = Vector2.zero;
        behavior[(int)Behavior.SEEK] = false;        
    }

    public void Arrive_On(Vector2 arriveTarget, float slowingRadius)
    {
        this.arriveTarget = arriveTarget;
        this.slowingRadius = slowingRadius;
        behavior[(int)Behavior.ARRIVE] = true;
    }
    public void Arrive_Off()
    {
        this.arriveTarget = Vector2.zero;
        behavior[(int)Behavior.ARRIVE] = false;
    }

    public Vector2 Calcule()
    {
        Vector2 force = Vector2.zero;

        if (On(Behavior.SEEK))
        {
            Vector2 steering = Seek(seekTarget);
            if (!AccumulatedForce(ref force, steering)) return force;
        }

        if (On(Behavior.ARRIVE))
        {
            Vector2 steering = Arrive(arriveTarget);
            if (!AccumulatedForce(ref force, steering)) return force;
        }

        return force;
    }




    /*
    *  PRIVATE FUNCTION
    */
    private bool AccumulatedForce(ref Vector2 currentForce, Vector2 forceToAdd)
    {
        currentForce = Vector2.ClampMagnitude(currentForce + forceToAdd, agent.MaxForce);

        if (currentForce.magnitude == agent.MaxForce)
            return false;

        return true;

    }
    
    private Vector2 Seek(Vector2 target)
    {
        Vector2 desiredVelocity = target - agent.GetPosition();
        desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed;

        Vector2 steering = desiredVelocity - agent.Velocity;

        return steering;
    }

    private Vector2 Arrive(Vector2 target)
    {
        Vector2 desiredVelocity = target - agent.GetPosition();

        // Check the distance to detect whether the character is inside the slowing area
        float distance = desiredVelocity.magnitude;
        if (distance < slowingRadius) 
            desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed * (distance / slowingRadius); // Inside the slowing area
        else 
            desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed;  // Outside the slowing area.

        Vector2 steering = desiredVelocity - agent.Velocity;

        return steering;
    }


}
