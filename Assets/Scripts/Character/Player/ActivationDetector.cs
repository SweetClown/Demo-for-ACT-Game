using UnityEngine;

namespace SweetClown {
    public class ActivationDetector : MonoBehaviour
    {
        public PlayerManager player;

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            AICharacterManager aiCharacter = other.GetComponent<AICharacterManager>();

            if (aiCharacter != null)
                aiCharacter.DeactivateCharacter(player);
        }
    }
}
