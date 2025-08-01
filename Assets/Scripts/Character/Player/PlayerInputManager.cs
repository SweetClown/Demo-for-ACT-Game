using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        public PlayerManager player;
        // 1. Find a way to read the values of a joy stick or wasd keys
        // 2. Move Character based on those
        PlayerControls playerControls;

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("Lock On Input")]
        [SerializeField] bool lockOn_Input;
        [SerializeField] bool lockOn_Left_Input;
        [SerializeField] bool lockOn_Right_Input;
        private Coroutine lockOnCoroutine;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movement_Input;
        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;

        [Header("Player Action Input")]
        [SerializeField] bool dodge_Input = false;
        [SerializeField] bool sprint_Input = false;
        [SerializeField] bool jump_Input = false;
        [SerializeField] bool switch_Right_Weapon_Input = false;
        [SerializeField] bool switch_Left_Weapon_Input = false;
        [SerializeField] bool switch_Quick_Slot_Input = false;
        [SerializeField] bool Interaction_Input = false;
        [SerializeField] bool use_Item_Input = false;

        [Header("Bumper Inputs")]
        [SerializeField] bool LeftClick_Input = false;
        [SerializeField] bool RKey_Input = false;

        [Header("Trigger Inputs")]
        [SerializeField] bool RightClick_Input = false;
        [SerializeField] bool Hold_RightClick_Input = false;

        [Header("Two Hand Inputs")]
        [SerializeField] bool Two_Hand_Input = false;
        [SerializeField] bool Two_Hand_Right_Weapon_Input = false;
        [SerializeField] bool Two_Hand_Left_Weapon_Input = false;


        [Header("QUED Inputs")]
        [SerializeField] private bool input_Que_Is_Active = false;
        [SerializeField] float default_Que_Input_Timer = 0.35f;
        [SerializeField] float que_Input_Timer = 0;
        [SerializeField] bool que_LeftClick_Input = false;
        [SerializeField] bool que_RightClick_Input = false;

        [Header("UI Inputs")]
        [SerializeField] bool openCharacterMenuInput = false;
        [SerializeField] bool closeMenuInput = false;

        [Header("Skill Inputs")]
        [SerializeField] bool CapsLock_Input = false;





        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }

        }

        private void Update()
        {
            HandleAllInputs();
        }
        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // If we are loading into our world scent, enable our player controls
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            // otherwise disable our player controls
            else
            {
                instance.enabled = false;

                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movement_Input = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

                // Actions
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
                playerControls.PlayerActions.SwitchQuickSlotItem.performed += i => switch_Quick_Slot_Input = true;
                playerControls.PlayerActions.Interaction.performed += i => Interaction_Input = true;
                playerControls.PlayerActions.X.performed += i => use_Item_Input = true;


                // Bumpers
                playerControls.PlayerActions.Attack.performed += i => LeftClick_Input = true;
                playerControls.PlayerActions.Defense.performed += i => RKey_Input = true;
                playerControls.PlayerActions.Defense.canceled += i => player.playerNetworkManager.isBlocking.Value = false ;

                //Triggers
                playerControls.PlayerActions.HeavyAttack.performed += i => RightClick_Input = true;
                playerControls.PlayerActions.HoldingHeavyAttack.performed += i => Hold_RightClick_Input = true;
                playerControls.PlayerActions.HoldingHeavyAttack.canceled += i => Hold_RightClick_Input = false;

                //Skills
                playerControls.PlayerActions.Skill.performed += i => CapsLock_Input = true;

                //Two Hand
                playerControls.PlayerActions.TwoHandWeapon.performed += i => Two_Hand_Input = true;
                playerControls.PlayerActions.TwoHandWeapon.canceled += i => Two_Hand_Input = false;

                playerControls.PlayerActions.TwoHandRightWeapon.performed += i => Two_Hand_Right_Weapon_Input = true;
                playerControls.PlayerActions.TwoHandRightWeapon.canceled += i => Two_Hand_Right_Weapon_Input = false;

                playerControls.PlayerActions.TwoHandLeftWeapon.performed += i => Two_Hand_Left_Weapon_Input = true;
                playerControls.PlayerActions.TwoHandLeftWeapon.canceled += i => Two_Hand_Left_Weapon_Input = false;

                //Lock On
                playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;

                //Holding the input , set the bool to true
                playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
                // Releasing the input, set the bool to false
                playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;

                //Qued Inputs
                playerControls.PlayerActions.QueueAttack.performed += i => QueInput(ref que_LeftClick_Input);
                playerControls.PlayerActions.QueueHeavyAttack.performed += i => QueInput(ref que_RightClick_Input);

                //UI Inputs
                playerControls.PlayerActions.Dodge.performed += i => closeMenuInput = true;
                playerControls.PlayerActions.OpenCharacterMenu.performed += i => openCharacterMenuInput = true;

            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            //If we destroy this object, unsubscribe from this event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void HandleAllInputs()
        {
            HandleUseItemInput();
            HandleLockOnInput();
            HandleCameraMovementInput();
            HandlePlayerMovementInput();
            HandleDodgeInput();
            HandleSprinting();
            HandleJumpInput();
            HandleLeftClickInput();
            HandleRKeyInput();
            HandleRightClickInput();
            HandleSkillInput();
            HandleChangeRightClickInput();
            HandleLockOnSwitchTargetInput();
            HandleSwitchLeftWeaponInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchQuickSlotInput();
            HandleQuedInputs();
            HandleInteractionInput();
            HandleTwoHandInput();
            HandleOpenCharacterMenuInput();
            HandleCloseUIInput();
        }

        private void HandleUseItemInput()
        {
            if (use_Item_Input) 
            {
                use_Item_Input = false;

                if (PlayerUIManager.instance.menuWindowIsOpen)
                    return;

                if (player.playerInventoryManager.currentQuickSlotItem != null) 
                {
                    player.playerInventoryManager.currentQuickSlotItem.AttemptToUseItem(player);

                    player.playerNetworkManager.NotifyServerOfQuickSlotItemActionServerRpc(NetworkManager.Singleton.LocalClientId, player.playerInventoryManager.currentQuickSlotItem.itemID);
                }
            }
        }
        private void HandleTwoHandInput() 
        {
            if (!Two_Hand_Input)
                return;

            if (Two_Hand_Right_Weapon_Input)
            {
                // If we are using the two hand input and pressing the right two hand button we want to stop the regular attack input
                LeftClick_Input = false;
                Two_Hand_Right_Weapon_Input = false;
                player.playerNetworkManager.isBlocking.Value = false;

                if (player.playerNetworkManager.isTwoHandingWeapon.Value)
                {
                    //If we are two handing a weapon already, change the is twohanding bool to false which triggers an "OnValueChanged" function, which un-twohands current weapon
                    //Which Un-TwoHands Current Weapon
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                    return;
                }
                else
                {
                    //If we are not already two handing, change the right two hand bool to true, which triggers an OnValueChanged function
                    //This function two hands the rightr weapon
                    player.playerNetworkManager.isTwoHandingRightWeapon.Value = true;
                    return;
                }
            }
            else if (Two_Hand_Left_Weapon_Input) 
            {
                // If we are using the two hand input and pressing the left two hand button we want to stop the regular attack input
                RKey_Input = false;
                Two_Hand_Left_Weapon_Input = false;
                player.playerNetworkManager.isBlocking.Value = false;

                if (player.playerNetworkManager.isTwoHandingWeapon.Value)
                {
                    //If we are two handing a weapon already, change the is twohanding bool to false which triggers an "OnValueChanged" function, which un-twohands current weapon
                    //Which Un-TwoHands Current Weapon
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                    return;
                }
                else
                {
                    //If we are not already two handing, change the left two hand bool to true, which triggers an OnValueChanged function
                    //This function two hands the rightr weapon
                    player.playerNetworkManager.isTwoHandingLeftWeapon.Value = true;
                    return;
                }
            }
        }

        private void HandleLockOnInput()
        {
            //Check For dead target
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false;
                    //Attempt to find new target

                    //This assures us that the coroutine never runs muiltple times overlapping itself
                    if (lockOnCoroutine != null)
                        StopCoroutine(lockOnCoroutine);

                    lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
                }
            }

            if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;
                PlayerCamera.instance.ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
                //Disable Lock On
                return;
            }

            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;

                // If we are aiming using ranged weapon return (do not allow lock whilst aiming)

                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
            }
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if (lockOn_Left_Input)
            {
                lockOn_Left_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }

            if (lockOn_Right_Input)
            {
                lockOn_Right_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }

        //Movement

        private void HandlePlayerMovementInput()
        {
            vertical_Input = movement_Input.y;
            horizontal_Input = movement_Input.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1f;
            }

            if (player == null)
            {
                return;
            }

            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
            }
            else 
            {
                player.playerNetworkManager.isMoving.Value = false;
            }

            //If we are not locked on, only use the move amount 
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }

            //If we are locked on pass the horizontal movement as well

        }

        private void HandleCameraMovementInput()
        {
            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        //Actions

        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;

                //If menu or ui window is open do nothing
                if (PlayerUIManager.instance.menuWindowIsOpen)
                    return;

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprinting()
        {
            if (sprint_Input)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;

                //If we have a ui window open, simplay return without doing anything
                if (PlayerUIManager.instance.menuWindowIsOpen)
                    return;

                //Attempt to perform jump
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleLeftClickInput()
        {
            if (Two_Hand_Input)
                return;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            if (LeftClick_Input)
            {
                LeftClick_Input = false;

                //Todo : if we have a ui window open, return and do nothing

                player.playerNetworkManager.SetCharacterActionHand(true);

                //Todo : if we are tow handing weapon, use the two handed action

                player.playerCombatManager.PerformWeaponBaseAction(player.playerInventoryManager.currentRightHandWeapon.oh_LeftClick_Action,
                                                                   player.playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleRKeyInput()
        {
            if (Two_Hand_Input)
                return;

            if (RKey_Input)
            {
                RKey_Input = false;

                //Todo : if we have a ui window open, return and do nothing

                player.playerNetworkManager.SetCharacterActionHand(false);

                //Todo : if we are tow handing weapon, use the two handed action

                player.playerCombatManager.PerformWeaponBaseAction(player.playerInventoryManager.currentLeftHandWeapon.oh_RKey_Action,
                                                                   player.playerInventoryManager.currentLeftHandWeapon);
            }
        }

        private void HandleRightClickInput()
        {

            if (RightClick_Input)
            {
                RightClick_Input = false;

                //Todo : if we have a ui window open, return and do nothing

                player.playerNetworkManager.SetCharacterActionHand(true);

                //Todo : if we are tow handing weapon, use the two handed action

                player.playerCombatManager.PerformWeaponBaseAction(player.playerInventoryManager.currentRightHandWeapon.oh_RightClick_Action,
                                                                   player.playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleSkillInput()
        {

            if (CapsLock_Input)
            {
                CapsLock_Input = false;

                WeaponItem weaponPerformingSkill = player.playerCombatManager.SelectWeaponToPerformSkill();

                //Todo : if we have a ui window open, return and do nothing

                player.playerNetworkManager.SetCharacterActionHand(true);

                //Todo : if we are tow handing weapon, use the two handed action

                weaponPerformingSkill.skillAction.AttemptToPerformAction(player);
            }
        }

        private void HandleChangeRightClickInput()
        {
            //We only want to check for a charge if we are in an action that requires it 
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = Hold_RightClick_Input;
                }
            }
        }

        private void HandleSwitchRightWeaponInput() 
        {
            if (switch_Right_Weapon_Input) 
            {
                switch_Right_Weapon_Input = false;
                player.playerEquipmentManager.SwitchRightWeapon();
            }
        }

        private void HandleSwitchLeftWeaponInput()
        {
            if (switch_Left_Weapon_Input)
            {
                switch_Left_Weapon_Input = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }

        private void HandleSwitchQuickSlotInput()
        {
            if (switch_Quick_Slot_Input)
            {
                switch_Quick_Slot_Input = false;
                player.playerEquipmentManager.SwitchQuickSlotsItem();
            }
        }

        private void HandleInteractionInput() 
        {
            if (Interaction_Input) 
            {
                Interaction_Input = false;

                player.playerInteractionManager.Interact();
            }
        }

        //Passing a reference means we pass a specific bool, and not the value of that bool (true or false)
        private void QueInput(ref bool que_Input) 
        {
            //Reset All Qued Inputs So only one can que at a time
            que_LeftClick_Input = false;
            que_RightClick_Input = false;

            if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value) 
            {
                que_Input = true;
                que_Input_Timer = default_Que_Input_Timer;
                input_Que_Is_Active = true;
            }
        }

        private void ProcessQuedInput() 
        {
            if (player.isDead.Value)
                return;

            if (que_LeftClick_Input)
                LeftClick_Input = true;

            if (que_RightClick_Input)
                RightClick_Input = true;
        }

        private void HandleQuedInputs() 
        {
            if (input_Que_Is_Active) 
            {
                //While the timer is above 0, keep attempting to press the input
                if (que_Input_Timer > 0)
                {
                    que_Input_Timer -= Time.deltaTime;
                    ProcessQuedInput();
                }
                else 
                {
                    //Reset All qued input
                    que_LeftClick_Input = false;
                    que_RightClick_Input = false;
                    input_Que_Is_Active = false;
                    que_Input_Timer = 0;
                }
            }
        }

        private void HandleOpenCharacterMenuInput() 
        {
            if (openCharacterMenuInput) 
            {
                openCharacterMenuInput = false;

                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
                PlayerUIManager.instance.CloseAllMenuWindows();
                PlayerUIManager.instance.playerUICharacterMenuManager.OpenMenu();
            }
        }

        private void HandleCloseUIInput() 
        {
            if (closeMenuInput) 
            {
                closeMenuInput = false;

                if (PlayerUIManager.instance.menuWindowIsOpen) 
                {
                    PlayerUIManager.instance.CloseAllMenuWindows();
                }
            }
        }
    }
}
