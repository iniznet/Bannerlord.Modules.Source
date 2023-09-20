﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public interface IHeroDeveloper
	{
		void SetInitialLevel(int i);

		void AddSkillXp(SkillObject skill, float rawXp, bool isAffectedByFocusFactor = true, bool shouldNotify = true);

		void CheckInitialLevel();

		int UnspentFocusPoints { get; set; }

		int UnspentAttributePoints { get; set; }

		void ClearUnspentPoints();

		void AddFocus(SkillObject skill, int changeAmount, bool checkUnspentFocusPoints = true);

		void RemoveFocus(SkillObject skill, int changeAmount);

		void DeriveSkillsFromTraits(bool isByNaturalGrowth = false, CharacterObject template = null);

		void SetInitialSkillLevel(SkillObject skill, int newSkillValue);

		void InitializeSkillXp(SkillObject skill);

		void ClearHero();

		void AddPerk(PerkObject perk);

		float GetFocusFactor(SkillObject skill);

		void AddAttribute(CharacterAttribute attribute, int changeAmount, bool checkUnspentPoints = true);

		void RemoveAttribute(CharacterAttribute attrib, int changeAmount);

		void ChangeSkillLevel(SkillObject skill, int changeAmount, bool shouldNotify = true);

		int GetFocus(SkillObject skill);

		bool CanAddFocusToSkill(SkillObject skill);

		void AfterLoad();

		int GetTotalSkillPoints();

		int GetXpRequiredForLevel(int level);

		MBReadOnlyList<PerkObject> GetOneAvailablePerkForEachPerkPair();

		int GetRequiredFocusPointsToAddFocus(SkillObject skill);

		int TotalXp { get; }

		Hero Hero { get; }

		int GetSkillXpProgress(SkillObject skill);

		bool GetPerkValue(PerkObject perk);
	}
}
