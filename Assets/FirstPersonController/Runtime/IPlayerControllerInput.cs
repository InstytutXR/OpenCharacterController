using UnityEngine;

namespace FirstPersonController
{
    public interface IPlayerControllerInput
    {
        Vector2 moveInput { get; }
        Vector2 lookInput { get; }
        bool jump { get; }
        bool run { get; }
        bool crouch { get; }
        float lean { get; }
    }
}