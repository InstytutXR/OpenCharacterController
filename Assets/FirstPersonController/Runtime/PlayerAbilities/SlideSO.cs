using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Slide")]
    public sealed class SlideSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _speedRequiredToSlide = 3.5f;

        [SerializeField]
        private float _colliderHeight = 0.9f;

        [SerializeField]
        private float _eyeHeight = 0.8f;

        [SerializeField]
        private float _groundFriction = 0.8f;

        [SerializeField]
        private PhysicMaterialCombine _groundFrictionCombine = PhysicMaterialCombine.Multiply;

        [SerializeField]
        private float _playerMass = 10f;

        [SerializeField]
        private float _speedThresholdToExit = 0.8f;

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Slide(controller, input, this);
        }

        private class Slide : PlayerAbility
        {
            private readonly IPlayerController _controller;
            private readonly IPlayerControllerInput _input;
            private readonly SlideSO _so;

            public Slide(
                IPlayerController controller,
                IPlayerControllerInput input,
                SlideSO so
            )
            {
                _controller = controller;
                _input = input;
                _so = so;
            }

            public override bool isBlocking => true;

            public override bool canActivate => _input.crouch && _controller.speed >= _so._speedRequiredToSlide;

            public override void OnActivate()
            {
                _controller.ChangeHeight(_so._colliderHeight, _so._eyeHeight);
            }

            public override void OnDeactivate()
            {
                _controller.ResetHeight();
            }

            public override void FixedUpdate()
            {
                if (_controller.grounded)
                {
                    // Add gravity projected onto the ground plane for acceleration
                    var gravity = Physics.gravity;
                    var ground = _controller.groundNormal;
                    _controller.controlVelocity += (gravity - ground * Vector3.Dot(gravity, ground)) * Time.deltaTime;
                    _controller.controlVelocity = CustomPhysics.ApplyGroundFrictionToVelocity(
                        _controller.groundMaterial,
                        _controller.controlVelocity,
                        _so._groundFrictionCombine,
                        _so._groundFriction,
                        _so._playerMass
                    );
                }
                else
                {
                    _controller.ApplyAirDrag();
                }

                if (_controller.speed <= _so._speedThresholdToExit)
                {
                    Deactivate();
                }
            }
        }
    }
}