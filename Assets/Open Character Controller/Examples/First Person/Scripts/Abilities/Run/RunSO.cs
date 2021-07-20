using UnityEngine;
using OpenCharacterController;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Run")]
    public sealed class RunSO : PlayerAbilitySO
    {
        [SerializeField]
        private CharacterSpeed _speed = new CharacterSpeed(6f, 0.9f, 0.6f);
        public CharacterSpeed speed => _speed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Run(controller, this);
        }
    }
}