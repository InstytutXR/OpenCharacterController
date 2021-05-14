using UnityEngine;

namespace FirstPersonController
{
    public class Jump : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IPlayerControllerInput _input;
        private readonly JumpSO _so;

        public Jump(
            IPlayerController playerController,
            IPlayerControllerInput playerInput,
            JumpSO jumpSO
        )
        {
            _controller = playerController;
            _input = playerInput;
            _so = jumpSO;
        }

        public override bool canActivate => _controller.grounded && _input.jump;

        public override void OnActivate()
        {
            _controller.verticalVelocity = Mathf.Sqrt(2f * _so.height * -Physics.gravity.y);
            _controller.grounded = false;

            // Jump is a fire-and-forget ability; it doesn't need to stay activated
            Deactivate();
        }
    }
}
