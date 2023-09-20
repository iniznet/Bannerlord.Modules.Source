using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	public class MPNewsVM : ViewModel
	{
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

		private async void GetNewsItems()
		{
			await this._newsManager.GetNewsItems(false);
			this.RefreshNews();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this._newsManager = null;
		}

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

		private NewsManager _newsManager;

		private const int _numOfNewsItemsToShow = 4;

		private bool _hasValidNews;

		private MPNewsItemVM _mainNews;

		private MBBindingList<MPNewsItemVM> _importantNews;
	}
}
