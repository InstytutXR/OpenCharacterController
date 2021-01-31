using UnityEngine;
using static UnityEngine.Mathf;

namespace ModularFirstPerson
{
    public sealed class PlayerTurn : MonoBehaviour
    {
        [SerializeField, Tooltip("The component that provides user input to process.")]
        private PlayerInputProvider _input = default;

        [SerializeField, Tooltip("The transform that will be turned around the Y axis based on the player input.")]
        private Transform _targetTransform = default;

        private void Update()
        {
            var yaw = _targetTransform.localEulerAngles.y;
            yaw = Repeat(yaw + _input.lookHorizontal, 360f);
            _targetTransform.localEulerAngles = new Vector3(0, yaw, 0);
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