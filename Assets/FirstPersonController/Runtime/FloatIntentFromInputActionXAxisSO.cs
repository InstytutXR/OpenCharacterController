using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Intents/Float Intent From Input Action X Axis")]
    public sealed class FloatIntentFromInputActionXAxisSO : FloatIntentSO
    {
        [SerializeField]
        private InputActionReference _action;
        public InputActionReference action => _action;

        public override IFloatIntent Create()
        {
            return new FloatIntentFromInputActionXAxis(this);
        }
    }
}