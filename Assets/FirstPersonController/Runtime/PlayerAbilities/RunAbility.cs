using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class RunAbility : StatefulPlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(6f, 0.9f, 0.6f);

        public override bool CanActivate(PlayerController controller)
        {
            return controller.wantsToRun;
        }

        public override void OnEnter(PlayerController controller)
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.ApplyUserInputMovement(_speed);
            controller.TryActivate<JumpAbility, SlideAbility, CrouchAbility, WalkAbility>();
        }
    }
}
