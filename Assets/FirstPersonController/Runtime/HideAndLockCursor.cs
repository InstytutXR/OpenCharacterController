﻿using UnityEngine;

namespace FirstPersonController
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