using System;
using System.Threading.Tasks;

namespace TaleWorlds.AchievementSystem
{
	public class AchievementManager
	{
		public static IAchievementService AchievementService { get; set; } = new TestAchievementService();

		public static bool SetStat(string name, int value)
		{
			return AchievementManager.AchievementService.SetStat(name, value);
		}

		public static async Task<int> GetStat(string name)
		{
			return await AchievementManager.AchievementService.GetStat(name);
		}

		public static async Task<int[]> GetStats(string[] names)
		{
			return await AchievementManager.AchievementService.GetStats(names);
		}
	}
}
