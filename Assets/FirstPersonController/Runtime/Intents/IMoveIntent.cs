using UnityEngine;

namespace FirstPersonController
{
    public interface IMoveIntent : IIntent
    {
        Vector2 moveAmount { get; }
    }
}