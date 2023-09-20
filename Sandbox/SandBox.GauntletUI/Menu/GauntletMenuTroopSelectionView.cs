using System;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	[OverrideView(typeof(MenuTroopSelectionView))]
	public class GauntletMenuTroopSelectionView : MenuView
	{
		public GauntletMenuTroopSelectionView(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> changeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			this._onDone = onDone;
			this._fullRoster = fullRoster;
			this._initialSelections = initialSelections;
			this._changeChangeStatusOfTroop = changeChangeStatusOfTroop;
			this._maxSelectableTroopCount = maxSelectableTroopCount;
			this._minSelectableTroopCount = minSelectableTroopCount;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new GameMenuTroopSelectionVM(this._fullRoster, this._initialSelections, this._changeChangeStatusOfTroop, new Action<TroopRoster>(this.OnDone), this._maxSelectableTroopCount, this._minSelectableTroopCount)
			{
				IsEnabled = true
			};
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			base.Layer = new GauntletLayer(206, "GauntletLayer", false)
			{
				Name = "MenuTroopSelection"
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._movie = this._layerAsGauntletLayer.LoadMovie("GameMenuTroopSelection", this._dataSource);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._layerAsGauntletLayer);
			base.MenuViewContext.AddLayer(base.Layer);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInHideoutTroopManage(true);
			}
		}

		private void OnDone(TroopRoster obj)
		{
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			base.MenuViewContext.CloseTroopSelection();
			Action<TroopRoster> onDone = this._onDone;
			if (onDone == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onDone, new object[] { obj });
		}

		protected override void OnFinalize()
		{
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.MenuViewContext.RemoveLayer(base.Layer);
			this._movie = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			base.OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._dataSource != null)
			{
				this._dataSource.IsFiveStackModifierActive = base.Layer.Input.IsHotKeyDown("FiveStackModifier");
				this._dataSource.IsEntireStackModifierActive = base.Layer.Input.IsHotKeyDown("EntireStackModifier");
			}
			ScreenLayer layer = base.Layer;
			if (layer != null && layer.Input.IsHotKeyPressed("Exit"))
			{
				this._dataSource.ExecuteCancel();
			}
			else
			{
				ScreenLayer layer2 = base.Layer;
				if (layer2 != null && layer2.Input.IsHotKeyPressed("Confirm"))
				{
					this._dataSource.ExecuteDone();
				}
				else
				{
					ScreenLayer layer3 = base.Layer;
					if (layer3 != null && layer3.Input.IsHotKeyPressed("Reset"))
					{
						this._dataSource.ExecuteReset();
					}
				}
			}
			GameMenuTroopSelectionVM dataSource = this._dataSource;
			if (dataSource != null && !dataSource.IsEnabled)
			{
				base.MenuViewContext.CloseTroopSelection();
			}
		}

		private readonly Action<TroopRoster> _onDone;

		private readonly TroopRoster _fullRoster;

		private readonly TroopRoster _initialSelections;

		private readonly Func<CharacterObject, bool> _changeChangeStatusOfTroop;

		private readonly int _maxSelectableTroopCount;

		private readonly int _minSelectableTroopCount;

		private GauntletLayer _layerAsGauntletLayer;

		private GameMenuTroopSelectionVM _dataSource;

		private IGauntletMovie _movie;
	}
}
