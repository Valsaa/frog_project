using System;
using UnityEngine;

using B2B.GameKit.InputController;

namespace XDaddy.Character
{
    [Serializable]
    public class PlayerInput : InputController
    {
        // Inputs :
        [SerializeField]
        private KeyboardButton pause = new KeyboardButton(KeyCode.Escape);
        public KeyboardButton Pause { get { return pause; } }

        [SerializeField]
        private InputMouse mouse = new InputMouse();
        public InputMouse Mouse { get { return mouse; } }

        [SerializeField]
        private KeyboardButton action1 = new KeyboardButton(KeyCode.A);
        public KeyboardButton Action1 { get { return action1; } }
        [SerializeField]
        private KeyboardButton action2 = new KeyboardButton(KeyCode.Z);
        public KeyboardButton Action2 { get { return action2; } }
        [SerializeField]
        private KeyboardButton action3 = new KeyboardButton(KeyCode.E);
        public KeyboardButton Action3 { get { return action3; } }
        [SerializeField]
        private KeyboardButton action4 = new KeyboardButton(KeyCode.R);
        public KeyboardButton Action4 { get { return action4; } }


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
            ReadInput(mouse);

            ReadInput(action1);
            ReadInput(action2);
            ReadInput(action3);
            ReadInput(action4);

            ReadInput(debugMenuIsOpen);
        }

        // PUBLIC 
        public bool ReceivingInputMovement()
        {
            return mouse.ReceivingInput();
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