using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherNewsVM : ViewModel
	{
		public LauncherNewsVM(NewsManager newsManager, bool isDefaultMultiplayer)
		{
			this._newsManager = newsManager;
			this.NewsItems = new MBBindingList<LauncherNewsItemVM>();
			this.GetNewsItems(isDefaultMultiplayer);
			this.IsDisabledOnMultiplayer = false;
		}

		private async void GetNewsItems(bool isMultiplayer)
		{
			await this._newsManager.GetNewsItems(false);
			this.Refresh(isMultiplayer);
		}

		public void Refresh(bool isMultiplayer)
		{
			this.NewsItems.Clear();
			this.MainNews = new LauncherNewsItemVM(default(NewsItem), isMultiplayer);
			NewsItem.NewsTypes singleplayerMultiplayerEnum = (isMultiplayer ? NewsItem.NewsTypes.LauncherMultiplayer : NewsItem.NewsTypes.LauncherSingleplayer);
			Func<NewsType, bool> <>9__3;
			Func<NewsType, bool> <>9__4;
			List<IGrouping<int, NewsItem>> list = (from i in this._newsManager.NewsItems.Where(delegate(NewsItem n)
				{
					IEnumerable<NewsType> feeds = n.Feeds;
					Func<NewsType, bool> func;
					if ((func = <>9__3) == null)
					{
						func = (<>9__3 = (NewsType t) => t.Type == singleplayerMultiplayerEnum);
					}
					return feeds.Any(func) && !string.IsNullOrEmpty(n.Title) && !string.IsNullOrEmpty(n.NewsLink) && !string.IsNullOrEmpty(n.ImageSourcePath);
				}).ToList<NewsItem>().GroupBy(delegate(NewsItem i)
				{
					IEnumerable<NewsType> feeds2 = i.Feeds;
					Func<NewsType, bool> func2;
					if ((func2 = <>9__4) == null)
					{
						func2 = (<>9__4 = (NewsType t) => t.Type == singleplayerMultiplayerEnum);
					}
					return feeds2.First(func2).Index;
				})
					.ToList<IGrouping<int, NewsItem>>()
				orderby i.Key
				select i).ToList<IGrouping<int, NewsItem>>();
			int num = 0;
			while (num < list.Count && this.NewsItems.Count < 3)
			{
				NewsItem newsItem = list[num].First<NewsItem>();
				NewsItem newsItem2 = (newsItem.Equals(default(NewsItem)) ? default(NewsItem) : newsItem);
				this.NewsItems.Add(new LauncherNewsItemVM(newsItem2, isMultiplayer));
				num++;
			}
		}

		[DataSourceProperty]
		public bool IsDisabledOnMultiplayer
		{
			get
			{
				return this._isDisabledOnMultiplayer;
			}
			set
			{
				if (value != this._isDisabledOnMultiplayer)
				{
					this._isDisabledOnMultiplayer = value;
					base.OnPropertyChangedWithValue(value, "IsDisabledOnMultiplayer");
				}
			}
		}

		[DataSourceProperty]
		public LauncherNewsItemVM MainNews
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
					base.OnPropertyChangedWithValue<LauncherNewsItemVM>(value, "MainNews");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<LauncherNewsItemVM> NewsItems
		{
			get
			{
				return this._newsItems;
			}
			set
			{
				if (value != this._newsItems)
				{
					this._newsItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<LauncherNewsItemVM>>(value, "NewsItems");
				}
			}
		}

		private readonly NewsManager _newsManager;

		private const int _numOfNewsItemsToShow = 3;

		private LauncherNewsItemVM _mainNews;

		private MBBindingList<LauncherNewsItemVM> _newsItems;

		private bool _isDisabledOnMultiplayer;
	}
}
