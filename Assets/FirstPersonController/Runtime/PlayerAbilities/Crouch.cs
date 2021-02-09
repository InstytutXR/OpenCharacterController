using System;
using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Crouch")]
    public sealed class Crouch : PlayerAbility
    {
        [SerializeField]
        private float _colliderHeight = 0.9f;

        [SerializeField]
        private float _eyeHeight = 0.8f;

        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(0.8f, 1f, 1f);

        public override bool isBlocking => true;

        public override bool canActivate => input.crouch || !controller.canStandUp;

        public override void OnActivate()
        {
            controller.ChangeHeight(_colliderHeight, _eyeHeight);
        }

        public override void OnDeactivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            controller.ApplyUserInputMovement(_speed);

            if (!input.crouch && controller.canStandUp)
            {
                Deactivate();
            }
        }
    }
}
