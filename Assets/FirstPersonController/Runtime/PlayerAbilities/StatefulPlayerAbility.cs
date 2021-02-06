using System;

namespace FirstPersonController
{
    [Serializable]
    public abstract class StatefulPlayerAbility : PlayerAbility
    {
        public override void Activate(PlayerController controller)
        {
            controller.ChangeState(this);
        }

        public abstract void OnEnter(PlayerController controller);
        public override abstract void FixedUpdate(PlayerController controller);
    }
}
