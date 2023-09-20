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
	// Token: 0x02000017 RID: 23
	[OverrideView(typeof(MissionTournamentView))]
	public class MissionGauntletTournamentView : MissionView
	{
		// Token: 0x060000FC RID: 252 RVA: 0x00008362 File Offset: 0x00006562
		public MissionGauntletTournamentView()
		{
			this.ViewOrderPriority = 48;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000837C File Offset: 0x0000657C
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

		// Token: 0x060000FE RID: 254 RVA: 0x00008484 File Offset: 0x00006684
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

		// Token: 0x060000FF RID: 255 RVA: 0x000084E0 File Offset: 0x000066E0
		public override void AfterStart()
		{
			this._behavior = base.Mission.GetMissionBehavior<TournamentBehavior>();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("camera_instance");
			this._customCamera = Camera.CreateCamera();
			Vec3 vec = default(Vec3);
			gameEntity.GetCameraParamsFromCameraScript(this._customCamera, ref vec);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008534 File Offset: 0x00006734
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

		// Token: 0x06000101 RID: 257 RVA: 0x00008704 File Offset: 0x00006904
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

		// Token: 0x06000102 RID: 258 RVA: 0x00008753 File Offset: 0x00006953
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

		// Token: 0x06000103 RID: 259 RVA: 0x00008788 File Offset: 0x00006988
		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !this._viewEnabled;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00008793 File Offset: 0x00006993
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			this._dataSource.OnAgentRemoved(affectedAgent);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000087AC File Offset: 0x000069AC
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000087C9 File Offset: 0x000069C9
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x04000073 RID: 115
		private TournamentBehavior _behavior;

		// Token: 0x04000074 RID: 116
		private Camera _customCamera;

		// Token: 0x04000075 RID: 117
		private bool _viewEnabled = true;

		// Token: 0x04000076 RID: 118
		private IGauntletMovie _gauntletMovie;

		// Token: 0x04000077 RID: 119
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000078 RID: 120
		private TournamentVM _dataSource;
	}
}
