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
	[OverrideView(typeof(CreditsScreen))]
	public class GauntletCreditsScreen : ScreenBase
	{
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

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._creditsCategory.Unload();
			this._datasource.OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer.Input.IsHotKeyPressed("Exit"))
			{
				ScreenManager.PopScreen();
			}
		}

		private CreditsVM _datasource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private SpriteCategory _creditsCategory;
	}
}
