using System;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	[OverrideView(typeof(MenuBaseView))]
	public class GauntletMenuBaseView : MenuView
	{
		public GameMenuVM GameMenuDataSource { get; private set; }

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.GameMenuDataSource = new GameMenuVM(base.MenuContext);
			GameKey gameKey = HotKeyManager.GetCategory("Generic").GetGameKey(4);
			this.GameMenuDataSource.AddHotKey(16, gameKey);
			base.Layer = base.MenuViewContext.FindLayer<GauntletLayer>("BasicLayer");
			if (base.Layer == null)
			{
				base.Layer = new GauntletLayer(100, "GauntletLayer", false)
				{
					Name = "BasicLayer"
				};
				base.MenuViewContext.AddLayer(base.Layer);
			}
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("GameMenu", this.GameMenuDataSource);
			ScreenManager.TrySetFocus(base.Layer);
			this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			MBInformationManager.HideInformations();
			this.GainGamepadNavigationAfterSeconds(0.25f);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.GameMenuDataSource.Refresh(true);
		}

		protected override void OnResume()
		{
			base.OnResume();
			this.GameMenuDataSource.Refresh(true);
		}

		protected override void OnFinalize()
		{
			this.GameMenuDataSource.OnFinalize();
			this.GameMenuDataSource = null;
			ScreenManager.TryLoseFocus(base.Layer);
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._movie = null;
			base.OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.GameMenuDataSource.OnFrameTick();
		}

		protected override void OnMapConversationActivated()
		{
			base.OnMapConversationActivated();
			GauntletLayer layerAsGauntletLayer = this._layerAsGauntletLayer;
			if (((layerAsGauntletLayer != null) ? layerAsGauntletLayer._gauntletUIContext : null) != null)
			{
				this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		protected override void OnMapConversationDeactivated()
		{
			base.OnMapConversationDeactivated();
			GauntletLayer layerAsGauntletLayer = this._layerAsGauntletLayer;
			if (((layerAsGauntletLayer != null) ? layerAsGauntletLayer._gauntletUIContext : null) != null)
			{
				this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		protected override void OnMenuContextUpdated(MenuContext newMenuContext)
		{
			base.OnMenuContextUpdated(newMenuContext);
			this.GameMenuDataSource.UpdateMenuContext(newMenuContext);
		}

		protected override void OnBackgroundMeshNameSet(string name)
		{
			base.OnBackgroundMeshNameSet(name);
			this.GameMenuDataSource.Background = name;
		}

		private void GainGamepadNavigationAfterSeconds(float seconds)
		{
			this._layerAsGauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterTime(seconds, () => this.GameMenuDataSource.ItemList.Count > 0);
		}

		private GauntletLayer _layerAsGauntletLayer;

		private IGauntletMovie _movie;
	}
}
