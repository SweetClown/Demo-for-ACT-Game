using SweetClown;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Timeline;
using UnityEngine;
using Unity.Netcode;

namespace SweetClown
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;

        [Header("Damage")]
        [SerializeField] protected int baseDamage = 25;
        [SerializeField] protected int basePoiseDamage = 25;

        [Header("Recovery Timer")]
        public float actionRecoveryTimer = 0;

        [Header("Pivot")]
        public bool enablePivot = true;

        [Header("Target Information")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        public float minimumFOV = -35;
        public float maximumFOV = 35;

        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25;

        [Header("Stance Setting")]
        public float maxStance = 150;
        public float currentStance = 150;
        [SerializeField] float stanceRegeneratedPerSecond = 15;
        [SerializeField] bool ignoreStanceBreak = false;

        [Header("Stance Timer")]
        [SerializeField] float stanceRegenerationTimer = 0;
        [SerializeField] float stanceTickTimer = 0;
        [SerializeField] float defaultTimeUntilStanceRegenerationBegins = 3;

        [Header("Activation Range")]
        public List<PlayerManager> playersWithinActivationRange = new List<PlayerManager>();


        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }

        private void Update()
        {
            HandleStanceBreak();
        }

        public void AddPlayerToPlayersWithinRange(PlayerManager player) 
        {
            if (playersWithinActivationRange.Contains(player))
                return;

            playersWithinActivationRange.Add(player);

            for (int i = 0; i < playersWithinActivationRange.Count; i++) 
            {
                if (playersWithinActivationRange[i] == null)
                    playersWithinActivationRange.RemoveAt(i);
            }
        }

        public void RemovePlayerToPlayersWithinRange(PlayerManager player)
        {
            if (!playersWithinActivationRange.Contains(player))
                return;

            playersWithinActivationRange.Remove(player);

            for (int i = 0; i < playersWithinActivationRange.Count; i++)
            {
                if (playersWithinActivationRange[i] == null)
                    playersWithinActivationRange.RemoveAt(i);
            }
        }

        public void AwardRunesOnDeath(PlayerManager player) 
        {
            //Check if player is friendly to host
            if (player.characterGroup == CharacterGroup.Team02)
                return;

            //If want to give less or more runes to a client vs a host
            //if (NetworkManager.Singleton.IsHost) 
            //{

            //}

            //Award Runes
            player.playerStatsManager.AddRunes(aiCharacter.characterStatsManager.runesDroppedOnDeath);
        }

        private void HandleStanceBreak() 
        {
            if (!aiCharacter.IsOwner)
                return;

            if (aiCharacter.isDead.Value)
                return;

            if (stanceRegenerationTimer > 0)
            {
                stanceRegenerationTimer -= Time.deltaTime;
            }
            else 
            {
                stanceRegenerationTimer = 0;

                if (currentStance < maxStance)
                {
                    //Begin adding stance each tick
                    stanceTickTimer += Time.deltaTime;

                    if (stanceTickTimer >= 1)
                    {
                        stanceTickTimer = 0;
                        currentStance += stanceRegeneratedPerSecond;
                    }
                }
                else 
                {
                    currentStance = maxStance;
                }
            }

            if (currentStance <= 0) 
            {
                //Avoid The very high intensity damage animation get cover it(do not play the stance break animation)
                DamageIntensity previousDamageIntensity = WorldUtilityManager.Instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDamageTaken);

                if (previousDamageIntensity == DamageIntensity.Colossal) 
                {
                    currentStance = 1;
                    return;
                }

                currentStance = maxStance;

                if (ignoreStanceBreak)
                    return;

                aiCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly("Stance_Break_01", true);
            }
        }

        public void DamageStance(int stanceDamage) 
        {
            //When stance is damaged, the timer is reset to default stance timer, meaning constant attacks give no chance at recovering stance that is lost
            stanceRegenerationTimer = defaultTimeUntilStanceRegenerationBegins;

            currentStance -= stanceDamage;
        }

        public virtual void AlertCharacterToSound(Vector3 positionOfSound) 
        {
            if (!aiCharacter.IsOwner)
                return;

            if (aiCharacter.isDead.Value)
                return;

            if (aiCharacter.idle == null)
                return;

            if (aiCharacter.investigateSound == null)
                return;

            if (!aiCharacter.idle.willInvestigateSound)
                return;

            if (aiCharacter.idle.idleStateMode == IdleStateMode.Sleep && !aiCharacter.AICharacterNetworkManager.isAwake.Value) 
            {
                aiCharacter.AICharacterNetworkManager.isAwake.Value = true;
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(aiCharacter.AICharacterNetworkManager.wakingAnimation.Value.ToString(), true);
            }


            aiCharacter.investigateSound.positionOfSound = positionOfSound;
            aiCharacter.currentState = aiCharacter.currentState.SwitchState(aiCharacter, aiCharacter.investigateSound);
        }

        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, 
                                                            WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++) 
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter == null)
                    continue;

                if (targetCharacter == aiCharacter)
                    continue;

                if (targetCharacter.isDead.Value)
                    continue;

                if (WorldUtilityManager.Instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup)) 
                {
                    //If a potential target is found, it has to be in front of us
                    Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV) 
                    {
                        //Lastly, we check for enviro blocks
                        if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position,
                                                targetCharacter.characterCombatManager.lockOnTransform.position, 
                                                WorldUtilityManager.Instance.GetEnviroLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("Blocked");
                        }
                        else 
                        {
                            targetDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                            if (enablePivot)
                                PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }

        }

        public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            //Play a pivot animation depending on viewable angle of target
            if (aiCharacter.isPerformingAction)
                return;

            if (viewableAngle >= 20 && viewableAngle <= 60) 
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_45", true);
            }
            else if (viewableAngle >= -20 && viewableAngle <= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_45", true);
            }
            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_90", true);
            }
            else if (viewableAngle >= -61 && viewableAngle <= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_90", true);
            }
            else if (viewableAngle >= 111 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_90", true);
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_45", true);
            }
            else if (viewableAngle >= -111 && viewableAngle <= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_90", true);
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_45", true);
            }
            else if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_180", true);
            }
            else if (viewableAngle >= -146 && viewableAngle <= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_180", true);
            }

        }

        public virtual void PivotTowardsPosition(AICharacterManager aiCharacter, Vector3 position)
        {
            //Play a pivot animation depending on viewable angle of target
            if (aiCharacter.isPerformingAction)
                return;

            Vector3 targetsDirection = position - aiCharacter.transform.position;
            float viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(aiCharacter.transform, targetsDirection);

            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_45", true);
            }
            else if (viewableAngle >= -20 && viewableAngle <= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_45", true);
            }
            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_90", true);
            }
            else if (viewableAngle >= -61 && viewableAngle <= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_90", true);
            }
            else if (viewableAngle >= 111 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_90", true);
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_45", true);
            }
            else if (viewableAngle >= -111 && viewableAngle <= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_90", true);
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_45", true);
            }
            else if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Right_Turn_180", true);
            }
            else if (viewableAngle >= -146 && viewableAngle <= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Left_Turn_180", true);
            }

        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter) 
        {
            if (aiCharacter.AICharacterNetworkManager.isMoving.Value) 
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;

            if (!aiCharacter.characterLocomotionManager.canRotate)
                return;

            if (!aiCharacter.isPerformingAction)
                return;

            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }

        public void HandleActionRecovery(AICharacterManager aiCharacter) 
        {
            if (actionRecoveryTimer > 0) 
            {
                if (!aiCharacter.isPerformingAction) 
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}
