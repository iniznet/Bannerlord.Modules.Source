using System;
using System.Diagnostics;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	// Token: 0x0200007E RID: 126
	public class MPNewsItemVM : ViewModel
	{
		// Token: 0x06000B31 RID: 2865 RVA: 0x00027740 File Offset: 0x00025940
		public MPNewsItemVM(NewsItem item)
		{
			this.NewsImageUrl = item.ImageSourcePath;
			this.Category = item.Title;
			this.Title = item.Description;
			this._link = item.NewsLink + "?referrer=lobby";
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00027791 File Offset: 0x00025991
		private void ExecuteOpenLink()
		{
			if (!string.IsNullOrEmpty(this._link) && !PlatformServices.Instance.ShowOverlayForWebPage(this._link).Result)
			{
				Process.Start(new ProcessStartInfo(this._link)
				{
					UseShellExecute = true
				});
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x000277CF File Offset: 0x000259CF
		// (set) Token: 0x06000B34 RID: 2868 RVA: 0x000277D7 File Offset: 0x000259D7
		[DataSourceProperty]
		public string NewsImageUrl
		{
			get
			{
				return this._newsImageUrl;
			}
			set
			{
				if (value != this._newsImageUrl)
				{
					this._newsImageUrl = value;
					base.OnPropertyChangedWithValue<string>(value, "NewsImageUrl");
				}
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000B35 RID: 2869 RVA: 0x000277FA File Offset: 0x000259FA
		// (set) Token: 0x06000B36 RID: 2870 RVA: 0x00027802 File Offset: 0x00025A02
		[DataSourceProperty]
		public string Category
		{
			get
			{
				return this._category;
			}
			set
			{
				if (value != this._category)
				{
					this._category = value;
					base.OnPropertyChangedWithValue<string>(value, "Category");
				}
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x00027825 File Offset: 0x00025A25
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x0002782D File Offset: 0x00025A2D
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x04000560 RID: 1376
		private readonly string _link;

		// Token: 0x04000561 RID: 1377
		private string _newsImageUrl;

		// Token: 0x04000562 RID: 1378
		private string _category;

		// Token: 0x04000563 RID: 1379
		private string _title;
	}
}
