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
	// Token: 0x02000291 RID: 657
	public class MissionLobbyEquipmentNetworkComponent : MissionNetwork
	{
		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060022DD RID: 8925 RVA: 0x0007F4C3 File Offset: 0x0007D6C3
		// (set) Token: 0x060022DE RID: 8926 RVA: 0x0007F4CB File Offset: 0x0007D6CB
		public Equipment MyEquipment { get; private set; }

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x060022DF RID: 8927 RVA: 0x0007F4D4 File Offset: 0x0007D6D4
		// (set) Token: 0x060022E0 RID: 8928 RVA: 0x0007F4DC File Offset: 0x0007D6DC
		public MissionLobbyEquipmentChest MyChest { get; private set; }

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x060022E1 RID: 8929 RVA: 0x0007F4E8 File Offset: 0x0007D6E8
		// (remove) Token: 0x060022E2 RID: 8930 RVA: 0x0007F520 File Offset: 0x0007D720
		public event MissionLobbyEquipmentNetworkComponent.OnToggleLoadoutDelegate OnToggleLoadout;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x060022E3 RID: 8931 RVA: 0x0007F558 File Offset: 0x0007D758
		// (remove) Token: 0x060022E4 RID: 8932 RVA: 0x0007F590 File Offset: 0x0007D790
		public event MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate OnEquipmentRefreshed;

		// Token: 0x060022E5 RID: 8933 RVA: 0x0007F5C8 File Offset: 0x0007D7C8
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

		// Token: 0x060022E6 RID: 8934 RVA: 0x0007F650 File Offset: 0x0007D850
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

		// Token: 0x060022E7 RID: 8935 RVA: 0x0007F6B8 File Offset: 0x0007D8B8
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

		// Token: 0x060022E8 RID: 8936 RVA: 0x0007F72E File Offset: 0x0007D92E
		private void HandleServerEventEquipmentIndexUpdated(UpdateSelectedTroopIndex message)
		{
			message.Peer.GetComponent<MissionPeer>().SelectedTroopIndex = message.SelectedTroopIndex;
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x0007F748 File Offset: 0x0007D948
		private void SyncPerksForCurrentlySelectedTroop(SyncPerksForCurrentlySelectedTroop message)
		{
			MissionPeer component = message.Peer.GetComponent<MissionPeer>();
			for (int i = 0; i < 3; i++)
			{
				component.SelectPerk(i, message.PerkIndices[i], component.SelectedTroopIndex);
			}
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x0007F784 File Offset: 0x0007D984
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

		// Token: 0x060022EB RID: 8939 RVA: 0x0007F838 File Offset: 0x0007DA38
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

		// Token: 0x060022EC RID: 8940 RVA: 0x0007F870 File Offset: 0x0007DA70
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

		// Token: 0x060022ED RID: 8941 RVA: 0x0007F8CC File Offset: 0x0007DACC
		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			this.AddPeerEquipment(networkPeer.GetComponent<MissionPeer>());
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x0007F8DA File Offset: 0x0007DADA
		private void AddPeerEquipment(MissionPeer peer)
		{
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x0007F8DC File Offset: 0x0007DADC
		public Equipment GetEquipmentOf(MissionPeer peer)
		{
			return null;
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x0007F8E0 File Offset: 0x0007DAE0
		private void UnequipItem(EquipmentIndex itemIndex, Equipment equipment)
		{
			equipment[itemIndex] = default(EquipmentElement);
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x0007F900 File Offset: 0x0007DB00
		private void UnequipItem(EquipmentIndex itemIndex, MissionPeer peer)
		{
			Equipment equipment = this._playerEquipments[peer];
			if (equipment[itemIndex].Item != null)
			{
				this.UnequipItem(itemIndex, equipment);
			}
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x0007F933 File Offset: 0x0007DB33
		private bool EquipItem(Equipment equipment, ItemObject item, EquipmentIndex itemIndex)
		{
			if (ItemData.CanItemToEquipmentDragPossible(item.StringId, (int)itemIndex))
			{
				equipment[itemIndex] = new EquipmentElement(item, null, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x0007F958 File Offset: 0x0007DB58
		private bool EquipItem(MissionPeer peer, int itemChestIndex, EquipmentIndex itemIndex)
		{
			Equipment equipment = this._playerEquipments[peer];
			ItemObject item = this._playerChests[peer].GetItem(itemChestIndex);
			return this.EquipItem(equipment, item, itemIndex);
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x0007F990 File Offset: 0x0007DB90
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

		// Token: 0x060022F5 RID: 8949 RVA: 0x0007F9D0 File Offset: 0x0007DBD0
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

		// Token: 0x060022F6 RID: 8950 RVA: 0x0007FA1C File Offset: 0x0007DC1C
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

		// Token: 0x060022F7 RID: 8951 RVA: 0x0007FAB3 File Offset: 0x0007DCB3
		public bool TransferFromEquipmentSlotToEquipmentSlot(int draggedEquipmentIndex, int droppedEquipmentIndex)
		{
			Debug.FailedAssert("OBSOLETE FUNCTION: TransferFromEquipmentSlotToEquipmentSlot", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionLobbyEquipmentNetworkComponent.cs", "TransferFromEquipmentSlotToEquipmentSlot", 493);
			return false;
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060022F8 RID: 8952 RVA: 0x0007FACF File Offset: 0x0007DCCF
		public int CharacterCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060022F9 RID: 8953 RVA: 0x0007FAD2 File Offset: 0x0007DCD2
		public int CurrentCharacterIndex
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x0007FAD5 File Offset: 0x0007DCD5
		public void SetNextCharacter()
		{
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x0007FAD7 File Offset: 0x0007DCD7
		public void SetPreviousCharacter()
		{
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x0007FAD9 File Offset: 0x0007DCD9
		public void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale)
		{
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060022FD RID: 8957 RVA: 0x0007FADB File Offset: 0x0007DCDB
		public bool SingleCharacterMode
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x0007FADE File Offset: 0x0007DCDE
		public void ToggleLoadout(bool isActive)
		{
			if (this.OnToggleLoadout != null)
			{
				this.OnToggleLoadout(isActive);
			}
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x0007FAF4 File Offset: 0x0007DCF4
		private void OpenLoadout()
		{
			this.ToggleLoadout(true);
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x0007FAFD File Offset: 0x0007DCFD
		private void CloseLoadout()
		{
			this.ToggleLoadout(false);
		}

		// Token: 0x04000CFA RID: 3322
		private Dictionary<MissionPeer, Equipment> _playerEquipments;

		// Token: 0x04000CFB RID: 3323
		private Dictionary<MissionPeer, MissionLobbyEquipmentChest> _playerChests;

		// Token: 0x04000CFC RID: 3324
		private MultiplayerMissionAgentVisualSpawnComponent _agentVisualSpawnComponent;

		// Token: 0x04000CFD RID: 3325
		private bool _myEquipmentInitialized;

		// Token: 0x0200059C RID: 1436
		// (Invoke) Token: 0x06003B43 RID: 15171
		public delegate void OnEquipmentSetLoadedDelegate();

		// Token: 0x0200059D RID: 1437
		// (Invoke) Token: 0x06003B47 RID: 15175
		public delegate void OnToggleLoadoutDelegate(bool isActive);

		// Token: 0x0200059E RID: 1438
		// (Invoke) Token: 0x06003B4B RID: 15179
		public delegate void OnRefreshEquipmentEventDelegate(MissionPeer lobbyPeer);
	}
}
