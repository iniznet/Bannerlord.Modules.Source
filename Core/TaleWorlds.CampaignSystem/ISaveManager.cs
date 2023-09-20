using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000091 RID: 145
	public interface ISaveManager
	{
		// Token: 0x060010EB RID: 4331
		int GetAutoSaveInterval();

		// Token: 0x060010EC RID: 4332
		void OnSaveOver(bool isSuccessful, string newSaveGameName);
	}
}
