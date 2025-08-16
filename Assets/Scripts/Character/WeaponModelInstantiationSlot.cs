using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SweetClown
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;

        public void UnloadWeapon() 
        {
            if (currentWeaponModel != null) 
            {
                Destroy(currentWeaponModel);
            }
        }

        public void PlaceWeaponModelIntoSlot(GameObject weaponModel) 
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }

        public void PlaceWeaponModelInUnequippedSlot(GameObject weaponModel, WeaponClass weaponClass,PlayerManager player) 
        {
            //TODO, Move Weapon on back closer or More outward depening on chest equipment

            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            switch (weaponClass) 
            {
                case WeaponClass.StraightSword:
                    weaponModel.transform.localPosition = new Vector3(-0.09683679f, 0.2398416f, -0.2510437f);
                    weaponModel.transform.localRotation = Quaternion.Euler(26.387f, 56.684f, 5.85f);
                    break;
                case WeaponClass.NightSkySword:
                    weaponModel.transform.localPosition = new Vector3(-0.081f, 0.289f, -0.292f);
                    weaponModel.transform.localRotation = Quaternion.Euler(21.047f, 56.276f, -2.329f);
                    break;
            }
        }
    }
}
