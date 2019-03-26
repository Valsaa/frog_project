using System;
using System.Collections.Generic;
using UnityEngine;

namespace B2B.GameKit.InputController
{
    public abstract class InputAxis
    {
        [SerializeField] protected bool enabled = true;
        [SerializeField] protected float inputSensitivity = Single.Epsilon;
        [SerializeField] protected bool analog = true;
        protected bool receivingInput = false;
        protected float value = 0.0f;

        // Constructor
        public InputAxis()
        {
        }

        /// <summary>
        /// Interpret this Input
        /// </summary>
        protected void InterpretInput(float inputValue)
        {
            receivingInput = false;

            if (!enabled)
            {
                value = 0.0f;
                return;
            }

            if (analog)
            {
                if (Math.Abs(inputValue) > inputSensitivity)    { receivingInput = true; value = inputValue; }
                else                                            { receivingInput = false; value = 0f; }
                return;
            }

            bool positiveHeld = inputValue > inputSensitivity;
            bool negativeHeld = inputValue < -inputSensitivity;

            if (positiveHeld == negativeHeld)   { receivingInput = false; value = 0f; }
            else if (positiveHeld)              { receivingInput = true; value = 1f; }
            else                                { receivingInput = true; value = -1f; }
        }

        /// <summary>
        /// Get the last value
        /// </summary>
        public float GetValue()
        {
            if (!enabled)
                return 0.0f;

            return value;
        }
        /// <summary>
        /// Check if the value has been updated
        /// </summary>
        public bool ReceivingInput()
        {
            return receivingInput;
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
