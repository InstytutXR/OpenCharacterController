#if ENABLE_INPUT_SYSTEM

using System;
using UnityEngine.InputSystem;

namespace ModularFirstPerson
{
    /*
     * This is kind of like the UnityEngine.InputSystem.InputActionReference
     * at least in the goal, however this doesn't reference the actual input
     * asset. This has pros/cons as it requires the property drawer to make
     * some assumptions about how to find the list of actions, but it means
     * we don't serialize the extra reference and can ensure that we get the
     * action from the PlayerInput object and not the asset directly. I'm not
     * 100% sold on this being the right approach but it works for now.
     */
    [Serializable]
    public struct PlayerInputActionReference
    {
        public string guidAsString;

        public bool TryGetInputAction(PlayerInput input, out InputAction action)
        {
            if (Guid.TryParse(guidAsString, out var lookActionId))
            {
                action = input.actions.FindAction(lookActionId);
                return true;
            }
            else
            {
                action = null;
                return false;
            }
        }
    }
}

#endif // #if ENABLE_INPUT_SYSTEM