using UnityEngine;
using OpenCharacterController;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Run")]
    public sealed class RunSO : PlayerAbilitySO
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(6f, 0.9f, 0.6f);
        public PlayerSpeed speed => _speed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Run(controller, this);
        }
    }
}