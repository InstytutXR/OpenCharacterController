using System;
using UnityEngine;

namespace FirstPersonController
{
    // NOTE: This code is all very rough and is just used to get
    // the basic features in place. I intend to do a lot of cleanup
    // in here, hence the very messy naming and organization.
    [RequireComponent(typeof(CapsuleBody))]
    public sealed class PlayerController : MonoBehaviour
    {
        private Transform _transform;
        private IPlayerControllerInput _input;
        private CapsuleBody _body;

        private RaycastHit _lastGroundHit;

        // Final computed velocity carried between frames for dynamic motion (e.g. sliding)
        private Vector3 _velocity;

        private float _targetEyeHeight;

        [SerializeField]
        private float _acceleration = 2f;

        [SerializeField]
        private float _airDrag = 0.2f;

        [SerializeField]
        private float _airControl = 20f;

        [SerializeField]
        private float _defaultColliderHeight = 1.7f;

        [SerializeField]
        private float _defaultEyeHeight = 1.6f;

        [SerializeField]
        private float _eyeHeightAnimationSpeed = 10f;

        [SerializeField]
        private Transform _eyeHeightTransform = default;

        [SerializeField]
        private Transform _leanTransform = default;

        [SerializeField]
        private float _cameraCollisionRadius = 0.2f;

        [SerializeField] 
        private PlayerAbilities _abilities = default;

        [NonSerialized]
        public bool grounded;
        [NonSerialized]
        public float verticalVelocity;

        // Control velocity based on movement input and the ground normal
        [NonSerialized]
        public Vector3 controlVelocity;

        public float speed => _velocity.magnitude;
        public Vector3 groundNormal => _lastGroundHit.normal;
        public PhysicMaterial groundMaterial => _lastGroundHit.collider.material;
        public float cameraCollisionRadius => _cameraCollisionRadius;
        public IPlayerControllerInput input => _input;
        public Transform leanTransform => _leanTransform;

        public bool canStandUp
        {
            get
            {
                // Use an ever-so-slightly smaller radius so that our capsule check
                // here doesn't factor in any walls we're touching.
                // TODO: Should we be using a sphere cast up instead of a full capsule check?
                var testRadius = _body.radius - 0.001f;
                var point0 = _body.position + new Vector3(0, _body.stepHeight + testRadius, 0);
                var point1 = _body.position + new Vector3(0, _defaultColliderHeight - testRadius, 0);
                return !Physics.CheckCapsule(point0, point1, testRadius, ~(1 << _body.gameObject.layer));
            }
        }

        public void ResetHeight()
        {
            ChangeHeight(_defaultColliderHeight, _defaultEyeHeight);
        }

        public void ChangeHeight(float colliderHeight, float eyeHeight)
        {
            _body.height = colliderHeight;
            _targetEyeHeight = eyeHeight;
        }

        public void ApplyUserInputMovement(PlayerSpeed playerSpeed)
        {
            var movementRotation = Quaternion.Euler(0, _transform.eulerAngles.y, 0);
            if (grounded)
            {
                var groundOrientation = Quaternion.FromToRotation(
                    Vector3.up,
                    _lastGroundHit.normal
                );
                movementRotation = groundOrientation * movementRotation;
            }

            var moveInput = _input.moveInput;
            var moveVelocity = movementRotation * new Vector3(moveInput.x, 0, moveInput.y);

            var targetSpeed = Mathf.Lerp(
                controlVelocity.magnitude,
                playerSpeed.TargetSpeed(moveInput),
                _acceleration * Time.deltaTime
            );
            moveVelocity *= targetSpeed;

            if (grounded)
            {
                // 100% control on ground
                controlVelocity = moveVelocity;
            }
            else
            {
                if (moveVelocity.sqrMagnitude > 0)
                {
                    moveVelocity = Vector3.ProjectOnPlane(moveVelocity, Vector3.up);
                    controlVelocity = Vector3.Lerp(
                        controlVelocity,
                        moveVelocity,
                        _airControl * Time.deltaTime
                    );
                }

                ApplyAirDrag();
            }
        }

        public void ApplyAirDrag()
        {
            controlVelocity *= (1f / (1f + (_airDrag * Time.fixedDeltaTime)));
        }

