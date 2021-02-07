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

        public override bool IsBlocking()
        {
            return true;
        }
        
        public override bool CanActivate()
        {
            return input.crouch && controller.speed >= speedRequiredToSlide;
        }

        public override void OnActivate()
        {
            controller.ChangeHeight(colliderHeight, eyeHeight);
        }

        public override void OnDeactivate()
        {
            controller.ResetHeight();
        }

        public override void FixedUpdate()
        {
            if (controller.grounded)
            {
                // Add gravity projected onto the ground plane for acceleration
                var gravity = Physics.gravity;
                var ground = controller.groundNormal;
                controller.controlVelocity += (gravity - ground * Vector3.Dot(gravity, ground)) * Time.deltaTime;
                controller.controlVelocity = body.ApplyGroundFrictionToVelocity(
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
                Deactivate();
            }
        }
    }
}
