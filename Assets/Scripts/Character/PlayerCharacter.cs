using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XDaddy.Character
{
    public enum InputMouvementBy
    {
        Keyboard,
        Mouse,
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
        private CharacterController2D characterController2D;
        private Animator animator;

        private Vector2 moveVector;
        private Vector2 directionVector;
        private Vector2 targetPosition;

        // Delegate
        delegate void GroundedHorizontalMovementHandler(float speedScale = 1f);
        GroundedHorizontalMovementHandler GroundedHorizontalMovement;

        // Unity 3D function
        void Awake()
        {
            if (instance == null)   instance = this;
            else                    throw new UnityException("There cannot be more than one PlayerCharacter script.  The instances are " + instance.name + " and " + name + ".");

            characterController2D = GetComponent<CharacterController2D>();
            animator = GetComponent<Animator>();

            SetupSprite();
        }
        void Start()
        {
            if (inputMouvementBy == InputMouvementBy.Keyboard)
            {
                GroundedHorizontalMovement = KeyboardGroundedHorizontalMovement;
            }
            else // InputMouvementBy.Mouse
            {
                GroundedHorizontalMovement = MouseGroundedHorizontalMovement;
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
            characterController2D.Move(moveVector * Time.deltaTime);
        }

        // Private function
        private void ReadInput()
        {
            // Get all inputs
            playerInput.GetInputs();

            // Use all inputs
            GroundedHorizontalMovement();
            //animator.SetFloat("IsRunning", moveVector.normalized.magnitude);
        }

        private void KeyboardGroundedHorizontalMovement(float speedScale = 1f)
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
        private void MouseGroundedHorizontalMovement(float speedScale = 1f)
        {
            if (playerInput.MouseRight.GetDown())
            {
                targetPosition = Camera.main.ScreenToWorldPoint(playerInput.Mouse.GetCurrentPosition());

                directionVector = targetPosition - (Vector2)transform.position;
                directionVector = directionVector.normalized * maxSpeed * speedScale;
            }

            float distance = Vector2.Distance((Vector2)transform.position, targetPosition);
            if (distance < deltat * 0.1)
            {
                moveVector = new Vector2(0, 0);
            }
            else if (distance < deltat)
            {
                moveVector = Vector2.MoveTowards(moveVector, directionVector, groundDeceleration * Time.deltaTime);
            }
            else
            {
                moveVector = Vector2.MoveTowards(moveVector, directionVector, groundAcceleration * Time.deltaTime);
            }
        }

        void SetupSprite ()
        {
            // load the correct player sprite
            Sprite selectedSprite = new Sprite();
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
        }


    }
}