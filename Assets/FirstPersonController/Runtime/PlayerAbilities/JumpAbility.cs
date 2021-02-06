using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class JumpAbility : PlayerAbility
    {
        [SerializeField]
        private float _jumpHeight = 1.5f;

        public override bool CanActivate(PlayerController controller)
        {
            return controller.grounded && controller.wantsToJump;
        }

        public override void Activate(PlayerController controller)
        {
            controller.verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * -Physics.gravity.y);
            controller.grounded = false;
        }
    }
}
