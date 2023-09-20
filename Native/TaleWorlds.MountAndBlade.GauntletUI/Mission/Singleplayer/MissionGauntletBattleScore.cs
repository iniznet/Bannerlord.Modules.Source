using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x0200002E RID: 46
	[OverrideView(typeof(MissionBattleScoreUIHandler))]
	public class MissionGauntletBattleScore : MissionView
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600022F RID: 559 RVA: 0x0000C105 File Offset: 0x0000A305
		public ScoreboardBaseVM DataSource
		{
			get
			{
				return this._dataSource;
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000C10D File Offset: 0x0000A30D
		public MissionGauntletBattleScore(ScoreboardBaseVM scoreboardVM)
		{
			this._dataSource = scoreboardVM;
			this.ViewOrderPriority = 15;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000C124 File Offset: 0x0000A324
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.Mission.IsFriendlyMission = false;
			this._dataSource.Initialize(base.MissionScreen, base.Mission, null, new Action<bool>(this.ToggleScoreboard));
			this._isSiegeScoreboard = base.Mission.HasMissionBehavior<SiegeDeploymentMissionController>();
			this.CreateView();
			ScoreboardBaseVM dataSource = this._dataSource;
			ScoreboardHotkeys scoreboardHotkeys = default(ScoreboardHotkeys);
			scoreboardHotkeys.ShowMouseHotkey = HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetGameKey(35);
			scoreboardHotkeys.ShowScoreboardHotkey = HotKeyManager.GetCategory("Generic").GetGameKey(4);
			scoreboardHotkeys.DoneInputKey = HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm");
			scoreboardHotkeys.FastForwardKey = HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetHotKey("ToggleFastForward");
			dataSource.SetShortcuts(scoreboardHotkeys);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000C1F8 File Offset: 0x0000A3F8
		private void CreateView()
		{
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SPScoreboard", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ScoreboardHotKeyCategory"));
			GameKeyContext category = HotKeyManager.GetCategory("ScoreboardHotKeyCategory");
			if (!base.MissionScreen.SceneLayer.Input.IsCategoryRegistered(category))
			{
				base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(category);
			}
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000C2CC File Offset: 0x0000A4CC
		public override void OnMissionScreenFinalize()
		{
			base.Mission.OnMainAgentChanged -= this.Mission_OnMainAgentChanged;
			base.MissionScreen.GetSpectatedCharacter = null;
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000C32C File Offset: 0x0000A52C
		public override bool OnEscape()
		{
			if (this._dataSource.ShowScoreboard)
			{
				this.OnClose();
				return true;
			}
			return base.OnEscape();
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000C349 File Offset: 0x0000A549
		public override void EarlyStart()
		{
			base.EarlyStart();
			base.Mission.OnMainAgentChanged += this.Mission_OnMainAgentChanged;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000C368 File Offset: 0x0000A568
		public override void OnDeploymentFinished()
		{
			this._isPreparationEnded = true;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000C371 File Offset: 0x0000A571
		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent == null)
			{
				this._dataSource.OnMainHeroDeath();
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000C38C File Offset: 0x0000A58C
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick(dt);
			if (MissionGauntletBattleScore._forceScoreboardToggle || this._dataSource.IsOver || this._dataSource.IsMainCharacterDead || TaleWorlds.InputSystem.Input.IsGamepadActive)
			{
				bool flag = this.CanOpenScoreboard() && (base.Mission.InputManager.IsGameKeyPressed(4) || this._gauntletLayer.Input.IsGameKeyPressed(4));
				if (flag && !this._dataSource.ShowScoreboard)
				{
					IBattleEndLogic battleEndLogic = base.Mission.MissionBehaviors.FirstOrDefault((MissionBehavior behavior) => behavior is IBattleEndLogic) as IBattleEndLogic;
					if (battleEndLogic != null)
					{
						battleEndLogic.SetNotificationDisabled(true);
					}
					this._toOpen = true;
				}
				if (flag && this._dataSource.ShowScoreboard)
				{
					IBattleEndLogic battleEndLogic2 = base.Mission.MissionBehaviors.FirstOrDefault((MissionBehavior behavior) => behavior is IBattleEndLogic) as IBattleEndLogic;
					if (battleEndLogic2 != null)
					{
						battleEndLogic2.SetNotificationDisabled(false);
					}
					this.OnClose();
				}
			}
			else
			{
				bool flag2 = this.CanOpenScoreboard() && (base.Mission.InputManager.IsHotKeyDown("HoldShow") || this._gauntletLayer.Input.IsHotKeyDown("HoldShow"));
				if (flag2 && !this._dataSource.ShowScoreboard)
				{
					IBattleEndLogic battleEndLogic3 = base.Mission.MissionBehaviors.FirstOrDefault((MissionBehavior behavior) => behavior is IBattleEndLogic) as IBattleEndLogic;
					if (battleEndLogic3 != null)
					{
						battleEndLogic3.SetNotificationDisabled(true);
					}
					this._toOpen = true;
				}
				if (!flag2 && this._dataSource.ShowScoreboard)
				{
					IBattleEndLogic battleEndLogic4 = base.Mission.MissionBehaviors.FirstOrDefault((MissionBehavior behavior) => behavior is IBattleEndLogic) as IBattleEndLogic;
					if (battleEndLogic4 != null)
					{
						battleEndLogic4.SetNotificationDisabled(false);
					}
					this.OnClose();
				}
			}
			if (this._toOpen && base.MissionScreen.SetDisplayDialog(true))
			{
				this.OnOpen();
			}
			if (this._dataSource.IsMainCharacterDead && !this._dataSource.IsOver && (base.Mission.InputManager.IsHotKeyReleased("ToggleFastForward") || this._gauntletLayer.Input.IsHotKeyReleased("ToggleFastForward")))
			{
				this._dataSource.IsFastForwarding = !this._dataSource.IsFastForwarding;
				this._dataSource.ExecuteFastForwardAction();
			}
			if (this._dataSource.IsOver && this._dataSource.ShowScoreboard && (base.Mission.InputManager.IsHotKeyPressed("Confirm") || this._gauntletLayer.Input.IsHotKeyPressed("Confirm")))
			{
				this._dataSource.ExecuteQuitAction();
			}
			if (this._dataSource.ShowScoreboard && !base.DebugInput.IsControlDown() && base.DebugInput.IsHotKeyPressed("ShowHighlightsSummary"))
			{
				HighlightsController missionBehavior = base.Mission.GetMissionBehavior<HighlightsController>();
				if (missionBehavior != null)
				{
					missionBehavior.ShowSummary();
				}
			}
			bool flag3 = base.Mission.InputManager.IsGameKeyPressed(35) || this._gauntletLayer.Input.IsGameKeyPressed(35);
			if (this._dataSource.ShowScoreboard && !this._isMouseEnabled && flag3)
			{
				this.SetMouseState(true);
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000C711 File Offset: 0x0000A911
		private bool CanOpenScoreboard()
		{
			return !base.MissionScreen.IsRadialMenuActive && !base.MissionScreen.IsPhotoModeEnabled && !base.Mission.IsOrderMenuOpen;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000C73D File Offset: 0x0000A93D
		private void ToggleScoreboard(bool value)
		{
			if (value)
			{
				this._toOpen = true;
				return;
			}
			this.OnClose();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000C750 File Offset: 0x0000A950
		private void OnOpen()
		{
			this._toOpen = false;
			if (this._dataSource.ShowScoreboard || (this._isSiegeScoreboard && !this._isPreparationEnded))
			{
				base.MissionScreen.SetDisplayDialog(false);
				return;
			}
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 7);
			this._dataSource.ShowScoreboard = true;
			base.MissionScreen.SetCameraLockState(true);
			if (this._dataSource.IsOver || this._dataSource.IsMainCharacterDead)
			{
				this.SetMouseState(true);
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000C7DC File Offset: 0x0000A9DC
		private void OnClose()
		{
			if (!this._dataSource.ShowScoreboard)
			{
				return;
			}
			base.MissionScreen.SetDisplayDialog(false);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._dataSource.ShowScoreboard = false;
			base.MissionScreen.SetCameraLockState(false);
			this.SetMouseState(false);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000C834 File Offset: 0x0000AA34
		private void SetMouseState(bool isEnabled)
		{
			this._gauntletLayer.IsFocusLayer = isEnabled;
			if (isEnabled)
			{
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
			else
			{
				ScreenManager.TryLoseFocus(this._gauntletLayer);
			}
			ScoreboardBaseVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.SetMouseState(isEnabled);
			}
			this._isMouseEnabled = isEnabled;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000C893 File Offset: 0x0000AA93
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000C8D0 File Offset: 0x0000AAD0
		[CommandLineFunctionality.CommandLineArgumentFunction("force_toggle", "scoreboard")]
		public static string ForceScoreboardToggle(List<string> args)
		{
			int num;
			if (args.Count == 1 && int.TryParse(args[0], out num) && (num == 0 || num == 1))
			{
				MissionGauntletBattleScore._forceScoreboardToggle = num == 1;
				return "Force Scoreboard Toggle is: " + (MissionGauntletBattleScore._forceScoreboardToggle ? "ON" : "OFF");
			}
			return "Format is: scoreboard.force_toggle 0-1";
		}

		// Token: 0x04000117 RID: 279
		private ScoreboardBaseVM _dataSource;

		// Token: 0x04000118 RID: 280
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000119 RID: 281
		private bool _isPreparationEnded;

		// Token: 0x0400011A RID: 282
		private bool _isSiegeScoreboard;

		// Token: 0x0400011B RID: 283
		private bool _toOpen;

		// Token: 0x0400011C RID: 284
		private bool _isMouseEnabled;

		// Token: 0x0400011D RID: 285
		private static bool _forceScoreboardToggle;
	}
}
