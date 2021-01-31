#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularFirstPerson
{
    public sealed class NewPlayerInputProvider : PlayerInputProvider
    {
        private InputAction _lookAction;
        private InputAction _moveAction;

        private Vector2 _look;
        private Vector2 _move;

        [SerializeField]
        private PlayerInput _input = default;

        // TODO: In-editor validation of these values and/or some kind of nice action picker UI
        [SerializeField]
        private string _lookActionName = "Look";

        [SerializeField]
        private string _moveActionName = "Move";

        public override float lookHorizontal => _look.x;
        public override float lookVertical => _look.y;
        public override Vector2 moveInput => _move;

        private void OnEnable()
        {
            _lookAction = _input.actions.FindAction(_lookActionName, throwIfNotFound: true);
            _lookAction.performed += OnLookPerformed;
            _lookAction.canceled += OnLookCanceled;

            _moveAction = _input.actions.FindAction(_moveActionName, throwIfNotFound: true);
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _lookAction.performed -= OnLookPerformed;
            _lookAction.canceled -= OnLookCanceled;

            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
        }

        private void OnValidate()
        {
            if (!_input)
            {
                _input = GetComponent<PlayerInput>();
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