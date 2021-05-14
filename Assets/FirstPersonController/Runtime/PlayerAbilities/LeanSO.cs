using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Lean")]
    public sealed class LeanSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _leanDistanceX = 0.65f;

        [SerializeField]
        private float _leanDistanceY = -.05f;

        [SerializeField]
        private float _leanAngle = 10f;

        [SerializeField]
        private float _leanAnimationSpeed = 10f;

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Lean(controller, input, this);
        }

        private class Lean : PlayerAbility
        {
            private readonly IPlayerController _controller;
            private readonly IPlayerControllerInput _input;
            private readonly LeanSO _so;

            public Lean(
                IPlayerController controller,
                IPlayerControllerInput input,
                LeanSO so
            )
            {
                _controller = controller;
                _input = input;
                _so = so;
            }

            public override bool updatesWhenNotActive => true;

            public override void FixedUpdate()
            {
                // Because we update when not active, we need to be careful
                // when we apply input.
                var amount = isActive ? _input.lean : 0;
                var leanTransform = _controller.leanTransform;

                var eyeLocalRot = leanTransform.localEulerAngles;
                var desiredEyeRotThisFrame = Mathf.LerpAngle(
                    eyeLocalRot.z,
                    -amount * _so._leanAngle,
                    _so._leanAnimationSpeed * Time.deltaTime
                );

                var targetEyeLocalPos = new Vector3(
                    amount * _so._leanDistanceX,
                    Mathf.Abs(amount) * _so._leanDistanceY,
                    0
                );
                var desiredEyePosThisFrame = Vector3.Lerp(
                    leanTransform.localPosition,
                    targetEyeLocalPos,
                    _so._leanAnimationSpeed * Time.deltaTime
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
                        targetEyeLocalPos.magnitude,
                        ~_controller.layerMask
                    );

                    if (didHit && desiredEyePosThisFrame.sqrMagnitude > (hit.distance * hit.distance))
                    {
                        desiredEyePosThisFrame = leanTransform.parent.InverseTransformPoint(
                            ray.origin + ray.direction * hit.distance
                        );

                        // Scale rotation to be the same percentage as our distance
                        desiredEyeRotThisFrame = Mathf.LerpAngle(
                            eyeLocalRot.z,
                            -amount * _so._leanAngle * (hit.distance / targetEyeLocalPos.magnitude),
                            _so._leanAnimationSpeed * Time.deltaTime
                        );
                    }
                }
                else
                {
                    desiredEyePosThisFrame = Vector3.Lerp(
                        leanTransform.localPosition,
                        Vector3.zero,
                        _so._leanAnimationSpeed * Time.deltaTime
                    );
                }

                leanTransform.localPosition = desiredEyePosThisFrame;

                eyeLocalRot.z = desiredEyeRotThisFrame;
                leanTransform.localEulerAngles = eyeLocalRot;
            }
        }
    }
}