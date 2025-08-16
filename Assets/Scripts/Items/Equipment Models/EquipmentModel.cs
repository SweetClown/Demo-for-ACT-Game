using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Equipment Model")]
    public class EquipmentModel : ScriptableObject
    {
        public EquipmentModelType equipmentModelType;
        public string maleEquipmentName;
        public string femaleEquipmentName;

        public void LoadModel(PlayerManager player, bool isMale) 
        {
            if (isMale)
            {
                LoadMaleModel(player);
            }
            else 
            {
                LoadFemaleModel(player);
            }
        }

        private void LoadMaleModel(PlayerManager player) 
        {
            switch (equipmentModelType) 
            {
                case EquipmentModelType.FullHelmet:
                    foreach (var model in player.playerEquipmentManager.maleHeadFullHelmets) 
                    {
                        if (model.gameObject.name == maleEquipmentName) 
                        {
                            model.gameObject.SetActive(true);
                            //model.gameObject.GetComponent<Renderer>().material = Instantiate(equipmentMaterial);
                        }
                    }
                    break;
                case EquipmentModelType.OpenHelmet:
                    break;
                case EquipmentModelType.Hood:
                    break;
                case EquipmentModelType.HelemetAcessorie:
                    break;
                case EquipmentModelType.FaceCover:
                    break;
                case EquipmentModelType.Torso:
                    break;
                case EquipmentModelType.Back:
                    break;
                case EquipmentModelType.RightShoulder:
                    break;
                case EquipmentModelType.RightUpperArm:
                    break;
                case EquipmentModelType.RightElbow:
                    break;
                case EquipmentModelType.RightLowerArm:
                    break;
                case EquipmentModelType.RightHand:
                    break;
                case EquipmentModelType.LeftShoulder:
                    break;
                case EquipmentModelType.LeftUpperArm:
                    break;
                case EquipmentModelType.LeftElbow:
                    break;
                case EquipmentModelType.LeftLowerArm:
                    break;
                case EquipmentModelType.LeftHand:
                    break;
                case EquipmentModelType.Hips:
                    break;
                case EquipmentModelType.HipsAttachment:
                    break;
                case EquipmentModelType.RightLeg:
                    break;
                case EquipmentModelType.RightKnee:
                    break;
                case EquipmentModelType.LeftLeg:
                    break;
                case EquipmentModelType.LeftKnee:
                    break;


            }
        }

        private void LoadFemaleModel(PlayerManager player) 
        {

        }
    }
}
