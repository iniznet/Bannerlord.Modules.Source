using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicMissionTavernComponent : MusicMissionPeacefulComponent
	{
		public override void PreInitialize()
		{
			base.PreInitialize();
			this.CurrentMood = -1;
			MBMusicManagerOld.StopMusic(false);
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
