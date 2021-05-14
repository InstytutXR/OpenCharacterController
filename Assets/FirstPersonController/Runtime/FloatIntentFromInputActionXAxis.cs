using UnityEngine;

namespace FirstPersonController
{
    public sealed class FloatIntentFromInputActionXAxis : IFloatIntent
    {
        private readonly FloatIntentFromInputActionXAxisSO _so;

        public FloatIntentFromInputActionXAxis(FloatIntentFromInputActionXAxisSO so)
        {
            _so = so;
        }

        public float amount => _so.action.action.ReadValue<Vector2>().x;
    }
}