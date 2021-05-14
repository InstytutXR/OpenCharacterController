using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Intents/Float Intent From Input Action X Axis")]
    public sealed class TurnIntentFromInputActionXAxisSO : TurnIntentSO
    {
        [SerializeField]
        private InputActionReference _action;
        public InputActionReference action => _action;

        public override ITurnIntent Create()
        {
            return new TurnIntentFromInputActionXAxis(this);
        }
    }
}