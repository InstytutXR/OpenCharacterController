using System;
using UnityEngine;

namespace FirstPersonController
{
    public abstract class PlayerAbility : ScriptableObject
    {
        public bool isActive { get; private set; }

        public PlayerController controller { get; private set; }

        public IPlayerControllerInput input => controller.input;

        public virtual bool isBlocking => false;

        public virtual bool updatesWhenNotActive => false;

        public virtual bool canActivate => true;

        public void Initialize(PlayerController playerController)
        {
            controller = playerController;
        }

        public void TryActivate()
        {
            if (!isActive && canActivate)
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