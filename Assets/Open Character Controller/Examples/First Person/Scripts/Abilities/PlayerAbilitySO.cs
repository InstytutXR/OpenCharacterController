using UnityEngine;

namespace OpenCharacterController.Examples
{
    public abstract class PlayerAbilitySO : ScriptableObject
    {
        public abstract PlayerAbility CreateAbility(IPlayerController controller);
    }
}