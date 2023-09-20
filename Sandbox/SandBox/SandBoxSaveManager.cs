using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x02000016 RID: 22
	public class SandBoxSaveManager : ISaveManager
	{
		// Token: 0x060000DA RID: 218 RVA: 0x000070A8 File Offset: 0x000052A8
		public int GetAutoSaveInterval()
		{
			return BannerlordConfig.AutoSaveInterval;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000070AF File Offset: 0x000052AF
		public void OnSaveOver(bool isSuccessful, string newSaveGameName)
		{
			if (isSuccessful)
			{
				BannerlordConfig.LatestSaveGameName = newSaveGameName;
				BannerlordConfig.Save();
			}
		}
	}
}
