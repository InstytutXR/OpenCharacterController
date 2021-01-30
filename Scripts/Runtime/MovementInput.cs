using UnityEngine;

namespace ModularFirstPerson
{
    public abstract class MovementInput : MonoBehaviour
    {
        public abstract Vector2 GetMovementInput();
    }
}
