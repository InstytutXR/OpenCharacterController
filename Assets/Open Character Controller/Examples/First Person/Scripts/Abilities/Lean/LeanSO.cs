using UnityEngine;

namespace OpenCharacterController.Examples
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Lean")]
    public sealed class LeanSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _leanDistanceX = 0.65f;
        public float leanDistanceX => _leanDistanceX;

        [SerializeField]
        private float _leanDistanceY = -.05f;
        public float leanDistanceY => _leanDistanceY;

        [SerializeField]
        private float _leanAngle = 10f;
        public float leanAngle => _leanAngle;

        [SerializeField]
        private float _leanAnimationSpeed = 10f;
        public float leanAnimationSpeed => _leanAnimationSpeed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Lean(controller, this);
        }
    }
}