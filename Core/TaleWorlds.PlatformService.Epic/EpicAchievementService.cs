using System;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;

namespace TaleWorlds.PlatformService.Epic
{
	// Token: 0x02000002 RID: 2
	internal class EpicAchievementService : IAchievementService
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public EpicAchievementService(EpicPlatformServices epicPlatformServices)
		{
			this._epicPlatformServices = epicPlatformServices;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		bool IAchievementService.SetStat(string name, int value)
		{
			return this._epicPlatformServices.SetStat(name, value);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002066 File Offset: 0x00000266
		Task<int> IAchievementService.GetStat(string name)
		{
			return this._epicPlatformServices.GetStat(name);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002074 File Offset: 0x00000274
		Task<int[]> IAchievementService.GetStats(string[] names)
		{
			return this._epicPlatformServices.GetStats(names);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002082 File Offset: 0x00000282
		bool IAchievementService.IsInitializationCompleted()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		private EpicPlatformServices _epicPlatformServices;
	}
}
