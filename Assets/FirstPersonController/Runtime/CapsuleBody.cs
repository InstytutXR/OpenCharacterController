using UnityEngine;

namespace FirstPersonController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class CapsuleBody : MonoBehaviour
    {
        private readonly Collider[] _overlapColliders = new Collider[16];

        private Rigidbody _body;
        private CapsuleCollider _collider;

        [SerializeField, Tooltip("The total height of the character. The capsule height is this value minus the step height.")]
        private float _height = 1.7f;

        [SerializeField, Tooltip("The collision radius of the character. If this value is less than half the computed capsule height, this value is replaced by half the computed capsule height.")]
        private float _radius = 0.35f;

        [SerializeField, Tooltip("The amount the character can step up or down while retaining a grounded state.")]
        private float _stepHeight = 0.35f;

        [Tooltip("Mask of all layers to use when raycasting for the ground.")]
        public LayerMask groundMask = 1; // "Default" layer by default

        [Tooltip("An extra value used when sweeping the capsule through the world to improve collision detection.")]
        public float skinThickness = 0.1f;

        public Bounds bounds =>
            new Bounds(
                _body.position + new Vector3(0, _height / 2f, 0),
                new Vector3(_collider.radius * 2f, _height, _collider.radius * 2f)
            );

        public float height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    var growing = value > _height;
                    _height = value;
                    ResizeCollider(growing);
                }
            }
        }

        public float radius
        {
            get => _radius;
            set
            {
                if (_radius != value)
                {
                    var growing = value > _radius;
                    _radius = value;
                    ResizeCollider(growing);
                }
            }
        }

        public float stepHeight
        {
            get => _stepHeight;
            set
            {
                if (_stepHeight != value)
                {
                    // Smaller step height means larger capsule
                    var growing = value < _stepHeight;
                    _stepHeight = value;
                    ResizeCollider(growing);
                }
            }
        }

        private void Start()
        {
            ResizeCollider();

            // Must be dynamic for collision checks to work
            _body.isKinematic = false;

            // Controller should handle gravity
            _body.useGravity = false;

            // Don't fall down, please
            _body.freezeRotation = true;

            // We're doing all collision checks ourselves so we don't want the
            // physics engine doing any collision detection/response.
            _body.detectCollisions = false;
        }

        public void MoveWithVelocity(ref Vector3 velocity)
        {
            // NOTE: Capping iterations here to avoid any chance of an infinite
            // loop. I don't know what situations would cause us to make more
            // than 10 moves before zeroing out our distance but whatever.
            const int MaxIterations = 10;

            var originalPosition = _body.position;
            var movement = velocity * Time.deltaTime;

            for (
                int iteration = 0;
                iteration < MaxIterations && !Mathf.Approximately(movement.sqrMagnitude, 0);
                iteration++
            )
            {
                var lateral = new Vector3(movement.x, 0, movement.z);
                var vertical = new Vector3(0, movement.y, 0);

                var remainingLateral = Sweep(lateral);
                var remainingVertical = Sweep(vertical);

                movement = remainingLateral + remainingVertical;
            }

            velocity = (_body.position - originalPosition) / Time.deltaTime;
        }

        private Vector3 Sweep(Vector3 movement)
        {
            // By default the SweepTest lets the player "sink" into walls by
            // running straight into them. The addition of this thickness
            // approach gives us more solid collisions.
            _body.position -= movement.normalized * skinThickness;
            movement += movement.normalized * skinThickness;

            var didCollide = _body.SweepTest(movement.normalized, out var hit, movement.magnitude);

            if (didCollide)
            {
                // TODO: OnCollision event

                var allowedMovement = movement.normalized * hit.distance;
                Translate(allowedMovement);
                movement -= allowedMovement;

                // Remove all movement opposite the normal of the surface we
                // collided with. This allows us to continue iterating to
                // "slide" players along a wall without constantly going back
                // into the wall.
                movement += hit.normal * Vector3.Dot(movement, -hit.normal);
            }
            else
            {
                Translate(movement);
                movement = Vector3.zero;
            }

            return movement;
        }

        public void Translate(Vector3 movement)
        {
            _body.MovePosition(_body.position + movement);
        }

        public bool CheckForGround(bool stickToGround, out RaycastHit hit, out float verticalMovementApplied)
        {
            const float paddingForFloatingPointErrors = 0.001f;
            var maximumDistance = _collider.center.y + paddingForFloatingPointErrors;

            if (stickToGround)
            {
                maximumDistance += _stepHeight;
            }

            var hitGround = Physics.Raycast(
                _body.position + _collider.center,
                Vector3.down,
                out hit,
                maximumDistance,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

            if (hitGround)
            {
                verticalMovementApplied = _collider.center.y - hit.distance;
                Translate(Vector3.up * verticalMovementApplied);
            }
            else
            {
                hit = default;
                verticalMovementApplied = default;
            }

            return hitGround;
        }

        private void ResizeCollider(bool growing = false)
        {
            _collider.height = _height - _stepHeight;
            _collider.radius = Mathf.Min(_radius, _height / 2f);
            var relativeCenterY = _collider.height * 0.5f + _stepHeight;
            _collider.center = new Vector3(0, relativeCenterY, 0);

            if (growing)
            {
                ResolveOverlaps();
            }
        }

        public bool CheckCapsule(Vector3 testPosition, float testHeight)
        {
            var capsuleHeight = testHeight - _stepHeight;
            var testRadius = Mathf.Min(_radius, capsuleHeight / 2f);
            var point0 = testPosition + new Vector3(0, _stepHeight + testRadius, 0);
            var point1 = testPosition + new Vector3(0, testHeight - testRadius, 0);
            return Physics.CheckCapsule(point0, point1, testRadius);
        }

        private void ResolveOverlaps()
        {
            // Iterative collision resolution to try and make the capsule not
            // colliding. If we fail, we simply fail and the capsule is left
            // colliding with the world.

            const int MaxIterations = 5;

            for (int iteration = 0; iteration < MaxIterations; iteration++)
            {
                var pointOffset = Vector3.up * (_collider.height - _collider.radius);
                var point0 = _collider.center + pointOffset;
                var point1 = _collider.center - pointOffset;

                var numColliders = Physics.OverlapCapsuleNonAlloc(
                  _body.position + point0,
                  _body.position + point1,
                  _collider.radius,
                  _overlapColliders
                );

                if (numColliders <= 0)
                {
                    break;
                }

                var shortestCollider = -1;
                var shortestDirection = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var shortestDistance = float.MaxValue;

                for (int colIndex = 0; colIndex < numColliders; colIndex++)
                {
                    var otherCollider = _overlapColliders[colIndex];

                    Vector3 otherPosition;
                    Quaternion otherRotation;
                    if (otherCollider.attachedRigidbody)
                    {
                        otherPosition = otherCollider.attachedRigidbody.position;
                        otherRotation = otherCollider.attachedRigidbody.rotation;
                    }
                    else
                    {
                        otherPosition = otherCollider.transform.position;
                        otherRotation = otherCollider.transform.rotation;
                    }

                    if (Physics.ComputePenetration(
                        _collider,
                        _body.position,
                        Quaternion.identity,
                        otherCollider,
                        otherPosition,
                        otherRotation,
                        out var direction,
                        out var distance
                    ))
                    {
                        if (distance < shortestDistance)
                        {
                            shortestCollider = colIndex;
                            shortestDistance = distance;
                            shortestDirection = direction;
                        }
                    }
                }

                if (shortestCollider >= 0)
                {
                    Translate(shortestDirection * shortestDistance);
                }
            }
        }

        private void OnValidate()
        {
            _body = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            ResizeCollider();
        }
    }
}
