using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.TeamSelection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MultiplayerTeamSelectUIHandler))]
	public class MissionGauntletTeamSelection : MissionView
	{
		public MissionGauntletTeamSelection()
		{
			this.ViewOrderPriority = 22;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			this._multiplayerTeamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			this._classLoadoutGauntletComponent = base.Mission.GetMissionBehavior<MissionGauntletClassLoadout>();
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			this._lobbyComponent.OnPostMatchEnded += this.OnClose;
			this._multiplayerTeamSelectComponent.OnSelectingTeam += new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.MissionLobbyComponentOnSelectingTeam);
			this._multiplayerTeamSelectComponent.OnUpdateTeams += this.MissionLobbyComponentOnUpdateTeams;
			this._multiplayerTeamSelectComponent.OnUpdateFriendsPerTeam += this.MissionLobbyComponentOnFriendsUpdated;
			this._scoreboardGauntletComponent = base.Mission.GetMissionBehavior<MissionGauntletMultiplayerScoreboard>();
			if (this._scoreboardGauntletComponent != null)
			{
				MissionGauntletMultiplayerScoreboard scoreboardGauntletComponent = this._scoreboardGauntletComponent;
				scoreboardGauntletComponent.OnScoreboardToggled = (Action<bool>)Delegate.Combine(scoreboardGauntletComponent.OnScoreboardToggled, new Action<bool>(this.OnScoreboardToggled));
			}
			this._multiplayerTeamSelectComponent.OnMyTeamChange += this.OnMyTeamChanged;
		}

		public override void OnMissionScreenFinalize()
		{
			this._missionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			this._lobbyComponent.OnPostMatchEnded -= this.OnClose;
			this._multiplayerTeamSelectComponent.OnSelectingTeam -= new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.MissionLobbyComponentOnSelectingTeam);
			this._multiplayerTeamSelectComponent.OnUpdateTeams -= this.MissionLobbyComponentOnUpdateTeams;
			this._multiplayerTeamSelectComponent.OnUpdateFriendsPerTeam -= this.MissionLobbyComponentOnFriendsUpdated;
			this._multiplayerTeamSelectComponent.OnMyTeamChange -= this.OnMyTeamChanged;
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				this._gauntletLayer = null;
			}
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._dataSource = null;
			}
			if (this._scoreboardGauntletComponent != null)
			{
				MissionGauntletMultiplayerScoreboard scoreboardGauntletComponent = this._scoreboardGauntletComponent;
				scoreboardGauntletComponent.OnScoreboardToggled = (Action<bool>)Delegate.Remove(scoreboardGauntletComponent.OnScoreboardToggled, new Action<bool>(this.OnScoreboardToggled));
			}
			base.OnMissionScreenFinalize();
		}

		public override bool OnEscape()
		{
			if (this._isActive && !this._dataSource.IsCancelDisabled)
			{
				this.OnClose();
				return true;
			}
			return base.OnEscape();
		}

		private void OnClose()
		{
			if (!this._isActive)
			{
				return;
			}
			this._isActive = false;
			this._disabledTeams = null;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.MissionScreen.SetCameraLockState(false);
			base.MissionScreen.SetDisplayDialog(false);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			if (this._classLoadoutGauntletComponent != null && this._classLoadoutGauntletComponent.IsForceClosed)
			{
				this._classLoadoutGauntletComponent.OnTryToggle(true);
			}
		}

		private void OnOpen()
		{
			if (this._isActive)
			{
				return;
			}
			this._isActive = true;
			string strValue = MultiplayerOptionsExtensions.GetStrValue(11, 0);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._dataSource = new MultiplayerTeamSelectVM(base.Mission, new Action<Team>(this.OnChangeTeamTo), new Action(this.OnAutoassign), new Action(this.OnClose), base.Mission.Teams, strValue);
			this._dataSource.RefreshDisabledTeams(this._disabledTeams);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerTeamSelection", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 3);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.SetCameraLockState(true);
			this.MissionLobbyComponentOnUpdateTeams();
			this.MissionLobbyComponentOnFriendsUpdated();
		}

		private void OnChangeTeamTo(Team targetTeam)
		{
			this._multiplayerTeamSelectComponent.ChangeTeam(targetTeam);
		}

		private void OnMyTeamChanged()
		{
			this.OnClose();
		}

		private void OnAutoassign()
		{
			this._multiplayerTeamSelectComponent.AutoAssignTeam(GameNetwork.MyPeer);
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isSynchronized && this._toOpen && base.MissionScreen.SetDisplayDialog(true))
			{
				this._toOpen = false;
				this.OnOpen();
			}
			MultiplayerTeamSelectVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.Tick(dt);
		}

		private void MissionLobbyComponentOnSelectingTeam(List<Team> disabledTeams)
		{
			this._disabledTeams = disabledTeams;
			this._toOpen = true;
		}

		private void MissionLobbyComponentOnFriendsUpdated()
		{
			if (!this._isActive)
			{
				return;
			}
			IEnumerable<MissionPeer> enumerable = from x in this._multiplayerTeamSelectComponent.GetFriendsForTeam(base.Mission.AttackerTeam)
				select x.GetComponent<MissionPeer>();
			IEnumerable<MissionPeer> enumerable2 = from x in this._multiplayerTeamSelectComponent.GetFriendsForTeam(base.Mission.DefenderTeam)
				select x.GetComponent<MissionPeer>();
			this._dataSource.RefreshFriendsPerTeam(enumerable, enumerable2);
		}

		private void MissionLobbyComponentOnUpdateTeams()
		{
			if (!this._isActive)
			{
				return;
			}
			List<Team> disabledTeams = this._multiplayerTeamSelectComponent.GetDisabledTeams();
			this._dataSource.RefreshDisabledTeams(disabledTeams);
			int playerCountForTeam = this._multiplayerTeamSelectComponent.GetPlayerCountForTeam(base.Mission.AttackerTeam);
			int playerCountForTeam2 = this._multiplayerTeamSelectComponent.GetPlayerCountForTeam(base.Mission.DefenderTeam);
			int intValue = MultiplayerOptionsExtensions.GetIntValue(18, 0);
			int intValue2 = MultiplayerOptionsExtensions.GetIntValue(19, 0);
			this._dataSource.RefreshPlayerAndBotCount(playerCountForTeam, playerCountForTeam2, intValue, intValue2);
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

		private void OnMyClientSynchronized()
		{
			this._isSynchronized = true;
		}

		private GauntletLayer _gauntletLayer;

		private MultiplayerTeamSelectVM _dataSource;

		private MissionNetworkComponent _missionNetworkComponent;

		private MultiplayerTeamSelectComponent _multiplayerTeamSelectComponent;

		private MissionGauntletMultiplayerScoreboard _scoreboardGauntletComponent;

		private MissionGauntletClassLoadout _classLoadoutGauntletComponent;

		private MissionLobbyComponent _lobbyComponent;

		private List<Team> _disabledTeams;

		private bool _toOpen;

		private bool _isSynchronized;

		private bool _isActive;
	}
}
