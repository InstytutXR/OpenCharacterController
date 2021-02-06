using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class WalkAbility : StatefulPlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public override void OnEnter(PlayerController controller)
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToRun)
            {
                controller.ChangeState<RunAbility>();
            }
            else if (controller.wantsToCrouch)
            {
                controller.ChangeState<CrouchAbility>();
            }
        }
    }
}
