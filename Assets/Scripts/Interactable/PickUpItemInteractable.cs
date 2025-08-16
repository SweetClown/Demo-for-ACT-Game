using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SweetClown
{
    public class PickUpItemInteractable : Interactable
    {
        public ItemPickUpType pickUpType;

        [Header("Item")]
        [SerializeField] Item item;

        [Header("Creature Loot Pick Up")]
        public NetworkVariable<int> itemID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<ulong> droppingCreatureID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public bool trackDroppingCreaturesPosition = true;

        [Header("World Spawn Pick Up")]
        [SerializeField] int worldSpawnInteractableID;
        [SerializeField] bool hasBeenLooted = false;

        [Header("Drop SFX")]
        [SerializeField] AudioClip itemDropSFX;
        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();

            audioSource = GetComponent<AudioSource>();
        }

        protected override void Start()
        {
            base.Start();

            
            if (pickUpType == ItemPickUpType.WorldSpawn)
                CheckIfWorldItemWasAlreadyLooted();

        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            itemID.OnValueChanged += OnItemIDChanged;
            networkPosition.OnValueChanged += OnNetworkPositionChanged;
            droppingCreatureID.OnValueChanged += OnDroppingCreaturesIDChanged;

            if (pickUpType == ItemPickUpType.CharacterDrop)
                audioSource.PlayOneShot(itemDropSFX);

            if (!IsOwner) 
            {
                OnItemIDChanged(0, itemID.Value);
                OnNetworkPositionChanged(Vector3.zero, networkPosition.Value);
                OnDroppingCreaturesIDChanged(0, droppingCreatureID.Value);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            itemID.OnValueChanged -= OnItemIDChanged;
            networkPosition.OnValueChanged -= OnNetworkPositionChanged;
            droppingCreatureID.OnValueChanged -= OnDroppingCreaturesIDChanged;
        }

        private void CheckIfWorldItemWasAlreadyLooted() 
        {
            //If the player not the host, hide the item
            if (!NetworkManager.Singleton.IsHost) 
            {
                gameObject.SetActive(false);
                return;
            }

            //Compare the data of looted item id with this item id
            if (!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldSpawnInteractableID)) 
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, false);
            }

            hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[worldSpawnInteractableID];
            //If it has been looted, hide the gameobject
            if (hasBeenLooted) 
            {
                gameObject.SetActive(false);
            }
        }

        public override void Interact(PlayerManager player)
        {
            if (player.isDead.Value)
                return;

            if (player.isPerformingAction)
                return;

            base.Interact(player);

            //Play sfx
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX);

            //Play An animation
            player.playerAnimatorManager.PlayTargetActionAnimation("Pick_Up_Item_01", true);

            //Add item to inventory
            player.playerInventoryManager.AddItemToInventory(item);

            //Display UI
            PlayerUIManager.instance.playerUIPopUpManager.SendItemPopUp(item, 1);
            //Save Loot status if it is a world spawn
            if (pickUpType == ItemPickUpType.WorldSpawn) 
            {
                if (WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey((int)(worldSpawnInteractableID)))
                { 
                    WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(worldSpawnInteractableID);
                }

                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, true);
            }

            //Hide or Destroy the game Object
            DestroyThisNetworkObjectServerRpc();

        }

        protected void OnItemIDChanged(int oldValue, int newValue) 
        {
            if (pickUpType != ItemPickUpType.CharacterDrop)
                return;

            item = WorldItemDataBase.Instance.GetItemByID(itemID.Value);
        }

        protected void OnNetworkPositionChanged(Vector3 oldPosition, Vector3 newPosition) 
        {
            if (pickUpType != ItemPickUpType.CharacterDrop)
                return;

            transform.position = networkPosition.Value;
        }

        protected void OnDroppingCreaturesIDChanged(ulong oldID, ulong newID) 
        {
            if (pickUpType != ItemPickUpType.CharacterDrop)
                return;

            if (trackDroppingCreaturesPosition)
                StartCoroutine(TrackDroppingCreaturesPosition());
        }

        protected IEnumerator TrackDroppingCreaturesPosition() 
        {
            AICharacterManager droppingCreature = NetworkManager.Singleton.SpawnManager.SpawnedObjects[droppingCreatureID.Value].gameObject.GetComponent<AICharacterManager>();
            bool trackCreature = false;

            if (droppingCreature != null)
                trackCreature = true;

            if (trackCreature) 
            {
                while (gameObject.activeInHierarchy) 
                {
                    transform.position = droppingCreature.characterCombatManager.lockOnTransform.position;
                    yield return null;
                }
            }

            yield return null;

        }

        [ServerRpc(RequireOwnership = false)]
        protected void DestroyThisNetworkObjectServerRpc() 
        {
            if (IsServer) 
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
