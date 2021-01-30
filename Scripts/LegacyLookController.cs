#if ENABLE_LEGACY_INPUT_MANAGER

using UnityEngine;

namespace ModularFirstPerson
{
    public sealed class LegacyLookController : LookController
    {
        public string yawAxis = "Mouse X";
        public string pitchAxis = "Mouse Y";
        public bool invertPitchInput = false;

        public override void UpdateLook(ref float yaw, ref float pitch)
        {
            yaw += Input.GetAxis(yawAxis);

            var pitchDelta = Input.GetAxis(pitchAxis);
            if (invertPitchInput)
            {
                pitchDelta = -pitchDelta;
            }

            pitch += pitchDelta;
        }
    }
}

#endif