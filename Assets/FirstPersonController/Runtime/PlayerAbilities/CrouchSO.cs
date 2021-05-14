using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Crouch")]
    public sealed class CrouchSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _colliderHeight = 0.9f;

        [SerializeField]
        private float _eyeHeight = 0.8f;

        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(0.8f, 1f, 1f);

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Crouch(controller, input, this);
        }

        private class Crouch : PlayerAbility
        {
            private readonly CrouchSO _so;
            private readonly IPlayerController _controller;
            private readonly IPlayerControllerInput _input;

            public override bool isBlocking => true;

            public override bool canActivate => _input.crouch || !_controller.canStandUp;

            public Crouch(
                IPlayerController controller,
                IPlayerControllerInput input,
                CrouchSO so)
            {
                _controller = controller;
                _input = input;
                _so = so;
            }

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
                _controller.ApplyUserInputMovement(_so._speed);

                if (!_input.crouch && _controller.canStandUp)
                {
                    Deactivate();
                }
            }
        }
    }
}
