using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class WalkAbility : PlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public override bool IsBlocking()
        {
            return true;
        }
        
        public override bool CanActivate()
        {
            return controller.wantsToWalk;
        }

        public override void OnActivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            controller.ApplyUserInputMovement(_speed);
        }
    }
}
