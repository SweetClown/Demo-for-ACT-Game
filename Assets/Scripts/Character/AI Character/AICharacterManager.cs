using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;

namespace SweetClown
{
    public class AICharacterManager : CharacterManager
    {
        [Header("Character Name")]
        public string characterName = "";

        [HideInInspector] public AICharacterCombatManager AICharacterCombatManager;
        [HideInInspector] public AICharacterNetworkManager AICharacterNetworkManager;
        [HideInInspector] public AICharacterLocomotionManager AICharacterLocomotionManager;
        [HideInInspector] public AICharacterInventoryManager AICharacterInventoryManager;


        [Header("Navmesh Agent")]
        public NavMeshAgent navMeshAgent;

        [Header("Current State")]
        public AIState currentState;

        [Header("States")]
        public AIIdleState idle;
        public PursueTargetState pursueTarget;
        public CombatStanceState combatStance;
        public AttackState attack;
        public InvestigateSoundState investigateSound;

        [Header("Activation Range")]
        protected AIActivationRange range;

        protected override void Awake()
        {
            base.Awake();

            AICharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
            AICharacterCombatManager = GetComponent<AICharacterCombatManager>();
            AICharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            AICharacterInventoryManager = GetComponent<AICharacterInventoryManager>();

            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        }

        protected override void Start()
        {
            base.Start();

            //If the animator or gameobject becomes disabled, we will keep our current animation when re-enabled
            //This is especially usefil for disabling enemies they are far away, and Re-Enabling them later keeping them in specific states (Like sleep, or dead)
            animator.keepAnimatorStateOnDisable = true;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                // Use a copy of the scriptable objects, so the originals are not modified
                idle = Instantiate(idle);
                pursueTarget = Instantiate(pursueTarget);
                combatStance = Instantiate(combatStance);
                attack = Instantiate(attack);
                investigateSound = Instantiate(investigateSound);
                currentState = idle;
            }

            AICharacterNetworkManager.currentHealth.OnValueChanged += AICharacterNetworkManager.CheckHP;
            AICharacterNetworkManager.isBlocking.OnValueChanged += AICharacterNetworkManager.OnIsBlockingChanged;

            if (!AICharacterNetworkManager.isAwake.Value)
                animator.Play(AICharacterNetworkManager.sleepingAnimation.Value.ToString());

            if (isDead.Value)
                animator.Play("Dead_01");

            CreateActivationRange();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            AICharacterNetworkManager.currentHealth.OnValueChanged -= AICharacterNetworkManager.CheckHP;
            AICharacterNetworkManager.isBlocking.OnValueChanged -= AICharacterNetworkManager.OnIsBlockingChanged;
        }

        protected override void Update()
        {
            base.Update();

            AICharacterCombatManager.HandleActionRecovery(this);

            if (navMeshAgent == null)
                return;

            if (IsOwner)
                ProcessStateMachine();

            if (!navMeshAgent.enabled)
                return;

            Vector3 positionDifference = navMeshAgent.transform.position - transform.position;

            if (positionDifference.magnitude > 0.2f)
                navMeshAgent.transform.localPosition = Vector3.zero;

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsOwner) 
            {
                ProcessStateMachine();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (characterUIManager.hasFloatingHPBar) 
                characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (characterUIManager.hasFloatingHPBar)
                characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHPChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (range != null)
                Destroy(range);
        }

        private void ProcessStateMachine()
        {
            AIState nextState = currentState?.Tick(this);

            if (nextState != null) 
            {
                currentState = nextState;
            }

            //The position /Rotation should be reset only after the state machine has processed it,s tick
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;

            if (AICharacterCombatManager.currentTarget != null) 
            {
                AICharacterCombatManager.targetsDirection = AICharacterCombatManager.currentTarget.transform.position - transform.position;
                AICharacterCombatManager.viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, AICharacterCombatManager.targetsDirection);
                AICharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, AICharacterCombatManager.currentTarget.transform.position);
            }

            if (navMeshAgent.enabled)
            {
                Vector3 agentDestination = navMeshAgent.destination;
                float remainingDistance = Vector3.Distance(agentDestination, transform.position);

                if (remainingDistance > navMeshAgent.stoppingDistance)
                {
                    AICharacterNetworkManager.isMoving.Value = true;
                }
                else
                {
                    AICharacterNetworkManager.isMoving.Value = false;
                }
            }
            else 
            {
                AICharacterNetworkManager.isMoving.Value = false;
            }

        }

        //Activation
        public void ActivateCharacter(PlayerManager player) 
        {
            AICharacterCombatManager.AddPlayerToPlayersWithinRange(player);

            //if (player.IsLocalPlayer) 
            //{

            //}

            if (!NetworkManager.Singleton.IsHost)
                return;

            if (AICharacterCombatManager.playersWithinActivationRange.Count > 0)
            {
                AICharacterNetworkManager.isActive.Value = true;
            }
            else 
            {
                AICharacterNetworkManager.isActive.Value = false;
            }

        }

        public void DeactivateCharacter(PlayerManager player)
        {
            AICharacterCombatManager.RemovePlayerToPlayersWithinRange(player);

            //if (player.IsLocalPlayer) 
            //{

            //}

            if (range != null)
            {
                range.gameObject.transform.position = transform.position;
                range.gameObject.SetActive(true);
            }

            if (!NetworkManager.Singleton.IsHost)
                return;

            if (AICharacterCombatManager.playersWithinActivationRange.Count > 0)
            {
                AICharacterNetworkManager.isActive.Value = true;
            }
            else 
            {
                AICharacterCombatManager.SetTarget(null);
                AICharacterNetworkManager.isActive.Value = false;
            }
        }

        public void CreateActivationRange() 
        {
            if (range == null)
            {
                GameObject rangeGameObject = Instantiate(WorldAIManager.instance.rangeGameObject);
                rangeGameObject.transform.position = transform.position;

                range = rangeGameObject.GetComponent<AIActivationRange>();
                range.SetOwnerOfRange(this);
            }
            else 
            {
                range.transform.position = transform.position;
                range.gameObject.SetActive(true);
            }
        }
    }
}
