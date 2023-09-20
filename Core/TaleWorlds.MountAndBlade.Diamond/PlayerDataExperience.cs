using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public struct PlayerDataExperience
	{
		public int Experience { get; private set; }

		public int Level
		{
			get
			{
				return PlayerDataExperience.CalculateLevelFromExperience(this.Experience);
			}
		}

		public int ExperienceToNextLevel
		{
			get
			{
				return PlayerDataExperience.CalculateExperienceFromLevel(this.Level + 1) - this.Experience;
			}
		}

		public int ExperienceInCurrentLevel
		{
			get
			{
				return this.Experience - PlayerDataExperience.CalculateExperienceFromLevel(this.Level);
			}
		}

		static PlayerDataExperience()
		{
			PlayerDataExperience.InitializeXPRequirements();
		}

		public PlayerDataExperience(int experience)
		{
			this.Experience = experience;
		}

		public static int CalculateLevelFromExperience(int experience)
		{
			int num = 1;
			int i = 0;
			while (i <= experience)
			{
				i += PlayerDataExperience.ExperienceRequiredForLevel(num + 1);
				if (i <= experience)
				{
					num++;
				}
			}
			return num;
		}

		public static int CalculateExperienceFromLevel(int level)
		{
			if (level == 1)
			{
				return 0;
			}
			if (level < PlayerDataExperience._maxLevelForXPRequirementCalculation)
			{
				return PlayerDataExperience._levelToXP[level];
			}
			return PlayerDataExperience.ExperienceRequiredForLevel(level) + PlayerDataExperience.CalculateExperienceFromLevel(level - 1);
		}

		public static int ExperienceRequiredForLevel(int level)
		{
			return Convert.ToInt32(Math.Floor(100.0 * Math.Pow((double)(level - 1), 1.03)));
		}

		private static void InitializeXPRequirements()
		{
			PlayerDataExperience._levelToXP = new int[PlayerDataExperience._maxLevelForXPRequirementCalculation];
			int num = 0;
			for (int i = 2; i < PlayerDataExperience._maxLevelForXPRequirementCalculation; i++)
			{
				num += PlayerDataExperience.ExperienceRequiredForLevel(i);
				PlayerDataExperience._levelToXP[i] = num;
			}
		}

		private static int[] _levelToXP;

		private static readonly int _maxLevelForXPRequirementCalculation = 30;
	}
}
