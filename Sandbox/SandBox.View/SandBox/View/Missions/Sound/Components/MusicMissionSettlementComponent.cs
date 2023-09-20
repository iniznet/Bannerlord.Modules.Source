using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicMissionSettlementComponent : MusicMissionPeacefulComponent
	{
		public override void PreInitialize()
		{
			base.PreInitialize();
			base.PeacefulTracks.Add(1);
			this.CurrentMood = base.SelectNewPeacefulTrack();
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (MBMusicManagerOld.GetCurrentMood() != this.CurrentMood)
			{
				MBMusicManagerOld.SetMood(this.CurrentMood, 0.1f, true, true);
			}
		}
	}
}
