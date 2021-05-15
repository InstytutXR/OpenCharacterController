using UnityEngine;

namespace FirstPersonController
{
    public abstract class LookIntentSO : ScriptableObject
    {
        public abstract ILookIntent Create();
    }
}