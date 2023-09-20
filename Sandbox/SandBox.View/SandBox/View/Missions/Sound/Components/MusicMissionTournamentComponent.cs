using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicMissionTournamentComponent : MusicMissionActionComponent
	{
		public MusicMissionTournamentComponent()
		{
			this.TrackUpdateInterval = 1f;
		}

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

		protected override float CalculateIntensityFromDamageFromPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageFromPlayer(), 0.75f);
		}

		protected override float CalculateIntensityFromDamageToPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageToPlayer(), 0.75f);
		}

		protected override float CalculateIntensityFromEnemiesInDuelRange()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesInDuelRange(), 0.55f);
		}

		protected override float CalculateIntensityFromEnemiesAround()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesAround(), 1.5f);
		}

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
