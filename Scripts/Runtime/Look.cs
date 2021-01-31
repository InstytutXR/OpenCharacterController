using UnityEngine;
using static UnityEngine.Mathf;

namespace ModularFirstPerson
{
    public sealed class Look : MonoBehaviour
    {
        private float _pitch = 0f;
        private float _yaw = 0f;

        [SerializeField, Tooltip("The component that provides user input to process.")]
        private PlayerInputProvider _input = default;

        [SerializeField, Tooltip("The transform that will be rotated based on the yaw/pitch values.")]
        private Transform _targetTransform = default;

        [Tooltip("The minimum pitch value allowed, specified in degrees.")]
        public float minPitch = -85f;

        [Tooltip("The maximum pitch value allowed, specified in degrees.")]
        public float maxPitch = 85f;

        private void Update()
        {
            _pitch = Clamp(_pitch + _input.lookVertical, minPitch, maxPitch);
            _yaw = Repeat(_yaw + _input.lookHorizontal, 360f);

            _targetTransform.localEulerAngles = new Vector3(_pitch, _yaw, 0);
        }

        private void OnValidate()
        {
            if (_input == null)
            {
                _input = GetComponent<PlayerInputProvider>();
            }

            if (!_targetTransform)
            {
                _targetTransform = transform;
            }
        }
    }
}