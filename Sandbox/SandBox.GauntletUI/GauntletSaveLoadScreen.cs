using System;
using SandBox.View;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	[OverrideView(typeof(SaveLoadScreen))]
	public class GauntletSaveLoadScreen : ScreenBase
	{
		public GauntletSaveLoadScreen(bool isSaving)
		{
			this._isSaving = isSaving;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			bool flag = LinQuick.FirstOrDefaultQ<GameState>(GameStateManager.Current.GameStates, (GameState s) => s is MapState) != null;
			this._dataSource = new SaveLoadVM(this._isSaving, flag);
			this._dataSource.SetDeleteInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Delete"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			if (Game.Current != null)
			{
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			}
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_saveload"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.LoadMovie("SaveLoadScreen", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);
			if (BannerlordConfig.ForceVSyncInMenus)
			{
				Utilities.SetForceVsync(true);
			}
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (!this._dataSource.IsBusyWithAnAction)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					this._dataSource.ExecuteDone();
					UISoundsHelper.PlayUISound("event:/ui/panels/next");
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("Confirm") && !this._gauntletLayer.IsFocusedOnInput())
				{
					this._dataSource.ExecuteLoadSave();
					UISoundsHelper.PlayUISound("event:/ui/panels/next");
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("Delete") && !this._gauntletLayer.IsFocusedOnInput())
				{
					this._dataSource.DeleteSelectedSave();
					UISoundsHelper.PlayUISound("event:/ui/panels/next");
				}
			}
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (Game.Current != null)
			{
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			}
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._spriteCategory.Unload();
			Utilities.SetForceVsync(false);
		}

		private GauntletLayer _gauntletLayer;

		private SaveLoadVM _dataSource;

		private SpriteCategory _spriteCategory;

		private readonly bool _isSaving;
	}
}
