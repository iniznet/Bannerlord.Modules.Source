using System;
using System.Diagnostics;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000015 RID: 21
	public class LauncherNewsItemVM : ViewModel
	{
		// Token: 0x060000BA RID: 186 RVA: 0x0000480C File Offset: 0x00002A0C
		public LauncherNewsItemVM(NewsItem item, bool isMultiplayer)
		{
			this.Category = item.Title;
			this.Title = item.Description;
			this.NewsImageUrl = item.ImageSourcePath;
			this._link = item.NewsLink + (isMultiplayer ? "?referrer=launchermp" : "?referrer=launchersp");
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00004867 File Offset: 0x00002A67
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000488E File Offset: 0x00002A8E
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00004896 File Offset: 0x00002A96
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

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000048B9 File Offset: 0x00002AB9
		// (set) Token: 0x060000BF RID: 191 RVA: 0x000048C1 File Offset: 0x00002AC1
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

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000048E4 File Offset: 0x00002AE4
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000048EC File Offset: 0x00002AEC
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

		// Token: 0x0400005F RID: 95
		private string _link;

		// Token: 0x04000060 RID: 96
		private string _newsImageUrl;

		// Token: 0x04000061 RID: 97
		private string _category;

		// Token: 0x04000062 RID: 98
		private string _title;
	}
}
