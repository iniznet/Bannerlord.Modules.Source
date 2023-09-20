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
	// Token: 0x02000027 RID: 39
	[OverrideView(typeof(MapEscapeMenuView))]
	public class GauntletMapEscapeMenuView : MapView
	{
		// Token: 0x06000174 RID: 372 RVA: 0x0000B523 File Offset: 0x00009723
		public GauntletMapEscapeMenuView(List<EscapeMenuItemVM> items)
		{
			this._menuItems = items;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000B534 File Offset: 0x00009734
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

		// Token: 0x06000176 RID: 374 RVA: 0x0000B5F8 File Offset: 0x000097F8
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu") || base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				MapScreen.Instance.CloseEscapeMenu();
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000B644 File Offset: 0x00009844
		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu") || base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				MapScreen.Instance.CloseEscapeMenu();
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000B690 File Offset: 0x00009890
		protected override bool IsEscaped()
		{
			return base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu");
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000B6A8 File Offset: 0x000098A8
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

		// Token: 0x040000BF RID: 191
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000C0 RID: 192
		private EscapeMenuVM _escapeMenuDatasource;

		// Token: 0x040000C1 RID: 193
		private IGauntletMovie _escapeMenuMovie;

		// Token: 0x040000C2 RID: 194
		private readonly List<EscapeMenuItemVM> _menuItems;
	}
}
