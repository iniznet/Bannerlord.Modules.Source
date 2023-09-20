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
	[GameStateScreen(typeof(BannerEditorState))]
	public class GauntletBannerEditorScreen : ScreenBase, IGameStateListener
	{
		public GauntletBannerEditorScreen(BannerEditorState bannerEditorState)
		{
			LoadingWindow.EnableGlobalLoadingWindow();
			this._clan = bannerEditorState.GetClan();
			this._bannerEditorLayer = new BannerEditorView(bannerEditorState.GetCharacter(), bannerEditorState.GetClan().Banner, new ControlCharacterCreationStage(this.OnDone), new TextObject("{=WiNRdfsm}Done", null), new ControlCharacterCreationStage(this.OnCancel), new TextObject("{=3CpNUnVl}Cancel", null), null, null, null, null, null);
			this._bannerEditorLayer.DataSource.SetClanRelatedRules(bannerEditorState.GetClan().Kingdom == null);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._bannerEditorLayer.OnTick(dt);
		}

		public void OnDone()
		{
			uint primaryColor = this._bannerEditorLayer.DataSource.BannerVM.GetPrimaryColor();
			uint sigilColor = this._bannerEditorLayer.DataSource.BannerVM.GetSigilColor();
			this._clan.Color = primaryColor;
			this._clan.Color2 = sigilColor;
			this._clan.UpdateBannerColor(primaryColor, sigilColor);
			Game.Current.GameStateManager.PopState(0);
		}

		public void OnCancel()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

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

		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._bannerEditorLayer.GauntletLayer);
			base.AddLayer(this._bannerEditorLayer.SceneLayer);
		}

		protected override void OnDeactivate()
		{
			this._bannerEditorLayer.OnDeactivate();
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

		private const int ViewOrderPriority = 15;

		private readonly BannerEditorView _bannerEditorLayer;

		private readonly Clan _clan;
	}
}
