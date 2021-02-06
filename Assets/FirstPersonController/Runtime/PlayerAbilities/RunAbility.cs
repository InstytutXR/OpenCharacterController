using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class RunAbility : StatefulPlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(6f, 0.9f, 0.6f);

        public override void OnEnter(PlayerController controller)
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToSlide && controller.CanSlide())
            {
                controller.ChangeState<SlideAbility>();
            }
            else if (controller.wantsToCrouch)
            {
                controller.ChangeState<CrouchAbility>();
            }
            else if (controller.wantsToWalk)
            {
                controller.ChangeState<WalkAbility>();
            }
        }
    }
}
