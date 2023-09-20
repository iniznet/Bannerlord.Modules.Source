using System;
using System.Collections.Generic;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapEscapeMenuView))]
	public class GauntletMapEscapeMenuView : MapView
	{
		public GauntletMapEscapeMenuView(List<EscapeMenuItemVM> items)
		{
			this._menuItems = items;
		}

		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._escapeMenuDatasource = new EscapeMenuVM(this._menuItems, null);
			base.Layer = new GauntletLayer(4400, "GauntletLayer", false)
			{
				IsFocusLayer = true
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._escapeMenuMovie = this._layerAsGauntletLayer.LoadMovie("EscapeMenu", this._escapeMenuDatasource);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.MapScreen.AddLayer(base.Layer);
			base.MapScreen.PauseAmbientSounds();
			ScreenManager.TrySetFocus(base.Layer);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu") || base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				MapScreen.Instance.CloseEscapeMenu();
			}
		}

		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu") || base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				MapScreen.Instance.CloseEscapeMenu();
			}
		}

		protected override bool IsEscaped()
		{
			return base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu");
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			base.Layer.InputRestrictions.ResetInputRestrictions();
			base.MapScreen.RemoveLayer(base.Layer);
			base.MapScreen.RestartAmbientSounds();
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			this._escapeMenuDatasource = null;
			this._escapeMenuMovie = null;
		}

		private GauntletLayer _layerAsGauntletLayer;

		private EscapeMenuVM _escapeMenuDatasource;

		private IGauntletMovie _escapeMenuMovie;

		private readonly List<EscapeMenuItemVM> _menuItems;
	}
}
