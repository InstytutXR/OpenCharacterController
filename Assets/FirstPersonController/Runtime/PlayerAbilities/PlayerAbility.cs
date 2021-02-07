using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public abstract class PlayerAbility
    {
        public bool isActive { get; private set; }
        
        public PlayerController controller { get; private set; }

        public IPlayerControllerInput input => controller.input;

        public CapsuleBody body => controller.body;

        public void Initialize(PlayerController playerController)
        {
            this.controller = playerController;
        }

        public void TryActivate()
        {
            if (!isActive && CanActivate())
            {
                isActive = true;
                OnActivate();
            }
        }

        public void Deactivate()
        {
            if (isActive)
            {
                isActive = false;
                OnDeactivate();
            }
        }
        
        public virtual bool IsBlocking() => false;
        
        public virtual bool CanActivate()
        {
            return true;
        }

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        public virtual void FixedUpdate()
        {
        }
    }
}
