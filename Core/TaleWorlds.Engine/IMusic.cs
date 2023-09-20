using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003B RID: 59
	[ApplicationInterfaceBase]
	internal interface IMusic
	{
		// Token: 0x06000526 RID: 1318
		[EngineMethod("get_free_music_channel_index", false)]
		int GetFreeMusicChannelIndex();

		// Token: 0x06000527 RID: 1319
		[EngineMethod("load_clip", false)]
		void LoadClip(int index, string pathToClip);

		// Token: 0x06000528 RID: 1320
		[EngineMethod("unload_clip", false)]
		void UnloadClip(int index);

		// Token: 0x06000529 RID: 1321
		[EngineMethod("is_clip_loaded", false)]
		bool IsClipLoaded(int index);

		// Token: 0x0600052A RID: 1322
		[EngineMethod("play_music", false)]
		void PlayMusic(int index);

		// Token: 0x0600052B RID: 1323
		[EngineMethod("play_delayed", false)]
		void PlayDelayed(int index, int delayMilliseconds);

		// Token: 0x0600052C RID: 1324
		[EngineMethod("is_music_playing", false)]
		bool IsMusicPlaying(int index);

		// Token: 0x0600052D RID: 1325
		[EngineMethod("pause_music", false)]
		void PauseMusic(int index);

		// Token: 0x0600052E RID: 1326
		[EngineMethod("stop_music", false)]
		void StopMusic(int index);

		// Token: 0x0600052F RID: 1327
		[EngineMethod("set_volume", false)]
		void SetVolume(int index, float volume);
	}
}
