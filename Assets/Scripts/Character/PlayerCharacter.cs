using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using B2B.GameKit.Maths;

namespace XDaddy.Character
{
    public enum InputMouvementBy
    {
        Keyboard,
        Mouse,
        MouseSmoothStep
    }

    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        // Instance
        protected static PlayerCharacter instance;
        public static PlayerCharacter Instance
        {
            get { return instance; }
        }

        // public parameters
        public InputMouvementBy inputMouvementBy = InputMouvementBy.Keyboard;
        [Range(1f, 200f)] public float maxSpeed = 10f;
        [Range(1f, 1000f)] public float groundAcceleration = 50f;
        [Range(1f, 1000f)] public float groundDeceleration = 50f;
        [Range(0.01f, 10f)] public float deltat = 0.02f;

        // private parameters
        [SerializeField]
        private PlayerInput playerInput = new PlayerInput();
        private CharacterController2D characterController2D;
        private Animator animator;

        private Vector2 moveVector;
        private Vector2 targetPosition;
        private Vector2 directionVector;
        private bool isInMovement;
        private bool isInDeceleration;
        private float currentSpeed;

        // Delegate
        delegate void Grounded2DMovementHandler(float speedScale = 1f);
        Grounded2DMovementHandler Grounded2DMovement;

        // Unity 3D function
        void Awake()
        {
            if (instance == null)   instance = this;
            else                    throw new UnityException("There cannot be more than one PlayerCharacter script.  The instances are " + instance.name + " and " + name + ".");

            characterController2D = GetComponent<CharacterController2D>();
            animator = GetComponent<Animator>();

        }
        void Start()
        {
            if (inputMouvementBy == InputMouvementBy.Keyboard)
            {
                Grounded2DMovement = KeyboardGrounded2DMovement;
            }
            else if (inputMouvementBy == InputMouvementBy.Mouse)
            {
                Grounded2DMovement = MouseGrounded2DMovement;
            }
            else // InputMouvementBy.MouseSmoothStep
            {
                Grounded2DMovement = MouseGrounded2DMovement_SmoothStep;
            }
        }
        void OnEnable()
        {
        }
        void OnDisable()
        {
            instance = null;
        }

        void Update()
        {
            ReadInput();
        }
        void FixedUpdate()
        {
            characterController2D.Move(moveVector /* Time.deltaTime*/);
        }

        /***************************************************************
         *                      Private function
        ***************************************************************/
        private void ReadInput()
        {
            // Get all inputs
            playerInput.GetInputs();

            // Use all inputs
            Grounded2DMovement();
            //animator.SetFloat("IsRunning", moveVector.normalized.magnitude);
        }

        private void KeyboardGrounded2DMovement(float speedScale = 1f)
        {
            float desiredSpeedX = playerInput.Horizontal.GetValue() * maxSpeed * speedScale;
            float desiredSpeedY = playerInput.Vertical.GetValue() * maxSpeed * speedScale;
            float acceleration = playerInput.ReceivingInputMovement() ? groundAcceleration : groundDeceleration;

            if (playerInput.Horizontal.ReceivingInput() && playerInput.Vertical.ReceivingInput())
            {
                desiredSpeedX *= 0.7071f;
                desiredSpeedY *= 0.7071f;
            }

            moveVector.x = Mathf.MoveTowards(moveVector.x, desiredSpeedX, acceleration * Time.deltaTime);
            moveVector.y = Mathf.MoveTowards(moveVector.y, desiredSpeedY, acceleration * Time.deltaTime);
        }
        private void MouseGrounded2DMovement(float speedScale = 1f)
        {
            if (playerInput.MouseRight.GetDown())
            {
                targetPosition = Camera.main.ScreenToWorldPoint(playerInput.Mouse.GetCurrentPosition());
                directionVector = (targetPosition - (Vector2)transform.position).normalized;
                currentSpeed = 0f;
                isInMovement = true;
                isInDeceleration = false;
            }

            // Check if the flag for movement is true
            if (isInMovement)
            {
                // Calcul the distance to decelerate
                //float decelerationDistance = currentSpeed * Time.deltaTime + (0.5f * groundDeceleration * Time.deltaTime * Time.deltaTime);
                float decelerationDistance = (currentSpeed * currentSpeed) / (2 * groundDeceleration);
                // Calcul the remaining distance
                float remainingDistance = Vector3.Distance(transform.position, targetPosition);

                if (!isInDeceleration && decelerationDistance < remainingDistance)
                {
                    // Calcul the current speed with acceleration
                    currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * speedScale, groundAcceleration * Time.deltaTime);
                }
                else
                {
                    // Calcul the current speed with deceleration
                    currentSpeed = Mathf.MoveTowards(currentSpeed, 0, groundDeceleration * Time.deltaTime);
                    isInDeceleration = true;
                }
                
                // Define the new move vector
                moveVector = directionVector * currentSpeed * Time.deltaTime;

                // Check if the current gameObject position and the clicked position are equal
                if (currentSpeed == 0 || remainingDistance <= deltat)
                {
                    // Set the movement indicator flag to false                                      
                    isInMovement = false;
                    currentSpeed = 0;
                    moveVector = new Vector2(0, 0);
                    Debug.Log("I am here");
                }
            }          
            
        }
        private void MouseGrounded2DMovement_SmoothStep(float speedScale = 1f)
        {
            if (playerInput.MouseRight.GetDown())
            {
                targetPosition = Camera.main.ScreenToWorldPoint(playerInput.Mouse.GetCurrentPosition());
                directionVector = (targetPosition - (Vector2)transform.position).normalized;
                isInMovement = true;
            }

            // Check if the flag for movement is true
            if (isInMovement)
            {
                // Calcul the current speed
                float speed = Maths.BlendingSmoothStep(0.1f, 0.2f, 0.9f, 1.0f, Time.deltaTime) * maxSpeed;             

                // Define the new move vector
                moveVector = directionVector * speed;
                
                // Check if the current gameObject position and the clicked position are equal
                if (Vector3.Distance(transform.position, targetPosition) <= deltat)
                {
                    // Set the movement indicator flag to false                                      
                    isInMovement = false;
                    moveVector = new Vector2(0, 0);
                    Debug.Log("I am here");
                }
            }

        }

        // Just for visualizing the decelerate radius around the target
        private void OnDrawGizmos()
        {
            if (isInMovement)
            {
                if (inputMouvementBy == InputMouvementBy.Keyboard)
                {

                }
                else if (inputMouvementBy == InputMouvementBy.Mouse)
                {
                    float decelerationDistance = (currentSpeed * currentSpeed) / (2 * groundDeceleration);
                    Gizmos.DrawWireSphere(targetPosition, decelerationDistance);
                }
                else // InputMouvementBy.MouseSmoothStep
                {
                }
                
            }
        }

    }
}