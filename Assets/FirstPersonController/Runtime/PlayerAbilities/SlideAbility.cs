using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class SlideAbility : StatefulPlayerAbility
    {
        public float speedRequiredToSlide = 3.5f;
        public float colliderHeight = 0.9f;
        public float eyeHeight = 0.8f;
        public float groundFriction = 0.8f;
        public PhysicMaterialCombine groundFrictionCombine = PhysicMaterialCombine.Multiply;
        public float speedThresholdToExit = 0.8f;

        public override bool CanActivate(PlayerController controller)
        {
            return controller.wantsToSlide && controller.speed >= speedRequiredToSlide;
        }

        public override void OnEnter(PlayerController controller)
        {
            controller.ChangeHeight(colliderHeight, eyeHeight);
        }

        public override void FixedUpdate(PlayerController controller)
        {
            controller.TryActivate<JumpAbility>();

            if (controller.grounded)
            {
                // Add gravity projected onto the ground plane for acceleration
                var gravity = Physics.gravity;
                var ground = controller.groundNormal;
                controller.controlVelocity += (gravity - ground * Vector3.Dot(gravity, ground)) * Time.deltaTime;
                controller.controlVelocity = controller.ApplyGroundFrictionToVelocity(
                    controller.controlVelocity,
                    groundFrictionCombine,
                    groundFriction
                );
            }
            else
            {
                controller.ApplyAirDrag();
            }

            if (controller.speed <= speedThresholdToExit)
            {
                if (!controller.CanStandUp() ||
                    !controller.TryActivate<RunAbility>())
                { 
                    controller.Activate<CrouchAbility>();
                }
            }
        }
    }
}
