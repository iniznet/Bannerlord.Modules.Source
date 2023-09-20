using System;
using SandBox.Tournaments.MissionLogics;
using SandBox.View.Missions.Tournaments;
using SandBox.ViewModelCollection.Tournament;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Missions
{
	[OverrideView(typeof(MissionTournamentView))]
	public class MissionGauntletTournamentView : MissionView
	{
		public MissionGauntletTournamentView()
		{
			this.ViewOrderPriority = 48;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new TournamentVM(new Action(this.DisableUi), this._behavior);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._gauntletMovie = this._gauntletLayer.LoadMovie("Tournament", this._dataSource);
			base.MissionScreen.CustomCamera = this._customCamera;
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._gauntletMovie = null;
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			base.OnMissionScreenFinalize();
		}

		public override void AfterStart()
		{
			this._behavior = base.Mission.GetMissionBehavior<TournamentBehavior>();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("camera_instance");
			this._customCamera = Camera.CreateCamera();
			Vec3 vec = default(Vec3);
			gameEntity.GetCameraParamsFromCameraScript(this._customCamera, ref vec);
		}

		public override void OnMissionTick(float dt)
		{
			if (this._behavior == null)
			{
				return;
			}
			if (this._gauntletLayer.IsFocusLayer && this._dataSource.IsCurrentMatchActive)
			{
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
			}
			else if (!this._gauntletLayer.IsFocusLayer && !this._dataSource.IsCurrentMatchActive)
			{
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
			if (this._dataSource.IsBetWindowEnabled)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this._dataSource.ExecuteBet();
					this._dataSource.IsBetWindowEnabled = false;
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					this._dataSource.IsBetWindowEnabled = false;
				}
			}
			if (!this._viewEnabled && ((this._behavior.LastMatch != null && this._behavior.CurrentMatch == null) || this._behavior.CurrentMatch.IsReady))
			{
				this._dataSource.Refresh();
				this.ShowUi();
			}
			if (!this._viewEnabled && this._dataSource.CurrentMatch.IsValid)
			{
				TournamentMatch currentMatch = this._behavior.CurrentMatch;
				if (currentMatch != null && currentMatch.State == 1)
				{
					this._dataSource.CurrentMatch.RefreshActiveMatch();
				}
			}
			if (this._dataSource.IsOver && this._viewEnabled && !base.DebugInput.IsControlDown() && base.DebugInput.IsHotKeyPressed("ShowHighlightsSummary"))
			{
				HighlightsController missionBehavior = base.Mission.GetMissionBehavior<HighlightsController>();
				if (missionBehavior == null)
				{
					return;
				}
				missionBehavior.ShowSummary();
			}
		}

		private void DisableUi()
		{
			if (!this._viewEnabled)
			{
				return;
			}
			base.MissionScreen.UpdateFreeCamera(this._customCamera.Frame);
			base.MissionScreen.CustomCamera = null;
			this._viewEnabled = false;
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
		}

		private void ShowUi()
		{
			if (this._viewEnabled)
			{
				return;
			}
			base.MissionScreen.CustomCamera = this._customCamera;
			this._viewEnabled = true;
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
		}

		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !this._viewEnabled;
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			this._dataSource.OnAgentRemoved(affectedAgent);
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		private TournamentBehavior _behavior;

		private Camera _customCamera;

		private bool _viewEnabled = true;

		private IGauntletMovie _gauntletMovie;

		private GauntletLayer _gauntletLayer;

		private TournamentVM _dataSource;
	}
}
