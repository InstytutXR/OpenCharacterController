using UnityEngine;

namespace ModularFirstPerson
{
    internal static class UnityVectorExtensions
    {
        public static Vector3 WithoutY(this Vector3 v) => new Vector3(v.x, 0, v.z);
    }
}
