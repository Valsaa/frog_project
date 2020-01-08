using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B2B.GameKit.InputController
{
    public abstract class InputMouseButton
    {
        [SerializeField]
        protected bool enabled = true;

        protected StateButton state;            // State of the button
        protected float startTime;              // Time hold started
        private float totalMovement;            // Total movement for this pointer, since being held down
        
        private bool startedDrag = false;       // Did it just start now?
        private bool endDrag = false;           // Did it just end now?

        protected bool startedHold = false;     // Did it just start now?
        protected bool endHold = false;         // Did it just end now?    

        protected bool isTapped = false;        // Has this input been tapped? Quick enough (with no drift) to be a tap?
        protected bool isDoubleTapped = false;  // Has this input been double tapped? Quick enough (with no drift) to be a double tap? 
        protected bool canDoubleTap = false;    // Check if you can double tap
        protected float startDoubleTapTime;     // Time doubleTap started

        //private bool startedOverUI;             // Tracks if this pointer began over UI

        // Constructor
        public InputMouseButton()
        {
        }

        /// <summary>
        /// Interpret this Input
        /// </summary>
        protected virtual void InterpretInput(bool inputValue, Vector2 deltaPosition, float dragThresholdMouse, float holdTime, float tapTime, float doubleTapTime)
        {
            if (!enabled)
                return;

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
                            startTime = Time.realtimeSinceStartup;
                            totalMovement = 0;
                        }
                        break;

                    case StateButton.DOWN:
                    case StateButton.HELD:
                        {
                            state = StateButton.HELD;
                            totalMovement += deltaPosition.magnitude;

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
                            totalMovement += deltaPosition.magnitude;

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
                            totalMovement += deltaPosition.magnitude;
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
                        }
                        break;
                }
            }
        }

        
        /// <summary>
        /// Is this mouse button down 
        /// </summary>
        public bool GetDown()
        {
            if (!enabled)
                return false;

            return state == StateButton.DOWN;
        }
        /// <summary>
        /// Is this mouse button up 
        /// </summary>
        public bool GetUp()
        {
            if (!enabled)
                return false;

            return state == StateButton.UP;
        }
        /// <summary>
        /// Is this mouse button held
        /// </summary>
        public bool GetHeld()
        {
            if (!enabled)
                return false;

            return state == StateButton.HELD;
        }
        /// <summary>
        /// Has this input been tapped? Quick enough (with no drift) to be a tap? 
        /// </summary>
        public bool GetTap()
        {
            if (!enabled)
                return false;

            return isTapped;
        }
        /// <summary>
        /// Has this input been double tapped? 
        /// </summary>
        public bool GetDoubleTap()
        {
            if (!enabled)
                return false;

            return isDoubleTapped;
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
        /// Did it just start now?
        /// </summary>
        public bool StartedHold()
        {
            if (!enabled)
                return false;

            return startedHold;
        }
        /// <summary>
        /// Did it just end now?
        /// </summary>
        public bool EndHold()
        {
            if (!enabled)
                return false;

            return endHold;
        }
        /// <summary>
        /// Is this input dragged ?
        /// </summary>
        public bool IsHolding()
        {
            if (!enabled)
                return false;

            return state == StateButton.HOLD && !startedHold && !endHold;
        }

        /// <summary>
        /// Time hold started
        /// </summary>
        public float GetStartTime()
        {
            if (!enabled)
                return 0f;

            return startTime;
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
        /// Check if the value has been updated
        /// </summary>
        public bool ReceivingInput()
        {
            return state != StateButton.NONE;
        }

        /// <summary>
        /// Tracks if this pointer began over UI
        /// </summary>
        /*public bool StartedOverUI()
        {
            if (!enabled)
                return false;

            return startedOverUI;
        }*/

        /// <summary>
        /// Enable this input
        /// </summary>
        public void Enable()
        {
            enabled = true;
        }
        /// <summary>
        /// Disable this input
        /// </summary>
        public void Disable()
        {
            enabled = false;
        }
    }


}
