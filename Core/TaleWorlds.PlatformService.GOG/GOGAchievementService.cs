using System;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x02000004 RID: 4
	public class GOGAchievementService : IAchievementService
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public GOGAchievementService(GOGPlatformServices epicPlatformServices)
		{
			this._gogPlatformServices = epicPlatformServices;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002067 File Offset: 0x00000267
		bool IAchievementService.SetStat(string name, int value)
		{
			return this._gogPlatformServices.SetStat(name, value);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002078 File Offset: 0x00000278
		async Task<int> IAchievementService.GetStat(string name)
		{
			return await this._gogPlatformServices.GetStat(name);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020C8 File Offset: 0x000002C8
		async Task<int[]> IAchievementService.GetStats(string[] names)
		{
			return await this._gogPlatformServices.GetStats(names);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002115 File Offset: 0x00000315
		bool IAchievementService.IsInitializationCompleted()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		private GOGPlatformServices _gogPlatformServices;
	}
}
