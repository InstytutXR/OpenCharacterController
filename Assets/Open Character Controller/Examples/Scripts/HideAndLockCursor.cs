using UnityEngine;

namespace OpenCharacterController.Examples
{
    public sealed class HideAndLockCursor : MonoBehaviour
    {
        private void Update()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}