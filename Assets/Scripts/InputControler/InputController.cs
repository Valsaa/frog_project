using System;
using System.Collections.Generic;
using UnityEngine;


namespace B2B.GameKit.InputController
{
    public abstract class InputController
    {
        // Xbox button
        public enum XboxControllerButtonType
        {
            A,
            B,
            X,
            Y,
            Leftstick,
            Rightstick,
            Back,
            Start,
            LeftBumper,
            RightBumper,
        }
        // Xbox button to name
        protected static readonly Dictionary<int, string> XboxButtonTypeToName = new Dictionary<int, string>
        {
            {(int)XboxControllerButtonType.A, "joystick button 0"},
            {(int)XboxControllerButtonType.B, "joystick button 1"},
            {(int)XboxControllerButtonType.X, "joystick button 2"},
            {(int)XboxControllerButtonType.Y, "joystick button 3"},           
            {(int)XboxControllerButtonType.LeftBumper, "joystick button 4"},
            {(int)XboxControllerButtonType.RightBumper, "joystick button 5"},
            {(int)XboxControllerButtonType.Back, "joystick button 6"},
            {(int)XboxControllerButtonType.Start, "joystick button 7"},
            {(int)XboxControllerButtonType.Leftstick, "joystick button 8"},
            {(int)XboxControllerButtonType.Rightstick, "joystick button 9"},
        };

        /// <summary>
        /// Axis type defined in the Unity3d InputManager
        /// !!! Need to be change if the InputManager Axis change !!!
        /// </summary>
        public enum AxisType
        {
            Horizontal,
            Vertical,
            Fire1,
            Fire2,
            Fire3,
            Jump,
            MouseX,
            MouseY,
            MouseScrollWheel,
            Submit,
            Cancel,
        }
        protected static readonly Dictionary<int, string> AxisTypeToName = new Dictionary<int, string>
        {
            {(int)AxisType.Horizontal, "Horizontal"},
            {(int)AxisType.Vertical, "Vertical"},
            {(int)AxisType.Fire1, "Fire1"},
            {(int)AxisType.Fire2, "Fire2"},
            {(int)AxisType.Fire3, "Fire3"},
            {(int)AxisType.Jump, "Jump"},
            {(int)AxisType.MouseX, "Mouse X"},
            {(int)AxisType.MouseY, "Mouse Y"},
            {(int)AxisType.MouseScrollWheel, "Mouse ScrollWheel"},
            {(int)AxisType.Submit, "Submit"},
            {(int)AxisType.Cancel, "Cancel"},
        };

        /// <summary>
        /// How quickly flick velocity is accumulated with movements
        /// </summary>
        [SerializeField]
        protected float flickAccumulationFactor = 0.8f;
        /// <summary>
        /// How far mouse must move before starting a drag
        /// </summary>
        [SerializeField]
        protected float dragThresholdMouse = 1.0f;
        /// <summary>
        /// Flick movement threshold
        /// </summary>
        [SerializeField]
        protected float flickThreshold = 2f;
        /// <summary>
        /// How long before a touch is considered a hold
        /// </summary>
        [SerializeField]
        protected float holdTime = 0.8f;
        /// <summary>
        /// How long before a touch can no longer be considered a tap
        /// </summary>
        [SerializeField]
        protected float tapTime = 0.2f;
        /// <summary>
        /// How long before a touch can no longer be considered a double tap. Time between 2 tap.
        /// </summary>
        [SerializeField]
        protected float doubleTapTime = 0.2f;

        
        [Serializable]
        public class KeyboardButton : InputButton
        {
            [SerializeField]
            private KeyCode key;
            
            // Constructor
            public KeyboardButton(KeyCode key)
            {
                this.key = key;
            }


            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(float flickAccumulationFactor, float dragThresholdMouse, float flickThreshold, float holdTime, float tapTime, float doubleTapTime)
            {
                InterpretInput(InputReplay.Instance.GetKey(key), flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            }

            /// <summary>
            /// Return this KeyCode 
            /// </summary>
            public KeyCode GetKey()
            {
                return key;
            }
        }

