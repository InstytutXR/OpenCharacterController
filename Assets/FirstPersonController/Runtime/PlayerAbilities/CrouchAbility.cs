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

        public void OnActivate(PlayerController controller)
        {
            controller.ChangeHeight(_colliderHeight, _eyeHeight);
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();
            controller.ApplyUserInputMovement(_speed);

            if (controller.wantsToStandUp && controller.CanStandUp())
            {
                if (controller.wantsToRun)
                {
                    controller.ChangeState(PlayerState.Running);
                }
                else
                {
                    controller.ChangeState(PlayerState.Walking);
                }
            }
        }
    }
}
