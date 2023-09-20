using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x0200002B RID: 43
	public class MusicMissionTavernComponent : MusicMissionPeacefulComponent
	{
		// Token: 0x0600015F RID: 351 RVA: 0x00010ECF File Offset: 0x0000F0CF
		public override void PreInitialize()
		{
			base.PreInitialize();
			this.CurrentMood = -1;
			MBMusicManagerOld.StopMusic(false);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00010EE4 File Offset: 0x0000F0E4
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
