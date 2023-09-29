using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Scoreboard;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MissionScoreboardUIHandler))]
	public class MissionGauntletMultiplayerScoreboard : MissionView
	{
		[UsedImplicitly]
		public MissionGauntletMultiplayerScoreboard(bool isSingleTeam)
		{
			this._isSingleTeam = isSingleTeam;
			this.ViewOrderPriority = 25;
		}

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
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._scoreboardStayDuration = MissionLobbyComponent.PostMatchWaitDuration / 2f;
			this._teamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			this.RegisterEvents();
			if (this._dataSource != null)
			{
				this._dataSource.IsActive = false;
			}
		}

		public override void OnRemoveBehavior()
		{
			this.UnregisterEvents();
			this.FinalizeLayer();
			base.OnRemoveBehavior();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this.UnregisterEvents();
			this.FinalizeLayer();
			base.OnMissionScreenFinalize();
		}

		private void RegisterEvents()
		{
			if (base.MissionScreen != null)
			{
				base.MissionScreen.OnSpectateAgentFocusIn += new MissionScreen.OnSpectateAgentDelegate(this.HandleSpectateAgentFocusIn);
				base.MissionScreen.OnSpectateAgentFocusOut += new MissionScreen.OnSpectateAgentDelegate(this.HandleSpectateAgentFocusOut);
			}
			this._missionLobbyComponent.CurrentMultiplayerStateChanged += this.MissionLobbyComponentOnCurrentMultiplayerStateChanged;
			this._missionLobbyComponent.OnCultureSelectionRequested += this.OnCultureSelectionRequested;
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam += new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
		}

		private void UnregisterEvents()
		{
			if (base.MissionScreen != null)
			{
				base.MissionScreen.OnSpectateAgentFocusIn -= new MissionScreen.OnSpectateAgentDelegate(this.HandleSpectateAgentFocusIn);
				base.MissionScreen.OnSpectateAgentFocusOut -= new MissionScreen.OnSpectateAgentDelegate(this.HandleSpectateAgentFocusOut);
			}
			this._missionLobbyComponent.CurrentMultiplayerStateChanged -= this.MissionLobbyComponentOnCurrentMultiplayerStateChanged;
			this._missionLobbyComponent.OnCultureSelectionRequested -= this.OnCultureSelectionRequested;
			if (this._teamSelectComponent != null)
			{
				this._teamSelectComponent.OnSelectingTeam -= new MultiplayerTeamSelectComponent.OnSelectingTeamDelegate(this.OnSelectingTeam);
			}
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
		}

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
			if (Input.IsGamepadActive)
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

		private void HandleSpectateAgentFocusOut(Agent followedAgent)
		{
			if (followedAgent.MissionPeer != null)
			{
				MissionPeer component = followedAgent.MissionPeer.GetComponent<MissionPeer>();
				this._dataSource.DecreaseSpectatorCount(component);
			}
		}

		private void HandleSpectateAgentFocusIn(Agent followedAgent)
		{
			if (followedAgent.MissionPeer != null)
			{
				MissionPeer component = followedAgent.MissionPeer.GetComponent<MissionPeer>();
				this._dataSource.IncreaseSpectatorCount(component);
			}
		}

		private void MissionLobbyComponentOnCurrentMultiplayerStateChanged(MissionLobbyComponent.MultiplayerGameState newState)
		{
			this._isMissionEnding = newState == 2;
		}

		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine)
			{
				this.FinalizeLayer();
				this.InitializeLayer();
			}
		}

		private void FinalizeLayer()
		{
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
			}
			if (this._gauntletLayer != null)
			{
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
			}
			this._gauntletLayer = null;
			this._dataSource = null;
			this._isActive = false;
		}

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

		private void OnSelectingTeam(List<Team> disableTeams)
		{
			this.ToggleScoreboard(false);
		}

		private void OnCultureSelectionRequested()
		{
			this.ToggleScoreboard(false);
		}

		private GauntletLayer _gauntletLayer;

		private MissionScoreboardVM _dataSource;

		private bool _isSingleTeam;

		private bool _isActive;

		private bool _isMissionEnding;

		private bool _mouseRequstedWhileScoreboardActive;

		private bool _isMouseVisible;

		private MissionLobbyComponent _missionLobbyComponent;

		private MultiplayerTeamSelectComponent _teamSelectComponent;

		public Action<bool> OnScoreboardToggled;

		private float _scoreboardStayDuration;

		private float _scoreboardStayTimeElapsed;
	}
}
