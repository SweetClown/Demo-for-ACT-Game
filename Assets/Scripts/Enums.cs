using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
   
}

public enum CharacterSlot 
{
    CharacterSlot_01,
    CharacterSlot_02,
    CharacterSlot_03,
    CharacterSlot_04,
    CharacterSlot_05,
    CharacterSlot_06,
    CharacterSlot_07,
    CharacterSlot_08,
    CharacterSlot_09,
    CharacterSlot_10,
    No_Slot
}

public enum CharacterGroup 
{
    Team01,
    Team02
}

public enum CharacterAttribute 
{
    Vigor,
    Mind,
    Endurance,
    Strength,
    Dexterity,
    Intelligence,
    Faith
}

public enum WeaponModelSlot 
{
    RightHand,
    LeftHandWeaponSlot,
    LeftHandShieldSlot,
    backSlot,
    //Right Hips
    //Left Hips
    //Back Hips
}

public enum WeaponModelType 
{
    Weapon,
    Shield
}

public enum WeaponClass 
{
    StraightSword,
    NightSkySword,
    RoundShield,
    Fist,
}

public enum SpellClass 
{
    Incantation,
    Sorcery
}

public enum EquipmentModelType 
{
    FullHelmet,
    OpenHelmet,
    Hood,
    HelemetAcessorie,
    FaceCover,
    Torso,
    Back,
    RightShoulder,
    RightUpperArm,
    RightElbow,
    RightLowerArm,
    RightHand,
    LeftShoulder,
    LeftUpperArm,
    LeftElbow,
    LeftLowerArm,
    LeftHand,
    Hips,
    HipsAttachment,
    RightLeg,
    RightKnee,
    LeftLeg,
    LeftKnee,
}

public enum EquipmentType
{
    RightWeapon01, //0
    RightWeapon02, //1
    RightWeapon03, //2
    LeftWeapon01, //3
    LeftWeapon02, //4 
    LeftWeapon03, //5
    Head,//6
    Body,//7
    Legs,//8
    Hands,//9
    QuickSlot01, //10
    QuickSlot02, //11
    QuickSlot03 //12

}

public enum AttackType 
{
    LightAttack01,
    LightAttack02,
    HeavyAttack01,
    HeavyAttack02,
    ChargedAttack01,
    ChargedAttack02,
    RunningAttack01,
    RollingAttack01,
    BackStepAttack01,
    LightJumpingAttack01,
    HeavyJumpingAttack01,
}

public enum DamageIntensity 
{
    Ping,
    Light,
    Medium,
    Heavy,
    Colossal
}

public enum ItemPickUpType 
{
    WorldSpawn,
    CharacterDrop,
}

public enum IdleStateMode 
{
    Idle,
    Patrol,
    Sleep
}
