#if ENABLE_LEGACY_INPUT_MANAGER

using UnityEngine;

namespace FirstPersonController
{
    public sealed class LegacyPlayerInputProvider
        : MonoBehaviour
        , IPlayerTurnInput
        , IPlayerLookUpDownInput
        , IPlayerControllerInput
    {
        private float _lookHorizontal;
        private float _lookVertical;
        private Vector2 _movementInput;
        private bool _jump;

        public string LookHorizontalAxis = "Mouse X";
        public string LookVerticalAxis = "Mouse Y";
        public bool InvertLookVertical = false;

        public string MoveHorizontalAxis = "Horizontal";
        public string MoveVerticalAxis = "Vertical";

        public string JumpButton = "Jump";

        public float lookHorizontal => _lookHorizontal;
        public float lookVertical => _lookVertical;
        public Vector2 moveInput => _movementInput;
        public bool jump => _jump;

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

            _jump = Input.GetButton(JumpButton);
        }
    }
}

#endif // #if ENABLE_LEGACY_INPUT_MANAGER