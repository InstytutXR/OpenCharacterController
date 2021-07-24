using UnityEngine;

namespace OpenCharacterController.Examples
{
    public interface ILookIntent : IIntent
    {
        Vector2 lookAmount { get; }
    }
}