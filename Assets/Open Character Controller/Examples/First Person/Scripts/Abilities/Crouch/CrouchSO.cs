using UnityEngine;
using OpenCharacterController;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Crouch")]
    public sealed class CrouchSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _colliderHeight = 0.9f;
        public float colliderHeight => _colliderHeight;

        [SerializeField]
        private float _eyeHeight = 0.8f;
        public float eyeHeight => _eyeHeight;

        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(0.8f, 1f, 1f);
        public PlayerSpeed speed => _speed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Crouch(controller, this);
        }
    }
}
