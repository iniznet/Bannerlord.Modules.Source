using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002E RID: 46
	[OverrideView(typeof(MapOverlayView))]
	public class GauntletMapOverlayView : MapView
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x0000BFD9 File Offset: 0x0000A1D9
		public GauntletMapOverlayView(GameOverlays.MapOverlayType type)
		{
			this._type = type;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000BFE8 File Offset: 0x0000A1E8
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._overlayDataSource = this.GetOverlay(this._type);
			base.Layer = new GauntletLayer(201, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
			GameOverlays.MapOverlayType type = this._type;
			if (type == 1)
			{
				this._movie = this._layerAsGauntletLayer.LoadMovie("ArmyOverlay", this._overlayDataSource);
				(this._overlayDataSource as ArmyMenuOverlayVM).OpenArmyManagement = new Action(this.OpenArmyManagement);
				this._armyManagementDatasource = new ArmyManagementVM(new Action(this.CloseArmyManagement));
				this._armyManagementDatasource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
				this._armyManagementDatasource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
				this._armyManagementDatasource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			}
			else
			{
				Debug.FailedAssert("This kind of overlay not supported in gauntlet", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Map\\GauntletMapOverlayView.cs", "CreateLayout", 66);
			}
			base.MapScreen.AddLayer(base.Layer);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000C143 File Offset: 0x0000A343
		public GameMenuOverlay GetOverlay(GameOverlays.MapOverlayType mapOverlayType)
		{
			if (mapOverlayType == 1)
			{
				return new ArmyMenuOverlayVM();
			}
			Debug.FailedAssert("Game menu overlay: " + mapOverlayType.ToString() + " could not be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.GauntletUI\\Map\\GauntletMapOverlayView.cs", "GetOverlay", 79);
			return null;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000C17D File Offset: 0x0000A37D
		protected override void OnArmyLeft()
		{
			base.MapScreen.RemoveArmyOverlay();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000C18C File Offset: 0x0000A38C
		protected override void OnFinalize()
		{
			if (this._armyManagementLayer != null)
			{
				this.CloseArmyManagement();
			}
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			if (this._gauntletArmyManagementMovie != null)
			{
				this._layerAsGauntletLayer.ReleaseMovie(this._gauntletArmyManagementMovie);
			}
			base.MapScreen.RemoveLayer(base.Layer);
			this._movie = null;
			this._overlayDataSource = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			SpriteCategory armyManagementCategory = this._armyManagementCategory;
			if (armyManagementCategory != null)
			{
				armyManagementCategory.Unload();
			}
			base.OnFinalize();
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000C215 File Offset: 0x0000A415
		protected override void OnHourlyTick()
		{
			base.OnHourlyTick();
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.HourlyTick();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000C230 File Offset: 0x0000A430
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null && this._overlayDataSource != null)
			{
				this._overlayDataSource.IsInfoBarExtended = mapScreen.IsBarExtended;
			}
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.OnFrameTick(dt);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000C27C File Offset: 0x0000A47C
		protected override bool IsEscaped()
		{
			if (this._armyManagementDatasource != null)
			{
				this._armyManagementDatasource.ExecuteCancel();
				return true;
			}
			return false;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000C294 File Offset: 0x0000A494
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

		// Token: 0x060001B0 RID: 432 RVA: 0x0000C2AC File Offset: 0x0000A4AC
		protected override void OnResume()
		{
			base.OnResume();
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.Refresh();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C2C4 File Offset: 0x0000A4C4
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
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
			this.HadleArmyManagementInput();
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C374 File Offset: 0x0000A574
		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null && this._overlayDataSource != null)
			{
				this._overlayDataSource.IsInfoBarExtended = mapScreen.IsBarExtended;
			}
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource == null)
			{
				return;
			}
			overlayDataSource.OnFrameTick(dt);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000C3C0 File Offset: 0x0000A5C0
		private void OpenArmyManagement()
		{
			this._armyManagementDatasource = new ArmyManagementVM(new Action(this.CloseArmyManagement));
			this._armyManagementDatasource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._armyManagementDatasource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._armyManagementDatasource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._armyManagementDatasource.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._armyManagementCategory = spriteData.SpriteCategories["ui_armymanagement"];
			this._armyManagementCategory.Load(resourceContext, uiresourceDepot);
			this._armyManagementLayer = new GauntletLayer(300, "GauntletLayer", false);
			this._gauntletArmyManagementMovie = this._armyManagementLayer.LoadMovie("ArmyManagement", this._armyManagementDatasource);
			this._armyManagementLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
			this._armyManagementLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._armyManagementLayer);
			base.MapScreen.AddLayer(this._armyManagementLayer);
			this._timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
			Campaign.Current.TimeControlMode = 0;
			Campaign.Current.SetTimeControlModeLock(true);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInArmyManagement(true);
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000C570 File Offset: 0x0000A770
		private void CloseArmyManagement()
		{
			if (this._armyManagementLayer != null && this._gauntletArmyManagementMovie != null)
			{
				this._armyManagementLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._armyManagementLayer);
				this._armyManagementLayer.InputRestrictions.ResetInputRestrictions();
				this._armyManagementLayer.ReleaseMovie(this._gauntletArmyManagementMovie);
				base.MapScreen.RemoveLayer(this._armyManagementLayer);
			}
			ArmyManagementVM armyManagementDatasource = this._armyManagementDatasource;
			if (armyManagementDatasource != null)
			{
				armyManagementDatasource.OnFinalize();
			}
			this._gauntletArmyManagementMovie = null;
			this._armyManagementDatasource = null;
			this._armyManagementLayer = null;
			GameMenuOverlay overlayDataSource = this._overlayDataSource;
			if (overlayDataSource != null)
			{
				overlayDataSource.Refresh();
			}
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInArmyManagement(false);
			}
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(4));
			Campaign.Current.SetTimeControlModeLock(false);
			Campaign.Current.TimeControlMode = this._timeControlModeBeforeArmyManagementOpened;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000C654 File Offset: 0x0000A854
		private void HadleArmyManagementInput()
		{
			if (this._armyManagementLayer != null && this._armyManagementDatasource != null)
			{
				if (this._armyManagementLayer.Input.IsHotKeyReleased("Exit"))
				{
					this._armyManagementDatasource.ExecuteCancel();
					return;
				}
				if (this._armyManagementLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this._armyManagementDatasource.ExecuteDone();
					return;
				}
				if (this._armyManagementLayer.Input.IsHotKeyReleased("Reset"))
				{
					this._armyManagementDatasource.ExecuteReset();
					return;
				}
				if (this._armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && this._armyManagementDatasource.FocusedItem != null)
				{
					this._armyManagementDatasource.FocusedItem.ExecuteAction();
				}
			}
		}

		// Token: 0x040000DB RID: 219
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000DC RID: 220
		private GameMenuOverlay _overlayDataSource;

		// Token: 0x040000DD RID: 221
		private readonly GameOverlays.MapOverlayType _type;

		// Token: 0x040000DE RID: 222
		private IGauntletMovie _movie;

		// Token: 0x040000DF RID: 223
		private bool _isContextMenuEnabled;

		// Token: 0x040000E0 RID: 224
		private GauntletLayer _armyManagementLayer;

		// Token: 0x040000E1 RID: 225
		private SpriteCategory _armyManagementCategory;

		// Token: 0x040000E2 RID: 226
		private ArmyManagementVM _armyManagementDatasource;

		// Token: 0x040000E3 RID: 227
		private IGauntletMovie _gauntletArmyManagementMovie;

		// Token: 0x040000E4 RID: 228
		private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;
	}
}
