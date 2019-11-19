using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : MonoBehaviour
{/*
    #region Shape
    /// <summary>
    /// Radius of the agent in world units.
    /// This is visualized in the scene view as a yellow cylinder around the character.
    /// </summary>
    public float radius = 0.5f;

    /// <summary>
    /// Height of the agent in world units.
    /// This is visualized in the scene view as a yellow cylinder around the character.
    /// </summary>
    public float height = 2;

    /// <summary>
    /// Offset along the Y coordinate, normally the pivot of the character is at the character's feet.
    /// </summary>
    public float centerOffset
    {
        get { return height * 0.5f; }
        set { height = value * 2; }
    }
    #endregion


    #region Pathfinding
    /// <summary>
    /// Enables or disables recalculating the path at regular intervals.
    /// Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
    ///
    /// Note that this only disables automatic path recalculations. If you call the "SearchPath()" method a path will still be calculated.
    /// </summary>
    public bool canSearch = true;

    /// <summary>
    /// Determines how often the agent will search for new paths (in seconds).
    /// The agent will plan a new path to the target every N seconds.
    ///
    /// If you have fast moving targets or AIs, you might want to set it to a lower value.
    /// </summary>
    public float repathRate = 0.5f;

    /// <summary>
    /// Position in the world that this agent should move to.
    ///
    /// If no destination has been set yet, then (+infinity, +infinity, +infinity) will be returned.
    ///
    /// Note that setting this property does not immediately cause the agent to recalculate its path.
    /// So it may take some time before the agent starts to move towards this point.
    ///
    /// If you are setting a destination and then want to know when the agent has reached that destination
    /// then you should check both "pathPending" and "reachedEndOfPath":
    /// <code>
    /// IEnumerator Start () {
    ///     ai.destination = somePoint;
    ///     // Start to search for a path to the destination immediately
    ///     // Note that the result may not become available until after a few frames
    ///     // ai.pathPending will be true while the path is being calculated
    ///     ai.SearchPath();
    ///     // Wait until we know for sure that the agent has calculated a path to the destination we set above
    ///     while (ai.pathPending || !ai.reachedEndOfPath) {
    ///         yield return null;
    ///     }
    ///     // The agent has reached the destination now
    /// }
    /// </code>
    /// </summary>
    public Vector3 destination { get; set; }

    /// <summary>
    /// Point on the path which the agent is currently moving towards.
    /// This is usually a point a small distance ahead of the agent
    /// or the end of the path.
    ///
    /// If the agent does not have a path at the moment, then the agent's current position will be returned.
    /// </summary>
    public Vector3 steeringTarget { get; private set; }

    /// <summary>
    /// Remaining distance along the current path to the end of the path.
    /// If the agent does not currently have a path, then positive infinity will be returned.
    ///
    /// Note: This is the distance to the end of the path, which may or may not be at the "destination".
    /// If the character cannot reach the destination it will try to move as close as possible to it.
    ///
    /// Warning: Since path requests are asynchronous, there is a small delay between a path request being sent and this value being updated with the new calculated path.
    /// </summary>
    public float remainingDistance { get; private set; }

    /// <summary>
    /// True if the ai has reached the "destination".
    ///
    /// This value will be updated immediately when the "destination" is changed (in contrast to "reachedEndOfPath"), however since path requests are asynchronous
    /// it will use an approximation until it sees the real path result. What this property does is to check the distance to the end of the current path, and add to that the distance
    /// from the end of the path to the "destination" (i.e. is assumes it is possible to move in a straight line between the end of the current path to the destination) and then checks if that total
    /// distance is less than "endReachedDistance". This property is therefore only a best effort, but it will work well for almost all use cases.
    /// 
    /// The cases which could be problematic are if an agent is standing next to a very thin wall and the destination suddenly changes to the other side of that thin wall.
    /// During the time that it takes for the path to be calculated the agent may see itself as alredy having reached the destination because the destination only moved a very small distance (the wall was thin),
    /// even though it may actually be quite a long way around the wall to the other side.
    /// </summary>
    public bool reachedDestination { get; private set; }

    /// <summary>
    /// True if the agent has reached the end of the current path.
    ///
    /// Note that setting the "destination" does not immediately update the path, nor is there any guarantee that the
    /// AI will actually be able to reach the destination that you set. The AI will try to get as close as possible.
    /// Often you want to use "reachedDestination" instead which is easier to work with.
    /// </summary>
    public bool reachedEndOfPath { get; private set; }
    
    /// <summary>
    /// True if this agent currently has a path that it follows
    /// </summary>
    public bool hasPath { get; private set; }

    /// <summary>
    /// True if a path is currently being calculated
    /// </summary>
    public bool pathPending { get; private set; }
      


    /// <summary>
    /// True if the path should be automatically recalculated as soon as possible
    /// </summary>
    protected virtual bool shouldRecalculatePath
    {
        get
        {
            return Time.time - lastRepath >= repathRate && !waitingForPathCalculation && canSearch && !float.IsPositiveInfinity(destination.x);
        }
    }
    
    /// <summary>Only when the previous path has been calculated should the script consider searching for a new path</summary>
    protected bool waitingForPathCalculation = false;

    /// <summary>Time when the last path request was started</summary>
    protected float lastRepath = float.NegativeInfinity;

    #endregion


    #region Movement
    /// <summary>
    /// Enables or disables movement completely.
    /// If you want the agent to stand still, but still react to local avoidance and use gravity: use "isStopped" instead.
    ///
    /// This is also useful if you want to have full control over when the movement calculations run.
    /// </summary>
    public bool canMove = true;

    /// <summary>
    /// Gets or sets if the agent should stop moving.
    /// If this is set to true the agent will immediately start to slow down as quickly as it can to come to a full stop.
    /// The agent will still react to local avoidance and gravity (if applicable), but it will not try to move in any particular direction.
    ///
    /// The current path of the agent will not be cleared, so when this is set
    /// to false again the agent will continue moving along the previous path.
    ///
    /// The "steeringTarget" property will continue to indicate the point which the agent would move towards if it would not be stopped.
    /// </summary>
    public bool isStopped = false;

    /// <summary>
    /// If true, the AI will rotate to face the movement direction.
    /// </summary>
    public bool enableRotation = true;

    /// <summary>
    /// Max speed in world units per second
    /// </summary>
    public float maxSpeed = 1;

    /// <summary>
    /// How quickly the agent accelerates.
    /// Positive values represent an acceleration in world units per second squared.
    /// Negative values are interpreted as an inverse time of how long it should take for the agent to reach its max speed.
    /// For example if it should take roughly 0.4 seconds for the agent to reach its max speed then this field should be set to -1/0.4 = -2.5.
    /// For a negative value the final acceleration will be: -acceleration*maxSpeed.
    /// This behaviour exists mostly for compatibility reasons.
    ///
    /// In the Unity inspector there are two modes: Default and Custom. In the Default mode this field is set to -2.5 which means that it takes about 0.4 seconds for the agent to reach its top speed.
    /// In the Custom mode you can set the acceleration to any positive value.
    /// </summary>
    public float maxAcceleration = -2.5f;

    /// <summary>
    /// Rotation speed in degrees per second.
    /// Rotation is calculated using Quaternion.RotateTowards. This variable represents the rotation speed in degrees per second.
    /// The higher it is, the faster the character will be able to rotate.
    /// </summary>
    public float rotationSpeed = 360;



    /// <summary>
    /// Position of the agent. In world space.
    /// </summary>
    public Vector3 position { get { return tr.position; } }

    /// <summary>
    /// Rotation of the agent. In world space.
    /// </summary>
    public Quaternion rotation { get { return tr.rotation; } }

    /// <summary>
    /// Actual velocity that the agent is moving with. In world units per second.
    /// </summary>
    public Vector3 velocity
    {
        get
        {
            return lastDeltaTime > 0.000001f ? (prevPosition1 - prevPosition2) / lastDeltaTime : Vector3.zero;
        }
    }

    /// <summary>
    /// Velocity that this agent wants to move with.
    /// Includes gravity and local avoidance if applicable. In world units per second.
    /// </summary>
    public Vector3 desiredVelocity { get; private set; }



    /// <summary>
    /// Delta time used for movement during the last frame
    /// </summary>
    protected float lastDeltaTime;

    /// <summary>
    /// Last frame index when "prevPosition1" was updated
    /// </summary>
    protected int prevFrame;

    /// <summary>
    /// Position of the character at the end of the last frame
    /// </summary>
    protected Vector3 prevPosition1;

    /// <summary>
    /// Position of the character at the end of the frame before the last frame
    /// </summary>
    protected Vector3 prevPosition2;

    /// <summary>
    /// Amount which the character wants or tried to move with during the last frame
    /// </summary>
    protected Vector2 lastDeltaPosition;

    /// <summary>
    /// Accumulated movement deltas from the "Move" method
    /// </summary>
    protected Vector3 accumulatedMovementDelta = Vector3.zero;

    /// <summary>
    /// Cached Transform component
    /// </summary>
    protected Transform tr;

    /// <summary>
    /// Cached Rigidbody component
    /// </summary>
    protected Rigidbody2D rigid2D;

    #endregion



    /// <summary>
    /// True if the Start method has been executed.
    /// Used to test if coroutines should be started in OnEnable to prevent calculating paths
    /// in the awake stage (or rather before start on frame 0).
    /// </summary>
    bool startHasRun = false;

    protected AIBase()
    {
        // Note that this needs to be set here in the constructor and not in e.g Awake
        // because it is possible that other code runs and sets the destination property
        // before the Awake method on this script runs.
        destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }


    #region Unity

    /// <summary>
    /// Called when the component is enabled
    /// </summary>
    protected virtual void OnEnable()
    {
        FindComponents();
        // Make sure we receive callbacks when paths are calculated
        seeker.pathCallback += OnPathComplete;
        Init();
    }
    /// <summary>
    /// Called when the component is disabled
    /// </summary>
    protected virtual void OnDisable()
    {
        CancelCurrentPathRequest();

        // Make sure we no longer receive callbacks when paths complete
        seeker.pathCallback -= OnPathComplete;
        
        accumulatedMovementDelta = Vector3.zero;
        lastDeltaTime = 0;
    }

    /// <summary>
    /// Starts searching for paths.
    /// If you override this method you should in most cases call base.Start () at the start of it.
    /// </summary>
    protected virtual void Start()
    {
        startHasRun = true;
        Init();
    }

    /// <summary>
    /// Called every frame.
    /// If no rigidbodies are used then all movement happens here.
    /// </summary>
    protected virtual void Update()
    {
        if (shouldRecalculatePath) SearchPath();
                
        if (rigid2D == null && canMove)
        {
            Vector3 nextPosition;
            Quaternion nextRotation;
            MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
            FinalizeMovement(nextPosition, nextRotation);
        }
    }

    /// <summary>
    /// Called every physics update.
    /// If rigidbodies are used then all movement happens here.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (!(rigid2D == null) && canMove)
        {
            Vector3 nextPosition;
            Quaternion nextRotation;
            MovementUpdate(Time.fixedDeltaTime, out nextPosition, out nextRotation);
            FinalizeMovement(nextPosition, nextRotation);
        }
    }

    #endregion


    #region Method
    protected virtual void FindComponents()
    {
        tr = transform;
        rigid2D = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
    }

    void Init()
    {
        if (startHasRun)
        {
            // Clamp the agent to the navmesh (which is what the Teleport call will do essentially. Though only some movement scripts require this, like RichAI).
            // The Teleport call will also make sure some variables are properly initialized (like #prevPosition1 and #prevPosition2)
            Teleport(position, false);
            lastRepath = float.NegativeInfinity;
            if (shouldRecalculatePath) SearchPath();
        }
    }

    protected void CancelCurrentPathRequest()
    {
        waitingForPathCalculation = false;
        // Abort calculation of the current path
        if (seeker != null) seeker.CancelCurrentPathRequest();
    }

    /// <summary>
    /// Instantly move the agent to a new position.
    /// This will trigger a path recalculation (if clearPath is true, which is the default) so if you want to teleport the agent and change its "destination"
    /// it is recommended that you set the "destination" before calling this method.
    ///
    /// The current path will be cleared by default.
    /// </summary>
    public virtual void Teleport(Vector3 newPosition, bool clearPath = true)
    {
        if (clearPath) CancelCurrentPathRequest();
        tr.position = prevPosition1 = prevPosition2 = newPosition;
        if (clearPath) SearchPath();
    }

    /// <summary>
    /// Calculate how the character wants to move during this frame.
    ///
    /// Note that this does not actually move the character. You need to call "FinalizeMovement" for that.
    /// This is called automatically unless "canMove" is false.
    ///
    /// To handle movement yourself you can disable "canMove" and call this method manually.
    /// This code will replicate the normal behavior of the component:
    ///     <code>
    ///         void Update ()
    ///         {
    ///             // Disable the AIs own movement code
    ///             ai.canMove = false;
    ///             Vector3 nextPosition;
    ///             Quaternion nextRotation;
    ///             // Calculate how the AI wants to move
    ///             ai.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
    ///             // Modify nextPosition and nextRotation in any way you wish
    ///             // Actually move the AI
    ///             ai.FinalizeMovement(nextPosition, nextRotation);
    ///         }
    ///     </code>
    /// </summary>
    /// <param name="deltaTime">time to simulate movement for. Usually set to Time.deltaTime.</param>
    /// <param name="nextPosition">the position that the agent wants to move to during this frame.</param>
    /// <param name="nextRotation">the rotation that the agent wants to rotate to during this frame.</param>
    public void MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
    {
        lastDeltaTime = deltaTime;
        MovementUpdateInternal(deltaTime, out nextPosition, out nextRotation);
    }

    /// <summary>
    /// Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not.
    /// </summary>
    /// <param name="deltaTime">time to simulate movement for. Usually set to Time.deltaTime.</param>
    /// <param name="nextPosition">the position that the agent wants to move to during this frame.</param>
    /// <param name="nextRotation">the rotation that the agent wants to rotate to during this frame.</param>
    protected abstract void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation);

    /// <summary>
    /// Recalculate the current path.
    /// You can for example use this if you want very quick reaction times when you have changed the "destination"
    /// so that the agent does not have to wait until the next automatic path recalculation (see "canSearch").
    ///
    /// If there is an ongoing path calculation, it will be canceled, so make sure you leave time for the paths to get calculated before calling this function again.
    ///
    /// If no "destination" has been set yet then nothing will be done.
    ///
    /// Note: The path result may not become available until after a few frames.
    /// During the calculation time the "pathPending" property will return true.
    /// </summary>
    public virtual void SearchPath()
    {
        if (float.IsPositiveInfinity(destination.x)) return;
        if (onSearchPath != null) onSearchPath();

        lastRepath = Time.time;
        waitingForPathCalculation = true;

        seeker.CancelCurrentPathRequest();        

        // This is where we should search to
        // Request a path to be calculated from our current position to the destination
        seeker.StartPath(position, destination);
    }

    /// <summary>
    /// Called when a requested path has been calculated
    /// </summary>
    protected abstract void OnPathComplete(Path newPath);

    /// <summary>
    /// Make the AI follow the specified path.
    /// In case the path has not been calculated, the script will call seeker.StartPath to calculate it.
    /// This means the AI may not actually start to follow the path until in a few frames when the path has been calculated.
    /// The "pathPending" field will as usual return true while the path is being calculated.
    ///
    /// In case the path has already been calculated it will immediately replace the current path the AI is following.
    /// This is useful if you want to replace how the AI calculates its paths.
    /// Note that if you calculate the path using seeker.StartPath then this script will already pick it up because it is listening for
    /// all paths that the Seeker finishes calculating. In that case you do not need to call this function.
    ///
    /// You can disable the automatic path recalculation by setting the "canSearch" field to false.
    ///
    /// <code>
    /// // Disable the automatic path recalculation
    /// ai.canSearch = false;
    /// var pointToAvoid = enemy.position;
    /// // Make the AI flee from the enemy.
    /// // The path will be about 20 world units long (the default cost of moving 1 world unit is 1000).
    /// var path = FleePath.Construct(ai.position, pointToAvoid, 1000 * 20);
    /// ai.SetPath(path);
    /// </code>
    /// </summary>
    public void SetPath(Path path)
    {
        if (path.PipelineState == PathState.Created)
        {
            // Path has not started calculation yet
            lastRepath = Time.time;
            waitingForPathCalculation = true;
            seeker.CancelCurrentPathRequest();
            seeker.StartPath(path);
        }
        else if (path.PipelineState == PathState.Returned)
        {
            // Path has already been calculated

            // We might be calculating another path at the same time, and we don't want that path to override this one. So cancel it.
            if (seeker.GetCurrentPath() != path) seeker.CancelCurrentPathRequest();
            else throw new System.ArgumentException("If you calculate the path using seeker.StartPath then this script will pick up the calculated path anyway as it listens for all paths the Seeker finishes calculating. You should not call SetPath in that case.");

            OnPathComplete(path);
        }
        else
        {
            // Path calculation has been started, but it is not yet complete. Cannot really handle this.
            throw new System.ArgumentException("You must call the SetPath method with a path that either has been completely calculated or one whose path calculation has not been started at all. It looks like the path calculation for the path you tried to use has been started, but is not yet finished.");
        }
    }

    /// <summary>
    /// Simulates rotating the agent towards the specified direction and returns the new rotation.
    ///
    /// Note that this only calculates a new rotation, it does not change the actual rotation of the agent.
    /// </summary>
    /// <param name="direction">Direction in the movement plane to rotate towards.</param>
    /// <param name="maxDegrees">Maximum number of degrees to rotate this frame.</param>
    protected Quaternion SimulateRotationTowards(Vector2 direction, float maxDegrees)
    {
        if (direction != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // This causes the character to only rotate around the Z axis
            targetRotation *= Quaternion.Euler(90, 0, 0);

            return Quaternion.RotateTowards(rotation, targetRotation, maxDegrees);
        }
        return rotation;
    }

    /// <summary>
    /// Move the agent.
    ///
    /// This is intended for external movement forces such as those applied by wind, conveyor belts, knockbacks etc.
    /// 
    /// The agent will not be moved immediately when calling this method. Instead this offset will be stored and then
    /// applied the next time the agent runs its movement calculations (which is usually later this frame or the next frame).
    /// If you want to move the agent immediately then call:
    ///     <code>
    ///         ai.Move(someVector);
    ///         ai.FinalizeMovement(ai.position, ai.rotation);
    ///     </code>
    /// </summary>
    /// <param name="deltaPosition">Direction and distance to move the agent in world space.</param>
    public virtual void Move(Vector3 deltaPosition)
    {
        accumulatedMovementDelta += deltaPosition;
    }

    /// <summary>
    /// Moves the agent to a position.
    ///
    /// This is used if you want to override how the agent moves. For example if you are using
    /// root motion with Mecanim.
    ///
    /// This will use a Rigidbody2D or the Transform component depending on what options are available.
    /// </summary>
    /// <param name="nextPosition">New position of the agent.</param>
    /// <param name="nextRotation">New rotation of the agent. If #enableRotation is false then this parameter will be ignored.</param>
    public virtual void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
    {
        if (enableRotation) FinalizeRotation(nextRotation);
        FinalizePosition(nextPosition);
    }

    void FinalizeRotation(Quaternion nextRotation)
    {
        if (rigid2D != null) rigid2D.MoveRotation(nextRotation.eulerAngles.z);
        else tr.rotation = nextRotation;
    }

    void FinalizePosition(Vector3 nextPosition)
    {
        // Use a local variable, it is significantly faster
        Vector3 currentPosition = position;
        currentPosition = nextPosition + accumulatedMovementDelta;
        
        // Note that rigid.MovePosition may or may not move the character immediately.
        // Check the Unity documentation for the special cases.
        if (rigid2D != null) rigid2D.MovePosition(currentPosition);
        else tr.position = currentPosition;

        accumulatedMovementDelta = Vector3.zero;
        UpdateVelocity();
    }

    protected void UpdateVelocity()
    {
        var currentFrame = Time.frameCount;

        if (currentFrame != prevFrame) prevPosition2 = prevPosition1;
        prevPosition1 = position;
        prevFrame = currentFrame;
    }

    #endregion

    

    public static readonly Color ShapeGizmoColor = new Color(240 / 255f, 213 / 255f, 30 / 255f);

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying || !enabled)
            FindComponents();

        Gizmos.color = ShapeGizmoColor;
        Gizmos.DrawSphere(position, radius * tr.localScale.x);

        if (!float.IsPositiveInfinity(destination.x) && Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(destination, 0.2f);
        }
    }
    */
}
