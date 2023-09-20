using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000039 RID: 57
	[OverrideView(typeof(MissionLobbyEquipmentUIHandler))]
	public class MissionGauntletClassLoadout : MissionView
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000F8EE File Offset: 0x0000DAEE
		// (set) Token: 0x060002BF RID: 703 RVA: 0x0000F8F6 File Offset: 0x0000DAF6
		public bool IsForceClosed { get; private set; }

		// Token: 0x060002C0 RID: 704 RVA: 0x0000F900 File Offset: 0x0000DB00
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 20;
			this._missionLobbyEquipmentNetworkComponent = base.Mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this._gameModeClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._teamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam += new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			this._scoreboardGauntletComponent = base.Mission.GetMissionBehavior<MissionGauntletMultiplayerScoreboard>();
			if (this._scoreboardGauntletComponent != null)
			{
				MissionGauntletMultiplayerScoreboard scoreboardGauntletComponent = this._scoreboardGauntletComponent;
				scoreboardGauntletComponent.OnScoreboardToggled = (Action<bool>)Delegate.Combine(scoreboardGauntletComponent.OnScoreboardToggled, new Action<bool>(this.OnScoreboardToggled));
			}
			this._missionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			if (this._missionNetworkComponent != null)
			{
				this._missionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			}
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			this._missionLobbyEquipmentNetworkComponent.OnToggleLoadout += new MissionLobbyEquipmentNetworkComponent.OnToggleLoadoutDelegate(this.OnTryToggle);
			this._missionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed += new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnPeerEquipmentRefreshed);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000FA1C File Offset: 0x0000DC1C
		private void OnMyClientSynchronized()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			this._myRepresentative = ((myPeer != null) ? myPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>() : null);
			MissionRepresentativeBase myRepresentative = this._myRepresentative;
			myRepresentative.OnGoldUpdated = (Action)Delegate.Combine(myRepresentative.OnGoldUpdated, new Action(this.OnGoldUpdated));
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000FA6C File Offset: 0x0000DC6C
		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine && newTeam != null && (newTeam.IsAttacker || newTeam.IsDefender))
			{
				if (this._isActive)
				{
					this.OnTryToggle(false);
				}
				this.OnTryToggle(true);
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000FA9F File Offset: 0x0000DC9F
		private void OnRefreshSelection(MultiplayerClassDivisions.MPHeroClass heroClass)
		{
			this._lastSelectedHeroClass = heroClass;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000FAA8 File Offset: 0x0000DCA8
		public override void OnMissionScreenFinalize()
		{
			if (this._gauntletLayer != null)
			{
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				this._gauntletLayer = null;
			}
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._dataSource = null;
			}
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam -= new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			if (this._scoreboardGauntletComponent != null)
			{
				MissionGauntletMultiplayerScoreboard scoreboardGauntletComponent = this._scoreboardGauntletComponent;
				scoreboardGauntletComponent.OnScoreboardToggled = (Action<bool>)Delegate.Remove(scoreboardGauntletComponent.OnScoreboardToggled, new Action<bool>(this.OnScoreboardToggled));
			}
			if (this._missionNetworkComponent != null)
			{
				this._missionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
				if (this._myRepresentative != null)
				{
					MissionRepresentativeBase myRepresentative = this._myRepresentative;
					myRepresentative.OnGoldUpdated = (Action)Delegate.Remove(myRepresentative.OnGoldUpdated, new Action(this.OnGoldUpdated));
				}
			}
			this._missionLobbyEquipmentNetworkComponent.OnToggleLoadout -= new MissionLobbyEquipmentNetworkComponent.OnToggleLoadoutDelegate(this.OnTryToggle);
			this._missionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed -= new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnPeerEquipmentRefreshed);
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000FBD0 File Offset: 0x0000DDD0
		private void CreateView()
		{
			MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._dataSource = new MultiplayerClassLoadoutVM(missionBehavior, new Action<MultiplayerClassDivisions.MPHeroClass>(this.OnRefreshSelection), this._lastSelectedHeroClass);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerClassLoadout", this._dataSource);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000FC47 File Offset: 0x0000DE47
		public void OnTryToggle(bool isActive)
		{
			if (isActive)
			{
				this._tryToInitialize = true;
				return;
			}
			this.IsForceClosed = false;
			this.OnToggled(false);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000FC64 File Offset: 0x0000DE64
		private bool OnToggled(bool isActive)
		{
			if (this._isActive == isActive)
			{
				return true;
			}
			if (!base.MissionScreen.SetDisplayDialog(isActive))
			{
				return false;
			}
			if (isActive)
			{
				this.CreateView();
				this._dataSource.Tick(1f);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				base.MissionScreen.AddLayer(this._gauntletLayer);
			}
			else
			{
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				this._dataSource.OnFinalize();
				this._dataSource = null;
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				this._gauntletLayer = null;
			}
			this._isActive = isActive;
			return true;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000FD0C File Offset: 0x0000DF0C
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._tryToInitialize && GameNetwork.IsMyPeerReady && PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).HasSpawnedAgentVisuals && this.OnToggled(true))
			{
				this._tryToInitialize = false;
			}
			if (this._isActive)
			{
				this._dataSource.Tick(dt);
				MissionMultiplayerGameModeFlagDominationClient missionMultiplayerGameModeFlagDominationClient;
				if (base.Input.IsHotKeyReleased("ForfeitSpawn") && (missionMultiplayerGameModeFlagDominationClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>() as MissionMultiplayerGameModeFlagDominationClient) != null)
				{
					missionMultiplayerGameModeFlagDominationClient.OnRequestForfeitSpawn();
				}
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000FD90 File Offset: 0x0000DF90
		private void OnSelectingTeam(List<Team> disableTeams)
		{
			this.IsForceClosed = true;
			this.OnToggled(false);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000FDA1 File Offset: 0x0000DFA1
		private void OnScoreboardToggled(bool isEnabled)
		{
			if (isEnabled)
			{
				GauntletLayer gauntletLayer = this._gauntletLayer;
				if (gauntletLayer == null)
				{
					return;
				}
				gauntletLayer.InputRestrictions.ResetInputRestrictions();
				return;
			}
			else
			{
				GauntletLayer gauntletLayer2 = this._gauntletLayer;
				if (gauntletLayer2 == null)
				{
					return;
				}
				gauntletLayer2.InputRestrictions.SetInputRestrictions(true, 7);
				return;
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000FDD3 File Offset: 0x0000DFD3
		private void OnPeerEquipmentRefreshed(MissionPeer peer)
		{
			if (this._gameModeClient.GameType == 6 || this._gameModeClient.GameType == 5)
			{
				MultiplayerClassLoadoutVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.OnPeerEquipmentRefreshed(peer);
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000FE02 File Offset: 0x0000E002
		private void OnGoldUpdated()
		{
			MultiplayerClassLoadoutVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnGoldUpdated();
		}

		// Token: 0x04000169 RID: 361
		private MultiplayerClassLoadoutVM _dataSource;

		// Token: 0x0400016A RID: 362
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400016B RID: 363
		private MissionRepresentativeBase _myRepresentative;

		// Token: 0x0400016C RID: 364
		private MissionNetworkComponent _missionNetworkComponent;

		// Token: 0x0400016D RID: 365
		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		// Token: 0x0400016E RID: 366
		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		// Token: 0x0400016F RID: 367
		private MultiplayerTeamSelectComponent _teamSelectComponent;

		// Token: 0x04000170 RID: 368
		private MissionGauntletMultiplayerScoreboard _scoreboardGauntletComponent;

		// Token: 0x04000171 RID: 369
		private MultiplayerClassDivisions.MPHeroClass _lastSelectedHeroClass;

		// Token: 0x04000173 RID: 371
		private bool _tryToInitialize;

		// Token: 0x04000174 RID: 372
		private bool _isActive;
	}
}
