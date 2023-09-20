using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	// Token: 0x0200007F RID: 127
	public class MPNewsVM : ViewModel
	{
		// Token: 0x06000B39 RID: 2873 RVA: 0x00027850 File Offset: 0x00025A50
		public MPNewsVM(NewsManager newsManager)
		{
			this._newsManager = newsManager;
			this.ImportantNews = new MBBindingList<MPNewsItemVM>();
			this.GetNewsItems();
			if (this.MainNews != null)
			{
				this.HasValidNews = true;
			}
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x00027880 File Offset: 0x00025A80
		private async void GetNewsItems()
		{
			await this._newsManager.GetNewsItems(false);
			this.RefreshNews();
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x000278BC File Offset: 0x00025ABC
		private void RefreshNews()
		{
			this.ImportantNews.Clear();
			List<IGrouping<int, NewsItem>> list = (from i in (from i in this._newsManager.NewsItems.Where((NewsItem n) => n.Feeds.Any((NewsType t) => t.Type == NewsItem.NewsTypes.MultiplayerLobby) && !string.IsNullOrEmpty(n.Title) && !string.IsNullOrEmpty(n.NewsLink) && !string.IsNullOrEmpty(n.ImageSourcePath)).ToList<NewsItem>()
					group i by i.Feeds.First((NewsType t) => t.Type == NewsItem.NewsTypes.MultiplayerLobby).Index).ToList<IGrouping<int, NewsItem>>()
				orderby i.Key
				select i).ToList<IGrouping<int, NewsItem>>();
			int num = 0;
			while (num < list.Count && this.ImportantNews.Count + 1 < 4)
			{
				NewsItem newsItem = list[num].First<NewsItem>();
				NewsItem newsItem2 = (newsItem.Equals(default(NewsItem)) ? default(NewsItem) : newsItem);
				if (num == 0)
				{
					this.MainNews = new MPNewsItemVM(newsItem2);
				}
				else
				{
					this.ImportantNews.Add(new MPNewsItemVM(newsItem2));
				}
				num++;
			}
			if (this.MainNews != null)
			{
				this.HasValidNews = true;
			}
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x000279E6 File Offset: 0x00025BE6
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._newsManager = null;
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x000279F5 File Offset: 0x00025BF5
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x000279FD File Offset: 0x00025BFD
		[DataSourceProperty]
		public bool HasValidNews
		{
			get
			{
				return this._hasValidNews;
			}
			set
			{
				if (value != this._hasValidNews)
				{
					this._hasValidNews = value;
					base.OnPropertyChangedWithValue(value, "HasValidNews");
				}
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x00027A1B File Offset: 0x00025C1B
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x00027A23 File Offset: 0x00025C23
		[DataSourceProperty]
		public MPNewsItemVM MainNews
		{
			get
			{
				return this._mainNews;
			}
			set
			{
				if (value != this._mainNews)
				{
					this._mainNews = value;
					base.OnPropertyChangedWithValue<MPNewsItemVM>(value, "MainNews");
				}
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x00027A41 File Offset: 0x00025C41
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x00027A49 File Offset: 0x00025C49
		[DataSourceProperty]
		public MBBindingList<MPNewsItemVM> ImportantNews
		{
			get
			{
				return this._importantNews;
			}
			set
			{
				if (value != this._importantNews)
				{
					this._importantNews = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPNewsItemVM>>(value, "ImportantNews");
				}
			}
		}

		// Token: 0x04000564 RID: 1380
		private NewsManager _newsManager;

		// Token: 0x04000565 RID: 1381
		private const int _numOfNewsItemsToShow = 4;

		// Token: 0x04000566 RID: 1382
		private bool _hasValidNews;

		// Token: 0x04000567 RID: 1383
		private MPNewsItemVM _mainNews;

		// Token: 0x04000568 RID: 1384
		private MBBindingList<MPNewsItemVM> _importantNews;
	}
}
