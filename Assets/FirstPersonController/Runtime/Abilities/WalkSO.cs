using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Walk")]
    public sealed class WalkSO : PlayerAbilitySO
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);
        public PlayerSpeed speed => _speed;

        public override PlayerAbility CreateAbility(IPlayerController controller)
        {
            return new Walk(controller, this);
        }
    }
}