using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultCharacterDevelopmentModel : CharacterDevelopmentModel
	{
		public DefaultCharacterDevelopmentModel()
		{
			this.InitializeSkillsRequiredForLevel();
			this.InitializeXpRequiredForSkillLevel();
		}

		public override List<Tuple<SkillObject, int>> GetSkillsDerivedFromTraits(Hero hero = null, CharacterObject templateCharacter = null, bool isByNaturalGrowth = false)
		{
			List<Tuple<SkillObject, int>> list = new List<Tuple<SkillObject, int>>();
			Occupation occupation = ((hero != null) ? hero.Occupation : templateCharacter.Occupation);
			if (templateCharacter == null)
			{
				templateCharacter = hero.CharacterObject;
			}
			int num = templateCharacter.GetTraitLevel(DefaultTraits.Commander);
			int num2 = templateCharacter.GetTraitLevel(DefaultTraits.Manager);
			int num3 = templateCharacter.GetTraitLevel(DefaultTraits.Trader);
			int num4 = templateCharacter.GetTraitLevel(DefaultTraits.Politician);
			int traitLevel = templateCharacter.GetTraitLevel(DefaultTraits.Siegecraft);
			int traitLevel2 = templateCharacter.GetTraitLevel(DefaultTraits.SergeantCommandSkills);
			int traitLevel3 = templateCharacter.GetTraitLevel(DefaultTraits.ScoutSkills);
			int traitLevel4 = templateCharacter.GetTraitLevel(DefaultTraits.Surgery);
			int traitLevel5 = templateCharacter.GetTraitLevel(DefaultTraits.Blacksmith);
			int num5 = templateCharacter.GetTraitLevel(DefaultTraits.RogueSkills);
			int num6 = templateCharacter.GetTraitLevel(DefaultTraits.Fighter);
			if (occupation == Occupation.Merchant)
			{
				num6 = 3;
				num2 = 6;
				num3 = 8;
				num4 = 5;
				num = 2;
			}
			else if (occupation == Occupation.GangLeader)
			{
				num6 = 6;
				num2 = 3;
				num3 = 3;
				num4 = 5;
				num = 3;
				num5 = 6;
			}
			else if (occupation == Occupation.RuralNotable || occupation == Occupation.Artisan || occupation == Occupation.Headman)
			{
				num6 = 4;
				num2 = 4;
				num3 = 2;
				num4 = 5;
			}
			else if (occupation == Occupation.Preacher)
			{
				num6 = 2;
				num4 = 7;
			}
			int num7 = MathF.Max(num * 10 + MBRandom.RandomInt(10), traitLevel2 * 5 + MBRandom.RandomInt(10));
			int num8 = MathF.Max(num * 5 + MBRandom.RandomInt(10), traitLevel2 * 10 + MBRandom.RandomInt(10));
			int num9 = num2 * 10 + MBRandom.RandomInt(10);
			int num10 = num3 * 10 + MBRandom.RandomInt(10);
			int num11 = traitLevel * 10 + MBRandom.RandomInt(10);
			int num12 = traitLevel3 * 10 + MBRandom.RandomInt(10);
			int num13 = num4 * 10 + MBRandom.RandomInt(10);
			int num14 = num5 * 10 + MBRandom.RandomInt(10);
			int num15 = traitLevel4 * 10 + MBRandom.RandomInt(10);
			int num16 = traitLevel5 * 10 + MBRandom.RandomInt(10);
			num10 = Math.Max(num9 - 20, num10);
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Steward, num9));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Trade, num10));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Crafting, num16));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Medicine, num15));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Roguery, num14));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Leadership, num7));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Tactics, num8));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Engineering, num11));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Scouting, num12));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Charm, num13));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.KnightFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.CavalryFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.HorseArcherFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.HopliteFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.PeltastFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.HuscarlFightingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.ArcherFIghtingSkills));
			num6 = MathF.Max(num6, templateCharacter.GetTraitLevel(DefaultTraits.CrossbowmanStyle));
			int num17 = num6 * 30 + MBRandom.RandomInt(10);
			int num18 = num6 * 30 + MBRandom.RandomInt(10);
			int num19 = num6 * 30 + MBRandom.RandomInt(10);
			int num20 = num6 * 25 + MBRandom.RandomInt(10);
			int num21 = num6 * 20 + MBRandom.RandomInt(10);
			int num22 = num6 * 20 + MBRandom.RandomInt(10);
			int num23 = num6 * 20 + MBRandom.RandomInt(10);
			int num24 = num6 * 20 + MBRandom.RandomInt(10);
			if (templateCharacter.GetTraitLevel(DefaultTraits.KnightFightingSkills) > 0)
			{
				num24 += 50;
				num17 += 10;
				num19 += 20;
				num20 -= 30;
				num21 -= 30;
				num22 -= 30;
				num23 += 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.CavalryFightingSkills) > 0)
			{
				num24 += 50;
				num19 += 10;
				num22 += 30;
				num20 -= 20;
				num21 -= 40;
				num18 -= 20;
				num23 -= 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.HorseArcherFightingSkills) > 0)
			{
				num24 += 40;
				num20 += 30;
				num19 -= 10;
				num18 -= 30;
				num21 -= 10;
				num22 -= 10;
				num23 -= 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.BossFightingSkills) > 0)
			{
				num17 += 135;
				num18 += 10;
				num19 += 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.ArcherFIghtingSkills) > 0)
			{
				num18 -= 20;
				num19 -= 30;
				num24 -= 30;
				num21 -= 20;
				num22 -= 20;
				num20 += 60;
				num17 -= 10;
				num23 += 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.HuscarlFightingSkills) > 0)
			{
				num18 += 50;
				num19 += 20;
				num20 -= 20;
				num21 -= 20;
				num22 -= 20;
				num23 += 10;
				num24 -= 20;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.PeltastFightingSkills) > 0)
			{
				num22 += 30;
				num23 += 30;
				num17 += 10;
				num18 -= 20;
				num19 -= 20;
				num20 -= 20;
				num21 -= 20;
				num24 -= 10;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.HopliteFightingSkills) > 0)
			{
				num19 += 20;
				num17 += 10;
				num18 -= 10;
				num23 += 10;
				num20 -= 20;
				num21 -= 20;
				num24 -= 10;
				num22 -= 20;
			}
			if (templateCharacter.GetTraitLevel(DefaultTraits.CrossbowmanStyle) > 0)
			{
				num21 += 60;
				num22 -= 20;
				num19 -= 20;
				num18 -= 10;
				num20 -= 20;
				num23 -= 10;
				num24 -= 20;
			}
			if (occupation == Occupation.Lord)
			{
				num24 += 20;
				num24 = MathF.Max(num24, 100);
			}
			if (occupation == Occupation.Wanderer)
			{
				if (num17 < num6 * 30)
				{
					num17 = MBRandom.RandomInt(5);
				}
				if (num18 < num6 * 30)
				{
					num18 = MBRandom.RandomInt(5);
				}
				if (num19 < num6 * 30)
				{
					num19 = MBRandom.RandomInt(5);
				}
				if (num20 < num6 * 25)
				{
					num20 = MBRandom.RandomInt(5);
				}
				if (num21 < num6 * 20)
				{
					num21 = MBRandom.RandomInt(5);
				}
				if (num22 < num6 * 20)
				{
					num22 = MBRandom.RandomInt(5);
				}
				if (num23 < num6 * 20)
				{
					num23 = MBRandom.RandomInt(5);
				}
				if (num24 < num6 * 20)
				{
					num24 = MBRandom.RandomInt(5);
				}
			}
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.OneHanded, num17));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.TwoHanded, num18));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Polearm, num19));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Bow, num20));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Crossbow, num21));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Throwing, num22));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Athletics, num23));
			list.Add(new Tuple<SkillObject, int>(DefaultSkills.Riding, num24));
			if (hero != null)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					SkillObject item = list[i].Item1;
					float item2 = (float)list[i].Item2;
					float skillScalingModifierForAge = Campaign.Current.Models.AgeModel.GetSkillScalingModifierForAge(hero, item, isByNaturalGrowth);
					int num25 = MathF.Floor(item2 * skillScalingModifierForAge);
					list[i] = new Tuple<SkillObject, int>(item, num25);
				}
			}
			return list;
		}

		private void InitializeSkillsRequiredForLevel()
		{
			int num = 1000;
			int num2 = 1;
			this._skillsRequiredForLevel[0] = 0;
			this._skillsRequiredForLevel[1] = 1;
			for (int i = 2; i < this._skillsRequiredForLevel.Length; i++)
			{
				num2 += num;
				this._skillsRequiredForLevel[i] = num2;
				num += 1000 + num / 5;
			}
		}

		public override int MaxFocusPerSkill
		{
			get
			{
				return 5;
			}
		}

		public override int MaxAttribute
		{
			get
			{
				return 10;
			}
		}

		public override int SkillsRequiredForLevel(int level)
		{
			if (level > 62)
			{
				return int.MaxValue;
			}
			return this._skillsRequiredForLevel[level];
		}

		public override int GetMaxSkillPoint()
		{
			return int.MaxValue;
		}

		private void InitializeXpRequiredForSkillLevel()
		{
			int num = 30;
			this._xpRequiredForSkillLevel[0] = num;
			for (int i = 1; i < 1024; i++)
			{
				num += 10 + i;
				this._xpRequiredForSkillLevel[i] = this._xpRequiredForSkillLevel[i - 1] + num;
			}
		}

		public override int GetXpRequiredForSkillLevel(int skillLevel)
		{
			if (skillLevel > 1024)
			{
				skillLevel = 1024;
			}
			if (skillLevel <= 0)
			{
				return 0;
			}
			return this._xpRequiredForSkillLevel[skillLevel - 1];
		}

		public override int GetSkillLevelChange(Hero hero, SkillObject skill, float skillXp)
		{
			int num = 0;
			int skillValue = hero.GetSkillValue(skill);
			for (int i = 0; i < 1024 - skillValue; i++)
			{
				int num2 = skillValue + i;
				if (num2 < 1023)
				{
					if (skillXp < (float)this._xpRequiredForSkillLevel[num2])
					{
						break;
					}
					num++;
				}
			}
			return num;
		}

		public override int GetXpAmountForSkillLevelChange(Hero hero, SkillObject skill, int skillLevelChange)
		{
			int skillValue = hero.GetSkillValue(skill);
			return this._xpRequiredForSkillLevel[skillValue + skillLevelChange] - this._xpRequiredForSkillLevel[skillValue];
		}

		public override void GetTraitLevelForTraitXp(Hero hero, TraitObject trait, int xpValue, out int traitLevel, out int clampedTraitXp)
		{
			clampedTraitXp = xpValue;
			int num = ((trait.MinValue < -1) ? (-6000) : ((trait.MinValue == -1) ? (-2500) : 0));
			int num2 = ((trait.MaxValue > 1) ? 6000 : ((trait.MaxValue == 1) ? 2500 : 0));
			if (xpValue > num2)
			{
				clampedTraitXp = num2;
			}
			else if (xpValue < num)
			{
				clampedTraitXp = num;
			}
			traitLevel = ((clampedTraitXp <= -4000) ? (-2) : ((clampedTraitXp <= -1000) ? (-1) : ((clampedTraitXp < 1000) ? 0 : ((clampedTraitXp < 4000) ? 1 : 2))));
			if (traitLevel < trait.MinValue)
			{
				traitLevel = trait.MinValue;
				return;
			}
			if (traitLevel > trait.MaxValue)
			{
				traitLevel = trait.MaxValue;
			}
		}

		public override int GetTraitXpRequiredForTraitLevel(TraitObject trait, int traitLevel)
		{
			if (traitLevel < -1)
			{
				return -4000;
			}
			if (traitLevel == -1)
			{
				return -1000;
			}
			if (traitLevel == 0)
			{
				return 0;
			}
			if (traitLevel != 1)
			{
				return 4000;
			}
			return 1000;
		}

		public override int AttributePointsAtStart
		{
			get
			{
				return 15;
			}
		}

		public override int LevelsPerAttributePoint
		{
			get
			{
				return 4;
			}
		}

		public override int FocusPointsPerLevel
		{
			get
			{
				return 1;
			}
		}

		public override int FocusPointsAtStart
		{
			get
			{
				return 5;
			}
		}

		public override ExplainedNumber CalculateLearningLimit(int attributeValue, int focusValue, TextObject attributeName, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			explainedNumber.Add((float)((attributeValue - 1) * 10), attributeName, null);
			explainedNumber.Add((float)(focusValue * 30), DefaultCharacterDevelopmentModel._skillFocusText, null);
			explainedNumber.LimitMin(0f);
			return explainedNumber;
		}

		public override float CalculateLearningRate(Hero hero, SkillObject skill)
		{
			int level = hero.Level;
			int attributeValue = hero.GetAttributeValue(skill.CharacterAttribute);
			int focus = hero.HeroDeveloper.GetFocus(skill);
			int skillValue = hero.GetSkillValue(skill);
			return this.CalculateLearningRate(attributeValue, focus, skillValue, level, skill.CharacterAttribute.Name, false).ResultNumber;
		}

		public override ExplainedNumber CalculateLearningRate(int attributeValue, int focusValue, int skillValue, int characterLevel, TextObject attributeName, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(1.25f, includeDescriptions, null);
			explainedNumber.AddFactor(0.4f * (float)attributeValue, attributeName);
			explainedNumber.AddFactor((float)focusValue * 1f, DefaultCharacterDevelopmentModel._skillFocusText);
			int num = MathF.Round(this.CalculateLearningLimit(attributeValue, focusValue, null, false).ResultNumber);
			if (skillValue > num)
			{
				int num2 = skillValue - num;
				explainedNumber.AddFactor(-1f - 0.1f * (float)num2, DefaultCharacterDevelopmentModel._overLimitText);
			}
			explainedNumber.LimitMin(0f);
			return explainedNumber;
		}

		private const int MaxCharacterLevels = 62;

		private const int SkillPointsAtLevel1 = 1;

		private const int SkillPointsGainNeededInitialValue = 1000;

		private const int SkillPointsGainNeededIncreasePerLevel = 1000;

		private readonly int[] _skillsRequiredForLevel = new int[63];

		private const int FocusPointsPerLevelConst = 1;

		private const int LevelsPerAttributePointConst = 4;

		private const int FocusPointsAtStartConst = 5;

		private const int AttributePointsAtStartConst = 15;

		private const int MaxSkillLevels = 1024;

		private readonly int[] _xpRequiredForSkillLevel = new int[1024];

		private const int XpRequirementForFirstLevel = 30;

		private const int MaxSkillPoint = 2147483647;

		private const float BaseLearningRate = 1.25f;

		private const int TraitThreshold2 = 4000;

		private const int TraitMaxValue1 = 2500;

		private const int TraitThreshold1 = 1000;

		private const int TraitMaxValue2 = 6000;

		private const int SkillLevelVariant = 10;

		private static readonly TextObject _skillFocusText = new TextObject("{=MRktqZwu}Skill Focus", null);

		private static readonly TextObject _overLimitText = new TextObject("{=bcA7ZuyO}Learning Limit Exceeded", null);
	}
}
