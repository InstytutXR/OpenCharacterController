﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonController
{
    [Serializable]
    public class PlayerAbilities : IEnumerable<PlayerAbility>
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
            bool isBlocked = false;
            
            foreach (var ability in _abilities)
            {
                if (isBlocked)
                {
                    ability.Deactivate();
                }
                else
                {
                    ability.TryActivate();
                }

                if (ability.isActive || ability.updatesWhenNotActive)
                {
                    ability.FixedUpdate();
                }

                // We do a second check of isActive before blocking
                // so that if an ability deactivates itself we allow
                // other abilities to trigger this frame.
                if (ability.isActive && ability.isBlocking)
                {
                    isBlocked = true;
                }
            }
        }

        IEnumerator<PlayerAbility> IEnumerable<PlayerAbility>.GetEnumerator() => _abilities.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _abilities.GetEnumerator();
    }
}