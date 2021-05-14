using UnityEngine;

namespace FirstPersonController
{
    public abstract class FloatIntentSO : ScriptableObject
    {
        public abstract IFloatIntent Create();
    }
}