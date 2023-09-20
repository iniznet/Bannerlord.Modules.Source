using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000193 RID: 403
	public abstract class MapTrackModel : GameModel
	{
		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06001A05 RID: 6661
		public abstract float MaxTrackLife { get; }

		// Token: 0x06001A06 RID: 6662
		public abstract float GetSkipTrackChance(MobileParty mobileParty);

		// Token: 0x06001A07 RID: 6663
		public abstract float GetMaxTrackSpottingDistanceForMainParty();

		// Token: 0x06001A08 RID: 6664
		public abstract bool CanPartyLeaveTrack(MobileParty mobileParty);

		// Token: 0x06001A09 RID: 6665
		public abstract float GetTrackDetectionDifficultyForMainParty(Track track, float trackSpottingDistance);

		// Token: 0x06001A0A RID: 6666
		public abstract float GetSkillFromTrackDetected(Track track);

		// Token: 0x06001A0B RID: 6667
		public abstract int GetTrackLife(MobileParty mobileParty);

		// Token: 0x06001A0C RID: 6668
		public abstract TextObject TrackTitle(Track track);

		// Token: 0x06001A0D RID: 6669
		public abstract IEnumerable<ValueTuple<TextObject, string>> GetTrackDescription(Track track);

		// Token: 0x06001A0E RID: 6670
		public abstract uint GetTrackColor(Track track);

		// Token: 0x06001A0F RID: 6671
		public abstract float GetTrackScale(Track track);
	}
}
