using UnityEngine;

namespace OpenCharacterController.Examples
{
    public interface ICrouchIntent : IIntent
    {
        bool wantsToStartCrouching { get; }
        bool wantsToStopCrouching { get; }
        Vector2 moveDirection { get; }
    }
}