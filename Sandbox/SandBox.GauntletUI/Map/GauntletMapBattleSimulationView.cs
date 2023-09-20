using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(BattleSimulationMapView))]
	public class GauntletMapBattleSimulationView : MapView
	{
		public GauntletMapBattleSimulationView(BattleSimulation battleSimulation)
		{
			this._battleSimulation = battleSimulation;
		}

		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new SPScoreboardVM(this._battleSimulation);
			this._dataSource.Initialize(null, null, new Action(base.MapState.EndBattleSimulation), null);
			ScoreboardBaseVM dataSource = this._dataSource;
			ScoreboardHotkeys scoreboardHotkeys = default(ScoreboardHotkeys);
			scoreboardHotkeys.ShowMouseHotkey = null;
			scoreboardHotkeys.ShowScoreboardHotkey = null;
			scoreboardHotkeys.DoneInputKey = HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm");
			scoreboardHotkeys.FastForwardKey = HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetHotKey("ToggleFastForward");
			dataSource.SetShortcuts(scoreboardHotkeys);
			base.Layer = new GauntletLayer(101, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ScoreboardHotKeyCategory"));
			this._gauntletMovie = this._layerAsGauntletLayer.LoadMovie("SPScoreboard", this._dataSource);
			this._dataSource.ExecutePlayAction();
			base.Layer.IsFocusLayer = true;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.MapScreen.AddLayer(base.Layer);
			ScreenManager.TrySetFocus(base.Layer);
		}

		protected override void OnFinalize()
		{
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer.IsFocusLayer = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
			ScreenManager.TryLoseFocus(base.Layer);
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._gauntletMovie = null;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._dataSource != null && base.Layer != null)
			{
				if (!this._dataSource.IsOver && base.Layer.Input.IsHotKeyReleased("ToggleFastForward"))
				{
					this._dataSource.IsFastForwarding = !this._dataSource.IsFastForwarding;
					this._dataSource.ExecuteFastForwardAction();
					return;
				}
				if (this._dataSource.IsOver && this._dataSource.ShowScoreboard && base.Layer.Input.IsHotKeyPressed("Confirm"))
				{
					this._dataSource.ExecuteQuitAction();
				}
			}
		}

		private GauntletLayer _layerAsGauntletLayer;

		private IGauntletMovie _gauntletMovie;

		private SPScoreboardVM _dataSource;

		private readonly BattleSimulation _battleSimulation;
	}
}
