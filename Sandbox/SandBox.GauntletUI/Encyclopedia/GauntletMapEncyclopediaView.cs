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
	// Token: 0x02000036 RID: 54
	[OverrideView(typeof(MapEncyclopediaView))]
	public class GauntletMapEncyclopediaView : MapEncyclopediaView
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x0000DB58 File Offset: 0x0000BD58
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

		// Token: 0x060001E3 RID: 483 RVA: 0x0000DC0E File Offset: 0x0000BE0E
		internal void OnTick(float dt)
		{
			EncyclopediaData encyclopediaData = this._encyclopediaData;
			if (encyclopediaData == null)
			{
				return;
			}
			encyclopediaData.OnTick();
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000DC20 File Offset: 0x0000BE20
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

		// Token: 0x060001E5 RID: 485 RVA: 0x0000DCC0 File Offset: 0x0000BEC0
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

		// Token: 0x060001E6 RID: 486 RVA: 0x0000DD21 File Offset: 0x0000BF21
		public override void CloseEncyclopedia()
		{
			this._encyclopediaData.CloseEncyclopedia();
			this._encyclopediaData = null;
			base.IsEncyclopediaOpen = false;
		}

		// Token: 0x040000FF RID: 255
		private EncyclopediaHomeVM _homeDatasource;

		// Token: 0x04000100 RID: 256
		private EncyclopediaNavigatorVM _navigatorDatasource;

		// Token: 0x04000101 RID: 257
		private EncyclopediaData _encyclopediaData;

		// Token: 0x04000102 RID: 258
		public EncyclopediaListViewDataController ListViewDataController;

		// Token: 0x04000103 RID: 259
		private SpriteCategory _spriteCategory;

		// Token: 0x04000104 RID: 260
		private Game _game;
	}
}
