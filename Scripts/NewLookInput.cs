#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularFirstPerson
{
    public sealed class NewLookInput : LookInput
    {
        private InputAction _action;
        private Vector2 _lookDelta;

        [SerializeField]
        private PlayerInput _input = default;

        [SerializeField]
        private string _lookAction = "Look";

        private void OnEnable()
        {
            _action = _input.actions.FindAction(_lookAction, throwIfNotFound: true);
            _action.performed += OnLookActionPerformed;
            _action.canceled += OnLookActionCanceled;
        }

        private void OnDisable()
        {
            _action.performed -= OnLookActionPerformed;
            _action.canceled -= OnLookActionCanceled;
        }

        private void OnValidate()
        {
            if (!_input)
            {
                _input = GetComponent<PlayerInput>();
            }
        }

        public override Vector2 GetLookDelta()
        {
            return _lookDelta;
        }

        private void OnLookActionPerformed(InputAction.CallbackContext ctx)
        {
            _lookDelta = ctx.ReadValue<Vector2>();
        }

        private void OnLookActionCanceled(InputAction.CallbackContext ctx)
        {
            _lookDelta = Vector2.zero;
        }
    }
}

#endif