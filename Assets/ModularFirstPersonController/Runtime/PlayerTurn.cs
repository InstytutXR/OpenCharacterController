using UnityEngine;
using static UnityEngine.Mathf;

namespace ModularFirstPerson
{
    public sealed class PlayerTurn : MonoBehaviour
    {
        private Transform _transform = default;

        [SerializeField, Tooltip("The component that provides user input to process.")]
        private PlayerInputProvider _input = default;

        private void OnEnable()
        {
            _transform = transform;
        }

        private void Update()
        {
            var yaw = _transform.localEulerAngles.y;
            yaw = Repeat(yaw + _input.lookHorizontal, 360f);
            _transform.localEulerAngles = new Vector3(0, yaw, 0);
        }

        private void OnValidate()
        {
            if (_input == null)
            {
                _input = GetComponent<PlayerInputProvider>();
            }
        }
    }
}