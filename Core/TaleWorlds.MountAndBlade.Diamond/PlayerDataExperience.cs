using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013C RID: 316
	public struct PlayerDataExperience
	{
		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060007B9 RID: 1977 RVA: 0x0000C803 File Offset: 0x0000AA03
		// (set) Token: 0x060007BA RID: 1978 RVA: 0x0000C80B File Offset: 0x0000AA0B
		public int Experience { get; private set; }

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060007BB RID: 1979 RVA: 0x0000C814 File Offset: 0x0000AA14
		public int Level
		{
			get
			{
				return PlayerDataExperience.CalculateLevelFromExperience(this.Experience);
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x0000C821 File Offset: 0x0000AA21
		public int ExperienceToNextLevel
		{
			get
			{
				return PlayerDataExperience.CalculateExperienceFromLevel(this.Level + 1) - this.Experience;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060007BD RID: 1981 RVA: 0x0000C837 File Offset: 0x0000AA37
		public int ExperienceInCurrentLevel
		{
			get
			{
				return this.Experience - PlayerDataExperience.CalculateExperienceFromLevel(this.Level);
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0000C84B File Offset: 0x0000AA4B
		static PlayerDataExperience()
		{
			PlayerDataExperience.InitializeXPRequirements();
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0000C859 File Offset: 0x0000AA59
		public PlayerDataExperience(int experience)
		{
			this.Experience = experience;
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0000C864 File Offset: 0x0000AA64
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

		// Token: 0x060007C1 RID: 1985 RVA: 0x0000C88F File Offset: 0x0000AA8F
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

		// Token: 0x060007C2 RID: 1986 RVA: 0x0000C8B6 File Offset: 0x0000AAB6
		public static int ExperienceRequiredForLevel(int level)
		{
			return Convert.ToInt32(Math.Floor(100.0 * Math.Pow((double)(level - 1), 1.03)));
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0000C8E0 File Offset: 0x0000AAE0
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

		// Token: 0x0400038B RID: 907
		private static int[] _levelToXP;

		// Token: 0x0400038C RID: 908
		private static readonly int _maxLevelForXPRequirementCalculation = 30;
	}
}
