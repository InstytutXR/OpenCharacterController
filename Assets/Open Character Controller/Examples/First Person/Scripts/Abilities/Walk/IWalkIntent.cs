using UnityEngine;

namespace OpenCharacterController.Examples
{
    public interface IWalkIntent : IIntent
    {
        Vector2 moveDirection { get; }
    }
}