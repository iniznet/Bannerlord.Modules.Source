using System;
using System.Diagnostics;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherNewsItemVM : ViewModel
	{
		public LauncherNewsItemVM(NewsItem item, bool isMultiplayer)
		{
			this.Category = item.Title;
			this.Title = item.Description;
			this.NewsImageUrl = item.ImageSourcePath;
			this._link = item.NewsLink + (isMultiplayer ? "?referrer=launchermp" : "?referrer=launchersp");
		}

		private void ExecuteOpenLink()
		{
			if (!string.IsNullOrEmpty(this._link))
			{
				Process.Start(new ProcessStartInfo(this._link)
				{
					UseShellExecute = true
				});
			}
		}

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

		private string _link;

		private string _newsImageUrl;

		private string _category;

		private string _title;
	}
}
