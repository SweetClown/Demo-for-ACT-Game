using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [System.Serializable]
    public class SerializablePoison : ISerializationCallbackReceiver
    {
        [SerializeField] public int itemID;

        public PoisonItem GetPoison()
        {
            PoisonItem poison = WorldItemDataBase.Instance.GetPosionFromSerializedData(this);
            return poison;
        }

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {

        }
    }
}