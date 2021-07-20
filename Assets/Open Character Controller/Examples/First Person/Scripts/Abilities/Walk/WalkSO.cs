using UnityEngine;
using OpenCharacterController;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Walk")]
    public sealed class WalkSO : PlayerAbilitySO
    {
        [SerializeField]
        private CharacterSpeed _speed = new CharacterSpeed(2f, 1f, 0.95f);
        public CharacterSpeed speed => _speed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Walk(controller, this);
        }
    }
}