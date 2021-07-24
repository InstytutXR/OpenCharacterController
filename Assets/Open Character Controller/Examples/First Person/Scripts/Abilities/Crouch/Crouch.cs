namespace OpenCharacterController.Examples
{
    public class Crouch : PlayerAbility
    {
        private readonly CrouchSO _so;
        private readonly IPlayerController _controller;
        private readonly ICrouchIntent _intent;

        public override bool isBlocking => true;

        public override bool canActivate => _intent.wantsToStartCrouching || !_controller.canStandUp;

        public Crouch(IPlayerController controller, CrouchSO so)
        {
            _controller = controller;
            _intent = controller.GetIntent<ICrouchIntent>();
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
            _controller.ApplyUserInputMovement(_intent.moveDirection, _so.speed);

            if (_intent.wantsToStopCrouching && _controller.canStandUp)
            {
                Deactivate();
            }
        }
    }
}
