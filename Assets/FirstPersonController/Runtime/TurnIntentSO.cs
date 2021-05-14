using UnityEngine;

namespace FirstPersonController
{
    public abstract class TurnIntentSO : ScriptableObject
    {
        public abstract ITurnIntent Create();
    }
}