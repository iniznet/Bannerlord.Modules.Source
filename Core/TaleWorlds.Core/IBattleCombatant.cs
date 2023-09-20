using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000079 RID: 121
	public interface IBattleCombatant
	{
		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000781 RID: 1921
		TextObject Name { get; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000782 RID: 1922
		BattleSideEnum Side { get; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000783 RID: 1923
		BasicCultureObject BasicCulture { get; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000784 RID: 1924
		BasicCharacterObject General { get; }

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000785 RID: 1925
		Tuple<uint, uint> PrimaryColorPair { get; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000786 RID: 1926
		Tuple<uint, uint> AlternativeColorPair { get; }

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000787 RID: 1927
		Banner Banner { get; }

		// Token: 0x06000788 RID: 1928
		int GetTacticsSkillAmount();
	}
}
