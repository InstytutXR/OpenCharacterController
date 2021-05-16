namespace FirstPersonController
{
    public class Walk : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly WalkSO _so;

        public Walk(IPlayerController controller, WalkSO so)
        {
            _controller = controller;
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
            _controller.ApplyUserInputMovement(_so.speed);
        }
    }
}