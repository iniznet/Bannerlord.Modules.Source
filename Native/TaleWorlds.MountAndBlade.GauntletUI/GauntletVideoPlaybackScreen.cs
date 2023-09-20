using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.VideoPlayback;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	[GameStateScreen(typeof(VideoPlaybackState))]
	public class GauntletVideoPlaybackScreen : VideoPlaybackScreen
	{
		public GauntletVideoPlaybackScreen(VideoPlaybackState videoPlaybackState)
			: base(videoPlaybackState)
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			string subtitleExtensionOfLanguage = LocalizedTextManager.GetSubtitleExtensionOfLanguage(BannerlordConfig.Language);
			string text = this._videoPlaybackState.SubtitleFileBasePath + "_" + subtitleExtensionOfLanguage + ".srt";
			List<SRTHelper.SubtitleItem> list = null;
			if (!string.IsNullOrEmpty(this._videoPlaybackState.SubtitleFileBasePath))
			{
				if (File.Exists(text))
				{
					list = SRTHelper.SrtParser.ParseStream(new FileStream(text, FileMode.Open, FileAccess.Read), Encoding.UTF8);
				}
				else
				{
					Debug.FailedAssert("No Subtitle file exists in path: " + text, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletVideoPlaybackScreen.cs", "OnInitialize", 41);
				}
			}
			this._layer = new GauntletLayer(100002, "GauntletLayer", false);
			this._dataSource = new VideoPlaybackVM();
			this._layer.LoadMovie("VideoPlayer", this._dataSource);
			this._dataSource.SetSubtitles(list);
			this._layer.InputRestrictions.SetInputRestrictions(false, 7);
			base.AddLayer(this._layer);
		}

		protected override void OnVideoPlaybackTick(float dt)
		{
			base.OnVideoPlaybackTick(dt);
			this._dataSource.Tick(this._totalElapsedTimeSinceVideoStart);
		}

		private GauntletLayer _layer;

		private VideoPlaybackVM _dataSource;
	}
}
