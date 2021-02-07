using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public class PlayerAbilities
    {
        [SerializeReference]
        private List<PlayerAbility> _abilities = new List<PlayerAbility>
        {
            new SlideAbility(),
            new JumpAbility(),
            new RunAbility(),
            new LeanAbility(),
            new CrouchAbility(),
            new WalkAbility(),
        };

        public void Initialize(PlayerController controller)
        {
            foreach (var ability in _abilities)
            {
                ability.Initialize(controller);
            }
        }

        public void FixedUpdate()
        {
            foreach (var ability in _abilities)
            {
                ability.TryActivate();

                if (ability.isActive)
                {
                    ability.FixedUpdate();
                }

                // We do a second check of isActive before blocking
                // so that if an ability deactivates itself we allow
                // other abilities to trigger this frame.
                if (ability.isActive && ability.IsBlocking())
                {
                    break;
                }
            }
        }
    }
}