using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000093 RID: 147
	[EngineClass("rglVideo_player_view")]
	public sealed class VideoPlayerView : View
	{
		// Token: 0x06000B55 RID: 2901 RVA: 0x0000C7E5 File Offset: 0x0000A9E5
		internal VideoPlayerView(UIntPtr meshPointer)
			: base(meshPointer)
		{
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0000C7EE File Offset: 0x0000A9EE
		public static VideoPlayerView CreateVideoPlayerView()
		{
			return EngineApplicationInterface.IVideoPlayerView.CreateVideoPlayerView();
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0000C7FA File Offset: 0x0000A9FA
		public void PlayVideo(string videoFileName, string soundFileName, float framerate)
		{
			EngineApplicationInterface.IVideoPlayerView.PlayVideo(base.Pointer, videoFileName, soundFileName, framerate);
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0000C80F File Offset: 0x0000AA0F
		public void StopVideo()
		{
			EngineApplicationInterface.IVideoPlayerView.StopVideo(base.Pointer);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0000C821 File Offset: 0x0000AA21
		public bool IsVideoFinished()
		{
			return EngineApplicationInterface.IVideoPlayerView.IsVideoFinished(base.Pointer);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0000C833 File Offset: 0x0000AA33
		public void FinalizePlayer()
		{
			EngineApplicationInterface.IVideoPlayerView.Finalize(base.Pointer);
		}
	}
}
