using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Look")]
    public sealed class LookSO : PlayerAbilitySO
    {
        [SerializeField]
        private LookIntentSO _intent;

        [SerializeField, Range(0f, 90f), Tooltip("The maximum angle the player can look up, specified in degrees.")]
        private float _maxAngleUp = 85f;
        public float maxAngleUp => _maxAngleUp;

        [SerializeField, Range(0f, 90f), Tooltip("The maximum angle the player can look down, specified in degrees.")]
        private float _maxAngleDown = 85f;
        public float maxAngleDown => _maxAngleDown;

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Look(controller, _intent.Create(), this);
        }
    }
}
