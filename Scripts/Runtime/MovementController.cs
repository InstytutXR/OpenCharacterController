using UnityEngine;

namespace ModularFirstPerson
{
    public abstract class MovementController : MonoBehaviour
    {
        public virtual void OnCapsuleCharacterCollision(Vector3 movement, in RaycastHit hit)
        {
        }
    }
}