        [Serializable]
        public class ControllerButton : InputButton
        {
            [SerializeField]
            private XboxControllerButtonType controllerButton;

            // Constructor
            public ControllerButton(XboxControllerButtonType controllerKey)
            {
                this.controllerButton = controllerKey;
            }


            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(float flickAccumulationFactor, float dragThresholdMouse, float flickThreshold, float holdTime, float tapTime, float doubleTapTime)
            {
                //InterpretInput(InputReplay.Instance.GetKey(XboxButtonTypeToName[(int)controllerButton]), flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            }

            /// <summary>
            /// Return this KeyCode 
            /// </summary>
            public XboxControllerButtonType GetKey()
            {
                return controllerButton;
            }
        }

        [Serializable]
        public class Axis : InputAxis
        {
            [SerializeField] private AxisType axisType;              

            // Constructor
            public Axis(AxisType axisType)
            {
                this.axisType = axisType;
            }

            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput()
            {
                // The Horizontal and Vertical ranges change from 0 to +1 or -1 with increase/decrease in 0.05f steps.
                // GetAxisRaw has changes from 0 to 1 or -1 immediately, so with no steps.
                InterpretInput(InputReplay.Instance.GetAxis(AxisTypeToName[(int)axisType]));
            }
        }

        [Serializable]
        public class MouseButton : InputButton
        {            
            public MouseButtonType keyMouse;

            private Vector2 startPosition;          // Position where the input started
            private Vector2 currentPosition;        // Current pointer position
            private Vector2 previousPosition;       // Previous frame's pointer position
            private Vector2 deltaPosition;          // Movement delta for this frame
            private Vector2 flickVelocity;          // Flick velocity is a moving average of deltas
            private float totalMovement;            // Total movement for this pointer, since being held down
            
            private bool mouseMovedOnThisFrame;     // Tracks if the mouse moved on this frame
            //private bool startedOverUI;             // Tracks if this pointer began over UI

            private bool startedDrag = false;       // Did it just start now?
            private bool endDrag = false;           // Did it just end now?
            
            // Constructor
            public MouseButton(MouseButtonType keyMouse)
            {
                this.keyMouse = keyMouse;
            }

            /// <summary>
            /// Determine the flick velocity
            /// </summary>
            private void Flick(float flickAccumulationFactor, float flickThreshold)
            {
                float moveDist = deltaPosition.magnitude;
                totalMovement += moveDist;

                // Flick?
                if (moveDist > flickThreshold)
                {
                    flickVelocity = (flickVelocity * (1 - flickAccumulationFactor)) + (deltaPosition * flickAccumulationFactor);
                }
                else
                {
                    flickVelocity = Vector2.zero;
                }
            }

