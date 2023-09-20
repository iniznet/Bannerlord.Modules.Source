using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	public abstract class MusicMissionPeacefulComponent : MusicBaseComponent
	{
		protected MBList<MBMusicManagerOld.MusicMood> PeacefulTracks { get; set; }

		protected MusicMissionPeacefulComponent()
		{
			this.PeacefulTracks = new MBList<MBMusicManagerOld.MusicMood>();
		}

		protected MBMusicManagerOld.MusicMood SelectNewPeacefulTrack()
		{
			MBMusicManagerOld.MusicMood musicMood = this.CurrentMood;
			if (this.PeacefulTracks.Count((MBMusicManagerOld.MusicMood x) => x != this.CurrentMood) >= 1)
			{
				musicMood = Extensions.GetRandomElementWithPredicate<MBMusicManagerOld.MusicMood>(this.PeacefulTracks, (MBMusicManagerOld.MusicMood x) => x != this.CurrentMood);
			}
			return musicMood;
		}

		public override MusicPriority GetPriority()
		{
			return MusicPriority.MissionLow;
		}

		public override bool IsActive()
		{
			return true;
		}
	}
}
