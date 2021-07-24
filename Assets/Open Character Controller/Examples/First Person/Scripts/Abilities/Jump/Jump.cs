using UnityEngine;

namespace OpenCharacterController.Examples
{
    public class Jump : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IJumpIntent _intent;
        private readonly JumpSO _so;

        public Jump(IPlayerController controller, JumpSO so)
        {
            _controller = controller;
            _intent = _controller.GetIntent<IJumpIntent>();
            _so = so;
        }

        public override bool canActivate => _controller.grounded && _intent.wantsToJump;

        public override void OnActivate()
        {
            _controller.verticalVelocity = Mathf.Sqrt(2f * _so.height * -Physics.gravity.y);
            _controller.grounded = false;

            // Jump is a fire-and-forget ability; it doesn't need to stay activated
            Deactivate();
        }
    }
}
