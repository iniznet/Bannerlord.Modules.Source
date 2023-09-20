using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class TroopUpgradeTracker
	{
		public void AddTrackedTroop(PartyBase party, CharacterObject character)
		{
			if (character.IsHero)
			{
				int count = Skills.All.Count;
				int[] array = new int[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = character.GetSkillValue(Skills.All[i]);
				}
				this._heroSkills[character.HeroObject] = array;
				return;
			}
			int num = party.MemberRoster.FindIndexOfTroop(character);
			if (num >= 0)
			{
				TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(num);
				int num2 = this.CalculateReadyToUpgradeSafe(ref elementCopyAtIndex, party);
				this._upgradedRegulars[new Tuple<PartyBase, CharacterObject>(party, character)] = num2;
			}
		}

		public IEnumerable<SkillObject> CheckSkillUpgrades(Hero hero)
		{
			if (!this._heroSkills.IsEmpty<KeyValuePair<Hero, int[]>>())
			{
				int[] oldSkillLevels = this._heroSkills[hero];
				int num;
				for (int i = 0; i < Skills.All.Count; i = num)
				{
					SkillObject skill = Skills.All[i];
					int newSkillLevel = hero.CharacterObject.GetSkillValue(skill);
					while (newSkillLevel > oldSkillLevels[i])
					{
						oldSkillLevels[i]++;
						yield return skill;
					}
					skill = null;
					num = i + 1;
				}
				oldSkillLevels = null;
			}
			yield break;
		}

		public int CheckUpgradedCount(PartyBase party, CharacterObject character)
		{
			int num = 0;
			if (!character.IsHero)
			{
				int num2 = party.MemberRoster.FindIndexOfTroop(character);
				int num5;
				if (num2 >= 0)
				{
					TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(num2);
					int num3 = this.CalculateReadyToUpgradeSafe(ref elementCopyAtIndex, party);
					int num4;
					if (this._upgradedRegulars.TryGetValue(new Tuple<PartyBase, CharacterObject>(party, character), out num4) && num3 > num4)
					{
						num4 = MathF.Min(elementCopyAtIndex.Number, num4);
						num = num3 - num4;
						this._upgradedRegulars[new Tuple<PartyBase, CharacterObject>(party, character)] = num3;
					}
				}
				else if (this._upgradedRegulars.TryGetValue(new Tuple<PartyBase, CharacterObject>(party, character), out num5) && num5 > 0)
				{
					num = -num5;
				}
			}
			return num;
		}

		private int CalculateReadyToUpgradeSafe(ref TroopRosterElement el, PartyBase owner)
		{
			int num = 0;
			CharacterObject character = el.Character;
			if (!character.IsHero && character.UpgradeTargets.Length != 0)
			{
				int num2 = 0;
				for (int i = 0; i < character.UpgradeTargets.Length; i++)
				{
					int upgradeXpCost = character.GetUpgradeXpCost(owner, i);
					if (num2 < upgradeXpCost)
					{
						num2 = upgradeXpCost;
					}
				}
				if (num2 > 0)
				{
					num = (el.Xp + el.DeltaXp) / num2;
				}
			}
			return MathF.Max(MathF.Min(el.Number, num), 0);
		}

		private Dictionary<Tuple<PartyBase, CharacterObject>, int> _upgradedRegulars = new Dictionary<Tuple<PartyBase, CharacterObject>, int>();

		private Dictionary<Hero, int[]> _heroSkills = new Dictionary<Hero, int[]>();
	}
}
