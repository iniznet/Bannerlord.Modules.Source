using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Credits;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000004 RID: 4
	[OverrideView(typeof(CreditsScreen))]
	public class GauntletCreditsScreen : ScreenBase
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002AAC File Offset: 0x00000CAC
		protected override void OnInitialize()
		{
			base.OnInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._creditsCategory = spriteData.SpriteCategories["ui_credits"];
			this._creditsCategory.Load(resourceContext, uiresourceDepot);
			this._datasource = new CreditsVM();
			string text = ModuleHelper.GetModuleFullPath("Native") + "ModuleData/" + "Credits.xml";
			this._datasource.FillFromFile(text);
			this._gauntletLayer = new GauntletLayer(100, "GauntletLayer", false);
			this._gauntletLayer.IsFocusLayer = true;
			base.AddLayer(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._movie = this._gauntletLayer.LoadMovie("CreditsScreen", this._datasource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002BA8 File Offset: 0x00000DA8
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._creditsCategory.Unload();
			this._datasource.OnFinalize();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002BC6 File Offset: 0x00000DC6
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyPressed("Exit"))
			{
				ScreenManager.PopScreen();
			}
		}

		// Token: 0x0400000E RID: 14
		private CreditsVM _datasource;

		// Token: 0x0400000F RID: 15
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000010 RID: 16
		private IGauntletMovie _movie;

		// Token: 0x04000011 RID: 17
		private SpriteCategory _creditsCategory;
	}
}
