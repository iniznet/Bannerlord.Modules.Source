using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox
{
	public class BoostSkillCheatGroup : GameplayCheatGroup
	{
		public override IEnumerable<GameplayCheatBase> GetCheats()
		{
			foreach (SkillObject skillObject in Skills.All)
			{
				yield return new BoostSkillCheatGroup.BoostSkillCheeat(skillObject);
			}
			List<SkillObject>.Enumerator enumerator = default(List<SkillObject>.Enumerator);
			yield break;
			yield break;
		}

		public override TextObject GetName()
		{
			return new TextObject("{=SFn4UFd4}Boost Skill", null);
		}

		public class BoostSkillCheeat : GameplayCheatItem
		{
			public BoostSkillCheeat(SkillObject skillToBoost)
			{
				this._skillToBoost = skillToBoost;
			}

			public override void ExecuteCheat()
			{
				int num = 50;
				if (Hero.MainHero.GetSkillValue(this._skillToBoost) + num > 330)
				{
					num = 330 - Hero.MainHero.GetSkillValue(this._skillToBoost);
				}
				Hero.MainHero.HeroDeveloper.ChangeSkillLevel(this._skillToBoost, num, false);
			}

			public override TextObject GetName()
			{
				return this._skillToBoost.GetName();
			}

			private readonly SkillObject _skillToBoost;
		}
	}
}
