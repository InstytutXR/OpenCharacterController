using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Turn")]
    public sealed class TurnSO : PlayerAbilitySO
    {
        [SerializeField]
        private TurnIntentSO _intent;

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Turn(controller, _intent.Create());
        }
    }
}
