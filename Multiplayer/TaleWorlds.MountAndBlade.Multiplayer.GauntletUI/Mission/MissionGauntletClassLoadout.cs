using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MissionLobbyEquipmentUIHandler))]
	public class MissionGauntletClassLoadout : MissionView
	{
		public bool IsActive { get; private set; }

		public bool IsForceClosed { get; private set; }

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

		private void OnMyClientSynchronized()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			this._myRepresentative = ((myPeer != null) ? myPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>() : null);
			MissionRepresentativeBase myRepresentative = this._myRepresentative;
			myRepresentative.OnGoldUpdated = (Action)Delegate.Combine(myRepresentative.OnGoldUpdated, new Action(this.OnGoldUpdated));
		}

		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine && newTeam != null && (newTeam.IsAttacker || newTeam.IsDefender))
			{
				if (this.IsActive)
				{
					this.OnTryToggle(false);
				}
				this.OnTryToggle(true);
			}
		}

		private void OnRefreshSelection(MultiplayerClassDivisions.MPHeroClass heroClass)
		{
			this._lastSelectedHeroClass = heroClass;
		}

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

		private bool OnToggled(bool isActive)
		{
			if (this.IsActive == isActive)
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
			this.IsActive = isActive;
			return true;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._tryToInitialize && GameNetwork.IsMyPeerReady && PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).HasSpawnedAgentVisuals && this.OnToggled(true))
			{
				this._tryToInitialize = false;
			}
			if (this.IsActive)
			{
				this._dataSource.Tick(dt);
				MissionMultiplayerGameModeFlagDominationClient missionMultiplayerGameModeFlagDominationClient;
				if (base.Input.IsHotKeyReleased("ForfeitSpawn") && (missionMultiplayerGameModeFlagDominationClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>() as MissionMultiplayerGameModeFlagDominationClient) != null)
				{
					missionMultiplayerGameModeFlagDominationClient.OnRequestForfeitSpawn();
				}
			}
		}

		private void OnSelectingTeam(List<Team> disableTeams)
		{
			this.IsForceClosed = true;
			this.OnToggled(false);
		}

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

		private void OnGoldUpdated()
		{
			MultiplayerClassLoadoutVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnGoldUpdated();
		}

		private MultiplayerClassLoadoutVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private MissionRepresentativeBase _myRepresentative;

		private MissionNetworkComponent _missionNetworkComponent;

		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		private MultiplayerTeamSelectComponent _teamSelectComponent;

		private MissionGauntletMultiplayerScoreboard _scoreboardGauntletComponent;

		private MultiplayerClassDivisions.MPHeroClass _lastSelectedHeroClass;

		private bool _tryToInitialize;
	}
}