            /// <summary>
            /// Interpret this Input
            /// </summary>
            protected override void InterpretInput(bool inputValue, float flickAccumulationFactor, float dragThresholdMouse, float flickThreshold, float holdTime, float tapTime, float doubleTapTime)
            {
                if (!enabled)
                    return;

                // Get mouse data
                previousPosition = currentPosition;
                currentPosition = InputReplay.Instance.mousePosition;
                deltaPosition = currentPosition - previousPosition;
                mouseMovedOnThisFrame = deltaPosition.sqrMagnitude >= Mathf.Epsilon;

                // Reset each frame, need to be triggered only once
                startedHold = false;
                endHold = false;
                startedDrag = false;
                endDrag = false;
                isTapped = false;
                isDoubleTapped = false;

                if (inputValue)
                {
                    switch (state)
                    {
                        case StateButton.UP:
                        case StateButton.NONE:
                            {
                                state = StateButton.DOWN;

                                // First press
                                startTime = Time.realtimeSinceStartup;
                                startPosition = InputReplay.Instance.mousePosition;
                                //startedOverUI = EventSystem.current.IsPointerOverGameObject( (int)keyMouse - 1 );

                                // Reset some stuff
                                totalMovement = 0;
                                flickVelocity = Vector2.zero;
                            }
                            break;

                        case StateButton.DOWN:
                        case StateButton.HELD:
                            {
                                state = StateButton.HELD;

                                // Flick?
                                Flick(flickAccumulationFactor, flickThreshold);

                                // Dragging?
                                if (totalMovement > dragThresholdMouse)
                                {
                                    state = StateButton.DRAG;
                                    startedDrag = true;
                                }

                                // Stationary? (Hold)
                                if (Time.realtimeSinceStartup - startTime >= holdTime)
                                {
                                    startedHold = true;
                                    state = StateButton.HOLD;
                                }
                            }
                            break;

                        case StateButton.HOLD:
                            {
                                // Flick?
                                Flick(flickAccumulationFactor, flickThreshold);

                                // Dragging?
                                if (totalMovement > dragThresholdMouse)
                                {
                                    state = StateButton.DRAG;
                                    startedDrag = true;
                                    endHold = true;
                                }
                            }
                            break;

                        case StateButton.DRAG:
                            {
                                // Flick?
                                Flick(flickAccumulationFactor, flickThreshold);
                            }
                            break;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case StateButton.DOWN:
                        case StateButton.HELD:
                            {
                                state = StateButton.UP;

                                // Quick enough (with no drift) to be a tap?
                                if (Time.realtimeSinceStartup - startTime < tapTime)
                                {
                                    // Is a Double tap ?
                                    if (canDoubleTap && Time.realtimeSinceStartup - startDoubleTapTime <= doubleTapTime + tapTime)
                                    {
                                        isDoubleTapped = true;
                                        canDoubleTap = false;
                                    }
                                    else
                                    {
                                        isTapped = true;
                                        canDoubleTap = true;
                                        startDoubleTapTime = Time.realtimeSinceStartup;
                                    }
                                }
                            }
                            break;

                        case StateButton.HOLD:
                            {
                                state = StateButton.UP;
                                endHold = true;
                            }
                            break;

                        case StateButton.DRAG:
                            {
                                state = StateButton.UP;
                                endDrag = true;
                            }
                            break;

                        case StateButton.UP:
                            {
                                // Reset the state machine
                                state = StateButton.NONE;
                                startTime = 0;
                                totalMovement = 0;
                                flickVelocity = Vector2.zero;
                            }
                            break;
                    }
                }
            }
            

            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(float flickAccumulationFactor, float dragThresholdMouse, float flickThreshold, float holdTime, float tapTime, float doubleTapTime)
            {
                InterpretInput(InputReplay.Instance.GetMouseButton((int)keyMouse), flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            }
            
            /// <summary>
            /// Did it just start now?
            /// </summary>
            public bool StartedDrag()
            {
                if (!enabled)
                    return false;

                return startedDrag;
            }
            /// <summary>
            /// Did it just end now?
            /// </summary>
            public bool EndDrag()
            {
                if (!enabled)
                    return false;

                return endDrag;
            }
            /// <summary>
            /// Is this input dragged ?
            /// </summary>
            public bool IsDragging()
            {
                if (!enabled)
                    return false;

                return state == StateButton.DRAG && !startedDrag && !endDrag;
            }
                        
            /// <summary>
            /// Position where the input started
            /// </summary>
            public Vector2 GetStartPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return startPosition;
            }
            /// <summary>
            /// Current pointer position
            /// </summary>
            public Vector2 GetCurrentPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return currentPosition;
            }
            /// <summary>
            /// Previous frame's pointer position
            /// </summary>
            public Vector2 GetPreviousPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return previousPosition;
            }
            /// <summary>
            /// Movement delta for this frame
            /// </summary>
            public Vector2 GetDeltaPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return deltaPosition;
            }
            /// <summary>
            /// Flick velocity is a moving average of deltas
            /// </summary>
            public Vector2 GetFlickVelocity()
            {
                if (!enabled)
                    return Vector2.zero;

                return flickVelocity;
            }
            /// <summary>
            /// Total movement for this pointer, since being held down
            /// </summary>
            public float GetTotalMovement()
            {
                if (!enabled)
                    return 0f;

                return totalMovement;
            }
            
