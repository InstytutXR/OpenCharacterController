#if ENABLE_LEGACY_INPUT_MANAGER

using UnityEngine;

namespace FirstPersonController
{
    public sealed class LegacyPlayerInputProvider : PlayerInputProvider
    {
        private float _lookHorizontal;
        private float _lookVertical;
        private Vector2 _movementInput;

        public string LookHorizontalAxis = "Mouse X";
        public string LookVerticalAxis = "Mouse Y";
        public bool InvertLookVertical = false;

        public string MoveHorizontalAxis = "Horizontal";
        public string MoveVerticalAxis = "Vertical";

        public override float lookHorizontal => _lookHorizontal;
        public override float lookVertical => _lookVertical;
        public override Vector2 moveInput => _movementInput;

        private void Update()
        {
            _lookHorizontal = Input.GetAxisRaw(LookHorizontalAxis);
            
            _lookVertical = Input.GetAxisRaw(LookVerticalAxis);
            if (InvertLookVertical)
            {
                _lookVertical = -_lookVertical;
            }

            _movementInput = new Vector2(
                Input.GetAxisRaw(MoveHorizontalAxis),
                Input.GetAxisRaw(MoveVerticalAxis)
            );
        }
    }
}

#endif // #if ENABLE_LEGACY_INPUT_MANAGER