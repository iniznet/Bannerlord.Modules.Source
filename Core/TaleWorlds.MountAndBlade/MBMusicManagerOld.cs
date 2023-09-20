using System;
using psai.net;

namespace TaleWorlds.MountAndBlade
{
	public static class MBMusicManagerOld
	{
		public static void Initialize()
		{
		}

		public static void SetMood(MBMusicManagerOld.MusicMood moodType, float intensity, bool holdIntensity, bool immediately = false)
		{
		}

		public static MBMusicManagerOld.MusicMood GetCurrentMood()
		{
			if (PsaiCore.IsInstanceInitialized())
			{
				PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
				if (psaiInfo.psaiState == PsaiState.playing)
				{
					return (MBMusicManagerOld.MusicMood)psaiInfo.effectiveThemeId;
				}
			}
			return MBMusicManagerOld.MusicMood.None;
		}

		public static void Update(float dt)
		{
			PsaiCore.Instance.Update();
		}

		public static void StopMusic(bool immediately)
		{
		}

		public static float GetCurrentIntensity()
		{
			if (PsaiCore.IsInstanceInitialized())
			{
				return PsaiCore.Instance.GetPsaiInfo().currentIntensity;
			}
			return 0f;
		}

		public static void EnterMenuMode(int menuThemeID)
		{
		}

		public static void LeaveMenuMode()
		{
		}

		public enum MusicMood
		{
			None = -1,
			LocationStandardDay = 1,
			AseraiTeaser = 4,
			Arena = 6,
			CombatTurnsOutNegative = 9,
			CombatMediumSize,
			CombatTurnsOutPositive,
			CombatSmallSize,
			CombatSiege,
			CombatNegativeEvent,
			CombatPositiveEvent,
			BattaniaTeaser = 17,
			SturgiaTeaser = 19,
			KhuzaitTeaser = 21,
			EmpireTeaser = 23,
			VlandiaTeaser = 25,
			BattleDefeated,
			CombatPaganA
		}
	}
}
