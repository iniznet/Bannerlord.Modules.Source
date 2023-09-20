using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000069 RID: 105
	public class Music
	{
		// Token: 0x06000852 RID: 2130 RVA: 0x0000844D File Offset: 0x0000664D
		public static int GetFreeMusicChannelIndex()
		{
			return EngineApplicationInterface.IMusic.GetFreeMusicChannelIndex();
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x00008459 File Offset: 0x00006659
		public static void LoadClip(int index, string pathToClip)
		{
			EngineApplicationInterface.IMusic.LoadClip(index, pathToClip);
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00008467 File Offset: 0x00006667
		public static void UnloadClip(int index)
		{
			EngineApplicationInterface.IMusic.UnloadClip(index);
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00008474 File Offset: 0x00006674
		public static bool IsClipLoaded(int index)
		{
			return EngineApplicationInterface.IMusic.IsClipLoaded(index);
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x00008481 File Offset: 0x00006681
		public static void PlayMusic(int index)
		{
			EngineApplicationInterface.IMusic.PlayMusic(index);
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x0000848E File Offset: 0x0000668E
		public static void PlayDelayed(int index, int deltaMilliseconds)
		{
			EngineApplicationInterface.IMusic.PlayDelayed(index, deltaMilliseconds);
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0000849C File Offset: 0x0000669C
		public static bool IsMusicPlaying(int index)
		{
			return EngineApplicationInterface.IMusic.IsMusicPlaying(index);
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x000084A9 File Offset: 0x000066A9
		public static void PauseMusic(int index)
		{
			EngineApplicationInterface.IMusic.PauseMusic(index);
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x000084B6 File Offset: 0x000066B6
		public static void StopMusic(int index)
		{
			EngineApplicationInterface.IMusic.StopMusic(index);
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x000084C3 File Offset: 0x000066C3
		public static void SetVolume(int index, float volume)
		{
			EngineApplicationInterface.IMusic.SetVolume(index, volume);
		}
	}
}
