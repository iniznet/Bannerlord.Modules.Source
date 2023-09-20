using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000012 RID: 18
	public class LoadingWindowManager : GlobalLayer, ILoadingWindowManager
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00004D78 File Offset: 0x00002F78
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

		// Token: 0x0600007F RID: 127 RVA: 0x00004E2B File Offset: 0x0000302B
		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			this._loadingWindowViewModel.Update();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004E3F File Offset: 0x0000303F
		void ILoadingWindowManager.EnableLoadingWindow()
		{
			this._loadingWindowViewModel.Enabled = true;
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004E76 File Offset: 0x00003076
		void ILoadingWindowManager.DisableLoadingWindow()
		{
			this._loadingWindowViewModel.Enabled = false;
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004EAC File Offset: 0x000030AC
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

		// Token: 0x06000083 RID: 131 RVA: 0x00004F50 File Offset: 0x00003150
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

		// Token: 0x04000054 RID: 84
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000055 RID: 85
		private LoadingWindowViewModel _loadingWindowViewModel;

		// Token: 0x04000056 RID: 86
		private SpriteCategory _sploadingCategory;

		// Token: 0x04000057 RID: 87
		private SpriteCategory _mpLoadingCategory;

		// Token: 0x04000058 RID: 88
		private SpriteCategory _mpBackgroundCategory;

		// Token: 0x04000059 RID: 89
		private bool _isMultiplayer;
	}
}
