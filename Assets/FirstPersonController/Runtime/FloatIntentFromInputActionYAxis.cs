using UnityEngine;

namespace FirstPersonController
{
    public sealed class FloatIntentFromInputActionYAxis : IFloatIntent
    {
        private readonly FloatIntentFromInputActionYAxisSO _so;

        public FloatIntentFromInputActionYAxis(FloatIntentFromInputActionYAxisSO so)
        {
            _so = so;
        }

        public float amount => _so.action.action.ReadValue<Vector2>().y;
    }
}