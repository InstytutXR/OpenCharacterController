using UnityEngine;
using static UnityEngine.Mathf;

namespace FirstPersonController
{
    public sealed class PlayerLookUpDown : MonoBehaviour
    {
        private IFloatIntent _look;

        [SerializeField]
        private FloatIntentSO _intent = default;

        private float _pitch = 0f;

        [Tooltip("The minimum pitch value allowed, specified in degrees.")]
        public float minPitch = -85f;

        [Tooltip("The maximum pitch value allowed, specified in degrees.")]
        public float maxPitch = 85f;

        private void OnEnable()
        {
            _look = _intent.Create();
        }

        private void Update()
        {
            _pitch = Clamp(_pitch + _look.amount, minPitch, maxPitch);
            transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Handle reference changes to the intent in the editor during playmode
            // by recreating the intent.
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                _look = _intent.Create();
            }
        }
#endif
    }
}