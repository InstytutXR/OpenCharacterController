#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    public sealed class ActionBasedPlayerInputProvider
        : MonoBehaviour
        , IPlayerTurnInput
        , IPlayerLookUpDownInput
        , IPlayerControllerInput
    {
        private InputAction _lookActionRef;
        private InputAction _moveActionRef;
        private InputAction _jumpActionRef;
        private InputAction _runActionRef;
        private InputAction _crouchActionRef;
        private InputAction _leanActionRef;

        private Vector2 _look;
        private Vector2 _move;
        private bool _jump;
        private bool _run;
        private bool _crouch;
        private float _lean;

        [SerializeField, Tooltip("An action that provides a Vector2 for turning and looking up/down.")]
        private PlayerInputActionReference _lookAction;

        [SerializeField, Tooltip("An action that provides a Vector2 for moving.")]
        private PlayerInputActionReference _moveAction;

        [SerializeField, Tooltip("A button action for jumping.")]
        private PlayerInputActionReference _jumpAction;

        [SerializeField, Tooltip("A button action for running.")]
        private PlayerInputActionReference _runAction;

        [SerializeField, Tooltip("A button action for crouching.")]
        private PlayerInputActionReference _crouchAction;

        [SerializeField, Tooltip("A 1D axis for leaning left and right.")]
        private PlayerInputActionReference _leanAction;

        public float lookHorizontal => _look.x;
        public float lookVertical => _look.y;
        public Vector2 moveInput => _move;
        public bool jump => _jump;
        public bool run => _run;
        public bool crouch => _crouch;
        public float lean => _lean;

        private void OnEnable()
        {
            _look = Vector2.zero;
            _move = Vector2.zero;
            _jump = false;
            _run = false;
            _crouch = false;
            _lean = 0;

            var playerInput = GetComponentInParent<PlayerInput>();

            if (_lookAction.TryGetInputAction(playerInput, out _lookActionRef))
            {
                _lookActionRef.performed += OnLookPerformed;
                _lookActionRef.canceled += OnLookCanceled;
            }

            if (_moveAction.TryGetInputAction(playerInput, out _moveActionRef))
            {
                _moveActionRef.performed += OnMovePerformed;
                _moveActionRef.canceled += OnMoveCanceled;
            }

            if (_jumpAction.TryGetInputAction(playerInput, out _jumpActionRef))
            {
                _jumpActionRef.performed += OnJumpPerformed;
                _jumpActionRef.canceled += OnJumpCanceled;
            }

            if (_runAction.TryGetInputAction(playerInput, out _runActionRef))
            {
                _runActionRef.performed += OnRunPerformed;
                _runActionRef.canceled += OnRunCanceled;
            }

            if (_crouchAction.TryGetInputAction(playerInput, out _crouchActionRef))
            {
                _crouchActionRef.performed += OnCrouchPerformed;
                _crouchActionRef.canceled += OnCrouchCanceled;
            }

            if (_leanAction.TryGetInputAction(playerInput, out _leanActionRef))
            {
                _leanActionRef.performed += OnLeanPerformed;
                _leanActionRef.canceled += OnLeanCanceled;
            }
        }

        private void OnDisable()
        {
            if (_lookActionRef != null)
            {
                _lookActionRef.performed -= OnLookPerformed;
                _lookActionRef.canceled -= OnLookCanceled;
            }

            if (_moveActionRef != null)
            {
                _moveActionRef.performed -= OnMovePerformed;
                _moveActionRef.canceled -= OnMoveCanceled;
            }

            if (_jumpActionRef != null)
            {
                _jumpActionRef.performed -= OnJumpPerformed;
                _jumpActionRef.canceled -= OnJumpCanceled;
            }

            if (_runActionRef != null)
            {
                _runActionRef.performed -= OnRunPerformed;
                _runActionRef.canceled -= OnRunCanceled;
            }

            if (_crouchActionRef != null)
            {
                _crouchActionRef.performed -= OnCrouchPerformed;
                _crouchActionRef.canceled -= OnCrouchCanceled;
            }

            if (_leanActionRef != null)
            {
                _leanActionRef.performed -= OnLeanPerformed;
                _leanActionRef.canceled -= OnLeanCanceled;
            }
        }

        private void OnLookPerformed(InputAction.CallbackContext ctx) => _look = ctx.ReadValue<Vector2>();
        private void OnLookCanceled(InputAction.CallbackContext ctx) => _look = Vector2.zero;
        private void OnMovePerformed(InputAction.CallbackContext ctx) => _move = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(InputAction.CallbackContext ctx) => _move = Vector2.zero;
        private void OnJumpPerformed(InputAction.CallbackContext ctx) => _jump = true;
        private void OnJumpCanceled(InputAction.CallbackContext ctx) => _jump = false;
        private void OnRunPerformed(InputAction.CallbackContext ctx) => _run = true;
        private void OnRunCanceled(InputAction.CallbackContext ctx) => _run = false;
        private void OnCrouchPerformed(InputAction.CallbackContext ctx) => _crouch = true;
        private void OnCrouchCanceled(InputAction.CallbackContext ctx) => _crouch = false;
        private void OnLeanPerformed(InputAction.CallbackContext ctx) => _lean = ctx.ReadValue<float>();
        private void OnLeanCanceled(InputAction.CallbackContext ctx) => _lean = 0;
    }
}

#endif // #if ENABLE_INPUT_SYSTEM