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
        /// How far mouse must move before starting a drag
        /// </summary>
        [SerializeField]
        protected float dragThresholdMouse = 1.0f;
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
            public void ReadInput(float holdTime, float tapTime, float doubleTapTime)
            {
                InterpretInput(Input.GetKey(key), holdTime, tapTime, doubleTapTime);
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
            public void ReadInput(float holdTime, float tapTime, float doubleTapTime)
            {
                //InterpretInput(InputReplay.Instance.GetKey(XboxButtonTypeToName[(int)controllerButton]), holdTime, tapTime, doubleTapTime);
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
        public class MouseButton : InputMouseButton
        {            
            public MouseButtonType keyMouse;
            
            // Constructor
            public MouseButton(MouseButtonType keyMouse)
            {
                this.keyMouse = keyMouse;
            }

            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(Vector2 deltaPosition, float dragThresholdMouse, float holdTime, float tapTime, float doubleTapTime)
            {
                InterpretInput(Input.GetMouseButton((int)keyMouse), deltaPosition, dragThresholdMouse, holdTime, tapTime, doubleTapTime);
            }      
        }

        [Serializable]
        public class InputMouse
        {
            [SerializeField]
            protected bool enabled = true;

            private MouseButton buttonLeft = new MouseButton(MouseButtonType.MouseLeft);
            public MouseButton ButtonLeft { get { return buttonLeft; } }
            private MouseButton buttonRight = new MouseButton(MouseButtonType.MouseRight);
            public MouseButton ButtonRight { get { return buttonRight; } }
            private MouseButton buttonMiddle = new MouseButton(MouseButtonType.MouseMiddle);
            public MouseButton ButtonMiddle { get { return buttonMiddle; } }

            private Vector2 currentPosition;        // Current pointer position
            private Vector2 previousPosition;       // Previous frame's pointer position
            private Vector2 deltaPosition;          // Movement delta for this frame
            private bool mouseMovedOnThisFrame;     // Tracks if the mouse moved on this frame


            // Constructor
            public InputMouse()
            {
            }

            /// <summary>
            /// Update this Input
            /// </summary>
            public void ReadInput(float dragThresholdMouse, float holdTime, float tapTime, float doubleTapTime)
            {
                if (!enabled)
                    return;

                previousPosition = currentPosition;
                currentPosition = Input.mousePosition;
                deltaPosition = currentPosition - previousPosition;
                mouseMovedOnThisFrame = deltaPosition.sqrMagnitude >= Mathf.Epsilon;

                buttonLeft.ReadInput(deltaPosition, dragThresholdMouse, holdTime, tapTime, doubleTapTime);
                buttonRight.ReadInput(deltaPosition, dragThresholdMouse, holdTime, tapTime, doubleTapTime);
                buttonMiddle.ReadInput(deltaPosition, dragThresholdMouse, holdTime, tapTime, doubleTapTime);
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
                return buttonLeft.ReceivingInput() || buttonMiddle.ReceivingInput() || buttonRight.ReceivingInput();
            }

            /// <summary>
            /// Return the scene mouse position, from main camera only !
            /// </summary>
            public Vector2 ScreenToWorldPoint2D()
            {
                Vector3 nextPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return new Vector2(nextPosition.x, nextPosition.y);
            }
            /// <summary>
            /// Return the scene mouse position, from main camera only !
            /// </summary>
            public Vector3 ScreenToWorldPoint3D()
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

            input.ReadInput(holdTime, tapTime, doubleTapTime);
            return true;
        }
        protected bool ReadInput(ControllerButton input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(holdTime, tapTime, doubleTapTime);
            return true;
        }
        protected bool ReadInput(Axis input)
        {
            if (!haveControl)
                return false;

            input.ReadInput();
            return true;
        }
        protected bool ReadInput(InputMouse input)
        {
            if (!haveControl)
                return false;

            input.ReadInput(dragThresholdMouse, holdTime, tapTime, doubleTapTime);
            return true;
        }

    }
}