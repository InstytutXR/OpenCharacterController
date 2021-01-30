using UnityEngine;

namespace ModularFirstPerson
{
    public sealed class Look : MonoBehaviour
    {
        private float _pitch = 0f;
        private float _yaw = 0f;

        [SerializeField, Tooltip("The component that will provide the input used to look.")]
        private LookController _controller = default;

        [SerializeField, Tooltip("The transform that will be controlled by the input.")]
        private Transform _targetTransform = default;

        [Tooltip("The minimum pitch value allowed, specified in degrees.")]
        public float minPitch = -85f;

        [Tooltip("The maximum pitch value allowed, specified in degrees.")]
        public float maxPitch = 85f;

        private void Update()
        {
            _controller.UpdateLook(ref _yaw, ref _pitch);

            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            _yaw = Mathf.Repeat(_yaw, 360f);

            _targetTransform.localEulerAngles = new Vector3(_pitch, _yaw, 0);
        }

        private void OnValidate()
        {
            if (_controller == null)
            {
                _controller = GetComponent<LookController>();
            }

            if (!_targetTransform)
            {
                _targetTransform = transform;
            }
        }
    }
}