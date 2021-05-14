using UnityEngine;

namespace FirstPersonController
{
    public sealed class LookUpDownIntentFromInputActionYAxis : ILookUpDownIntent
    {
        private readonly LookUpDownIntentFromInputActionYAxisSO _so;

        public LookUpDownIntentFromInputActionYAxis(LookUpDownIntentFromInputActionYAxisSO so)
        {
            _so = so;
        }

        public float amount => _so.action.action.ReadValue<Vector2>().y;
    }
}