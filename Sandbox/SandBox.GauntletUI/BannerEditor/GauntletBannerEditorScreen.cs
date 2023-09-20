using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.BannerEditor
{
	// Token: 0x0200003F RID: 63
	[GameStateScreen(typeof(BannerEditorState))]
	public class GauntletBannerEditorScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x06000272 RID: 626 RVA: 0x000126BC File Offset: 0x000108BC
		public GauntletBannerEditorScreen(BannerEditorState bannerEditorState)
		{
			LoadingWindow.EnableGlobalLoadingWindow();
			this._clan = bannerEditorState.GetClan();
			this._bannerEditorLayer = new BannerEditorView(bannerEditorState.GetCharacter(), bannerEditorState.GetClan().Banner, new ControlCharacterCreationStage(this.OnDone), new TextObject("{=WiNRdfsm}Done", null), new ControlCharacterCreationStage(this.OnCancel), new TextObject("{=3CpNUnVl}Cancel", null), null, null, null, null, null);
			this._bannerEditorLayer.DataSource.SetClanRelatedRules(bannerEditorState.GetClan().Kingdom == null);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0001274D File Offset: 0x0001094D
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._bannerEditorLayer.OnTick(dt);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00012764 File Offset: 0x00010964
		public void OnDone()
		{
			uint primaryColor = this._bannerEditorLayer.DataSource.BannerVM.GetPrimaryColor();
			uint sigilColor = this._bannerEditorLayer.DataSource.BannerVM.GetSigilColor();
			this._clan.Color = primaryColor;
			this._clan.Color2 = sigilColor;
			this._clan.UpdateBannerColor(primaryColor, sigilColor);
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x000127D2 File Offset: 0x000109D2
		public void OnCancel()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000127E4 File Offset: 0x000109E4
		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000127FC File Offset: 0x000109FC
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._bannerEditorLayer.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0001282B File Offset: 0x00010A2B
		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._bannerEditorLayer.GauntletLayer);
			base.AddLayer(this._bannerEditorLayer.SceneLayer);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00012855 File Offset: 0x00010A55
		protected override void OnDeactivate()
		{
			this._bannerEditorLayer.OnDeactivate();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00012862 File Offset: 0x00010A62
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00012864 File Offset: 0x00010A64
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00012866 File Offset: 0x00010A66
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00012868 File Offset: 0x00010A68
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x04000182 RID: 386
		private const int ViewOrderPriority = 15;

		// Token: 0x04000183 RID: 387
		private readonly BannerEditorView _bannerEditorLayer;

		// Token: 0x04000184 RID: 388
		private readonly Clan _clan;
	}
}
