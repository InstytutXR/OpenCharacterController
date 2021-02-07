using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class RunAbility : PlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(6f, 0.9f, 0.6f);

        public override bool isBlocking => true;

        public override bool canActivate => input.run && controller.canStandUp;

        public override void OnActivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            controller.ApplyUserInputMovement(_speed);

            if (!input.run)
            {
                Deactivate();
            }
        }
    }
}
