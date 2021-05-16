using UnityEngine;

namespace FirstPersonController
{
    public interface IWalkIntent : IIntent
    {
        Vector2 moveDirection { get; }
    }
}