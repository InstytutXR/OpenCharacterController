using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class JumpAbility : PlayerAbility
    {
        [SerializeField]
        private float _jumpHeight = 1.5f;

        public override bool canActivate => controller.grounded && input.jump;

        public override void OnActivate()
        {
            controller.verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * -Physics.gravity.y);
            controller.grounded = false;

            // Jump is a fire-and-forget ability; it doesn't need to stay activated
            Deactivate();
        }
    }
}
