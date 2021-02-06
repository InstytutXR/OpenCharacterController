using System;

namespace FirstPersonController
{
    [Serializable]
    public abstract class StatefulPlayerAbility : PlayerAbility
    {
        public abstract void OnEnter(PlayerController controller);
        public override abstract void FixedUpdate(PlayerController controller);
    }
}
