using UnityEngine;

namespace FirstPersonController
{
    public class Slide : PlayerAbility
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

        public override bool canActivate => _input.crouch && _controller.speed >= _so.speedRequiredToSlide;

        public override void OnActivate()
        {
            _controller.ChangeHeight(_so.colliderHeight, _so.eyeHeight);
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
                    _so.groundFrictionCombine,
                    _so.groundFriction,
                    _so.playerMass
                );
            }
            else
            {
                _controller.ApplyAirDrag();
            }

            if (_controller.speed <= _so.speedThresholdToExit)
            {
                Deactivate();
            }
        }
    }
}