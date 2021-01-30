using UnityEngine;

namespace ModularFirstPerson
{
    public abstract class LookController : MonoBehaviour
    {
        public abstract void UpdateLook(ref float yaw, ref float pitch);
    }
}