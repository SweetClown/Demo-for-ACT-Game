using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;

namespace SweetClown
{
    public class WorldItemDataBase : MonoBehaviour
    {
        public static WorldItemDataBase Instance;

        public WeaponItem unarmedWeapon;

        public GameObject pickUpItemPrefab;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Head Equipment")]
        [SerializeField] List<HeadEquipmentItem> headEquipment = new List<HeadEquipmentItem>();

        [Header("Body Equipment")]
        [SerializeField] List<BodyEquipmentItem> bodyEquipment = new List<BodyEquipmentItem>();

        [Header("Leg Equipment")]
        [SerializeField] List<LegEquipmentItem> legEquipment = new List<LegEquipmentItem>();

        [Header("Hand Equipment")]
        [SerializeField] List<HandEquipmentItem> handEquipment = new List<HandEquipmentItem>();

        [Header("Skills")]
        [SerializeField] List<Skill> skill = new List<Skill>();

        [Header("Spells")]
        [SerializeField] List<SpellItem> spell = new List<SpellItem>();

        [Header("Quick Slot")]
        [SerializeField] List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();


        // A List of every item we have in the game
        [Header("Items")]
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Add all of our weapons to the list of items
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            foreach (var item in headEquipment)
            {
                items.Add(item);
            }

            foreach (var item in bodyEquipment)
            {
                items.Add(item);
            }

            foreach (var item in legEquipment)
            {
                items.Add(item);
            }

            foreach (var item in handEquipment)
            {
                items.Add(item);
            }

            foreach (var item in skill)
            {
                items.Add(item);
            }

            foreach (var item in spell)
            {
                items.Add(item);
            }

            foreach (var item in quickSlotItems)
            {
                items.Add(item);
            }
            // Assign all of our items a unique item id
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        //Item DataBase
        public Item GetItemByID(int ID)
        {
            return items.FirstOrDefault(item => item.itemID == ID);
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }

        public HeadEquipmentItem GetHeadEquipmentByID(int ID)
        {
            return headEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public BodyEquipmentItem GetBodyEquipmentByID(int ID)
        {
            return bodyEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public LegEquipmentItem GetLegEquipmentByID(int ID)
        {
            return legEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public HandEquipmentItem GetHandEquipmentByID(int ID)
        {
            return handEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
        }

        public Skill GetSkillByID(int ID)
        {
            return skill.FirstOrDefault(item => item.itemID == ID);
        }

        public SpellItem GetSpellByID(int ID)
        {
            return spell.FirstOrDefault(item => item.itemID == ID);
        }

        public QuickSlotItem GetQuickSlotItemByID(int ID)
        {
            return quickSlotItems.FirstOrDefault(item => item.itemID == ID);
        }

        //Item Serialization

        public WeaponItem GetWeaponFromSerializedData(SerializableWeapon serializableWeapon)
        {
            WeaponItem weapon = null;

            if (GetWeaponByID(serializableWeapon.itemID))
                weapon = Instantiate(GetWeaponByID(serializableWeapon.itemID));

            if (weapon == null)
                return Instantiate(unarmedWeapon);

            if (GetSkillByID(serializableWeapon.skillID))
            {
                Skill skill = Instantiate(GetSkillByID(serializableWeapon.skillID));
                weapon.skillAction = skill;
            }

            return weapon;
        }

        public PoisonItem GetPosionFromSerializedData(SerializablePoison serializablePoison)
        {
            PoisonItem poison = null;

            if (GetQuickSlotItemByID(serializablePoison.itemID))
            {
                poison = Instantiate(GetQuickSlotItemByID(serializablePoison.itemID)) as PoisonItem;
            }

            return poison;
        }

        public QuickSlotItem GetQuickSlotItemFromSerializedData(SerializableQuickSlotItem serializableQuickSlotItem)
        {
            QuickSlotItem quickSlotItem = null;

            if (GetQuickSlotItemByID(serializableQuickSlotItem.itemID))
            {
                quickSlotItem = Instantiate(GetQuickSlotItemByID(serializableQuickSlotItem.itemID)) as QuickSlotItem;
                quickSlotItem.itemAmount = serializableQuickSlotItem.itemAmount;
            }

            return quickSlotItem;
        }
    }
}
