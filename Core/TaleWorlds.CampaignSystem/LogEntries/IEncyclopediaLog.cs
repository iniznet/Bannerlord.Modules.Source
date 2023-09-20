using System;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002F8 RID: 760
	public interface IEncyclopediaLog
	{
		// Token: 0x06002B9A RID: 11162
		bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase;

		// Token: 0x06002B9B RID: 11163
		TextObject GetEncyclopediaText();

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06002B9C RID: 11164
		CampaignTime GameTime { get; }
	}
}
