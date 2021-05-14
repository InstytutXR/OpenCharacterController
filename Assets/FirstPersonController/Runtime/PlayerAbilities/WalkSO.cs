using UnityEngine;

namespace FirstPersonController
{
    [CreateAssetMenu(menuName = "First Person Controller/Abilities/Walk")]
    public sealed class WalkSO : PlayerAbilitySO
    {
        [SerializeField]
        private PlayerSpeed _speed = new PlayerSpeed(2f, 1f, 0.95f);

        public override PlayerAbility CreateAbility(
            IPlayerController controller,
            IPlayerControllerInput input
        )
        {
            return new Walk(controller, this);
        }

        private class Walk : PlayerAbility
        {
            private readonly IPlayerController _controller;
            private readonly WalkSO _so;

            public Walk(IPlayerController controller, WalkSO walkSO)
            {
                _controller = controller;
                _so = walkSO;
            }

            public override bool isBlocking => true;

            public override bool canActivate => _controller.canStandUp;

            public override void OnActivate()
            {
                _controller.ResetHeight();
            }

            public override void FixedUpdate()
            {
                _controller.ApplyUserInputMovement(_so._speed);
            }
        }
    }
}