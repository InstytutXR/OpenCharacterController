using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Intents/Look Intent From Input Action")]
    public sealed class LookIntentFromInputActionSO : LookIntentSO
    {
        [SerializeField]
        private InputActionReference _action;
        public InputActionReference action => _action;

        public override ILookIntent Create()
        {
            return new LookIntentFromInputAction(this);
        }
    }
}