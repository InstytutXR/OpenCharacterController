using UnityEngine;

namespace FirstPersonController
{
    public sealed class TurnIntentFromInputActionXAxis : ITurnIntent
    {
        private readonly TurnIntentFromInputActionXAxisSO _so;

        public TurnIntentFromInputActionXAxis(TurnIntentFromInputActionXAxisSO so)
        {
            _so = so;
        }

        public float amount => _so.action.action.ReadValue<Vector2>().x;
    }
}