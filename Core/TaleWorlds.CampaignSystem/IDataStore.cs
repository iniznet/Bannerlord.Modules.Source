using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200002C RID: 44
	public interface IDataStore
	{
		// Token: 0x060002C6 RID: 710
		bool SyncData<T>(string key, ref T data);

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002C7 RID: 711
		bool IsSaving { get; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002C8 RID: 712
		bool IsLoading { get; }
	}
}
