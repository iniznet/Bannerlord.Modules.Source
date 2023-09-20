using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x0200002C RID: 44
	public class MusicMissionTournamentComponent : MusicMissionActionComponent
	{
		// Token: 0x06000162 RID: 354 RVA: 0x00010F14 File Offset: 0x0000F114
		public MusicMissionTournamentComponent()
		{
			this.TrackUpdateInterval = 1f;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00010F28 File Offset: 0x0000F128
		public override void PreInitialize()
		{
			base.PreInitialize();
			base.ActionTracks.Add(Tuple.Create<MBMusicManagerOld.MusicMood, string>(12, "small"));
			base.NegativeTracks.Add(9);
			base.PositiveTracks.Add(11);
			base.BattleLostTracks.Add(26);
			base.BattleWonTracks.Add("battania", 17);
			base.BattleWonTracks.Add("sturgia", 19);
			base.BattleWonTracks.Add("khuzait", 21);
			base.BattleWonTracks.Add("empire", 23);
			base.BattleWonTracks.Add("vlandia", 25);
			base.BattleWonTracks.Add("aserai", 4);
			this.CurrentMood = 6;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00010FEC File Offset: 0x0000F1EC
		protected override MBMusicManagerOld.MusicMood HandleNormalTrackSelection(bool forceUpdate = false)
		{
			MBMusicManagerOld.MusicMood musicMood;
			if (Mission.Current.Agents.Count == 0)
			{
				musicMood = 6;
			}
			else
			{
				musicMood = base.SelectNewActionTrack();
			}
			if (this.CurrentMood != musicMood)
			{
				this.IsNextMoodChangeInstant = true;
			}
			return musicMood;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00011028 File Offset: 0x0000F228
		protected override float CalculateIntensityFromDamageFromPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageFromPlayer(), 0.75f);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0001103A File Offset: 0x0000F23A
		protected override float CalculateIntensityFromDamageToPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageToPlayer(), 0.75f);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0001104C File Offset: 0x0000F24C
		protected override float CalculateIntensityFromEnemiesInDuelRange()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesInDuelRange(), 0.55f);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0001105E File Offset: 0x0000F25E
		protected override float CalculateIntensityFromEnemiesAround()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesAround(), 1.5f);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00011070 File Offset: 0x0000F270
		protected override float CalculateIntensityMisc()
		{
			float num = 0f;
			if (Mission.Current.Agents.Count == 0)
			{
				num = 1E-05f;
			}
			else if (Agent.Main == null)
			{
				num = 0.2f;
			}
			return num;
		}
	}
}
