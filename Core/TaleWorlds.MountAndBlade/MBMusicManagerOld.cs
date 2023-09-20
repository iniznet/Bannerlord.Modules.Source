using System;
using psai.net;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C0 RID: 448
	public static class MBMusicManagerOld
	{
		// Token: 0x060019D4 RID: 6612 RVA: 0x0005C0F3 File Offset: 0x0005A2F3
		public static void Initialize()
		{
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0005C0F5 File Offset: 0x0005A2F5
		public static void SetMood(MBMusicManagerOld.MusicMood moodType, float intensity, bool holdIntensity, bool immediately = false)
		{
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0005C0F8 File Offset: 0x0005A2F8
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

		// Token: 0x060019D7 RID: 6615 RVA: 0x0005C12A File Offset: 0x0005A32A
		public static void Update(float dt)
		{
			PsaiCore.Instance.Update();
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x0005C137 File Offset: 0x0005A337
		public static void StopMusic(bool immediately)
		{
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0005C13C File Offset: 0x0005A33C
		public static float GetCurrentIntensity()
		{
			if (PsaiCore.IsInstanceInitialized())
			{
				return PsaiCore.Instance.GetPsaiInfo().currentIntensity;
			}
			return 0f;
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0005C168 File Offset: 0x0005A368
		public static void EnterMenuMode(int menuThemeID)
		{
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x0005C16A File Offset: 0x0005A36A
		public static void LeaveMenuMode()
		{
		}

		// Token: 0x0200051B RID: 1307
		public enum MusicMood
		{
			// Token: 0x04001BE3 RID: 7139
			None = -1,
			// Token: 0x04001BE4 RID: 7140
			LocationStandardDay = 1,
			// Token: 0x04001BE5 RID: 7141
			AseraiTeaser = 4,
			// Token: 0x04001BE6 RID: 7142
			Arena = 6,
			// Token: 0x04001BE7 RID: 7143
			CombatTurnsOutNegative = 9,
			// Token: 0x04001BE8 RID: 7144
			CombatMediumSize,
			// Token: 0x04001BE9 RID: 7145
			CombatTurnsOutPositive,
			// Token: 0x04001BEA RID: 7146
			CombatSmallSize,
			// Token: 0x04001BEB RID: 7147
			CombatSiege,
			// Token: 0x04001BEC RID: 7148
			CombatNegativeEvent,
			// Token: 0x04001BED RID: 7149
			CombatPositiveEvent,
			// Token: 0x04001BEE RID: 7150
			BattaniaTeaser = 17,
			// Token: 0x04001BEF RID: 7151
			SturgiaTeaser = 19,
			// Token: 0x04001BF0 RID: 7152
			KhuzaitTeaser = 21,
			// Token: 0x04001BF1 RID: 7153
			EmpireTeaser = 23,
			// Token: 0x04001BF2 RID: 7154
			VlandiaTeaser = 25,
			// Token: 0x04001BF3 RID: 7155
			BattleDefeated,
			// Token: 0x04001BF4 RID: 7156
			CombatPaganA
		}
	}
}
