using System;

namespace FirstPersonController
{
    [Serializable]
    public abstract class StatefulPlayerAbility : PlayerAbility
    {
        public abstract void OnEnter(PlayerController controller);
        public abstract void FixedUpdate(PlayerController controller);
    }
}
