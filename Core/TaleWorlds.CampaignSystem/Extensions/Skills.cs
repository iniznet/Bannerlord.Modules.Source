using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000152 RID: 338
	public static class Skills
	{
		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001835 RID: 6197 RVA: 0x0007AFEE File Offset: 0x000791EE
		public static MBReadOnlyList<SkillObject> All
		{
			get
			{
				return Campaign.Current.AllSkills;
			}
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007AFFA File Offset: 0x000791FA
		public static SkillObject GetSkill(int i)
		{
			return Skills.All[i];
		}
	}
}
