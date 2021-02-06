using System;

namespace FirstPersonController
{
    [Serializable]
    public abstract class PlayerAbility
    {
        public virtual bool CanActivate(PlayerController controller)
        {
            return true;
        }

        public virtual void Activate(PlayerController controller)
        {
        }

        public virtual void FixedUpdate(PlayerController controller)
        {
        }
    }
}
