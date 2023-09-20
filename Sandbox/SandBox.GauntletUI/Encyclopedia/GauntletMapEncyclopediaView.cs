using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Encyclopedia
{
	[OverrideView(typeof(MapEncyclopediaView))]
	public class GauntletMapEncyclopediaView : MapEncyclopediaView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			SpriteData spriteData = UIResourceManager.SpriteData;
			this._spriteCategory = spriteData.SpriteCategories["ui_encyclopedia"];
			this._spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			this._homeDatasource = new EncyclopediaHomeVM(new EncyclopediaPageArgs(null));
			this._navigatorDatasource = new EncyclopediaNavigatorVM(new Func<string, object, bool, EncyclopediaPageVM>(this.ExecuteLink), new Action(this.CloseEncyclopedia));
			this.ListViewDataController = new EncyclopediaListViewDataController();
			this._game = Game.Current;
			Game game = this._game;
			game.AfterTick = (Action<float>)Delegate.Combine(game.AfterTick, new Action<float>(this.OnTick));
		}

		internal void OnTick(float dt)
		{
			EncyclopediaData encyclopediaData = this._encyclopediaData;
			if (encyclopediaData == null)
			{
				return;
			}
			encyclopediaData.OnTick();
		}

		private EncyclopediaPageVM ExecuteLink(string pageId, object obj, bool needsRefresh)
		{
			this._navigatorDatasource.NavBarString = string.Empty;
			if (this._encyclopediaData == null)
			{
				this._encyclopediaData = new EncyclopediaData(this, ScreenManager.TopScreen, this._homeDatasource, this._navigatorDatasource);
			}
			if (pageId == "LastPage")
			{
				Tuple<string, object> lastPage = this._navigatorDatasource.GetLastPage();
				pageId = lastPage.Item1;
				obj = lastPage.Item2;
			}
			base.IsEncyclopediaOpen = true;
			if (!this._spriteCategory.IsLoaded)
			{
				this._spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			}
			return this._encyclopediaData.ExecuteLink(pageId, obj, needsRefresh);
		}

		protected override void OnFinalize()
		{
			Game game = this._game;
			game.AfterTick = (Action<float>)Delegate.Remove(game.AfterTick, new Action<float>(this.OnTick));
			this._game = null;
			this._homeDatasource = null;
			this._navigatorDatasource.OnFinalize();
			this._navigatorDatasource = null;
			this._encyclopediaData = null;
			base.OnFinalize();
		}

		public override void CloseEncyclopedia()
		{
			this._encyclopediaData.CloseEncyclopedia();
			this._encyclopediaData = null;
			base.IsEncyclopediaOpen = false;
		}

		private EncyclopediaHomeVM _homeDatasource;

		private EncyclopediaNavigatorVM _navigatorDatasource;

		private EncyclopediaData _encyclopediaData;

		public EncyclopediaListViewDataController ListViewDataController;

		private SpriteCategory _spriteCategory;

		private Game _game;
	}
}
