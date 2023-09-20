using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.VideoPlayback
{
	// Token: 0x0200000B RID: 11
	public class VideoPlaybackVM : ViewModel
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00004878 File Offset: 0x00002A78
		public void Tick(float totalElapsedTimeInVideoInSeconds)
		{
			if (this.subTitleLines != null)
			{
				SRTHelper.SubtitleItem itemInTimeframe = this.GetItemInTimeframe(totalElapsedTimeInVideoInSeconds);
				if (itemInTimeframe != null)
				{
					this.SubtitleText = string.Join("\n", itemInTimeframe.Lines);
					return;
				}
				this.SubtitleText = string.Empty;
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000048BC File Offset: 0x00002ABC
		public SRTHelper.SubtitleItem GetItemInTimeframe(float timeInSecondsInVideo)
		{
			int num = (int)(timeInSecondsInVideo * 1000f);
			for (int i = 0; i < this.subTitleLines.Count; i++)
			{
				if (this.subTitleLines[i].StartTime < num && this.subTitleLines[i].EndTime > num)
				{
					return this.subTitleLines[i];
				}
			}
			return null;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000491E File Offset: 0x00002B1E
		public void SetSubtitles(List<SRTHelper.SubtitleItem> lines)
		{
			this.subTitleLines = lines;
			this.SubtitleText = string.Empty;
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00004932 File Offset: 0x00002B32
		// (set) Token: 0x060000CE RID: 206 RVA: 0x0000493A File Offset: 0x00002B3A
		[DataSourceProperty]
		public string SubtitleText
		{
			get
			{
				return this._subtitleText;
			}
			set
			{
				if (value != this._subtitleText)
				{
					this._subtitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "SubtitleText");
				}
			}
		}

		// Token: 0x04000053 RID: 83
		private List<SRTHelper.SubtitleItem> subTitleLines;

		// Token: 0x04000054 RID: 84
		private string _subtitleText;
	}
}
