using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicMissionAlleyFightComponent : MusicMissionActionComponent
	{
		public MusicMissionAlleyFightComponent()
		{
			this.TrackUpdateInterval = 1f;
		}

		public override void PreInitialize()
		{
			base.PreInitialize();
			base.ActionTracks.Add(Tuple.Create<MBMusicManagerOld.MusicMood, string>(12, "small"));
			this.CurrentMood = base.SelectNewActionTrack();
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

		public override bool IsActive()
		{
			return Mission.Current != null && Mission.Current.Mode == 2;
		}
	}
}
