using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Barter;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions
{
	// Token: 0x02000013 RID: 19
	[OverrideView(typeof(BarterView))]
	public class MissionGauntletBarterView : MissionView
	{
		// Token: 0x060000BC RID: 188 RVA: 0x00007048 File Offset: 0x00005248
		public override void AfterStart()
		{
			base.AfterStart();
			this._barter = Campaign.Current.BarterManager;
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Combine(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Combine(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000070BC File Offset: 0x000052BC
		private void OnBarterBegin(BarterData args)
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._barterCategory = spriteData.SpriteCategories["ui_barter"];
			this._barterCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new BarterVM(args);
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "Barter", false);
			this._gauntletLayer.LoadMovie("BarterScreen", this._dataSource);
			GameKeyContext category = HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory");
			this._gauntletLayer.Input.RegisterHotKeyCategory(category);
			GameKeyContext category2 = HotKeyManager.GetCategory("GenericPanelGameKeyCategory");
			this._gauntletLayer.Input.RegisterHotKeyCategory(category2);
			this._gauntletLayer.IsFocusLayer = true;
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "SceneLayer", "Barter" }, true);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000722C File Offset: 0x0000542C
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			BarterItemVM.IsFiveStackModifierActive = this.IsDownInGauntletLayer("FiveStackModifier");
			BarterItemVM.IsEntireStackModifierActive = this.IsDownInGauntletLayer("EntireStackModifier");
			if (this.IsReleasedInGauntletLayer("Confirm"))
			{
				if (!this._dataSource.IsOfferDisabled)
				{
					this._dataSource.ExecuteOffer();
					return;
				}
			}
			else
			{
				if (this.IsReleasedInGauntletLayer("Exit"))
				{
					this._dataSource.ExecuteCancel();
					return;
				}
				if (this.IsReleasedInGauntletLayer("Reset"))
				{
					this._dataSource.ExecuteReset();
				}
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000072B7 File Offset: 0x000054B7
		private bool IsDownInGauntletLayer(string hotKeyID)
		{
			GauntletLayer gauntletLayer = this._gauntletLayer;
			return gauntletLayer != null && gauntletLayer.Input.IsHotKeyDown(hotKeyID);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000072D0 File Offset: 0x000054D0
		private bool IsReleasedInGauntletLayer(string hotKeyID)
		{
			GauntletLayer gauntletLayer = this._gauntletLayer;
			return gauntletLayer != null && gauntletLayer.Input.IsHotKeyReleased(hotKeyID);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000072EC File Offset: 0x000054EC
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			BarterManager barter = this._barter;
			barter.BarterBegin = (BarterManager.BarterBeginEventDelegate)Delegate.Remove(barter.BarterBegin, new BarterManager.BarterBeginEventDelegate(this.OnBarterBegin));
			BarterManager barter2 = this._barter;
			barter2.Closed = (BarterManager.BarterCloseEventDelegate)Delegate.Remove(barter2.Closed, new BarterManager.BarterCloseEventDelegate(this.OnBarterClosed));
			this._gauntletLayer = null;
			BarterVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000736C File Offset: 0x0000556C
		private void OnBarterClosed()
		{
			base.MissionScreen.SetLayerCategoriesState(new string[] { "Barter" }, false);
			base.MissionScreen.SetLayerCategoriesState(new string[] { "Conversation" }, true);
			base.MissionScreen.SetLayerCategoriesState(new string[] { "SceneLayer" }, true);
			BarterItemVM.IsFiveStackModifierActive = false;
			BarterItemVM.IsEntireStackModifierActive = false;
			this._barterCategory.Unload();
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource = null;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00007424 File Offset: 0x00005624
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00007449 File Offset: 0x00005649
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x0400005B RID: 91
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400005C RID: 92
		private BarterVM _dataSource;

		// Token: 0x0400005D RID: 93
		private BarterManager _barter;

		// Token: 0x0400005E RID: 94
		private SpriteCategory _barterCategory;
	}
}
