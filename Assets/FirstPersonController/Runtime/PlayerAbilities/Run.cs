namespace FirstPersonController
{
    public class Run : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IPlayerControllerInput _input;
        private readonly RunSO _so;

        public Run(
            IPlayerController controller,
            IPlayerControllerInput input,
            RunSO so
        )
        {
            _controller = controller;
            _input = input;
            _so = so;
        }

        public override bool isBlocking => true;

        public override bool canActivate => _input.run && _controller.canStandUp;

        public override void OnActivate()
        {
            _controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            _controller.ApplyUserInputMovement(_so.speed);

            if (!_input.run)
            {
                Deactivate();
            }
        }
    }
}