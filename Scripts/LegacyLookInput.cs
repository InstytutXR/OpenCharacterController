#if ENABLE_LEGACY_INPUT_MANAGER

using UnityEngine;

namespace ModularFirstPerson
{
    public sealed class LegacyLookInput : LookInput
    {
        public string yawAxis = "Mouse X";
        public string pitchAxis = "Mouse Y";
        public bool invertPitchInput = false;

        public override Vector2 GetLookDelta()
        {
            var yaw = Input.GetAxis(yawAxis);

            var pitch = Input.GetAxis(pitchAxis);
            if (invertPitchInput)
            {
                pitch = -pitch;
            }

            return new Vector2(yaw, pitch);
        }
    }
}

#endif