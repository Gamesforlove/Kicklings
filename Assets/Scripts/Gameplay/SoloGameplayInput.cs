using UnityEngine;

namespace Gameplay
{
    internal static class SoloGameplayInput
    {
        public const string ControlScheme = "KeyboardSolo";

        public static bool WasPressedThisFrame => Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z);
        public static bool IsHeld => Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Z);
    }
}
