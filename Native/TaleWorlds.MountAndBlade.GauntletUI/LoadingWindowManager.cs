using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class LoadingWindowManager : GlobalLayer, ILoadingWindowManager
	{
		public LoadingWindowManager()
		{
			this._sploadingCategory = UIResourceManager.SpriteData.SpriteCategories["ui_loading"];
			this._sploadingCategory.InitializePartialLoad();
			this._loadingWindowViewModel = new LoadingWindowViewModel(new Action<bool, int>(this.HandleSPPartialLoading));
			this._loadingWindowViewModel.Enabled = false;
			this._loadingWindowViewModel.SetTotalGenericImageCount(this._sploadingCategory.SpriteSheetCount);
			bool flag = false;
			this._gauntletLayer = new GauntletLayer(100003, "GauntletLayer", flag);
			this._gauntletLayer.LoadMovie("LoadingWindow", this._loadingWindowViewModel);
			base.Layer = this._gauntletLayer;
			ScreenManager.AddGlobalLayer(this, false);
		}

		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			this._loadingWindowViewModel.Update();
		}

		void ILoadingWindowManager.EnableLoadingWindow()
		{
			this._loadingWindowViewModel.Enabled = true;
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
		}

		void ILoadingWindowManager.DisableLoadingWindow()
		{
			this._loadingWindowViewModel.Enabled = false;
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}

		public void SetCurrentModeIsMultiplayer(bool isMultiplayer)
		{
			if (this._isMultiplayer != isMultiplayer)
			{
				this._isMultiplayer = isMultiplayer;
				this._loadingWindowViewModel.IsMultiplayer = isMultiplayer;
				if (isMultiplayer)
				{
					this._mpLoadingCategory = UIResourceManager.SpriteData.SpriteCategories["ui_mploading"];
					this._mpLoadingCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
					this._mpBackgroundCategory = UIResourceManager.SpriteData.SpriteCategories["ui_mpbackgrounds"];
					this._mpBackgroundCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
					return;
				}
				this._mpLoadingCategory.Unload();
				this._mpBackgroundCategory.Unload();
			}
		}

		private void HandleSPPartialLoading(bool isLoading, int index)
		{
			if (isLoading)
			{
				SpriteCategory sploadingCategory = this._sploadingCategory;
				if (sploadingCategory == null)
				{
					return;
				}
				sploadingCategory.PartialLoadAtIndex(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot, index);
				return;
			}
			else
			{
				SpriteCategory sploadingCategory2 = this._sploadingCategory;
				if (sploadingCategory2 == null)
				{
					return;
				}
				sploadingCategory2.PartialUnloadAtIndex(index);
				return;
			}
		}

		private GauntletLayer _gauntletLayer;

		private LoadingWindowViewModel _loadingWindowViewModel;

		private SpriteCategory _sploadingCategory;

		private SpriteCategory _mpLoadingCategory;

		private SpriteCategory _mpBackgroundCategory;

		private bool _isMultiplayer;
	}
}
