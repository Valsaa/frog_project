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
    public class PlayerCharacter : BasicCharacter
    {
        // Instance
        protected static PlayerCharacter instance;
        public static PlayerCharacter Instance
        {
            get { return instance; }
        }

        // public types
        public enum SpriteSize { _32x32PX, _64x64PX , _96x64PX, _128x128PX };

        // public parameters
        public InputMouvementBy inputMouvementBy = InputMouvementBy.Keyboard;
        [Range(1f, 200f)] public float maxSpeed = 5f;
        [Range(1f, 1000f)] public float groundAcceleration = 100f;
        [Range(1f, 1000f)] public float groundDeceleration = 100f;
        public float deltat = 2;
        public SpriteSize playerSpriteSize;


        // private parameters
        [SerializeField]
        private PlayerInput playerInput = new PlayerInput();

        // gestion des sorts
        // private Sort selectedSort;
        [Tooltip("nom du fichier sans le .json")]
        public List<string> SortFileNameList;
        public List<Sort> SortList;

        private CharacterController2D characterController2D;
        //private Collider2D characterCollider;
        //private Animator animator;

        private Vector2 moveVector;
        private Vector2 targetPosition;
        private Vector2 directionVector;
        private bool isInMovement;
        private bool isInDeceleration;
        private float currentSpeed;

        public AudioSource playerSound;
        public AudioClip walkingSound;

        // Delegate
        delegate void Grounded2DMovementHandler(float speedScale = 1f);
        Grounded2DMovementHandler Grounded2DMovement;

        // Unity 3D function
        void Awake()
        {
            if (instance == null)   instance = this;
            else                    throw new UnityException("There cannot be more than one PlayerCharacter script.  The instances are " + instance.name + " and " + name + ".");

            characterController2D = GetComponent<CharacterController2D>();
            // animator = GetComponent<Animator>();

            SetupSprite();
        }
        void Start()
        {
            this.UpdateStats();

            SortList = Sort.GetSortListFromFileList(SortFileNameList);

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
            playerSound = this.gameObject.AddComponent<AudioSource>() as AudioSource;
            walkingSound = Resources.Load<AudioClip>("step");
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
            WalkingSound();
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

            ManageSorts();
        }

        private void KeyboardGrounded2DMovement(float speedScale = 1f)
        {
            float desiredSpeedX = playerInput.Horizontal.GetValue() * this.finalStats.speed/*maxSpeed*/ * speedScale;
            float desiredSpeedY = playerInput.Vertical.GetValue() * this.finalStats.speed/*maxSpeed*/ * speedScale;
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

        void SetupSprite ()
        {
            // load the correct player sprite
            Sprite selectedSprite = null;
            switch (playerSpriteSize)
            {
                case SpriteSize._32x32PX:
                    selectedSprite = Resources.Load<Sprite>("echelle-perso_32px_x1");
                    break;
                default:
                case SpriteSize._64x64PX:
                    selectedSprite = Resources.Load<Sprite>("echelle-perso_64px_x1");
                    break;
                case SpriteSize._96x64PX:
                    selectedSprite = Resources.Load<Sprite>("echelle-perso_96px_x1");
                    break;
                case SpriteSize._128x128PX:
                    selectedSprite = Resources.Load<Sprite>("echelle-perso_128px_x1");
                    break;
            }
            Debug.Log("loaded sprite : " + selectedSprite.name);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;

            // this.characterCollider = this.gameObject.AddComponent<BoxCollider2D>();
        }

        void ManageSorts()
        {
            // manage Sort selection
            if (playerInput.A.GetDown())
                //selectedSort = sortList.ToArray()[0];

            if (playerInput.MouseLeft.GetDown())
            {

            }
        }

        void WalkingSound()
        {/*
            if (characterController2D.Velocity != Vector2.zero)
            {
                if (!playerSound.isPlaying)
                    playerSound.PlayOneShot(walkingSound);
            }
            else playerSound.Stop();*/
        }

    }
}