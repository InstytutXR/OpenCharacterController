using UnityEngine;

namespace FirstPersonController
{
    public abstract class PlayerAbilitySO : ScriptableObject
    {
        public abstract PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        );
    }
}