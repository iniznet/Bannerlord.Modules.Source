using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000048 RID: 72
	[OverrideView(typeof(MultiplayerTeamSelectUIHandler))]
	public class MissionGauntletTeamSelection : MissionView
	{
		// Token: 0x0600034F RID: 847 RVA: 0x00012BDF File Offset: 0x00010DDF
		public MissionGauntletTeamSelection()
		{
			this.ViewOrderPriority = 22;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00012BF0 File Offset: 0x00010DF0
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

		// Token: 0x06000351 RID: 849 RVA: 0x00012D14 File Offset: 0x00010F14
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

		// Token: 0x06000352 RID: 850 RVA: 0x00012E2A File Offset: 0x0001102A
		public override bool OnEscape()
		{
			if (this._isActive && !this._dataSource.IsCancelDisabled)
			{
				this.OnClose();
				return true;
			}
			return base.OnEscape();
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00012E50 File Offset: 0x00011050
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

		// Token: 0x06000354 RID: 852 RVA: 0x00012EE8 File Offset: 0x000110E8
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

		// Token: 0x06000355 RID: 853 RVA: 0x00012FDB File Offset: 0x000111DB
		private void OnChangeTeamTo(Team targetTeam)
		{
			this._multiplayerTeamSelectComponent.ChangeTeam(targetTeam);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00012FE9 File Offset: 0x000111E9
		private void OnMyTeamChanged()
		{
			this.OnClose();
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00012FF1 File Offset: 0x000111F1
		private void OnAutoassign()
		{
			this._multiplayerTeamSelectComponent.AutoAssignTeam(GameNetwork.MyPeer);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00013004 File Offset: 0x00011204
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

		// Token: 0x06000359 RID: 857 RVA: 0x00013054 File Offset: 0x00011254
		private void MissionLobbyComponentOnSelectingTeam(List<Team> disabledTeams)
		{
			this._disabledTeams = disabledTeams;
			this._toOpen = true;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00013064 File Offset: 0x00011264
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

		// Token: 0x0600035B RID: 859 RVA: 0x00013100 File Offset: 0x00011300
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

		// Token: 0x0600035C RID: 860 RVA: 0x0001317F File Offset: 0x0001137F
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

		// Token: 0x0600035D RID: 861 RVA: 0x000131B1 File Offset: 0x000113B1
		private void OnMyClientSynchronized()
		{
			this._isSynchronized = true;
		}

		// Token: 0x040001C8 RID: 456
		private GauntletLayer _gauntletLayer;

		// Token: 0x040001C9 RID: 457
		private MultiplayerTeamSelectVM _dataSource;

		// Token: 0x040001CA RID: 458
		private MissionNetworkComponent _missionNetworkComponent;

		// Token: 0x040001CB RID: 459
		private MultiplayerTeamSelectComponent _multiplayerTeamSelectComponent;

		// Token: 0x040001CC RID: 460
		private MissionGauntletMultiplayerScoreboard _scoreboardGauntletComponent;

		// Token: 0x040001CD RID: 461
		private MissionGauntletClassLoadout _classLoadoutGauntletComponent;

		// Token: 0x040001CE RID: 462
		private MissionLobbyComponent _lobbyComponent;

		// Token: 0x040001CF RID: 463
		private List<Team> _disabledTeams;

		// Token: 0x040001D0 RID: 464
		private bool _toOpen;

		// Token: 0x040001D1 RID: 465
		private bool _isSynchronized;

		// Token: 0x040001D2 RID: 466
		private bool _isActive;
	}
}
