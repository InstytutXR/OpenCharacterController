using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class RunAbility : PlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(6f, 0.9f, 0.6f);

        public void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToSlide && controller.CanSlide())
            {
                controller.ChangeState(PlayerState.Sliding);
            }
            else if (controller.wantsToCrouch)
            {
                controller.ChangeState(PlayerState.Crouching);
            }
            else if (controller.wantsToWalk)
            {
                controller.ChangeState(PlayerState.Walking);
            }
        }
    }
}
