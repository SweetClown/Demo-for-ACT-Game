using SweetClown;
using UnityEngine;

public class AIActivationRange : MonoBehaviour
{
    [SerializeField] AICharacterManager RangeOwner;

    public void SetOwnerOfRange(AICharacterManager aiCharacter) 
    {
        RangeOwner = aiCharacter;
    }

    public void ReactivateAICharacter(PlayerManager player) 
    {
        if (RangeOwner == null)
            return;

        RangeOwner.ActivateCharacter(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        ActivationDetector detector = other.GetComponent<ActivationDetector>();

        if (detector == null)
            return;

        ReactivateAICharacter(detector.player);
    }
}
