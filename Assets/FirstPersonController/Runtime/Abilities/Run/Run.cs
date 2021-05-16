namespace FirstPersonController
{
    public class Run : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IRunIntent _intent;
        private readonly RunSO _so;

        public Run(IPlayerController controller, RunSO so)
        {
            _controller = controller;
            _intent = _controller.GetIntent<IRunIntent>();
            _so = so;
        }

        public override bool isBlocking => true;

        public override bool canActivate => _intent.wantsToStartRunning && _controller.canStandUp;

        public override void OnActivate()
        {
            _controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            _controller.ApplyUserInputMovement(_so.speed);

            if (_intent.wantsToStopRunning)
            {
                Deactivate();
            }
        }
    }
}