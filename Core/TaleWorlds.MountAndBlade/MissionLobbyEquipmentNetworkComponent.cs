using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MissionLobbyEquipmentNetworkComponent : MissionNetwork
	{
		public Equipment MyEquipment { get; private set; }

		public MissionLobbyEquipmentChest MyChest { get; private set; }

		public event MissionLobbyEquipmentNetworkComponent.OnToggleLoadoutDelegate OnToggleLoadout;

		public event MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate OnEquipmentRefreshed;

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._playerEquipments = new Dictionary<MissionPeer, Equipment>();
			this._playerChests = new Dictionary<MissionPeer, MissionLobbyEquipmentChest>();
			if (!GameNetwork.IsDedicatedServer)
			{
				this._agentVisualSpawnComponent = Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
				this._agentVisualSpawnComponent.OnMyAgentVisualSpawned += this.OpenLoadout;
				this._agentVisualSpawnComponent.OnMyAgentSpawnedFromVisual += this.CloseLoadout;
				this._agentVisualSpawnComponent.OnMyAgentVisualRemoved += this.CloseLoadout;
			}
		}

		protected override void OnEndMission()
		{
			if (!GameNetwork.IsDedicatedServer)
			{
				this._agentVisualSpawnComponent.OnMyAgentVisualSpawned -= this.OpenLoadout;
				this._agentVisualSpawnComponent.OnMyAgentSpawnedFromVisual -= this.CloseLoadout;
				this._agentVisualSpawnComponent.OnMyAgentVisualRemoved -= this.CloseLoadout;
				this._agentVisualSpawnComponent = null;
			}
			base.OnEndMission();
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<RequestTroopIndexChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventLobbyEquipmentUpdated));
				registerer.RegisterBaseHandler<TeamInitialPerkInfoMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventTeamInitialPerkInfoMessage));
				registerer.RegisterBaseHandler<RequestPerkChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventRequestPerkChange));
				return;
			}
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.RegisterBaseHandler<UpdateSelectedTroopIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventEquipmentIndexUpdated));
				registerer.RegisterBaseHandler<SyncPerksForCurrentlySelectedTroop>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.SyncPerksForCurrentlySelectedTroop));
			}
		}

		private void HandleServerEventEquipmentIndexUpdated(GameNetworkMessage baseMessage)
		{
			UpdateSelectedTroopIndex updateSelectedTroopIndex = (UpdateSelectedTroopIndex)baseMessage;
			updateSelectedTroopIndex.Peer.GetComponent<MissionPeer>().SelectedTroopIndex = updateSelectedTroopIndex.SelectedTroopIndex;
		}

		private void SyncPerksForCurrentlySelectedTroop(GameNetworkMessage baseMessage)
		{
			SyncPerksForCurrentlySelectedTroop syncPerksForCurrentlySelectedTroop = (SyncPerksForCurrentlySelectedTroop)baseMessage;
			MissionPeer component = syncPerksForCurrentlySelectedTroop.Peer.GetComponent<MissionPeer>();
			for (int i = 0; i < 3; i++)
			{
				component.SelectPerk(i, syncPerksForCurrentlySelectedTroop.PerkIndices[i], component.SelectedTroopIndex);
			}
		}

		private bool HandleClientEventLobbyEquipmentUpdated(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			RequestTroopIndexChange requestTroopIndexChange = (RequestTroopIndexChange)baseMessage;
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			SpawnComponent missionBehavior = base.Mission.GetMissionBehavior<SpawnComponent>();
			if (missionBehavior == null)
			{
				return false;
			}
			if (missionBehavior.AreAgentsSpawning() && component.SelectedTroopIndex != requestTroopIndexChange.SelectedTroopIndex)
			{
				if (component.Culture == null || requestTroopIndexChange.SelectedTroopIndex < 0 || MultiplayerClassDivisions.GetMPHeroClasses(component.Culture).Count<MultiplayerClassDivisions.MPHeroClass>() <= requestTroopIndexChange.SelectedTroopIndex)
				{
					component.SelectedTroopIndex = 0;
				}
				else
				{
					component.SelectedTroopIndex = requestTroopIndexChange.SelectedTroopIndex;
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new UpdateSelectedTroopIndex(peer, component.SelectedTroopIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, peer);
				if (this.OnEquipmentRefreshed != null)
				{
					this.OnEquipmentRefreshed(component);
				}
			}
			return true;
		}

		private bool HandleClientEventTeamInitialPerkInfoMessage(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			TeamInitialPerkInfoMessage teamInitialPerkInfoMessage = (TeamInitialPerkInfoMessage)baseMessage;
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			if (base.Mission.GetMissionBehavior<SpawnComponent>() == null)
			{
				return false;
			}
			component.OnTeamInitialPerkInfoReceived(teamInitialPerkInfoMessage.Perks);
			return true;
		}

		private bool HandleClientEventRequestPerkChange(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			RequestPerkChange requestPerkChange = (RequestPerkChange)baseMessage;
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			SpawnComponent missionBehavior = base.Mission.GetMissionBehavior<SpawnComponent>();
			if (missionBehavior == null)
			{
				return false;
			}
			if (component.SelectPerk(requestPerkChange.PerkListIndex, requestPerkChange.PerkIndex, -1) && missionBehavior.AreAgentsSpawning() && this.OnEquipmentRefreshed != null)
			{
				this.OnEquipmentRefreshed(component);
			}
			return true;
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.AddPeerEquipment(networkPeer.GetComponent<MissionPeer>());
		}

		private void AddPeerEquipment(MissionPeer peer)
		{
		}

		public Equipment GetEquipmentOf(MissionPeer peer)
		{
			return null;
		}

		private void UnequipItem(EquipmentIndex itemIndex, Equipment equipment)
		{
			equipment[itemIndex] = default(EquipmentElement);
		}

		private void UnequipItem(EquipmentIndex itemIndex, MissionPeer peer)
		{
			Equipment equipment = this._playerEquipments[peer];
			if (equipment[itemIndex].Item != null)
			{
				this.UnequipItem(itemIndex, equipment);
			}
		}

		private bool EquipItem(Equipment equipment, ItemObject item, EquipmentIndex itemIndex)
		{
			if (ItemData.CanItemToEquipmentDragPossible(item.StringId, (int)itemIndex))
			{
				equipment[itemIndex] = new EquipmentElement(item, null, null, false);
				return true;
			}
			return false;
		}

		private bool EquipItem(MissionPeer peer, int itemChestIndex, EquipmentIndex itemIndex)
		{
			Equipment equipment = this._playerEquipments[peer];
			ItemObject item = this._playerChests[peer].GetItem(itemChestIndex);
			return this.EquipItem(equipment, item, itemIndex);
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this._myEquipmentInitialized && !GameNetwork.IsServer && GameNetwork.IsMyPeerReady)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component != null)
				{
					this.AddPeerEquipment(component);
				}
			}
		}

		public void PerkUpdated(int perkList, int perkIndex)
		{
			if (GameNetwork.IsServer)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (this.OnEquipmentRefreshed != null)
				{
					this.OnEquipmentRefreshed(component);
					return;
				}
			}
			else
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestPerkChange(perkList, perkIndex));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		public void EquipmentUpdated()
		{
			if (GameNetwork.IsServer)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component.SelectedTroopIndex != component.NextSelectedTroopIndex)
				{
					component.SelectedTroopIndex = component.NextSelectedTroopIndex;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new UpdateSelectedTroopIndex(GameNetwork.MyPeer, component.SelectedTroopIndex));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, GameNetwork.MyPeer);
					if (this.OnEquipmentRefreshed != null)
					{
						this.OnEquipmentRefreshed(component);
						return;
					}
				}
			}
			else
			{
				MissionPeer component2 = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestTroopIndexChange(component2.NextSelectedTroopIndex));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		public bool TransferFromEquipmentSlotToEquipmentSlot(int draggedEquipmentIndex, int droppedEquipmentIndex)
		{
			Debug.FailedAssert("OBSOLETE FUNCTION: TransferFromEquipmentSlotToEquipmentSlot", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionLobbyEquipmentNetworkComponent.cs", "TransferFromEquipmentSlotToEquipmentSlot", 498);
			return false;
		}

		public int CharacterCount
		{
			get
			{
				return 1;
			}
		}

		public int CurrentCharacterIndex
		{
			get
			{
				return 0;
			}
		}

		public void SetNextCharacter()
		{
		}

		public void SetPreviousCharacter()
		{
		}

		public void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale)
		{
		}

		public bool SingleCharacterMode
		{
			get
			{
				return true;
			}
		}

		public void ToggleLoadout(bool isActive)
		{
			if (this.OnToggleLoadout != null)
			{
				this.OnToggleLoadout(isActive);
			}
		}

		private void OpenLoadout()
		{
			this.ToggleLoadout(true);
		}

		private void CloseLoadout()
		{
			this.ToggleLoadout(false);
		}

		private Dictionary<MissionPeer, Equipment> _playerEquipments;

		private Dictionary<MissionPeer, MissionLobbyEquipmentChest> _playerChests;

		private MultiplayerMissionAgentVisualSpawnComponent _agentVisualSpawnComponent;

		private bool _myEquipmentInitialized;

		public delegate void OnEquipmentSetLoadedDelegate();

		public delegate void OnToggleLoadoutDelegate(bool isActive);

		public delegate void OnRefreshEquipmentEventDelegate(MissionPeer lobbyPeer);
	}
}
