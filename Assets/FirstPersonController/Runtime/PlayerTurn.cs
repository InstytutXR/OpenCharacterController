using UnityEngine;
using static UnityEngine.Mathf;

namespace FirstPersonController
{
    public sealed class PlayerTurn : MonoBehaviour
    {
        private Transform _transform = default;
        private IPlayerTurnInput _input = default;

        private void OnEnable()
        {
            _transform = transform;
            _input = GetComponentInParent<IPlayerTurnInput>();
        }

        private void Update()
        {
            var yaw = _transform.localEulerAngles.y;
            yaw = Repeat(yaw + _input.lookHorizontal, 360f);
            _transform.localEulerAngles = new Vector3(0, yaw, 0);
        }
    }
}