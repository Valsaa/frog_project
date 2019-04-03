using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B2B.GameKit.InputController
{
    public abstract class InputButton
    {
        [SerializeField]
        protected bool enabled = true;

        protected StateButton state;            // State of the button
        protected float startTime;              // Time hold started

        protected bool startedHold = false;     // Did it just start now?
        protected bool endHold = false;         // Did it just end now?    

        protected bool isTapped = false;        // Has this input been tapped? Quick enough (with no drift) to be a tap?
        protected bool isDoubleTapped = false;  // Has this input been double tapped? Quick enough (with no drift) to be a double tap? 
        protected bool canDoubleTap = false;    // Check if you can double tap
        protected float startDoubleTapTime;     // Time doubleTap started

        // Constructor
        public InputButton()
        {
        }

        /// <summary>
        /// Interpret this Input
        /// </summary>
        protected virtual void InterpretInput(bool inputValue, float flickAccumulationFactor, float dragThresholdMouse, float flickThreshold, float holdTime, float tapTime, float doubleTapTime)
        {
            if (!enabled)
                return;

            // Reset each frame, need to be triggered only once
            startedHold = false;
            endHold = false;
            isTapped = false;
            isDoubleTapped = false;

            if (inputValue)
            {
                switch(state)
                {
                    case StateButton.UP:
                    case StateButton.NONE:
                        {
                            state = StateButton.DOWN;
                            startTime = Time.realtimeSinceStartup;
                        }
                        break;

                    case StateButton.DOWN:
                    case StateButton.HELD:
                        {
                            state = StateButton.HELD;

                            // Stationary? (Hold)
                            if (Time.realtimeSinceStartup - startTime >= holdTime)
                            {
                                startedHold = true;
                                state = StateButton.HOLD;
                            }
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

                    case StateButton.UP:
                        {
                            // Reset the state machine
                            state = StateButton.NONE;
                            startTime = 0;
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
        /// Check if the value has been updated
        /// </summary>
        public bool ReceivingInput()
        {
            return state != StateButton.NONE;
        }

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