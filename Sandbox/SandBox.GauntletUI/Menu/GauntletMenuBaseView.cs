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
	// Token: 0x02000019 RID: 25
	[OverrideView(typeof(MenuBaseView))]
	public class GauntletMenuBaseView : MenuView
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000088B2 File Offset: 0x00006AB2
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000088BA File Offset: 0x00006ABA
		public GameMenuVM GameMenuDataSource { get; private set; }

		// Token: 0x0600010C RID: 268 RVA: 0x000088C4 File Offset: 0x00006AC4
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

		// Token: 0x0600010D RID: 269 RVA: 0x000089B1 File Offset: 0x00006BB1
		protected override void OnActivate()
		{
			base.OnActivate();
			this.GameMenuDataSource.Refresh(true);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000089C5 File Offset: 0x00006BC5
		protected override void OnResume()
		{
			base.OnResume();
			this.GameMenuDataSource.Refresh(true);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000089DC File Offset: 0x00006BDC
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

		// Token: 0x06000110 RID: 272 RVA: 0x00008A32 File Offset: 0x00006C32
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.GameMenuDataSource.OnFrameTick();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00008A46 File Offset: 0x00006C46
		protected override void OnMapConversationActivated()
		{
			base.OnMapConversationActivated();
			GauntletLayer layerAsGauntletLayer = this._layerAsGauntletLayer;
			if (((layerAsGauntletLayer != null) ? layerAsGauntletLayer._gauntletUIContext : null) != null)
			{
				this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00008A77 File Offset: 0x00006C77
		protected override void OnMapConversationDeactivated()
		{
			base.OnMapConversationDeactivated();
			GauntletLayer layerAsGauntletLayer = this._layerAsGauntletLayer;
			if (((layerAsGauntletLayer != null) ? layerAsGauntletLayer._gauntletUIContext : null) != null)
			{
				this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00008AA8 File Offset: 0x00006CA8
		protected override void OnMenuContextUpdated(MenuContext newMenuContext)
		{
			base.OnMenuContextUpdated(newMenuContext);
			this.GameMenuDataSource.UpdateMenuContext(newMenuContext);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00008ABD File Offset: 0x00006CBD
		protected override void OnBackgroundMeshNameSet(string name)
		{
			base.OnBackgroundMeshNameSet(name);
			this.GameMenuDataSource.Background = name;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00008AD2 File Offset: 0x00006CD2
		private void GainGamepadNavigationAfterSeconds(float seconds)
		{
			this._layerAsGauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterTime(seconds, () => this.GameMenuDataSource.ItemList.Count > 0);
		}

		// Token: 0x0400007B RID: 123
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x0400007C RID: 124
		private IGauntletMovie _movie;
	}
}
