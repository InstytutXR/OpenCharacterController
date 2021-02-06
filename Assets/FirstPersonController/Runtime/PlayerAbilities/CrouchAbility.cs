using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class CrouchAbility : StatefulPlayerAbility
    {
        [SerializeField]
        private float _colliderHeight = 0.9f;

        [SerializeField]
        private float _eyeHeight = 0.8f;

        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(0.8f, 1f, 1f);

        public override void OnEnter(PlayerController controller)
        {
            controller.ChangeHeight(_colliderHeight, _eyeHeight);
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToStandUp && controller.CanStandUp())
            {
                if (controller.wantsToRun)
                {
                    controller.ChangeState<RunAbility>();
                }
                else
                {
                    controller.ChangeState<WalkAbility>();
                }
            }
        }
    }
}
