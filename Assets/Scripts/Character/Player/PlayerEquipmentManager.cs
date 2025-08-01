using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        [Header("Weapon Model Instantiation Slots")]
        public WeaponModelInstantiationSlot rightHandWeaponSlot;
        public WeaponModelInstantiationSlot leftHandWeaponSlot;
        public WeaponModelInstantiationSlot leftHandShieldSlot;
        public WeaponModelInstantiationSlot backSlot;

        [Header("Weapon Models")]
        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        [Header("Weapon Managers")]
        public WeaponManager rightWeaponManager;
        public WeaponManager leftWeaponManager;

        [Header("Male Equipment Models")]
        public GameObject maleFullHelmetObject;
        public GameObject[] maleHeadFullHelmets;
        public GameObject maleFullBodyObject;
        public GameObject[] maleBodyFullArmor;
        public GameObject maleFullLegObject;
        public GameObject[] maleLegFullArmor;
        public GameObject maleFullHandObject;
        public GameObject[] maleHandFullArmor;

        [Header("Debug")]
        [SerializeField] bool equipNewItems = false;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
            InitializeWeaponSlots();

            List<GameObject> maleFullHelmetsList = new List<GameObject>();
            List<GameObject> maleFullBodyArmorList = new List<GameObject>();
            List<GameObject> maleFullLegArmorList = new List<GameObject>();
            List<GameObject> maleFullHandArmorList = new List<GameObject>();

            //Head
            foreach (Transform child in maleFullHelmetObject.transform) 
            {
                maleFullHelmetsList.Add(child.gameObject);
            }

            maleHeadFullHelmets = maleFullHelmetsList.ToArray();

            //Body
            foreach (Transform child in maleFullBodyObject.transform)
            {
                maleFullBodyArmorList.Add(child.gameObject);
            }
            maleBodyFullArmor = maleFullBodyArmorList.ToArray();

            //Leg
            foreach (Transform child in maleFullLegObject.transform)
            {
                maleFullLegArmorList.Add(child.gameObject);
            }
            maleLegFullArmor = maleFullLegArmorList.ToArray();

            //Hand
            foreach (Transform child in maleFullBodyObject.transform)
            {
                maleFullHandArmorList.Add(child.gameObject);
            }
            maleHandFullArmor = maleFullHandArmorList.ToArray();
        }

        protected override void Start()
        {
            base.Start();

            LoadWeaponsOnBothHands();
        }

        private void Update()
        {
            if (equipNewItems) 
            {
                equipNewItems = false;
                EquipArmor();
            }
        }

        public void EquipArmor() 
        {

            LoadHeadEquipment(player.playerInventoryManager.headEquipment);
            LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);
            LoadLegEquipment(player.playerInventoryManager.legEquipment);
            LoadHandEquipment(player.playerInventoryManager.handEquipment);
        }

        //Quick Slots

        public void SwitchQuickSlotsItem()
        {
            if (!player.IsOwner)
                return;

            QuickSlotItem selectedItem = null;

            // Add one to our index to switch to the next potential weapon
            player.playerInventoryManager.quickSlotItemIndex += 1;

            // If our index is out of bounds, reset it to position #1 (0)
            if (player.playerInventoryManager.quickSlotItemIndex < 0 || player.playerInventoryManager.quickSlotItemIndex > 2)
            {
                player.playerInventoryManager.quickSlotItemIndex = 0;

                //We check if we are holding more than one weapon
                float itemCount = 0;
                QuickSlotItem firstItem = null;
                int firstItemPosition = 0;

                for (int i = 0; i < player.playerInventoryManager.quickSlotItemsInQuickSlots.Length; i++)
                {
                    if (player.playerInventoryManager.quickSlotItemsInQuickSlots[i] != null)
                    {
                        itemCount += 1;

                        if (firstItem == null)
                        {
                            firstItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[i];
                            firstItemPosition = i;
                        }
                    }
                }

                if (itemCount <= 1)
                {
                    player.playerInventoryManager.quickSlotItemIndex = -1;
                    selectedItem = null;
                    player.playerNetworkManager.currentQuickSlotID.Value = -1;
                }
                else
                {
                    player.playerInventoryManager.quickSlotItemIndex = firstItemPosition;
                    player.playerNetworkManager.currentQuickSlotID.Value = firstItem.itemID;
                }

                return;
            }

            if (player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex] != null)
            {
                selectedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex];

                player.playerNetworkManager.currentQuickSlotID.Value =
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex].itemID;
            }
            else 
            {
                player.playerNetworkManager.currentQuickSlotID.Value = -1;
            }

            if (selectedItem == null && player.playerInventoryManager.quickSlotItemIndex <= 2) 
            {
                SwitchQuickSlotsItem();
            }

        }

        //Equipment
        public void LoadHeadEquipment(HeadEquipmentItem equipment) 
        {
            UnloadHeadEquipmentModels();

            if (equipment == null) 
            {
                if (player.IsOwner)
                    player.playerNetworkManager.headEquipmentID.Value = -1; // -1 will never be an item id, so it will be null

                player.playerInventoryManager.headEquipment = null;
                return;
            }

            player.playerInventoryManager.headEquipment = equipment;

            foreach (var model in equipment.equipmentModels) 
            {
                model.LoadModel(player, true);
            }

            player.playerStatsManager.CalculateTotalArmorAbsorption();

            if (player.IsOwner)
                player.playerNetworkManager.headEquipmentID.Value = equipment.itemID;
        }

        public void UnloadHeadEquipmentModels() 
        {
            foreach (var model in maleHeadFullHelmets) 
            {
                model.SetActive(false);
            }

            //Re-Enable Head
            //Re-Enable Hair
        }

        public void LoadBodyEquipment(BodyEquipmentItem equipment)
        {
            UnloadBodyEquipmentModels();

            if (equipment == null)
            {
                if (player.IsOwner)
                    player.playerNetworkManager.bodyEquipmentID.Value = -1; // -1 will never be an item id, so it will be null

                player.playerInventoryManager.bodyEquipment = null;
                return;
            }

            player.playerInventoryManager.bodyEquipment = equipment;

            foreach (var model in equipment.equipmentModels)
            {
                model.LoadModel(player, true);
            }

            player.playerStatsManager.CalculateTotalArmorAbsorption();

            if (player.IsOwner)
                player.playerNetworkManager.bodyEquipmentID.Value = equipment.itemID;
        }

        public void UnloadBodyEquipmentModels()
        {
            foreach (var model in maleBodyFullArmor)
            {
                model.SetActive(false);
            }

            //Re-Enable Head
            //Re-Enable Hair
        }

        public void LoadLegEquipment(LegEquipmentItem equipment)
        {
            UnloadLegEquipmentModels();

            if (equipment == null)
            {
                if (player.IsOwner)
                    player.playerNetworkManager.legEquipmentID.Value = -1; // -1 will never be an item id, so it will be null

                player.playerInventoryManager.legEquipment = null;
                return;
            }

            player.playerInventoryManager.legEquipment = equipment;

            foreach (var model in equipment.equipmentModels)
            {
                model.LoadModel(player, true);
            }

            player.playerStatsManager.CalculateTotalArmorAbsorption();

            if (player.IsOwner)
                player.playerNetworkManager.legEquipmentID.Value = equipment.itemID;
        }

        public void UnloadLegEquipmentModels()
        {
            foreach (var model in maleBodyFullArmor)
            {
                model.SetActive(false);
            }

            //Re-Enable Head
            //Re-Enable Hair
        }

        public void LoadHandEquipment(HandEquipmentItem equipment)
        {
            UnloadHandEquipmentModels();

            if (equipment == null)
            {
                if (player.IsOwner)
                    player.playerNetworkManager.handEquipmentID.Value = -1; // -1 will never be an item id, so it will be null

                player.playerInventoryManager.handEquipment = null;
                return;
            }

            player.playerInventoryManager.handEquipment = equipment;

            foreach (var model in equipment.equipmentModels)
            {
                model.LoadModel(player, true);
            }

            player.playerStatsManager.CalculateTotalArmorAbsorption();

            if (player.IsOwner)
                player.playerNetworkManager.handEquipmentID.Value = equipment.itemID;
        }

        public void LoadQuickSlotEquipment(QuickSlotItem equipment) 
        {
            if (equipment == null) 
            {
                if (player.IsOwner)
                    player.playerNetworkManager.currentQuickSlotID.Value = -1;

                player.playerInventoryManager.currentQuickSlotItem = null;
                return;
            }

            player.playerInventoryManager.currentQuickSlotItem = equipment;

            if (player.IsOwner)
                player.playerNetworkManager.currentQuickSlotID.Value = equipment.itemID;

        }

        public void UnloadHandEquipmentModels()
        {
            foreach (var model in maleBodyFullArmor)
            {
                model.SetActive(false);
            }

            //Re-Enable Head
            //Re-Enable Hair
        }
        private void InitializeWeaponSlots() 
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (var weaponSlot in weaponSlots) 
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandWeaponSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandWeaponSlot) 
                {
                    leftHandWeaponSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandShieldSlot)
                {
                    leftHandShieldSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.backSlot)
                {
                    backSlot = weaponSlot;
                }
            }


        }

        public void LoadWeaponsOnBothHands() 
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        //Right Weapon
        public void SwitchRightWeapon()
        {
            if (!player.IsOwner)
                return;

            player.playerNetworkManager.isTwoHandingWeapon.Value = false;

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // Add one to our index to switch to the next potential weapon
            player.playerInventoryManager.rightHandWeaponIndex += 1;

            // If our index is out of bounds, reset it
            if (player.playerInventoryManager.rightHandWeaponIndex < 0 ||
                player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                // Reset to check first slot
                player.playerInventoryManager.rightHandWeaponIndex = -1;

                // Count valid weapons and find first weapon
                int weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = -1;

                // Check all slots for valid weapons
                for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInRightHandSlots[i] != null &&
                        player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCount++;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                // If we have weapons, switch to the first one
                if (weaponCount > 0 && firstWeapon != null)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    player.playerInventoryManager.currentRightHandWeapon = firstWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }
                else
                {
                    // If no weapons found, switch to unarmed
                    selectedWeapon = WorldItemDataBase.Instance.unarmedWeapon;
                    player.playerInventoryManager.currentRightHandWeapon = selectedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                return;
            }

            // Try to select weapon at current index
            if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex] != null &&
                player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                player.playerInventoryManager.currentRightHandWeapon = selectedWeapon;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                return;
            }

            // If no weapon found at current index, try next slot
            if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightWeapon();
            }
        }

        public void LoadRightWeapon() 
        {
            if (player.playerInventoryManager.currentRightHandWeapon != null) 
            {
                if (rightHandWeaponSlot.currentWeaponModel != null)
                    rightHandWeaponSlot.UnloadWeapon();

                //Bring in the new weapon
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandWeaponSlot.PlaceWeaponModelIntoSlot(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player ,player.playerInventoryManager.currentRightHandWeapon);
                player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
                //Assign weapons damage, to its collider
            }
        }


        //Left Weapon
        public void SwitchLeftWeapon()
        {
            if (!player.IsOwner)
                return;

            player.playerNetworkManager.isTwoHandingWeapon.Value = false;

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // Add one to our index to switch to the next potential weapon
            player.playerInventoryManager.leftHandWeaponIndex += 1;

            // If our index is out of bounds, reset it
            if (player.playerInventoryManager.leftHandWeaponIndex < 0 ||
                player.playerInventoryManager.leftHandWeaponIndex > 2)
            {
                // Reset to check first slot
                player.playerInventoryManager.leftHandWeaponIndex = -1;

                // Count valid weapons and find first weapon
                int weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = -1;

                // Check all slots for valid weapons
                for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInLeftHandSlots[i] != null &&
                        player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCount++;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                // If we have weapons, switch to the first one
                if (weaponCount > 0 && firstWeapon != null)
                {
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
                    player.playerInventoryManager.currentLeftHandWeapon = firstWeapon;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
                }
                else
                {
                    // If no weapons found, switch to unarmed
                    selectedWeapon = WorldItemDataBase.Instance.unarmedWeapon;
                    player.playerInventoryManager.currentLeftHandWeapon = selectedWeapon;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
                }
                return;
            }

            // Try to select weapon at current index
            if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex] != null &&
                player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];
                player.playerInventoryManager.currentLeftHandWeapon = selectedWeapon;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
                return;
            }

            // If no weapon found at current index, try next slot
            if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
            {
                SwitchLeftWeapon();
            }
        }
        public void LoadLeftWeapon() 
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                //Remove the old weapon
                if (leftHandWeaponSlot.currentWeaponModel != null)
                    leftHandWeaponSlot.UnloadWeapon();

                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);

                switch (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType) 
                {
                    case WeaponModelType.Weapon:
                        leftHandWeaponSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);
                        break;
                    case WeaponModelType.Shield:
                        leftHandShieldSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);
                        break;
                    default: 
                        break;
                }

                //Bring in the new weapon
                leftHandWeaponSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
            }
        }

        //Two Hand
        public void UnTwoHandWeapon() 
        {
            //Update Animator Controller to current main hand weapon
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
            //Remove the strength Bonus (Two handing a weapon makes your strength level (strength + (strength * 0.5))
            //Un-Two Hand the model and move the model that isnt being Two Handed back to its hand (If there is any)

            //Left Hand
            if (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType == WeaponModelType.Weapon)
            {
                leftHandWeaponSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);
            }
            else if (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType == WeaponModelType.Shield) 
            {
                leftHandShieldSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);
            }

            //Right Hand
            rightHandWeaponSlot.PlaceWeaponModelIntoSlot(rightHandWeaponModel);

            //Refresh the damage collider calculations
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }

        public void TwoHandRightWeapon()
        {
            if (player.playerInventoryManager.currentRightHandWeapon == WorldItemDataBase.Instance.unarmedWeapon) 
            {
                if (player.IsOwner) 
                {
                    player.playerNetworkManager.isTwoHandingRightWeapon.Value = false;
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                }

                return;
            }

            //Update Animator
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);

            //Place the non-two handed weapon model in the back slot
            backSlot.PlaceWeaponModelInUnequippedSlot(leftHandWeaponModel, player.playerInventoryManager.currentLeftHandWeapon.weaponClass, player);

            //Place the two hand weapon model in the main hand (Right Hand)
            rightHandWeaponSlot.PlaceWeaponModelIntoSlot(rightHandWeaponModel);

            //Refresh the damage collider calculations
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }

        public void TwoHandLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon == WorldItemDataBase.Instance.unarmedWeapon)
            {
                if (player.IsOwner)
                {
                    player.playerNetworkManager.isTwoHandingLeftWeapon.Value = false;
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                }

                return;
            }

            //Update Animator
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentLeftHandWeapon.weaponAnimator);

            //Place the non-two handed weapon model in the back slot
            backSlot.PlaceWeaponModelInUnequippedSlot(rightHandWeaponModel, player.playerInventoryManager.currentRightHandWeapon.weaponClass, player);

            //Place the two hand weapon model in the main hand (Right Hand)
            rightHandWeaponSlot.PlaceWeaponModelIntoSlot(leftHandWeaponModel);

            //Refresh the damage collider calculations
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }

        //Damage Colliders
        public void OpenDamageCollider() 
        {
            //Open right weapon damage collider
            if (player.playerNetworkManager.isUsingRightHand.Value) 
            {
                rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
            }
            //Open left weapon damage collider
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
            }

            //Play sfx
        }

        public void CloseDamageCollider()
        {
            //Open right weapon damage collider
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            //Open left weapon damage collider
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }

        }

        //Unhide Weapons

        public void UnHideWeapons() 
        {
            if (player.playerEquipmentManager.rightHandWeaponModel != null)
            {
                player.playerEquipmentManager.rightHandWeaponModel.SetActive(true);
            }

            if (player.playerEquipmentManager.leftHandWeaponModel != null)
            {
                player.playerEquipmentManager.leftHandWeaponModel.SetActive(true);
            }
        }
    }
}
