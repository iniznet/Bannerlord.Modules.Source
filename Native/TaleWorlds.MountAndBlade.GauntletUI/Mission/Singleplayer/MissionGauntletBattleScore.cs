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
	[OverrideView(typeof(MissionBattleScoreUIHandler))]
	public class MissionGauntletBattleScore : MissionView
	{
		public ScoreboardBaseVM DataSource
		{
			get
			{
				return this._dataSource;
			}
		}

		public MissionGauntletBattleScore(ScoreboardBaseVM scoreboardVM)
		{
			this._dataSource = scoreboardVM;
			this.ViewOrderPriority = 15;
		}

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

		public override bool OnEscape()
		{
			if (this._dataSource.ShowScoreboard)
			{
				this.OnClose();
				return true;
			}
			return base.OnEscape();
		}

		public override void EarlyStart()
		{
			base.EarlyStart();
			base.Mission.OnMainAgentChanged += this.Mission_OnMainAgentChanged;
		}

		public override void OnDeploymentFinished()
		{
			this._isPreparationEnded = true;
		}

		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent == null)
			{
				this._dataSource.OnMainHeroDeath();
			}
		}

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

		private bool CanOpenScoreboard()
		{
			return !base.MissionScreen.IsRadialMenuActive && !base.MissionScreen.IsPhotoModeEnabled && !base.Mission.IsOrderMenuOpen;
		}

		private void ToggleScoreboard(bool value)
		{
			if (value)
			{
				this._toOpen = true;
				return;
			}
			this.OnClose();
		}

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

		private ScoreboardBaseVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private bool _isPreparationEnded;

		private bool _isSiegeScoreboard;

		private bool _toOpen;

		private bool _isMouseEnabled;

		private static bool _forceScoreboardToggle;
	}
}
