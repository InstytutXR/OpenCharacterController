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
        private InputActionReference _lookAction;

        [SerializeField, Tooltip("An action that provides a Vector2 for moving.")]
        private InputActionReference _moveAction;

        [SerializeField, Tooltip("A button action for jumping.")]
        private InputActionReference _jumpAction;

        [SerializeField, Tooltip("A button action for running.")]
        private InputActionReference _runAction;

        [SerializeField, Tooltip("A button action for crouching.")]
        private InputActionReference _crouchAction;

        [SerializeField, Tooltip("A 1D axis for leaning left and right.")]
        private InputActionReference _leanAction;

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

            /*
             * InputActionReference's .action property loads from the asset but we want
             * to ensure we load from the PlayerInput component so that our mapping assignments
             * from the PlayerInput are handled properly by our code as well.
             */

            _lookActionRef = playerInput.actions.FindAction(_lookAction.action.id);
            if (_lookActionRef != null)
            {
                _lookActionRef.performed += OnLookPerformed;
                _lookActionRef.canceled += OnLookCanceled;
            }

            _moveActionRef = playerInput.actions.FindAction(_moveAction.action.id);
            if (_moveActionRef != null)
            {
                _moveActionRef.performed += OnMovePerformed;
                _moveActionRef.canceled += OnMoveCanceled;
            }

            _jumpActionRef = playerInput.actions.FindAction(_jumpAction.action.id);
            if (_jumpActionRef != null)
            {
                _jumpActionRef.performed += OnJumpPerformed;
                _jumpActionRef.canceled += OnJumpCanceled;
            }

            _runActionRef = playerInput.actions.FindAction(_runAction.action.id);
            if (_runActionRef != null)
            {
                _runActionRef.performed += OnRunPerformed;
                _runActionRef.canceled += OnRunCanceled;
            }

            _crouchActionRef = playerInput.actions.FindAction(_crouchAction.action.id);
            if (_crouchActionRef != null)
            {
                _crouchActionRef.performed += OnCrouchPerformed;
                _crouchActionRef.canceled += OnCrouchCanceled;
            }

            _leanActionRef = playerInput.actions.FindAction(_leanAction.action.id);
            if (_leanActionRef != null)
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