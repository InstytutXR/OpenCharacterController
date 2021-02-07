using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class CrouchAbility : PlayerAbility
    {
        [SerializeField]
        private float _colliderHeight = 0.9f;

        [SerializeField]
        private float _eyeHeight = 0.8f;

        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(0.8f, 1f, 1f);

        public override bool IsBlocking()
        {
            return true;
        }
        
        public override bool CanActivate()
        {
            return input.crouch;
        }

        public override void OnActivate()
        {
            controller.ChangeHeight(_colliderHeight, _eyeHeight);
        }

        public override void OnDeactivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            controller.ApplyUserInputMovement(_speed);

            if (!input.crouch && controller.CanStandUp())
            {
                Deactivate();
            }
        }
    }
}
