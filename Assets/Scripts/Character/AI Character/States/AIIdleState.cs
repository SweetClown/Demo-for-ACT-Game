using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
    public class AIIdleState : AIState
    {
        [Header("Idle Options")]
        public IdleStateMode idleStateMode;

        [Header("Patrol Options")]
        public AIPatrolPath aiPatrolPath;
        [SerializeField] bool hasFoundClosetPointNearCharacterSpawn = false; //If the character spawns closer to the second point, start at the second point
        [SerializeField] bool patrolComplete = false; //We finished the entire patrol yet
        [SerializeField] bool repeatPatrol = false;  //UPON Finishing, Do we repeat the path again
        [SerializeField] int patrolDestinationIndex; //Which point of the patrol are we currently working towards
        [SerializeField] bool hasPatrolDestination = false; // Do we have apoint we are currently working towards
        [SerializeField] Vector3 currentPatrolDestination; //The Specific Destination coords we are heading towards
        [SerializeField] float distanceFromCurrentDestination; //The distance from the ai character to the destination
        [SerializeField] float timeBetweenPatrols = 15; //Minimum Time Before Starting a New Patrol
        [SerializeField] float resetTimer = 0; //Actuve Timer Counting the time rested

        [Header("Sleep Options")]
        public bool willInvestigateSound = true;
        private bool sleepAnimationSet = false;
        [SerializeField] string sleepAnimation = "Sleep_01";
        [SerializeField] string wakingAnimation = "Wake_01";
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.AICharacterNetworkManager.isAwake.Value)
                aiCharacter.AICharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);

            aiCharacter.AICharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);

            switch (idleStateMode)
            {
                case IdleStateMode.Idle:
                    return Idle(aiCharacter);
                case IdleStateMode.Patrol:
                    return Patrol(aiCharacter);
                case IdleStateMode.Sleep:
                    return SleepUntilDisturbed(aiCharacter);
                default: 
                    break;
            }

            return this;
        }

        protected virtual AIState Idle(AICharacterManager aiCharacter) 
        {
            if (aiCharacter.characterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }
            else
            {
                return this;
            }
        }

        protected virtual AIState Patrol(AICharacterManager aiCharacter)
        {
            if (!aiCharacter.AICharacterLocomotionManager.isGrounded)
                return this;

            if (aiCharacter.isPerformingAction)
            {
                aiCharacter.navMeshAgent.enabled = false;
                aiCharacter.characterNetworkManager.isMoving.Value = false;
                return this;
            }

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            if (aiCharacter.AICharacterCombatManager.currentTarget != null)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            //If our patrol is complete and we will repeat it check for rest time
            if (patrolComplete && repeatPatrol)
            {

                //If the time has not exceeded its set limit, stop and wait
                if (timeBetweenPatrols > resetTimer)
                {
                    aiCharacter.navMeshAgent.enabled = false;
                    aiCharacter.characterNetworkManager.isMoving.Value = false;
                    resetTimer += Time.deltaTime;
                }
                else
                {
                    patrolDestinationIndex = -1;
                    hasPatrolDestination = false;
                    currentPatrolDestination = aiCharacter.transform.position;
                    patrolComplete = false;
                    resetTimer = 0;
                }

            }
            else if (patrolComplete && !repeatPatrol) 
            {
                aiCharacter.navMeshAgent.enabled = false;
                aiCharacter.characterNetworkManager.isMoving.Value = false;
            }

            //If we have a destination, move towards it
            if (hasPatrolDestination)
            {
                distanceFromCurrentDestination = Vector3.Distance(aiCharacter.transform.position, currentPatrolDestination);

                if (distanceFromCurrentDestination > 2)
                {
                    aiCharacter.navMeshAgent.enabled = true;
                    aiCharacter.AICharacterLocomotionManager.RotateTowardsAgent(aiCharacter);
                }
                else
                {
                    currentPatrolDestination = aiCharacter.transform.position;
                    hasPatrolDestination = false;
                }
            }
            //Otherwise, get a new destination
            else 
            {
                patrolDestinationIndex += 1;

                if (patrolDestinationIndex > aiPatrolPath.patrolPoints.Count - 1) 
                {
                    patrolComplete = true;
                    return this;
                }

                if (!hasFoundClosetPointNearCharacterSpawn)
                {
                    hasFoundClosetPointNearCharacterSpawn = true;
                    float closestDistance = Mathf.Infinity;

                    for (int i = 0; i < aiPatrolPath.patrolPoints.Count; i++)
                    {
                        float distanceFromThisPoint = Vector3.Distance(aiCharacter.transform.position, aiPatrolPath.patrolPoints[i]);

                        if (distanceFromThisPoint < closestDistance)
                        {
                            closestDistance = distanceFromThisPoint;
                            patrolDestinationIndex = i;
                            currentPatrolDestination = aiPatrolPath.patrolPoints[i];
                        }
                    }
                }
                else 
                {
                    currentPatrolDestination = aiPatrolPath.patrolPoints[patrolDestinationIndex];
                }

                hasPatrolDestination = true;
            }

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(currentPatrolDestination, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual AIState SleepUntilDisturbed(AICharacterManager aiCharacter) 
        {
            aiCharacter.navMeshAgent.enabled = false;

            //If we haven.t set our sleep animation, and the character is sleeping set the animation now
            if (!sleepAnimationSet && !aiCharacter.AICharacterNetworkManager.isAwake.Value) 
            {
                sleepAnimationSet = true;
                aiCharacter.AICharacterNetworkManager.sleepingAnimation.Value = sleepAnimation;
                aiCharacter.AICharacterNetworkManager.wakingAnimation.Value = wakingAnimation;
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(aiCharacter.AICharacterNetworkManager.sleepingAnimation.Value.ToString(), true);
            }

            if (aiCharacter.characterCombatManager.currentTarget != null && !aiCharacter.AICharacterNetworkManager.isAwake.Value) 
            {
                aiCharacter.AICharacterNetworkManager.isAwake.Value = true;

                if (!aiCharacter.isPerformingAction && !aiCharacter.isDead.Value)
                    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(aiCharacter.AICharacterNetworkManager.wakingAnimation.Value.ToString(), true);

                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }

            return this;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            sleepAnimationSet = false;
        }
    }
}
