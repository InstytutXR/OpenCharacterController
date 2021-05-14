namespace FirstPersonController
{
    public class Crouch : PlayerAbility
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
            _controller.ChangeHeight(_so.colliderHeight, _so.eyeHeight);
        }

        public override void OnDeactivate()
        {
            _controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            _controller.ApplyUserInputMovement(_so.speed);

            if (!_input.crouch && _controller.canStandUp)
            {
                Deactivate();
            }
        }
    }
}
