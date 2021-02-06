using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class SlideAbility : PlayerAbility
    {
        public float speedRequiredToSlide = 3.5f;
        public float colliderHeight = 0.9f;
        public float eyeHeight = 0.8f;
        public float groundFriction = 0.8f;
        public PhysicMaterialCombine groundFrictionCombine = PhysicMaterialCombine.Multiply;
        public float speedThresholdToExit = 0.8f;

        public bool CanActivate(PlayerController controller)
        {
            return controller.speed >= speedRequiredToSlide;
        }

        public void OnActivate(PlayerController controller)
        {
            controller.ChangeHeight(colliderHeight, eyeHeight);
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.TryJump();

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
                if (controller.wantsToRun && controller.CanStandUp())
                {
                    controller.ChangeState(PlayerState.Running);
                }
                else
                {
                    controller.ChangeState(PlayerState.Crouching);
                }
            }
        }
    }
}
