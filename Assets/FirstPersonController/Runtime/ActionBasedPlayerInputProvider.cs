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

        private Vector2 _look;
        private Vector2 _move;

        // TODO: In-editor validation of these values and/or some kind of nice action picker UI
        [SerializeField, Tooltip("An action that provides a Vector2 for turning and looking up/down.")]
        private PlayerInputActionReference _lookAction;

        [SerializeField, Tooltip("An action that provides a Vector2 for moving.")]
        private PlayerInputActionReference _moveAction;

        public float lookHorizontal => _look.x;
        public float lookVertical => _look.y;
        public Vector2 moveInput => _move;

        private void OnEnable()
        {
            _look = Vector2.zero;
            _move = Vector2.zero;

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
        }

        private void OnLookPerformed(InputAction.CallbackContext ctx)
        {
            _look = ctx.ReadValue<Vector2>();
        }

        private void OnLookCanceled(InputAction.CallbackContext ctx)
        {
            _look = Vector2.zero;
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _move = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _move = Vector2.zero;
        }
    }
}

#endif // #if ENABLE_INPUT_SYSTEM