        private void Start()
        {
            _abilities.Initialize(this);
            
            ResetHeight();
        }

        private void OnEnable()
        {
            _transform = transform;
            _input = GetComponent<IPlayerControllerInput>();
            _body = GetComponent<CapsuleBody>();
        }

        private void FixedUpdate()
        {
            CheckForGround();
            AddGravity();
            _abilities.FixedUpdate();
            ApplyVelocityToBody();
            AdjustEyeHeight();
        }

        private void CheckForGround()
        {
            var hitGround = _body.CheckForGround(
                grounded,
                out _lastGroundHit,
                out var verticalMovementApplied
            );

            // Whenever we hit ground we adjust our eye local coordinate space
            // in the opposite direction of the ground code so that our eyes
            // stay at the same world position and then interpolate where they
            // should be in AdjustEyeHeight later on.
            if (hitGround)
            {
                var eyeLocalPos = _eyeHeightTransform.localPosition;
                eyeLocalPos.y -= verticalMovementApplied;
                _eyeHeightTransform.localPosition = eyeLocalPos;
            }

            // Only grounded if the body detected ground AND we're not moving upwards
            var groundedNow = hitGround && verticalVelocity <= 0;
            var wasGrounded = grounded;

            grounded = groundedNow;

            if (!wasGrounded && groundedNow)
            {
                // TODO: OnBeginGrounded event
            }

            if (groundedNow)
            {
                verticalVelocity = 0;

                // Reproject our control velocity onto the ground plane without losing magnitude
                controlVelocity = (controlVelocity - groundNormal * Vector3.Dot(controlVelocity, groundNormal)).normalized * controlVelocity.magnitude;
            }
        }

        private void AddGravity()
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        private void ApplyVelocityToBody()
        {
            _velocity = controlVelocity;
            _velocity.y += verticalVelocity;
            _velocity = _body.MoveWithVelocity(_velocity);
        }

        private void AdjustEyeHeight()
        {
            var eyeLocalPos = _eyeHeightTransform.localPosition;
            var oldHeight = eyeLocalPos.y;

            // If we want to raise the eyes we check to see if there's something
            // above us. If we hit something, we clamp our eye position. Once
            // we're out from under whatever it is then we will fall through and
            // animate standing up.
            bool didCollide = Physics.SphereCast(
                new Ray(_body.position, Vector3.up),
                _cameraCollisionRadius,
                out var hit,
                _targetEyeHeight,
                ~(1 << gameObject.layer)
            );

            if (didCollide && oldHeight > hit.distance)
            {
                eyeLocalPos.y = hit.distance;
            }
            else
            {
                var remainingDistance = Mathf.Abs(oldHeight - _targetEyeHeight);
                if (remainingDistance < 0.01f)
                {
                    eyeLocalPos.y = _targetEyeHeight;
                }
                else
                {
                    // There's probably a better animation plan here than simple
                    // Lerp but for now it's reasonable.
                    eyeLocalPos.y = Mathf.Lerp(
                        oldHeight,
                        _targetEyeHeight,
                        _eyeHeightAnimationSpeed * Time.deltaTime
                    );
                }
            }

            _eyeHeightTransform.localPosition = eyeLocalPos;

            // If we're in the air we adjust the body relative to the eye height
            // to simulate raising the legs up. Otherwise crouching/uncrouching
            // midair feels super awkward.
            if (!grounded)
            {
                _body.Translate(new Vector3(0, oldHeight - eyeLocalPos.y, 0));
            }
        }
        
        private void OnGUI()
        {
            using (new GUILayout.AreaScope(new Rect(0, 0, 1000, 1000)))
            {
                bool isBlocked = false;
                
                foreach (var ability in _abilities)
                {
                    var abilityName = ability.GetType().Name;
                    if (ability.isActive)
                    {
                        abilityName += "*";
                    }
                    if (isBlocked)
                    {
                        abilityName += "!";
                    }
                    
                    GUILayout.Label(abilityName);

                    if (ability.isActive && ability.isBlocking)
                    {
                        isBlocked = true;
                    }
                }
            }
        }
    }
}
