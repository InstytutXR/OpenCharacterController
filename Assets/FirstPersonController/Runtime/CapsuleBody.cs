using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FirstPersonController
{
    public sealed class CapsuleBody : MonoBehaviour
    {
        private readonly Collider[] _overlapColliders = new Collider[16];

        private Rigidbody _body;
        private CapsuleCollider _collider;
        private RaycastHit _lastGroundHit;

        [SerializeField, Tooltip("The total height of the character. The capsule height is this value minus the step height.")]
        private float _height = 1.7f;

        [SerializeField, Tooltip("The collision radius of the character. If this value is less than half the computed capsule height, this value is replaced by half the computed capsule height.")]
        private float _radius = 0.35f;

        [SerializeField, Tooltip("The amount the character can step up or down while retaining a grounded state.")]
        private float _stepHeight = 0.35f;

        [SerializeField, Tooltip("An extra value used when sweeping the capsule through the world to improve collision detection.")]
        private float _skinThickness = 0.05f;

        [SerializeField, Tooltip("Mask of all layers to use when raycasting for the ground.")]
        private LayerMask _groundMask = 1; // "Default" layer by default

        public Vector3 position => _body.position;

        public Bounds bounds
        {
            get
            {
                // In order to use this in the editor when hitting F to frame
                // the body, we can't utilize the Rigidbody or CapsuleCollider
                // as those are created only when the game starts.
                var diameter = Mathf.Min(_radius, _height - _stepHeight / 2f) * 2f;
                return new Bounds(
                    transform.position + new Vector3(0, _height / 2f, 0),
                    new Vector3(diameter, _height, diameter)
                );
            }
        }

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
            CreateRigidbody();
            CreateCollider();
        }

        private void CreateRigidbody()
        {
            _body = gameObject.AddComponent<Rigidbody>();
#if UNITY_EDITOR
            _body.hideFlags = HideFlags.HideInInspector;
#endif

            // Must be dynamic for collision checks to work
            _body.isKinematic = false;

            // Controller should handle gravity
            _body.useGravity = false;

            // Don't fall down, please
            _body.freezeRotation = true;

            // We're doing all collision checks ourselves so we don't want the
            // physics engine doing any collision detection/response.
            _body.detectCollisions = false;

            // Interpolate the body for smoother motion
            _body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void CreateCollider()
        {
            _collider = gameObject.AddComponent<CapsuleCollider>();
#if UNITY_EDITOR
            _collider.hideFlags = HideFlags.HideInInspector;
#endif

            ResizeCollider();
        }

        public Vector3 MoveWithVelocity(Vector3 velocity)
        {
            // NOTE: Capping iterations here to avoid any chance of an infinite
            // loop. I don't know what situations would cause us to make more
            // than 10 moves before zeroing out our distance but whatever.
            const int MaxIterations = 10;

            var deltaTime = Time.deltaTime;
            var originalPosition = position;
            var movement = velocity * deltaTime;

            for (
                int iteration = 0;
                iteration < MaxIterations && !Mathf.Approximately(movement.sqrMagnitude, 0);
                iteration++
            )
            {
                Sweep(ref movement);
            }

            return (position - originalPosition) / deltaTime;
        }

        private void Sweep(ref Vector3 movement)
        {
            var moveDirection = movement.normalized;

            // Shift the body back just slightly before sweeping forward. This
            // prevents us from slipping into geometry when our collision
            // geometry is exactly planar with an object.
            var skinMovement = moveDirection * _skinThickness;
            _body.position -= skinMovement;
            movement += skinMovement;

            var didCollide = _body.SweepTest(
                moveDirection,
                out var hit,
                movement.magnitude
            );

            if (didCollide)
            {
                // TODO: OnCollision event

                var allowedMovement = moveDirection * hit.distance;
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
        }

        public void Translate(Vector3 movement)
        {
            _body.MovePosition(position + movement);
        }

        public bool CheckForGround(
            bool stickToGround,
            out RaycastHit hit,
            out float verticalMovementApplied
         )
        {
            const float PaddingForFloatingPointErrors = 0.001f;
            var maximumDistance = _collider.center.y + PaddingForFloatingPointErrors;

            if (stickToGround)
            {
                maximumDistance += _stepHeight;
            }

            maximumDistance -= _collider.radius;

            var origin = position + _collider.center;
            var hitGround = Physics.SphereCast(
                origin,
                _collider.radius,
                Vector3.down,
                out hit,
                maximumDistance,
                _groundMask,
                QueryTriggerInteraction.Ignore
            );

            if (hitGround)
            {
                // We're using a sphere but really want it to act like a
                // cylinder. This bit of math tries to add to the distance to
                // treat the curve of the sphere as if it was a cylinder.
                var cylinderCorrection = hit.point.y - (origin.y - hit.distance - _collider.radius);
                verticalMovementApplied =
                    _collider.center.y -
                    hit.distance -
                    _collider.radius +
                    cylinderCorrection;

                // Raycasts are interesting here. We want to provide a
                // RaycastHit to the caller so they have the normal and other
                // information to work with. However because we do a SphereCast
                // above we might be hitting an edge of a platform. So what we
                // do here is do a single point raycast to gauge if we're over a
                // ledge or not.
                if (Physics.Raycast(
                    origin,
                    Vector3.down,
                    out hit,
                    maximumDistance,
                    _groundMask,
                    QueryTriggerInteraction.Ignore
                ))
                {
                    _lastGroundHit = hit;
                }
                else
                {
                    hit = _lastGroundHit;
                }

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
            var capsuleHeight = _height - _stepHeight;
            _collider.height = capsuleHeight;
            _collider.radius = Mathf.Min(_radius, _height / 2f);
            _collider.center = new Vector3(0, capsuleHeight / 2f + _stepHeight, 0);

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
                int numColliders = FindOverlappingColliders();
                if (numColliders <= 0)
                {
                    break;
                }

                if (FindShortestPenetration(numColliders, out Vector3 translation))
                {
                    Translate(translation);
                }
            }
        }

        private bool FindShortestPenetration(
            int numColliders,
            out Vector3 translation
        )
        {
            var shortestDistance = float.MaxValue;
            var foundShortest = false;
            translation = default;

            for (int index = 0; index < numColliders; index++)
            {
                var otherCollider = _overlapColliders[index];

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
                    position,
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
                        foundShortest = true;
                        translation = direction * distance;
                        shortestDistance = distance;
                    }
                }
            }

            return foundShortest;
        }

        private int FindOverlappingColliders()
        {
            var pointOffset = position + Vector3.up * (_collider.height - _collider.radius);
            var point0 = _collider.center + pointOffset;
            var point1 = _collider.center - pointOffset;

            return Physics.OverlapCapsuleNonAlloc(
              point0,
              point1,
              _collider.radius,
              _overlapColliders
            );
        }

        private void OnValidate()
        {
            _height = Mathf.Max(_height, 0);
            _stepHeight = Mathf.Clamp(_stepHeight, 0, _height);

            var capsuleHeight = _height - _stepHeight;
            _radius = Mathf.Clamp(_radius, 0, capsuleHeight / 2f);

            _skinThickness = Mathf.Max(_skinThickness, 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var capsuleHeight = _height - _stepHeight;
            var radius = Mathf.Min(_radius, capsuleHeight / 2f);
            var center = new Vector3(0, capsuleHeight / 2f + _stepHeight, 0);

            var offset = capsuleHeight / 2f - radius;
            var point0 = center + Vector3.up * offset;
            var point1 = center + Vector3.down * offset;

            using (new Handles.DrawingScope(transform.localToWorldMatrix))
            {
                // Draw the capsule that is our geometry shape for most collision detection
                Handles.color = Color.green;

                Handles.DrawWireDisc(point0, Vector3.up, radius);
                Handles.DrawWireDisc(point1, Vector3.down, radius);

                Handles.DrawWireArc(point0, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawWireArc(point1, Vector3.left, Vector3.back, 180, radius);
                Handles.DrawWireArc(point0, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawWireArc(point1, Vector3.back, Vector3.left, -180, radius);

                Handles.DrawLine(
                    center + new Vector3(0, offset, -radius),
                    center + new Vector3(0, -offset, -radius)
                );
                Handles.DrawLine(
                    center + new Vector3(0, offset, radius),
                    center + new Vector3(0, -offset, radius)
                );
                Handles.DrawLine(
                    center + new Vector3(-radius, offset, 0),
                    center + new Vector3(-radius, -offset, 0)
                );
                Handles.DrawLine(
                    center + new Vector3(radius, offset, 0),
                    center + new Vector3(radius, -offset, 0)
                );

                // Draw the cylinder bottom we simulate when doing ground checks
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
                Handles.DrawLine(
                    new Vector3(radius, 0, 0), 
                    new Vector3(radius, _stepHeight + radius, 0)
                );
                Handles.DrawLine(
                    new Vector3(-radius, 0, 0), 
                    new Vector3(-radius, _stepHeight + radius, 0)
                );
                Handles.DrawLine(
                    new Vector3(0, 0, radius), 
                    new Vector3(0, _stepHeight + radius, radius)
                );
                Handles.DrawLine(
                    new Vector3(0, 0, -radius), 
                    new Vector3(0, _stepHeight + radius, -radius)
                );
            }
        }
#endif
    }
}
