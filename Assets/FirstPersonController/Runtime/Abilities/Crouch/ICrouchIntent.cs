using UnityEngine;

namespace FirstPersonController
{
    public interface ICrouchIntent : IIntent
    {
        bool wantsToStartCrouching { get; }
        bool wantsToStopCrouching { get; }
        Vector2 moveDirection { get; }
    }
}