using UnityEngine;

namespace FirstPersonController
{
    public abstract class LookUpDownIntentSO : ScriptableObject
    {
        public abstract ILookUpDownIntent Create();
    }
}