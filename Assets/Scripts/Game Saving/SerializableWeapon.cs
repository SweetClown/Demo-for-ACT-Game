using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [System.Serializable]
    public class SerializableWeapon : ISerializationCallbackReceiver
    {
        [SerializeField] public int itemID;
        [SerializeField] public int skillID;

        public WeaponItem GetWeapon()
        {
            WeaponItem weapon = WorldItemDataBase.Instance.GetWeaponFromSerializedData(this);
            return weapon;
        }

        public void OnAfterDeserialize()
        {
            
        }

        public void OnBeforeSerialize()
        {

        }
    }
}