using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000045 RID: 69
	[OverrideView(typeof(MissionScoreboardUIHandler))]
	public class MissionGauntletMultiplayerScoreboard : MissionView
	{
		// Token: 0x06000333 RID: 819 RVA: 0x000121D3 File Offset: 0x000103D3
		[UsedImplicitly]
		public MissionGauntletMultiplayerScoreboard(bool isSingleTeam)
		{
			this._isSingleTeam = isSingleTeam;
			this.ViewOrderPriority = 25;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000121EC File Offset: 0x000103EC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.InitializeLayer();
			base.Mission.IsFriendlyMission = false;
			GameKeyContext category = HotKeyManager.GetCategory("ScoreboardHotKeyCategory");
			if (!base.MissionScreen.SceneLayer.Input.IsCategoryRegistered(category))
			{
				base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(category);
			}
			base.MissionScreen.OnSpectateAgentFocusIn += this.HandleSpectateAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut += this.HandleSpectateAgentFocusOut;
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionLobbyComponent.CurrentMultiplayerStateChanged += this.MissionLobbyComponentOnCurrentMultiplayerStateChanged;
			this._missionLobbyComponent.OnCultureSelectionRequested += this.OnCultureSelectionRequested;
			this._scoreboardStayDuration = MissionLobbyComponent.PostMatchWaitDuration / 2f;
			this._teamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam += new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			if (this._dataSource != null)
			{
				this._dataSource.IsActive = false;
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00012320 File Offset: 0x00010520
		public override void OnMissionScreenFinalize()
		{
			base.MissionScreen.OnSpectateAgentFocusIn -= this.HandleSpectateAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut -= this.HandleSpectateAgentFocusOut;
			this._missionLobbyComponent.CurrentMultiplayerStateChanged -= this.MissionLobbyComponentOnCurrentMultiplayerStateChanged;
			this._missionLobbyComponent.OnCultureSelectionRequested -= this.OnCultureSelectionRequested;
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam -= new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			this.FinalizeLayer();
			base.OnMissionScreenFinalize();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x000123C8 File Offset: 0x000105C8
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._isMissionEnding)
			{
				if (this._scoreboardStayTimeElapsed >= this._scoreboardStayDuration)
				{
					this.ToggleScoreboard(false);
					return;
				}
				this._scoreboardStayTimeElapsed += dt;
			}
			this._dataSource.Tick(dt);
			if (TaleWorlds.InputSystem.Input.IsGamepadActive)
			{
				bool flag = base.MissionScreen.SceneLayer.Input.IsGameKeyPressed(4) || this._gauntletLayer.Input.IsGameKeyPressed(4);
				if (this._isMissionEnding)
				{
					this.ToggleScoreboard(true);
				}
				else if (flag && !base.MissionScreen.IsRadialMenuActive && !base.Mission.IsOrderMenuOpen)
				{
					this.ToggleScoreboard(!this._dataSource.IsActive);
				}
			}
			else
			{
				bool flag2 = base.MissionScreen.SceneLayer.Input.IsHotKeyDown("HoldShow") || this._gauntletLayer.Input.IsHotKeyDown("HoldShow");
				bool flag3 = this._isMissionEnding || (flag2 && !base.MissionScreen.IsRadialMenuActive && !base.Mission.IsOrderMenuOpen);
				this.ToggleScoreboard(flag3);
			}
			if (this._isActive && (base.MissionScreen.SceneLayer.Input.IsGameKeyPressed(35) || this._gauntletLayer.Input.IsGameKeyPressed(35)))
			{
				this._mouseRequstedWhileScoreboardActive = true;
			}
			bool flag4 = this._isMissionEnding || (this._isActive && this._mouseRequstedWhileScoreboardActive);
			this.SetMouseState(flag4);
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00012560 File Offset: 0x00010760
		private void ToggleScoreboard(bool isActive)
		{
			if (this._isActive != isActive)
			{
				this._isActive = isActive;
				this._dataSource.IsActive = this._isActive;
				base.MissionScreen.SetCameraLockState(this._isActive);
				if (!this._isActive)
				{
					this._mouseRequstedWhileScoreboardActive = false;
				}
				Action<bool> onScoreboardToggled = this.OnScoreboardToggled;
				if (onScoreboardToggled == null)
				{
					return;
				}
				onScoreboardToggled(this._isActive);
			}
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000125C4 File Offset: 0x000107C4
		private void SetMouseState(bool isMouseVisible)
		{
			if (this._isMouseVisible != isMouseVisible)
			{
				this._isMouseVisible = isMouseVisible;
				if (!this._isMouseVisible)
				{
					this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				}
				else
				{
					this._gauntletLayer.InputRestrictions.SetInputRestrictions(this._isMouseVisible, 3);
				}
				MissionScoreboardVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.SetMouseState(isMouseVisible);
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00012624 File Offset: 0x00010824
		private void HandleSpectateAgentFocusOut(Agent followedAgent)
		{
			if (followedAgent.MissionPeer != null)
			{
				MissionPeer component = followedAgent.MissionPeer.GetComponent<MissionPeer>();
				this._dataSource.DecreaseSpectatorCount(component);
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00012654 File Offset: 0x00010854
		private void HandleSpectateAgentFocusIn(Agent followedAgent)
		{
			if (followedAgent.MissionPeer != null)
			{
				MissionPeer component = followedAgent.MissionPeer.GetComponent<MissionPeer>();
				this._dataSource.IncreaseSpectatorCount(component);
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00012681 File Offset: 0x00010881
		private void MissionLobbyComponentOnCurrentMultiplayerStateChanged(MissionLobbyComponent.MultiplayerGameState newState)
		{
			this._isMissionEnding = newState == 2;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001268D File Offset: 0x0001088D
		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine)
			{
				this.FinalizeLayer();
				this.InitializeLayer();
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x000126A3 File Offset: 0x000108A3
		private void FinalizeLayer()
		{
			this._dataSource.OnFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource = null;
			this._isActive = false;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x000126D8 File Offset: 0x000108D8
		private void InitializeLayer()
		{
			this._dataSource = new MissionScoreboardVM(this._isSingleTeam, base.Mission);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerScoreboard", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ScoreboardHotKeyCategory"));
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._dataSource.IsActive = this._isActive;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00012780 File Offset: 0x00010980
		private void OnSelectingTeam(List<Team> disableTeams)
		{
			this.ToggleScoreboard(false);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00012789 File Offset: 0x00010989
		private void OnCultureSelectionRequested()
		{
			this.ToggleScoreboard(false);
		}

		// Token: 0x040001B5 RID: 437
		private GauntletLayer _gauntletLayer;

		// Token: 0x040001B6 RID: 438
		private MissionScoreboardVM _dataSource;

		// Token: 0x040001B7 RID: 439
		private bool _isSingleTeam;

		// Token: 0x040001B8 RID: 440
		private bool _isActive;

		// Token: 0x040001B9 RID: 441
		private bool _isMissionEnding;

		// Token: 0x040001BA RID: 442
		private bool _mouseRequstedWhileScoreboardActive;

		// Token: 0x040001BB RID: 443
		private bool _isMouseVisible;

		// Token: 0x040001BC RID: 444
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x040001BD RID: 445
		private MultiplayerTeamSelectComponent _teamSelectComponent;

		// Token: 0x040001BE RID: 446
		public Action<bool> OnScoreboardToggled;

		// Token: 0x040001BF RID: 447
		private float _scoreboardStayDuration;

		// Token: 0x040001C0 RID: 448
		private float _scoreboardStayTimeElapsed;
	}
}
