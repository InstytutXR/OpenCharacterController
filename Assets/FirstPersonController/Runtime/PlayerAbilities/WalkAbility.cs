using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class WalkAbility : StatefulPlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public override bool CanActivate(PlayerController controller)
        {
            return controller.wantsToWalk;
        }

        public override void OnEnter(PlayerController controller)
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.TryActivate<JumpAbility, RunAbility, CrouchAbility>();
            controller.ApplyUserInputMovement(_speed);
        }
    }
}
