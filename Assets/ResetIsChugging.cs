using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SG
{
    public class ResetIsChugging : StateMachineBehaviour
    {
        PlayerManager player;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (player == null)
                player = animator.GetComponent<PlayerManager>();

            if (player == null)
                return;

            if (player.playerNetworkManager.isChugging.Value && player.IsOwner) 
            {
                PoisonItem currentPoison = player.playerInventoryManager.currentQuickSlotItem as PoisonItem;

                if (currentPoison.healthPoison)
                {
                    if (player.playerNetworkManager.remainingHealthPoison.Value <= 0)
                    {
                        player.playerAnimatorManager.PlayTargetActionAnimation(currentPoison.emptyPoisonAnimation, false, false, true, true, false);
                        player.playerNetworkManager.HideWeaponsServerRpc();
                    }
                }
                else 
                {
                    if (player.playerNetworkManager.remainingManaPoison.Value <= 0)
                    {
                        player.playerAnimatorManager.PlayTargetActionAnimation(currentPoison.emptyPoisonAnimation, false, false, true, true, false);
                        player.playerNetworkManager.HideWeaponsServerRpc();
                    }
                }
            }

            if (player.playerNetworkManager.isChugging.Value)
            {
                PoisonItem currentPoison = player.playerInventoryManager.currentQuickSlotItem as PoisonItem;

                if (currentPoison.healthPoison)
                {
                    if (player.playerNetworkManager.remainingHealthPoison.Value <= 0)
                    {
                        Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                        GameObject emptyPoison = Instantiate(currentPoison.emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponModel.transform);
                        player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
                    }
                }
                else
                {
                    if (player.playerNetworkManager.remainingManaPoison.Value <= 0)
                    {
                        Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                        GameObject emptyPoison = Instantiate(currentPoison.emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponModel.transform);
                        player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
                    }
                }
            }

            //Reset is chugging
            if (player.IsOwner)
                player.playerNetworkManager.isChugging.Value = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
