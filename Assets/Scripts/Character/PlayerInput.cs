using System;
using UnityEngine;

using B2B.GameKit.InputController;

namespace XDaddy.Character
{
    [Serializable]
    public class PlayerInput : InputController
    {
        // Inputs  
        [SerializeField]
        private KeyboardButton pause = new KeyboardButton(KeyCode.Escape);
        public KeyboardButton Pause { get { return pause; } }

        [SerializeField]
        private KeyboardButton interact = new KeyboardButton(KeyCode.E);
        public KeyboardButton Interact { get { return interact; } }

        [SerializeField]
        private KeyboardButton jump = new KeyboardButton(KeyCode.Space);
        public KeyboardButton Jump { get { return jump; } }

        [SerializeField]
        private Axis horizontal = new Axis(AxisType.Horizontal);
        public Axis Horizontal { get { return horizontal; } }

        [SerializeField]
        private Axis vertical = new Axis(AxisType.Vertical);
        public Axis Vertical { get { return vertical; } }

        [SerializeField]
        private MouseButton mouseLeft = new MouseButton(MouseButtonType.MouseLeft);
        public MouseButton MouseLeft { get { return mouseLeft; } }

        [SerializeField]
        private MouseButton mouseRight = new MouseButton(MouseButtonType.MouseRight);
        public MouseButton MouseRight { get { return mouseRight; } }

        [SerializeField]
        private MouseButton mouseMiddle = new MouseButton(MouseButtonType.MouseMiddle);
        public MouseButton MouseMiddle { get { return mouseMiddle; } }

        [SerializeField]
        private InputMouse mouse = new InputMouse();
        public InputMouse Mouse { get { return mouse; } }

        // abilities
        [SerializeField]
        private KeyboardButton a = new KeyboardButton(KeyCode.A);
        public KeyboardButton A { get { return a; } }
        [SerializeField]
        private KeyboardButton z = new KeyboardButton(KeyCode.Z);
        public KeyboardButton Z { get { return z; } }
        [SerializeField]
        private KeyboardButton e = new KeyboardButton(KeyCode.E);
        public KeyboardButton E { get { return e; } }
        [SerializeField]
        private KeyboardButton r = new KeyboardButton(KeyCode.R);
        public KeyboardButton R { get { return r; } }


        [SerializeField]
        private KeyboardButton debugMenuIsOpen = new KeyboardButton(KeyCode.F12);

        // Contructor
        public PlayerInput()
        {
        }

        // Update all Input
        public override void GetInputs()
        {
            ReadInput(pause);
            ReadInput(interact);
            ReadInput(jump);
            ReadInput(horizontal);
            ReadInput(vertical);
            ReadInput(mouseLeft);
            ReadInput(mouseRight);
            ReadInput(mouseMiddle);
            ReadInput(mouse);

            ReadInput(a);
            ReadInput(z);
            ReadInput(e);
            ReadInput(r);
        }

        // PUBLIC 
        public bool ReceivingInputMovement()
        {
            return horizontal.ReceivingInput() || vertical.ReceivingInput();
        }


        // Debug function
        void OnGUI()
        {
            if (debugMenuIsOpen.GetDown())
            {
                /*const float height = 100;

                GUILayout.BeginArea(new Rect(30, Screen.height - height, 200, height));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Press F12 to close");

                bool meleeAttackEnabled = GUILayout.Toggle(MeleeAttack.Enabled, "Enable Melee Attack");
                bool rangeAttackEnabled = GUILayout.Toggle(RangedAttack.Enabled, "Enable Range Attack");

                if (meleeAttackEnabled != MeleeAttack.Enabled)
                {
                    if (meleeAttackEnabled)
                        MeleeAttack.Enable();
                    else
                        MeleeAttack.Disable();
                }

                if (rangeAttackEnabled != RangedAttack.Enabled)
                {
                    if (rangeAttackEnabled)
                        RangedAttack.Enable();
                    else
                        RangedAttack.Disable();
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();*/
            }
        }
    }
}