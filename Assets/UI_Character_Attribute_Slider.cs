using SG;
using UnityEngine;

public class UI_Character_Attribute_Slider : MonoBehaviour
{
    [SerializeField] CharacterAttribute sliderAttribute;

    public void SetCurrentSelectedAttribute() 
    {
        PlayerUIManager.instance.playerUILevelUpManager.currentSelectedAttribute = sliderAttribute;
    }
}
