using System;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;

namespace TaleWorlds.PlatformService.GOG
{
	public class GOGAchievementService : IAchievementService
	{
		public GOGAchievementService(GOGPlatformServices epicPlatformServices)
		{
			this._gogPlatformServices = epicPlatformServices;
		}

		bool IAchievementService.SetStat(string name, int value)
		{
			return this._gogPlatformServices.SetStat(name, value);
		}

		async Task<int> IAchievementService.GetStat(string name)
		{
			return await this._gogPlatformServices.GetStat(name);
		}

		async Task<int[]> IAchievementService.GetStats(string[] names)
		{
			return await this._gogPlatformServices.GetStats(names);
		}

		bool IAchievementService.IsInitializationCompleted()
		{
			return true;
		}

		private GOGPlatformServices _gogPlatformServices;
	}
}
