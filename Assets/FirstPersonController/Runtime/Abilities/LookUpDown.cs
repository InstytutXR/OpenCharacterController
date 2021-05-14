using UnityEngine;

namespace FirstPersonController
{
    public class LookUpDown : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly LookUpDownSO _so;
        private readonly ILookUpDownIntent _intent;

        private float _pitch;

        public override bool canActivate => true;

        public LookUpDown(IPlayerController controller, LookUpDownSO so, ILookUpDownIntent intent)
        {
            _controller = controller;
            _so = so;
            _intent = intent;
        }

        public override void Update()
        {
            _pitch = Mathf.Clamp(_pitch + _intent.amount, _so.minPitch, _so.maxPitch);
            _controller.lookUpDownTransform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }
    }
}
