using UnityEngine;

namespace FirstPersonController
{
    public interface ILookIntent : IIntent
    {
        Vector2 lookAmount { get; }
    }
}