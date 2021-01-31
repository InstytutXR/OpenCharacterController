using UnityEngine;

namespace ModularFirstPerson
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