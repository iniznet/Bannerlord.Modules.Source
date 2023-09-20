using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace SandBox.ViewModelCollection
{
	public class SkillObjectComparer : IComparer<PerkObject>
	{
		public int Compare(PerkObject x, PerkObject y)
		{
			int skillObjectTypeSortIndex = SandBoxUIHelper.GetSkillObjectTypeSortIndex(x.Skill);
			int num = SandBoxUIHelper.GetSkillObjectTypeSortIndex(y.Skill).CompareTo(skillObjectTypeSortIndex);
			if (num != 0)
			{
				return num;
			}
			return this.ResolveEquality(x, y);
		}

		private int ResolveEquality(PerkObject x, PerkObject y)
		{
			return x.RequiredSkillValue.CompareTo(y.RequiredSkillValue);
		}
	}
}
