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
	// Token: 0x02000023 RID: 35
	[OverrideView(typeof(BattleSimulationMapView))]
	public class GauntletMapBattleSimulationView : MapView
	{
		// Token: 0x0600014F RID: 335 RVA: 0x0000A533 File Offset: 0x00008733
		public GauntletMapBattleSimulationView(BattleSimulation battleSimulation)
		{
			this._battleSimulation = battleSimulation;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000A544 File Offset: 0x00008744
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

		// Token: 0x06000151 RID: 337 RVA: 0x0000A69C File Offset: 0x0000889C
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

		// Token: 0x06000152 RID: 338 RVA: 0x0000A704 File Offset: 0x00008904
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

		// Token: 0x040000AB RID: 171
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000AC RID: 172
		private IGauntletMovie _gauntletMovie;

		// Token: 0x040000AD RID: 173
		private SPScoreboardVM _dataSource;

		// Token: 0x040000AE RID: 174
		private readonly BattleSimulation _battleSimulation;
	}
}
