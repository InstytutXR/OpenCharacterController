using UnityEngine;

namespace OpenCharacterController.Examples
{
    public interface IRunIntent : IIntent
    {
        bool wantsToStartRunning { get; }
        bool wantsToStopRunning { get; }
        Vector2 moveDirection { get; }
    }
}