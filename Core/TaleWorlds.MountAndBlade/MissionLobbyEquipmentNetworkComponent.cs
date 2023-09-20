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
				registerer.Register<RequestTroopIndexChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestTroopIndexChange>(this.HandleClientEventLobbyEquipmentUpdated));
				registerer.Register<TeamInitialPerkInfoMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<TeamInitialPerkInfoMessage>(this.HandleClientEventTeamInitialPerkInfoMessage));
				registerer.Register<RequestPerkChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestPerkChange>(this.HandleClientEventRequestPerkChange));
				return;
			}
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.Register<UpdateSelectedTroopIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<UpdateSelectedTroopIndex>(this.HandleServerEventEquipmentIndexUpdated));
				registerer.Register<SyncPerksForCurrentlySelectedTroop>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncPerksForCurrentlySelectedTroop>(this.SyncPerksForCurrentlySelectedTroop));
			}
		}

		private void HandleServerEventEquipmentIndexUpdated(UpdateSelectedTroopIndex message)
		{
			message.Peer.GetComponent<MissionPeer>().SelectedTroopIndex = message.SelectedTroopIndex;
		}

		private void SyncPerksForCurrentlySelectedTroop(SyncPerksForCurrentlySelectedTroop message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			for (int i = 0; i < 3; i++)
			{
				component.SelectPerk(i, message.PerkIndices[i], component.SelectedTroopIndex);
			}
		}

		private bool HandleClientEventLobbyEquipmentUpdated(NetworkCommunicator peer, RequestTroopIndexChange message)
		{
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
			if (missionBehavior.AreAgentsSpawning() && component.SelectedTroopIndex != message.SelectedTroopIndex)
			{
				if (component.Culture == null || message.SelectedTroopIndex < 0 || MultiplayerClassDivisions.GetMPHeroClasses(component.Culture).Count<MultiplayerClassDivisions.MPHeroClass>() <= message.SelectedTroopIndex)
				{
					component.SelectedTroopIndex = 0;
				}
				else
				{
					component.SelectedTroopIndex = message.SelectedTroopIndex;
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

		private bool HandleClientEventTeamInitialPerkInfoMessage(NetworkCommunicator peer, TeamInitialPerkInfoMessage message)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component == null)
			{
				return false;
			}
			if (base.Mission.GetMissionBehavior<SpawnComponent>() == null)
			{
				return false;
			}
			component.OnTeamInitialPerkInfoReceived(message.Perks);
			return true;
		}

		private bool HandleClientEventRequestPerkChange(NetworkCommunicator peer, RequestPerkChange message)
		{
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
			if (component.SelectPerk(message.PerkListIndex, message.PerkIndex, -1) && missionBehavior.AreAgentsSpawning() && this.OnEquipmentRefreshed != null)
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
			Debug.FailedAssert("OBSOLETE FUNCTION: TransferFromEquipmentSlotToEquipmentSlot", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionLobbyEquipmentNetworkComponent.cs", "TransferFromEquipmentSlotToEquipmentSlot", 493);
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
