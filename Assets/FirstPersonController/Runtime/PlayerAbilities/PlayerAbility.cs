using System;

namespace FirstPersonController
{
    [Serializable]
    public abstract class PlayerAbility
    {
        public virtual void FixedUpdate(PlayerController controller)
        {
        }
    }
}
