using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class JumpAbility : PlayerAbility
    {
        [SerializeField]
        private float _jumpHeight = 1.5f;

        public bool Try(PlayerController controller)
        {
            if (controller.grounded && controller.wantsToJump)
            {
                controller.verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * -Physics.gravity.y);
                controller.grounded = false;
                controller.ChangeState(PlayerState.Walking);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
