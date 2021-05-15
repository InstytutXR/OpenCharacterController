using UnityEngine;

namespace FirstPersonController
{
    public class Look : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly IPlayerControllerInput _input;
        private readonly LookSO _so;

        public override bool canActivate => true;

        public Look(IPlayerController controller, IPlayerControllerInput input, LookSO so)
        {
            _controller = controller;
            _input = input;
            _so = so;
        }

        public override void Update()
        {
            var yaw = _controller.turnTransform.localEulerAngles.y;
            yaw = Mathf.Repeat(yaw + _input.lookInput.x, 360f);
            _controller.turnTransform.localEulerAngles = new Vector3(0, yaw, 0);

            var pitch = _controller.lookUpDownTransform.localEulerAngles.x;

            // Transform wraps pitch in [0, 360) range so we have to deal with that here
            if (pitch > 180.0f)
            {
                pitch -= 360.0f;
            }

            pitch = Mathf.Clamp(pitch + _input.lookInput.y, -_so.maxAngleUp, _so.maxAngleDown);
            _controller.lookUpDownTransform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }
}
