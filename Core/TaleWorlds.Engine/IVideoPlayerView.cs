using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000030 RID: 48
	[ApplicationInterfaceBase]
	internal interface IVideoPlayerView
	{
		// Token: 0x06000439 RID: 1081
		[EngineMethod("create_video_player_view", false)]
		VideoPlayerView CreateVideoPlayerView();

		// Token: 0x0600043A RID: 1082
		[EngineMethod("play_video", false)]
		void PlayVideo(UIntPtr pointer, string videoFileName, string soundFileName, float framerate);

		// Token: 0x0600043B RID: 1083
		[EngineMethod("stop_video", false)]
		void StopVideo(UIntPtr pointer);

		// Token: 0x0600043C RID: 1084
		[EngineMethod("is_video_finished", false)]
		bool IsVideoFinished(UIntPtr pointer);

		// Token: 0x0600043D RID: 1085
		[EngineMethod("finalize", false)]
		void Finalize(UIntPtr pointer);
	}
}
