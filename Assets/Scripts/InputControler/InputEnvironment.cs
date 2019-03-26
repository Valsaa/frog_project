using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B2B.GameKit.InputController
{
    // Input Type
    public enum InputType
    {
        MouseAndKeyboard,
        Controller,
    }

    // Mouse button
    public enum MouseButtonType
    {
        MouseLeft = 0,
        MouseRight,
        MouseMiddle,
        Mouse3,
        Mouse4,
        Mouse5,
        Mouse6,
    }

    // Mouse button
    public enum StateButton
    {
        NONE = 0,               // Is this mouse button have no state
        DOWN,                   // Is this mouse button down
        UP,                     // Is this mouse button up
        HELD,                   // Is this mouse button held
        HOLD,                   // Is this input holding ?
        DRAG,                   // Is this input dragging ?
    }

}
