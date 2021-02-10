using System;
using UnityEngine;

namespace FirstPersonController
{
    public abstract class PlayerAbility : ScriptableObject
    {
        public bool isActive { get; private set; }

        protected IPlayerController controller { get; private set; }

        protected IPlayerControllerInput input { get; private set; }

        public virtual bool isBlocking => false;

        public virtual bool updatesWhenNotActive => false;

        public virtual bool canActivate => true;

        public void Initialize(
            IPlayerController playerController, 
            IPlayerControllerInput playerInput
        )
        {
            controller = playerController;
            input = playerInput;
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