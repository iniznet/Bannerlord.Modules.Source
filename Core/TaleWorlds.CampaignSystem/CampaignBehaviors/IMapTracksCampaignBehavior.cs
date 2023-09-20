using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200039F RID: 927
	public interface IMapTracksCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x06003736 RID: 14134
		MBReadOnlyList<Track> DetectedTracks { get; }

		// Token: 0x06003737 RID: 14135
		void AddTrack(MobileParty target, Vec2 trackPosition, Vec2 trackDirection);

		// Token: 0x06003738 RID: 14136
		void AddMapArrow(TextObject pointerName, Vec2 trackPosition, Vec2 trackDirection, float life);
	}
}
