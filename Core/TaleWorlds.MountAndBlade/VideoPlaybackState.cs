using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000236 RID: 566
	public class VideoPlaybackState : GameState
	{
		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001F33 RID: 7987 RVA: 0x0006EE30 File Offset: 0x0006D030
		// (set) Token: 0x06001F34 RID: 7988 RVA: 0x0006EE38 File Offset: 0x0006D038
		public string VideoPath { get; private set; }

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001F35 RID: 7989 RVA: 0x0006EE41 File Offset: 0x0006D041
		// (set) Token: 0x06001F36 RID: 7990 RVA: 0x0006EE49 File Offset: 0x0006D049
		public string AudioPath { get; private set; }

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001F37 RID: 7991 RVA: 0x0006EE52 File Offset: 0x0006D052
		// (set) Token: 0x06001F38 RID: 7992 RVA: 0x0006EE5A File Offset: 0x0006D05A
		public float FrameRate { get; private set; }

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06001F39 RID: 7993 RVA: 0x0006EE63 File Offset: 0x0006D063
		// (set) Token: 0x06001F3A RID: 7994 RVA: 0x0006EE6B File Offset: 0x0006D06B
		public string SubtitleFileBasePath { get; private set; }

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06001F3B RID: 7995 RVA: 0x0006EE74 File Offset: 0x0006D074
		// (set) Token: 0x06001F3C RID: 7996 RVA: 0x0006EE7C File Offset: 0x0006D07C
		public bool CanUserSkip { get; private set; }

		// Token: 0x06001F3D RID: 7997 RVA: 0x0006EE85 File Offset: 0x0006D085
		public void SetStartingParameters(string videoPath, string audioPath, string subtitleFileBasePath, float frameRate = 30f, bool canUserSkip = true)
		{
			this.VideoPath = videoPath;
			this.AudioPath = audioPath;
			this.FrameRate = frameRate;
			this.SubtitleFileBasePath = subtitleFileBasePath;
			this.CanUserSkip = canUserSkip;
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x0006EEAC File Offset: 0x0006D0AC
		public void SetOnVideoFinisedDelegate(Action onVideoFinised)
		{
			this._onVideoFinised = onVideoFinised;
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x0006EEB5 File Offset: 0x0006D0B5
		public void OnVideoFinished()
		{
			Action onVideoFinised = this._onVideoFinised;
			if (onVideoFinised == null)
			{
				return;
			}
			onVideoFinised();
		}

		// Token: 0x04000B63 RID: 2915
		private Action _onVideoFinised;
	}
}
