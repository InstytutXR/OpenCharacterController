using UnityEngine;

namespace OpenCharacterController.Examples
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Jump")]
    public sealed class JumpSO : PlayerAbilitySO
    {
        [SerializeField]
        private float _height = 1.5f;
        public float height => _height;

        public override PlayerAbility CreateAbility(IPlayerController playerController)
        {
            return new Jump(playerController, this);
        }
    }
}
