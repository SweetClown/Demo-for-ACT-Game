using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance State")]
    public class CombatStanceState : AIState
    {
        [Header("Attacks")]
        public List<AICharacterAttackAction> AICharacterAttacks; //A list of all possible attacks this character can do
        public List<AICharacterAttackAction> potentialAttacks; //All attacks possible in this situatuion (based on angle, distance ect)
        public AICharacterAttackAction choosenAttack;
        public AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false; //If the character can perform a combo attack, after the initial attack
        [SerializeField] protected int chanceToPerformCombo = 25; //The chance of the character perform a combo on the next attack
        [SerializeField] bool hasRolledForComboChance = false; //If we have already rolled for the chance during this state


        [Header("Engagement Distance")]
        [SerializeField] public float maximumEngagementDistance = 5; //The distance we have to be away from the target before we enter the pursue target state

        [Header("Circling")]
        [SerializeField] bool willCircleTarget = false;
        private bool hasChoosenCirclePath = false;
        private float strafeMoveAmount;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            if (aiCharacter.AICharacterCombatManager.enablePivot) 
            {
                if (!aiCharacter.AICharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.AICharacterCombatManager.viewableAngle <= 30 || aiCharacter.AICharacterCombatManager.viewableAngle > 30)
                        aiCharacter.AICharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            //Rotate to face our target
            aiCharacter.AICharacterCombatManager.RotateTowardsAgent(aiCharacter);

            //If the target is no longer present, switch to Idle state
            if (aiCharacter.AICharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            if (willCircleTarget)
                SetCirclePath(aiCharacter);

            //If we do not have an attack, get one
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else 
            {
                aiCharacter.attack.currentAttack = choosenAttack;
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            //If we are outside of the combat engagement distance, switch to pursue target state
            if (aiCharacter.AICharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.AICharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aICharacter) 
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach (var potentialAttack in AICharacterAttacks) 
            {
                // If we are too close for this attack, check the next
                if (potentialAttack.minimumAttackDistance > aICharacter.AICharacterCombatManager.distanceFromTarget)
                    continue;

                //If we are too far for this attack, check the next
                if (potentialAttack.maximumAttackDistance < aICharacter.AICharacterCombatManager.distanceFromTarget)
                    continue;

                //If the target is outside minimum field of view for this attack, check the next
                if (potentialAttack.minimumAttackAngle > aICharacter.AICharacterCombatManager.viewableAngle)
                    continue;


                //If the target is outside maximum field of view for this attack, check the next attack
                if (potentialAttack.maximumAttackAngle < aICharacter.AICharacterCombatManager.viewableAngle)
                    continue;


                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
                return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks) 
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks) 
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight) 
                {
                    choosenAttack = attack;
                    previousAttack = choosenAttack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance) 
        {
            bool outcomeWillBePerformed = false;
            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage < outcomeChance)
                outcomeWillBePerformed = true;

            return outcomeWillBePerformed;
        }

        protected virtual void SetCirclePath(AICharacterManager aICharacter) 
        {
            if (Physics.CheckSphere(aICharacter.AICharacterCombatManager.lockOnTransform.position, aICharacter.characterController.radius + 0.25f, WorldUtilityManager.Instance.GetEnviroLayers())) 
            {
                //Stop Strafing /Circling because We were Hit Something, instand path towards Enemy
                //Use Abs incase its negative, to make it positive
                //This will make our character follow the navmesh agent and path towards the target
                Debug.Log("We are Colliding with something, ending strafe");
                aICharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, Mathf.Abs(strafeMoveAmount));
                return;
            }

            //Strafe
            Debug.Log("Start Strafe");
            aICharacter.characterAnimatorManager.SetAnimatorMovementParameters(strafeMoveAmount, 0);

            if (hasChoosenCirclePath)
                return;

            hasChoosenCirclePath = true;

            //Strafe left or Right
            int leftOrRightIndex = Random.Range(0, 100);

            if (leftOrRightIndex >= 50)
            {
                //Left
                strafeMoveAmount = -0.5f;
            }
            else 
            {
                //Right
                strafeMoveAmount = 0.5f;
            }
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;
            hasRolledForComboChance = false;
            hasChoosenCirclePath = false;
            strafeMoveAmount = 0;
        }


    }
}
