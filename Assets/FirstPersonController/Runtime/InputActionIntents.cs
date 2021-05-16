#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    /// <summary>
    /// A default implementation of the built-in player intents using the Unity Input System
    /// and Input Actions to map to the intents. Use of this component is not required and
    /// games can create their own intent implementations to match their specific needs.
    /// </summary>
    public sealed class InputActionIntents
        : MonoBehaviour
        , IMoveIntent
        , ILookIntent
        , IJumpIntent
        , IRunIntent
        , ICrouchIntent
        , ISlideIntent
        , ILeanIntent
    {
        private InputAction _moveActionRef;
        private InputAction _lookActionRef;
        private InputAction _jumpActionRef;
        private InputAction _runActionRef;
        private InputAction _crouchActionRef;
        private InputAction _leanActionRef;

        private Vector2 _move;
        private Vector2 _look;
        private bool _jump;
        private bool _run;
        private bool _crouch;
        private float _lean;

        [SerializeField, Tooltip("An action that provides a Vector2 for moving.")]
        private InputActionReference _moveAction;

        [SerializeField, Tooltip("An action that provides a Vector2 for looking.")]
        private InputActionReference _lookAction;

        [SerializeField, Tooltip("A button action for jumping.")]
        private InputActionReference _jumpAction;

        [SerializeField, Tooltip("A button action for running.")]
        private InputActionReference _runAction;

        [SerializeField, Tooltip("A button action for crouching.")]
        private InputActionReference _crouchAction;

        [SerializeField, Tooltip("A 1D axis for leaning left and right.")]
        private InputActionReference _leanAction;

        public Vector2 moveAmount => _move;
        public Vector2 lookAmount => _look;
        public bool wantsToJump => _jump;
        public bool wantsToStartRunning => _run;
        public bool wantsToStopRunning => !_run;
        public bool wantsToStartCrouching => _crouch;
        public bool wantsToStopCrouching => !_crouch;
        public bool wantsToSlide => _crouch;
        public float leanAmount => _lean;

        private void OnEnable()
        {
            _move = Vector2.zero;
            _look = Vector2.zero;
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

            _moveActionRef = playerInput.actions.FindAction(_moveAction.action.id);
            if (_moveActionRef != null)
            {
                _moveActionRef.performed += OnMovePerformed;
                _moveActionRef.canceled += OnMoveCanceled;
            }

            _lookActionRef = playerInput.actions.FindAction(_lookAction.action.id);
            if (_lookActionRef != null)
            {
                _lookActionRef.performed += OnLookPerformed;
                _lookActionRef.canceled += OnLookCanceled;
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
            if (_moveActionRef != null)
            {
                _moveActionRef.performed -= OnMovePerformed;
                _moveActionRef.canceled -= OnMoveCanceled;
            }

            if (_lookActionRef != null)
            {
                _lookActionRef.performed -= OnLookPerformed;
                _lookActionRef.canceled -= OnLookCanceled;
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

        private void OnMovePerformed(InputAction.CallbackContext ctx) => _move = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(InputAction.CallbackContext ctx) => _move = Vector2.zero;
        private void OnLookPerformed(InputAction.CallbackContext ctx) => _look = ctx.ReadValue<Vector2>();
        private void OnLookCanceled(InputAction.CallbackContext ctx) => _look = Vector2.zero;
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