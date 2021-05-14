using UnityEngine;

namespace FirstPersonController
{
    public interface IPlayerController
    {
        float speed { get; }
        Vector3 groundNormal { get; }
        PhysicMaterial groundMaterial { get; }
        float cameraCollisionRadius { get; }
        Transform turnTransform { get; }
        Transform lookUpDownTransform { get; }
        Transform leanTransform { get; }
        bool canStandUp { get; }
        bool grounded { get; set; }
        float verticalVelocity { get; set; }
        Vector3 controlVelocity { get; set; }
        LayerMask layerMask { get; }
        void ResetHeight();
        void ChangeHeight(float colliderHeight, float eyeHeight);
        void ApplyUserInputMovement(PlayerSpeed playerSpeed);
        void ApplyAirDrag();
        Vector3 TransformDirection(Vector3 direction);
    }
}