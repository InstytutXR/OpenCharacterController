using UnityEngine;

namespace FirstPersonController
{
    public sealed class LookIntentFromInputAction : ILookIntent
    {
        private readonly LookIntentFromInputActionSO _so;

        public LookIntentFromInputAction(LookIntentFromInputActionSO so)
        {
            _so = so;
        }

        public Vector2 amount => _so.action.action.ReadValue<Vector2>();
    }
}