using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class WalkAbility : PlayerAbility
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public override bool IsBlocking()
        {
            // NOTE: This kind of assumes that this is the bottom ability
            // in the player since we're marking it as blocking and always
            // allowing it to be activated.
            return true;
        }
        
        public override void OnActivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            controller.ApplyUserInputMovement(_speed);
        }
    }
}
