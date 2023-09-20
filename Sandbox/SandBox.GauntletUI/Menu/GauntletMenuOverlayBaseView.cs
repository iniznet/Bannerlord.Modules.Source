using System;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	// Token: 0x0200001A RID: 26
	[OverrideView(typeof(MenuOverlayBaseView))]
	public class GauntletMenuOverlayBaseView : MenuView
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00008B14 File Offset: 0x00006D14
		protected override void OnInitialize()
		{
			GameOverlays.MenuOverlayType menuOverlayType = Campaign.Current.GameMenuManager.GetMenuOverlayType(base.MenuContext);
			this._overlayDataSource = GameMenuOverlay.GetOverlay(menuOverlayType);
			base.Layer = new GauntletLayer(200, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._layerAsGauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
			base.MenuViewContext.AddLayer(base.Layer);
			if (this._overlayDataSource is EncounterMenuOverlayVM)
			{
				this._layerAsGauntletLayer.LoadMovie("EncounterOverlay", this._overlayDataSource);
			}
			else if (this._overlayDataSource is SettlementMenuOverlayVM)
			{
				this._layerAsGauntletLayer.LoadMovie("SettlementOverlay", this._overlayDataSource);
			}
			else if (this._overlayDataSource is ArmyMenuOverlayVM)
			{
				Debug.FailedAssert("Trying to open army overlay in menu. Should be opened in map overlay", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Menu\\GauntletMenuOverlayBaseView.cs", "OnInitialize", 46);
			}
			else
			{
				Debug.FailedAssert("Game menu overlay not supported in gauntlet overlay", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Menu\\GauntletMenuOverlayBaseView.cs", "OnInitialize", 50);
			}
			base.OnInitialize();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00008C34 File Offset: 0x00006E34
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource != null)
			{
				overlayDataSource.OnFrameTick(dt);
			}
			if (ScreenManager.TopScreen is MapScreen && this._overlayDataSource != null)
			{
				GameMenuOverlay overlayDataSource2 = this._overlayDataSource;
				MapScreen mapScreen = ScreenManager.TopScreen as MapScreen;
				overlayDataSource2.IsInfoBarExtended = mapScreen != null && mapScreen.IsBarExtended;
			}
			if (!this._isContextMenuEnabled && this._overlayDataSource.IsContextMenuEnabled)
			{
				this._isContextMenuEnabled = true;
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
			}
			else if (this._isContextMenuEnabled && !this._overlayDataSource.IsContextMenuEnabled)
			{
				this._isContextMenuEnabled = false;
				base.Layer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(base.Layer);
			}
			if (this._isContextMenuEnabled && base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				this._overlayDataSource.IsContextMenuEnabled = false;
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008D22 File Offset: 0x00006F22
		protected override void OnHourlyTick()
		{
			base.OnHourlyTick();
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.Refresh();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008D3A File Offset: 0x00006F3A
		protected override void OnOverlayTypeChange(GameOverlays.MenuOverlayType newType)
		{
			base.OnOverlayTypeChange(newType);
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.UpdateOverlayType(newType);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00008D54 File Offset: 0x00006F54
		protected override void OnActivate()
		{
			base.OnActivate();
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.Refresh();
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00008D6C File Offset: 0x00006F6C
		protected override void OnFinalize()
		{
			base.MenuViewContext.RemoveLayer(base.Layer);
			this._overlayDataSource.OnFinalize();
			this._overlayDataSource = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			base.OnFinalize();
		}

		// Token: 0x0400007D RID: 125
		private GameMenuOverlay _overlayDataSource;

		// Token: 0x0400007E RID: 126
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x0400007F RID: 127
		private bool _isContextMenuEnabled;
	}
}
