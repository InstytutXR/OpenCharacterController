using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Look")]
    public sealed class LookSO : PlayerAbilitySO
    {
        [SerializeField]
        private LookIntentSO _intent;

        [SerializeField, Tooltip("The minimum pitch value allowed, specified in degrees.")]
        private float _minPitch = -85f;
        public float minPitch => _minPitch;

        [SerializeField, Tooltip("The maximum pitch value allowed, specified in degrees.")]
        private float _maxPitch = 85f;
        public float maxPitch => _maxPitch;

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Look(controller, _intent.Create(), this);
        }
    }
}
