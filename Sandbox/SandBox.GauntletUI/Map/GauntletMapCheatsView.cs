using System;
using System.Collections.Generic;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Map.Cheat;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapCheatsView))]
	public class GauntletMapCheatsView : MapCheatsView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			IEnumerable<GameplayCheatBase> mapCheatList = GameplayCheatsManager.GetMapCheatList();
			this._dataSource = new GameplayCheatsVM(new Action(this.HandleClose), mapCheatList);
			this.InitializeKeyVisuals();
			base.Layer = new GauntletLayer(4500, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._layerAsGauntletLayer.LoadMovie("MapCheats", this._dataSource);
			this._layerAsGauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._layerAsGauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._layerAsGauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._layerAsGauntletLayer);
			base.MapScreen.AddLayer(this._layerAsGauntletLayer);
			base.MapScreen.SetIsMapCheatsActive(true);
			Campaign.Current.TimeControlMode = 0;
			Campaign.Current.SetTimeControlModeLock(true);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			GameplayCheatsVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._dataSource = null;
			base.MapScreen.SetIsMapCheatsActive(false);
			Campaign.Current.SetTimeControlModeLock(false);
		}

		private void HandleClose()
		{
			base.MapScreen.CloseGameplayCheats();
		}

		protected override bool IsEscaped()
		{
			return true;
		}

		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			this.HandleInput();
		}

		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			this.HandleInput();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.HandleInput();
		}

		private void HandleInput()
		{
			if (base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				GameplayCheatsVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.ExecuteClose();
			}
		}

		private void InitializeKeyVisuals()
		{
			this._dataSource.SetCloseInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
		}

		protected GauntletLayer _layerAsGauntletLayer;

		protected GameplayCheatsVM _dataSource;
	}
}
