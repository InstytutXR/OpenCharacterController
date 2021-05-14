using UnityEngine;
using static UnityEngine.Mathf;

namespace FirstPersonController
{
    public sealed class PlayerTurn : MonoBehaviour
    {
        private ITurnIntent _turn;

        [SerializeField]
        private TurnIntentSO _intent = default;

        private void OnEnable()
        {
            _turn = _intent.Create();
        }

        private void Update()
        {
            var yaw = transform.localEulerAngles.y;
            yaw = Repeat(yaw + _turn.amount, 360f);
            transform.localEulerAngles = new Vector3(0, yaw, 0);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Handle reference changes to the intent in the editor during playmode
            // by recreating the intent.
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                _turn = _intent.Create();
            }
        }
#endif
    }
}