using System;
using System.Threading.Tasks;
using TaleWorlds.AchievementSystem;

namespace TaleWorlds.PlatformService.Epic
{
	internal class EpicAchievementService : IAchievementService
	{
		public EpicAchievementService(EpicPlatformServices epicPlatformServices)
		{
			this._epicPlatformServices = epicPlatformServices;
		}

		bool IAchievementService.SetStat(string name, int value)
		{
			return this._epicPlatformServices.SetStat(name, value);
		}

		Task<int> IAchievementService.GetStat(string name)
		{
			return this._epicPlatformServices.GetStat(name);
		}

		Task<int[]> IAchievementService.GetStats(string[] names)
		{
			return this._epicPlatformServices.GetStats(names);
		}

		bool IAchievementService.IsInitializationCompleted()
		{
			return true;
		}

		private EpicPlatformServices _epicPlatformServices;
	}
}
