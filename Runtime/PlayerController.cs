using UnityEngine;

namespace ModularFirstPerson
{
    public sealed class PlayerController : MonoBehaviour
    {
        private bool _grounded;
        private RaycastHit _lastGroundHit;

        private float _verticalVelocity;

        // Control velocity based on movement input and the ground normal
        private Vector3 _controlVelocity;

        // Final computed velocity carried between frames for acceleration
        private Vector3 _velocity;

        [SerializeField, Tooltip("The component responsible for providing input to the motor.")]
        private PlayerInputProvider _input;

        [SerializeField]
        private CapsuleBody _body;

        [SerializeField]
        public float acceleration = 2f;

        [SerializeField]
        public float airDrag = 0.2f;

        [SerializeField]
        public float airControl = 20f;

        public PlayerSpeed speed = new PlayerSpeed(2f, 1f, 0.95f);

        public Transform cameraForward;

        private void OnValidate()
        {
            if (!_input)
            {
                _input = GetComponent<PlayerInputProvider>();
            }

            if (!_body)
            {
                _body = GetComponent<CapsuleBody>();
            }
        }

        private void FixedUpdate()
        {
            CheckForGround();
            ApplyUserInputMovement();

            _velocity = _controlVelocity + new Vector3(0, _verticalVelocity, 0);
            _body.MoveWithVelocity(ref _velocity);
        }

        private void ApplyUserInputMovement()
        {
            var movementRotation = Quaternion.Euler(0, cameraForward.eulerAngles.y, 0);
            if (_grounded)
            {
                movementRotation = Quaternion.FromToRotation(Vector3.up, _lastGroundHit.normal) * movementRotation;
            }

            var moveInput = _input.moveInput;
            var moveVelocity = movementRotation * new Vector3(moveInput.x, 0, moveInput.y);
            var targetSpeed = Mathf.Lerp(_controlVelocity.magnitude, speed.TargetSpeed(moveInput), acceleration * Time.deltaTime);
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
                    _controlVelocity = Vector3.Lerp(_controlVelocity, moveVelocity, airControl * Time.deltaTime);
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
            var hitGround = _body.CheckForGround(_grounded, out _lastGroundHit, out var verticalMovementApplied);

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

    }
}
