using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Intents/Float Intent From Input Action Y Axis")]
    public sealed class FloatIntentFromInputActionYAxisSO : FloatIntentSO
    {
        [SerializeField]
        private InputActionReference _action;
        public InputActionReference action => _action;

        public override IFloatIntent Create()
        {
            return new FloatIntentFromInputActionYAxis(this);
        }
    }
}