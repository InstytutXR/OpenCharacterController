using System;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public sealed class LeanAbility : PlayerAbility
    {
        [SerializeField]
        private Transform _leanTransform = default;

        [SerializeField]
        private float _leanDistanceX = 0.65f;

        [SerializeField]
        private float _leanDistanceY = -.05f;

        [SerializeField]
        private float _leanAngle = 10f;

        [SerializeField]
        private float _leanAnimationSpeed = 10f;

        public override void FixedUpdate()
        {
            var amount = controller.lean;

            var eyeLocalRot = _leanTransform.localEulerAngles;
            var desiredEyeRotThisFrame = Mathf.LerpAngle(
                eyeLocalRot.z,
                -amount * _leanAngle,
                _leanAnimationSpeed * Time.deltaTime
            );

            var targetEyeLocalPos = new Vector3(
                amount * _leanDistanceX,
                Mathf.Abs(amount) * _leanDistanceY,
                0
            );
            var desiredEyePosThisFrame = Vector3.Lerp(
                _leanTransform.localPosition,
                targetEyeLocalPos,
                _leanAnimationSpeed * Time.deltaTime
            );

            if (amount != 0)
            {
                var ray = new Ray(
                    _leanTransform.parent.position,
                    controller.transform.TransformDirection(targetEyeLocalPos.normalized)
                );

                var didHit = Physics.SphereCast(
                    ray,
                    controller.cameraCollisionRadius,
                    out var hit,
                    targetEyeLocalPos.magnitude,
                    ~(1 << controller.gameObject.layer)
                );

                if (didHit && desiredEyePosThisFrame.sqrMagnitude > (hit.distance * hit.distance))
                {
                    desiredEyePosThisFrame = _leanTransform.parent.InverseTransformPoint(
                        ray.origin + ray.direction * hit.distance
                    );

                    // Scale rotation to be the same percentage as our distance
                    desiredEyeRotThisFrame = Mathf.LerpAngle(
                        eyeLocalRot.z,
                        -amount * _leanAngle * (hit.distance / targetEyeLocalPos.magnitude),
                        _leanAnimationSpeed * Time.deltaTime
                    );
                }
            }
            else
            {
                desiredEyePosThisFrame = Vector3.Lerp(
                    _leanTransform.localPosition,
                    Vector3.zero,
                    _leanAnimationSpeed * Time.deltaTime
                );
            }

            _leanTransform.localPosition = desiredEyePosThisFrame;

            eyeLocalRot.z = desiredEyeRotThisFrame;
            _leanTransform.localEulerAngles = eyeLocalRot;
        }
    }
}
