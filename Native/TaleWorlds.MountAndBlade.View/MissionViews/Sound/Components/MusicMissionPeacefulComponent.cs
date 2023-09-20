using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	// Token: 0x0200005E RID: 94
	public abstract class MusicMissionPeacefulComponent : MusicBaseComponent
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x000217B8 File Offset: 0x0001F9B8
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x000217C0 File Offset: 0x0001F9C0
		protected MBList<MBMusicManagerOld.MusicMood> PeacefulTracks { get; set; }

		// Token: 0x06000429 RID: 1065 RVA: 0x000217C9 File Offset: 0x0001F9C9
		protected MusicMissionPeacefulComponent()
		{
			this.PeacefulTracks = new MBList<MBMusicManagerOld.MusicMood>();
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x000217DC File Offset: 0x0001F9DC
		protected MBMusicManagerOld.MusicMood SelectNewPeacefulTrack()
		{
			MBMusicManagerOld.MusicMood musicMood = this.CurrentMood;
			if (this.PeacefulTracks.Count((MBMusicManagerOld.MusicMood x) => x != this.CurrentMood) >= 1)
			{
				musicMood = Extensions.GetRandomElementWithPredicate<MBMusicManagerOld.MusicMood>(this.PeacefulTracks, (MBMusicManagerOld.MusicMood x) => x != this.CurrentMood);
			}
			return musicMood;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00021823 File Offset: 0x0001FA23
		public override MusicPriority GetPriority()
		{
			return MusicPriority.MissionLow;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00021826 File Offset: 0x0001FA26
		public override bool IsActive()
		{
			return true;
		}
	}
}
