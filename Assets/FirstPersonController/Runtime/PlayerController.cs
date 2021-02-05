using UnityEngine;

namespace FirstPersonController
{
    // NOTE: This code is all very rough and is just used to get
    // the basic features in place. I intend to do a lot of cleanup
    // in here, hence the very messy naming and organization.
    [RequireComponent(typeof(CapsuleBody))]
    public sealed class PlayerController : MonoBehaviour
    {
        private enum State
        {
            Walking,
            Running,
            Crouching,
        }

        private Transform _transform;
        private IPlayerControllerInput _input;
        private CapsuleBody _body;

        private State _state;
        private State _nextState;

        private bool _grounded;
        private RaycastHit _lastGroundHit;

        private float _verticalVelocity;

        // Control velocity based on movement input and the ground normal
        private Vector3 _controlVelocity;

        // Final computed velocity carried between frames for acceleration
        private Vector3 _velocity;

        private float _targetEyeHeight;

        [SerializeField]
        public float acceleration = 2f;

        [SerializeField]
        public float airDrag = 0.2f;

        [SerializeField]
        public float airControl = 20f;

        [SerializeField]
        private float _defaultColliderHeight = 1.7f;

        [SerializeField]
        private float _defaultEyeHeight = 1.6f;

        [SerializeField]
        private float _crouchColliderHeight = 0.9f;

        [SerializeField]
        private float _crouchEyeHeight = 0.8f;

        [SerializeField]
        private float _eyeHeightAnimationSpeed = 10f;

        [SerializeField]
        private Transform _eyeHeightTransform = default;

        [SerializeField]
        private float _cameraCollisionRadius = 0.2f;

        public float jumpHeight = 1.5f;

        public PlayerSpeed walkSpeed = new PlayerSpeed(2f, 1f, 0.95f);
        public PlayerSpeed runSpeed = new PlayerSpeed(4.5f, 0.9f, 0.6f);
        public PlayerSpeed crouchSpeed = new PlayerSpeed(0.8f, 1f, 1f);

        public void ResetHeight()
        {
            ChangeHeight(_defaultColliderHeight, _defaultEyeHeight);
        }

        public void ChangeHeight(float colliderHeight, float eyeHeight)
        {
            _body.height = colliderHeight;
            _targetEyeHeight = eyeHeight;
        }

        public bool CanStandUp()
        {
            return !_body.WouldCapsuleBeColliding(
                _body.position, 
                _defaultColliderHeight
            );
        }

        private void Start()
        {
            ResetHeight();

            _state = State.Walking;
            _nextState = State.Walking;
        }

        private void OnEnable()
        {
            _transform = transform;
            _input = GetComponent<IPlayerControllerInput>();
            _body = GetComponent<CapsuleBody>();
        }

        private void ChangeState(State nextState)
        {
            _nextState = nextState;
        }

        private void ApplyStateChange()
        {
            if (_nextState != _state)
            {
                _state = _nextState;

                switch (_state)
                {
                    case State.Crouching:
                        ChangeHeight(_crouchColliderHeight, _crouchEyeHeight);
                        break;
                    default:
                        ResetHeight();
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            ApplyStateChange();
            CheckForGround();
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;

            if (_grounded && _input.jump)
            {
                _verticalVelocity = Mathf.Sqrt(2f * jumpHeight * -Physics.gravity.y);
                _grounded = false;
            }

            ApplyUserInputMovement(); 

            _velocity = _body.MoveWithVelocity(
                _controlVelocity + 
                new Vector3(0, _verticalVelocity, 0)
            );

            AdjustEyeHeight();
        }

        private void ApplyUserInputMovement()
        {
            var movementRotation = Quaternion.Euler(0, _transform.eulerAngles.y, 0);
            if (_grounded)
            {
                var groundOrientation = Quaternion.FromToRotation(
                    Vector3.up,
                    _lastGroundHit.normal
                );
                movementRotation = groundOrientation * movementRotation;
            }

            var moveInput = _input.moveInput;

            switch (_state)
            {
                case State.Walking:
                    if (_input.run)
                    {
                        ChangeState(State.Running);
                    }
                    else if (_input.crouch)
                    {
                        ChangeState(State.Crouching);
                    }
                    break;
                case State.Running:
                    if (_input.crouch)
                    {
                        ChangeState(State.Crouching);
                    }
                    else if (!_input.run)
                    {
                        ChangeState(State.Walking);
                    }
                    break;
                case State.Crouching:
                    if (!_input.crouch && CanStandUp())
                    {
                        if (_input.run)
                        {
                            ChangeState(State.Running);
                        }
                        else
                        {
                            ChangeState(State.Walking);
                        }
                    }
                    break;
            }

            var moveVelocity = movementRotation * new Vector3(moveInput.x, 0, moveInput.y);

            PlayerSpeed speed = walkSpeed;
            switch (_state)
            {
                case State.Running:
                    speed = runSpeed;
                    break;
                case State.Crouching:
                    speed = crouchSpeed;
                    break;
            }

            var targetSpeed = Mathf.Lerp(
                _controlVelocity.magnitude, 
                speed.TargetSpeed(moveInput), 
                acceleration * Time.deltaTime
            );
            moveVelocity *= targetSpeed;

            if (_grounded)
            {
                // 100% control on ground
                _controlVelocity = moveVelocity;
            }
            else
            {
                if (moveVelocity.sqrMagnitude > 0)
                {
                    moveVelocity = Vector3.ProjectOnPlane(moveVelocity, Vector3.up);
                    _controlVelocity = Vector3.Lerp(
                        _controlVelocity, 
                        moveVelocity, 
                        airControl * Time.deltaTime
                    );
                }

                ApplyAirDrag();
            }
        }

        private void ApplyAirDrag()
        {
            _controlVelocity *= (1f / (1f + (airDrag * Time.fixedDeltaTime)));
        }

        private void CheckForGround()
        {
            var hitGround = _body.CheckForGround(
                _grounded,
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
            var groundedNow = hitGround && _verticalVelocity <= 0;
            var wasGrounded = _grounded;

            _grounded = groundedNow;

            if (!wasGrounded && groundedNow)
            {
                // TODO: OnBeginGrounded event
            }

            if (groundedNow)
            {
                _verticalVelocity = 0;

                // Reproject our control velocity onto the ground plane without losing magnitude
                var groundNormal = _lastGroundHit.normal;
                _controlVelocity = (_controlVelocity - groundNormal * Vector3.Dot(_controlVelocity, groundNormal)).normalized * _controlVelocity.magnitude;
            }
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
            if (!_grounded)
            {
                _body.Translate(new Vector3(0, oldHeight - eyeLocalPos.y, 0));
            }
        }
    }
}
