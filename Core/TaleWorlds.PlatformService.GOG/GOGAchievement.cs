using System;

namespace TaleWorlds.PlatformService.GOG
{
	public class GOGAchievement
	{
		public string AchievementName { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool Achieved { get; set; }

		public int Progress { get; set; }

		public int AchievementID;
	}
}
