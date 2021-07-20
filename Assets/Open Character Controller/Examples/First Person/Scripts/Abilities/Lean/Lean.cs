using UnityEngine;

namespace FirstPersonController
{
    public class Lean : PlayerAbility
    {
        private readonly IPlayerController _controller;
        private readonly ILeanIntent _intent;
        private readonly LeanSO _so;

        public override bool canActivate => true;
        public override bool updatesWhenNotActive => true;

        public Lean(IPlayerController controller, LeanSO so)
        {
            _controller = controller;
            _intent = _controller.GetIntent<ILeanIntent>();
            _so = so;
        }

        public override void FixedUpdate()
        {
            // Because we update when not active, we need to be careful
            // when we apply input.
            var amount = isActive ? _intent.leanAmount : 0;
            var leanTransform = _controller.leanTransform;

            var eyeLocalRot = leanTransform.localEulerAngles;
            var desiredEyeRotThisFrame = Mathf.LerpAngle(
                eyeLocalRot.z,
                -amount * _so.leanAngle,
                _so.leanAnimationSpeed * Time.deltaTime
            );

            var targetEyeLocalPos = new Vector3(
                amount * _so.leanDistanceX,
                Mathf.Abs(amount) * _so.leanDistanceY,
                0
            );
            var desiredEyePosThisFrame = Vector3.Lerp(
                leanTransform.localPosition,
                targetEyeLocalPos,
                _so.leanAnimationSpeed * Time.deltaTime
            );

            if (amount != 0)
            {
                var ray = new Ray(
                    leanTransform.parent.position,
                    _controller.TransformDirection(targetEyeLocalPos.normalized)
                );

                var didHit = Physics.SphereCast(
                    ray,
                    _controller.cameraCollisionRadius,
                    out var hit,
                    targetEyeLocalPos.magnitude
                );

                if (didHit && desiredEyePosThisFrame.sqrMagnitude > (hit.distance * hit.distance))
                {
                    desiredEyePosThisFrame = leanTransform.parent.InverseTransformPoint(
                        ray.origin + ray.direction * hit.distance
                    );

                    // Scale rotation to be the same percentage as our distance
                    desiredEyeRotThisFrame = Mathf.LerpAngle(
                        eyeLocalRot.z,
                        -amount * _so.leanAngle * (hit.distance / targetEyeLocalPos.magnitude),
                        _so.leanAnimationSpeed * Time.deltaTime
                    );
                }
            }
            else
            {
                desiredEyePosThisFrame = Vector3.Lerp(
                    leanTransform.localPosition,
                    Vector3.zero,
                    _so.leanAnimationSpeed * Time.deltaTime
                );
            }

            leanTransform.localPosition = desiredEyePosThisFrame;

            eyeLocalRot.z = desiredEyeRotThisFrame;
            leanTransform.localEulerAngles = eyeLocalRot;
        }
    }
}