            /// <summary>
            /// Tracks if the mouse moved on this frame
            /// </summary>
            public bool MouseMovedOnThisFrame()
            {
                if (!enabled)
                    return false;

                return mouseMovedOnThisFrame;
            }
            /// <summary>
            /// Tracks if this pointer began over UI
            /// </summary>
            /*public bool StartedOverUI()
            {
                if (!enabled)
                    return false;

                return startedOverUI;
            }   */        
        }

        [Serializable]
        public class InputMouse
        {
            [SerializeField]
            protected bool enabled = true;

            private Vector2 currentPosition;        // Current pointer position
            private Vector2 previousPosition;       // Previous frame's pointer position
            private Vector2 deltaPosition;          // Movement delta for this frame
            private Vector2 flickVelocity;          // Flick velocity is a moving average of deltas
            private bool mouseMovedOnThisFrame;     // Tracks if the mouse moved on this frame


            // Constructor
            public InputMouse()
            {
            }

            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(float flickAccumulationFactor, float flickThreshold)
            {
                if (!enabled)
                    return;

                previousPosition = currentPosition;
                currentPosition = InputReplay.Instance.mousePosition;
                deltaPosition = currentPosition - previousPosition;
                mouseMovedOnThisFrame = deltaPosition.sqrMagnitude >= Mathf.Epsilon;

                // Flick?
                if (deltaPosition.magnitude > flickThreshold)
                {
                    flickVelocity = (flickVelocity * (1 - flickAccumulationFactor)) + (deltaPosition * flickAccumulationFactor);
                }
                else
                {
                    flickVelocity = Vector2.zero;
                }
            }
                        
            /// <summary>
            /// Current pointer position
            /// </summary>
            public Vector2 GetCurrentPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return currentPosition;
            }
            /// <summary>
            /// Previous frame's pointer position
            /// </summary>
            public Vector2 GetPreviousPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return previousPosition;
            }
            /// <summary>
            /// Movement delta for this frame
            /// </summary>
            public Vector2 GetDeltaPosition()
            {
                if (!enabled)
                    return Vector2.zero;

                return deltaPosition;
            }
            /// <summary>
            /// Flick velocity is a moving average of deltas
            /// </summary>
            public Vector2 GetFlickVelocity()
            {
                if (!enabled)
                    return Vector2.zero;

                return flickVelocity;
            }
            
            /// <summary>
            /// Tracks if the mouse moved on this frame
            /// </summary>
            public bool MouseMovedOnThisFrame()
            {
                if (!enabled)
                    return false;

                return mouseMovedOnThisFrame;
            }
            
            /// <summary>
            /// Check if the value has been updated
            /// </summary>
            public bool ReceivingInput()
            {
                return MouseMovedOnThisFrame();
            }


            public void Enable()
            {
                enabled = true;
            }
            public void Disable()
            {
                enabled = false;
            }
        }

        // Parameters
        protected bool haveControl = true;
        public bool HaveControl
        {
            get { return haveControl; }
            set { haveControl = value; }
        }
        
        // Abstract function
        public abstract void GetInputs();

        // Protected function
        protected bool ReadInput(KeyboardButton input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            return true;
        }
        protected bool ReadInput(ControllerButton input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            return true;
        }
        protected bool ReadInput(Axis input)
        {
            if (!haveControl)
                return false;

            input.ReadInput();
            return true;
        }
        protected bool ReadInput(MouseButton input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(flickAccumulationFactor, dragThresholdMouse, flickThreshold, holdTime, tapTime, doubleTapTime);
            return true;
        }
        protected bool ReadInput(InputMouse input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(flickAccumulationFactor, flickThreshold);
            return true;
        }

    }
}