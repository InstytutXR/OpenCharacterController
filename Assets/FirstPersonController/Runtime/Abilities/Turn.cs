using UnityEngine;

namespace FirstPersonController
{
    public class Turn : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly ITurnIntent _turn;

        public override bool canActivate => true;

        public Turn(IPlayerController controller, ITurnIntent turn)
        {
            _controller = controller;
            _turn = turn;
        }

        public override void Update()
        {
            var yaw = _controller.turnTransform.localEulerAngles.y;
            yaw = Mathf.Repeat(yaw + _turn.amount, 360f);
            _controller.turnTransform.localEulerAngles = new Vector3(0, yaw, 0);
        }
    }
}
