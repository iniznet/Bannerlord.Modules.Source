using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.VideoPlayback
{
	public class VideoPlaybackVM : ViewModel
	{
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

		public void SetSubtitles(List<SRTHelper.SubtitleItem> lines)
		{
			this.subTitleLines = lines;
			this.SubtitleText = string.Empty;
		}

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

		private List<SRTHelper.SubtitleItem> subTitleLines;

		private string _subtitleText;
	}
}
