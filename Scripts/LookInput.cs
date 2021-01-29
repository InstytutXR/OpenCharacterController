using UnityEngine;

namespace ModularFirstPerson
{
    public abstract class LookInput : MonoBehaviour
    {
        public abstract Vector2 GetLookDelta();
    }
}