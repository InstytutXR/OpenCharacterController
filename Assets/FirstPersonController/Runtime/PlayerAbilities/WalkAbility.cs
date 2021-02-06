using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class WalkAbility : PlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToRun)
            {
                controller.ChangeState(PlayerState.Running);
            }
            else if (controller.wantsToCrouch)
            {
                controller.ChangeState(PlayerState.Crouching);
            }
        }
    }
}
