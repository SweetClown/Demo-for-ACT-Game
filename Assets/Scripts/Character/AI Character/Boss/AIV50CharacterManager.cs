using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class AIV50CharacterManager : AIBossCharacterManager
    {
        public AIBossSoundFXManager V50soundFXManager;

        protected override void Awake()
        {
            base.Awake();
            V50soundFXManager = GetComponent<AIBossSoundFXManager>();
        }
    }
}
