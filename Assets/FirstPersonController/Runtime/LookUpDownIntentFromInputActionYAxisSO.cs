using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Intents/Float Intent From Input Action Y Axis")]
    public sealed class LookUpDownIntentFromInputActionYAxisSO : LookUpDownIntentSO
    {
        [SerializeField]
        private InputActionReference _action;
        public InputActionReference action => _action;

        public override ILookUpDownIntent Create()
        {
            return new LookUpDownIntentFromInputActionYAxis(this);
        }
    }
}