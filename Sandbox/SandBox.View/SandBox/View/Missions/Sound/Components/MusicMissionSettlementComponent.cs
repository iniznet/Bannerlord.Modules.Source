using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x0200002A RID: 42
	public class MusicMissionSettlementComponent : MusicMissionPeacefulComponent
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00010E7F File Offset: 0x0000F07F
		public override void PreInitialize()
		{
			base.PreInitialize();
			base.PeacefulTracks.Add(1);
			this.CurrentMood = base.SelectNewPeacefulTrack();
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00010E9F File Offset: 0x0000F09F
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
