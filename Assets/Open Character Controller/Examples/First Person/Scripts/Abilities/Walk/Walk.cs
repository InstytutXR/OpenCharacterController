namespace OpenCharacterController.Examples
{
    public class Walk : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IWalkIntent _intent;
        private readonly WalkSO _so;

        public Walk(IPlayerController controller, WalkSO so)
        {
            _controller = controller;
            _intent = _controller.GetIntent<IWalkIntent>();
            _so = so;
        }

        public override bool isBlocking => true;

        public override bool canActivate => _controller.canStandUp;

        public override void OnActivate()
        {
            _controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            _controller.ApplyUserInputMovement(_intent.moveDirection, _so.speed);
        }
    }
}