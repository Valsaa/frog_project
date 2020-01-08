using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XDaddy.Character;

public class Agent : MonoBehaviour
{
    // Private parameters
    [SerializeField]
    private PlayerInput playerInput = new PlayerInput();
    private SteeringBehavior2D steering2D;
    private Vector2 nextPosition;

    // Public parameters
    [SerializeField]
    private float maxForce = 0.1f;
    public float MaxForce
    {
        get
        {
            return maxForce;
        }
        set
        {
            if (value < 0)
            {
                maxForce = 0.1f;
            }
            else
            {
                maxForce = value;
            }
        }
    }
    [SerializeField]
    private float maxSpeed = 4;
    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
        set
        {
            if (value < maxForce)
            {
                maxSpeed = maxForce;
            }
            else
            {
                maxSpeed = value;
            }
        }
    }
    [SerializeField]
    const float mass = 1;
    public float Mass
    {
        get
        {
            return mass;
        }
        /*set
        {
            if (value < 0)
            {
                mass = 0.001f;
            }
            else
            {
                mass = value;
            }
        }*/
    }

    private Vector2 velocity = new Vector2(0, 0);
    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }
    }

    public float slowingRadius = 1.0f;

    /*
     *  PUBLIC FUNCTION
     */
    public Vector2 GetPosition()
    {
        return transform.position;
    }
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public float GetSpeed()
    {
        return velocity.magnitude;
    }
    public void SetInitialVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }


    /*
     *  UNITY 3D FUNCTION
     */
    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        steering2D = new SteeringBehavior2D(this);
    }

    protected virtual void Update()
    {
        playerInput.GetInputs();

        // Get mouse position
        if (playerInput.Mouse.ButtonRight.GetDown())
        {
            nextPosition = playerInput.Mouse.ScreenToWorldPoint2D();
            steering2D.Arrive_On(nextPosition, slowingRadius);
        }

        Vector2 steeringForce = steering2D.Calcule();
        steeringForce = steeringForce / mass;

        velocity = Vector2.ClampMagnitude(velocity + steeringForce, maxSpeed);

        transform.position = transform.position + (new Vector3(velocity.x, velocity.y, 0) * Time.fixedDeltaTime);
    }

    protected virtual void FixedUpdate()
    {
    }


}
