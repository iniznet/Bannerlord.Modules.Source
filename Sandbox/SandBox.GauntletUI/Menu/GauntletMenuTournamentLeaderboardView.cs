using System;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	// Token: 0x0200001C RID: 28
	[OverrideView(typeof(MenuTournamentLeaderboardView))]
	public class GauntletMenuTournamentLeaderboardView : MenuView
	{
		// Token: 0x06000124 RID: 292 RVA: 0x00009124 File Offset: 0x00007324
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new TournamentLeaderboardVM
			{
				IsEnabled = true
			};
			base.Layer = new GauntletLayer(206, "GauntletLayer", false)
			{
				Name = "MenuTournamentLeaderboard"
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._movie = this._layerAsGauntletLayer.LoadMovie("GameMenuTournamentLeaderboard", this._dataSource);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			base.MenuViewContext.AddLayer(base.Layer);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000920C File Offset: 0x0000740C
		protected override void OnFinalize()
		{
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.MenuViewContext.RemoveLayer(base.Layer);
			this._movie = null;
			base.Layer = null;
			base.OnFinalize();
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00009278 File Offset: 0x00007478
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("Exit") || base.Layer.Input.IsHotKeyReleased("Confirm"))
			{
				this._dataSource.IsEnabled = false;
			}
			if (!this._dataSource.IsEnabled)
			{
				base.MenuViewContext.CloseTournamentLeaderboard();
			}
		}

		// Token: 0x04000083 RID: 131
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x04000084 RID: 132
		private TournamentLeaderboardVM _dataSource;

		// Token: 0x04000085 RID: 133
		private IGauntletMovie _movie;
	}
}
