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
	// Token: 0x02000010 RID: 16
	[GameStateScreen(typeof(VideoPlaybackState))]
	public class GauntletVideoPlaybackScreen : VideoPlaybackScreen
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00004ABA File Offset: 0x00002CBA
		public GauntletVideoPlaybackScreen(VideoPlaybackState videoPlaybackState)
			: base(videoPlaybackState)
		{
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004AC4 File Offset: 0x00002CC4
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

		// Token: 0x06000078 RID: 120 RVA: 0x00004BAD File Offset: 0x00002DAD
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._dataSource.Tick(this._totalElapsedTimeSinceVideoStart);
		}

		// Token: 0x0400004B RID: 75
		private GauntletLayer _layer;

		// Token: 0x0400004C RID: 76
		private VideoPlaybackVM _dataSource;
	}
}
