using UnityEngine;
using static UnityEngine.Mathf;

namespace FirstPersonController
{
    public sealed class PlayerLookUpDown : MonoBehaviour
    {
        private Transform _transform;
        private IPlayerLookUpDownInput _input;

        private float _pitch = 0f;

        [Tooltip("The minimum pitch value allowed, specified in degrees.")]
        public float minPitch = -85f;

        [Tooltip("The maximum pitch value allowed, specified in degrees.")]
        public float maxPitch = 85f;

        private void OnEnable()
        {
            _transform = transform;
            _input = GetComponentInParent<IPlayerLookUpDownInput>();
        }

        private void Update()
        {
            _pitch = Clamp(_pitch + _input.lookVertical, minPitch, maxPitch);
            _transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }
    }
}