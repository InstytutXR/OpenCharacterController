using UnityEngine;

namespace ModularFirstPerson
{
    public abstract class PlayerInputProvider : MonoBehaviour
    {
        public abstract float lookHorizontal { get; }
        public abstract float lookVertical { get; }
        public abstract Vector2 moveInput { get; }
    }
}