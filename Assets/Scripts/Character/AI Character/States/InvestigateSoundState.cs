using UnityEngine;
using UnityEngine.AI;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "A.I/States/Investigate Sound")]
    public class InvestigateSoundState : AIState
    {
        [Header("Flags")]
        [SerializeField] bool destinationSet = false;
        [SerializeField] bool destinationReached = false;

        [Header("Position")]
        public Vector3 positionOfSound = Vector3.zero;

        [Header("Investigation Timer")]
        [SerializeField] float investigationTime = 3;
        [SerializeField] float investigationTimer = 0;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            aiCharacter.AICharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);

            if (aiCharacter.AICharacterCombatManager.currentTarget != null)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            if (!destinationSet) 
            {
                destinationSet = true;
                aiCharacter.AICharacterCombatManager.PivotTowardsPosition(aiCharacter, positionOfSound);
                aiCharacter.navMeshAgent.enabled = true;

                if (!IsDestinationReachable(aiCharacter, positionOfSound))
                {
                    NavMeshHit hit;

                    if (NavMesh.SamplePosition(positionOfSound, out hit, 2, NavMesh.AllAreas))
                    {
                        NavMeshPath partialPath = new NavMeshPath();
                        aiCharacter.navMeshAgent.CalculatePath(hit.position, partialPath);
                        aiCharacter.navMeshAgent.SetPath(partialPath);
                    }
                }
                else 
                {
                    NavMeshPath path = new NavMeshPath();
                    aiCharacter.navMeshAgent.CalculatePath(positionOfSound, path);
                    aiCharacter.navMeshAgent.SetPath(path);
                }
            }

            aiCharacter.AICharacterCombatManager.RotateTowardsAgent(aiCharacter);

            float distanceFormDestination = Vector3.Distance(aiCharacter.transform.position, positionOfSound);

            if (distanceFormDestination <= aiCharacter.navMeshAgent.stoppingDistance) 
            {
                destinationReached = true;
            }

            if (destinationReached) 
            {
                if (investigationTimer < investigationTime)
                {
                    investigationTimer += Time.deltaTime;
                }
                else 
                {
                    return SwitchState(aiCharacter, aiCharacter.idle);
                }
            }

            return this;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            aiCharacter.navMeshAgent.enabled = false;
            destinationSet = false;
            destinationReached = false;
            positionOfSound = Vector3.zero;
            investigationTimer = 0;
        }
    }
}
