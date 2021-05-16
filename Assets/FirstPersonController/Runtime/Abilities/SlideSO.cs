using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Slide")]
    public sealed class SlideSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _speedRequiredToSlide = 3.5f;
        public float speedRequiredToSlide => _speedRequiredToSlide;

        [SerializeField]
        private float _colliderHeight = 0.9f;
        public float colliderHeight => _colliderHeight;

        [SerializeField]
        private float _eyeHeight = 0.8f;
        public float eyeHeight => _eyeHeight;

        [SerializeField]
        private float _groundFriction = 0.8f;
        public float groundFriction => _groundFriction;

        [SerializeField]
        private PhysicMaterialCombine _groundFrictionCombine = PhysicMaterialCombine.Multiply;
        public PhysicMaterialCombine groundFrictionCombine => _groundFrictionCombine;

        [SerializeField]
        private float _playerMass = 10f;
        public float playerMass => _playerMass;

        [SerializeField]
        private float _speedThresholdToExit = 0.8f;
        public float speedThresholdToExit => _speedThresholdToExit;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Slide(controller, this);
        }
    }
}