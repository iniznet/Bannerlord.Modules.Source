﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class HeroDeveloper : PropertyOwnerF<PropertyObject>, IHeroDeveloper
	{
		internal static void AutoGeneratedStaticCollectObjectsHeroDeveloper(object o, List<object> collectedObjects)
		{
			((HeroDeveloper)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._newFocuses);
			collectedObjects.Add(this.Hero);
		}

		internal static object AutoGeneratedGetMemberValueUnspentFocusPoints(object o)
		{
			return ((HeroDeveloper)o).UnspentFocusPoints;
		}

		internal static object AutoGeneratedGetMemberValueUnspentAttributePoints(object o)
		{
			return ((HeroDeveloper)o).UnspentAttributePoints;
		}

		internal static object AutoGeneratedGetMemberValueHero(object o)
		{
			return ((HeroDeveloper)o).Hero;
		}

		internal static object AutoGeneratedGetMemberValue_newFocuses(object o)
		{
			return ((HeroDeveloper)o)._newFocuses;
		}

		internal static object AutoGeneratedGetMemberValue_totalXp(object o)
		{
			return ((HeroDeveloper)o)._totalXp;
		}

		[SaveableProperty(101)]
		public int UnspentFocusPoints { get; set; }

		[SaveableProperty(102)]
		public int UnspentAttributePoints { get; set; }

		public bool IsDeveloperInitialized
		{
			get
			{
				return this.Hero != null;
			}
		}

		[SaveableProperty(103)]
		public Hero Hero { get; private set; }

		public int TotalXp
		{
			get
			{
				return this._totalXp;
			}
			private set
			{
				this._totalXp = value;
			}
		}

		public int GetSkillXpProgress(SkillObject skill)
		{
			int skillValue = this.Hero.GetSkillValue(skill);
			return MathF.Round(base.GetPropertyValue(skill)) - Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(skillValue);
		}

		internal HeroDeveloper(Hero hero)
		{
			this.Hero = hero;
			this._newFocuses = new CharacterSkills();
		}

		public void ClearUnspentPoints()
		{
			this.UnspentAttributePoints = 0;
			this.UnspentFocusPoints = 0;
		}

		public void ClearHero()
		{
			base.ClearAllProperty();
			this.ClearFocuses();
			this.Hero.ClearAttributes();
			this.Hero.ClearSkills();
			this.Hero.ClearPerks();
			this.UnspentFocusPoints = 0;
			this.UnspentAttributePoints = 0;
			this.Hero.ClearTraits();
			this.ClearHeroLevel();
		}

		public int GetTotalSkillPoints()
		{
			int num = 0;
			foreach (SkillObject skillObject in Skills.All)
			{
				num += this.Hero.GetSkillValue(skillObject);
			}
			return num;
		}

		public void ChangeSkillLevel(SkillObject skill, int changeAmount, bool shouldNotify = true)
		{
			int skillValue = this.Hero.GetSkillValue(skill);
			int num = skillValue + changeAmount;
			float num2 = 0f;
			float propertyValue = base.GetPropertyValue(skill);
			num2 -= propertyValue - (float)Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(skillValue);
			for (int i = skillValue + 1; i <= num; i++)
			{
				num2 += (float)(Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(i) - Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(i - 1));
			}
			this.AddSkillXp(skill, num2 + 1f, false, shouldNotify);
		}

		public void DeriveSkillsFromTraits(bool isByNaturalGrowth = false, CharacterObject template = null)
		{
			foreach (Tuple<SkillObject, int> tuple in Campaign.Current.Models.CharacterDevelopmentModel.GetSkillsDerivedFromTraits(this.Hero, template, isByNaturalGrowth))
			{
				SkillObject item = tuple.Item1;
				int item2 = tuple.Item2;
				if (this.Hero.GetSkillValue(item) < item2)
				{
					this.Hero.HeroDeveloper.SetInitialSkillLevel(item, item2);
				}
			}
			this.Hero.HeroDeveloper.CheckInitialLevel();
		}

		public void SetInitialSkillLevel(SkillObject skill, int newSkillValue)
		{
			int xpRequiredForSkillLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(newSkillValue);
			base.SetPropertyValue(skill, (float)xpRequiredForSkillLevel);
			this.Hero.SetSkillValue(skill, newSkillValue);
			this.InitializeSkillXp(skill);
		}

		public void AddSkillXp(SkillObject skill, float rawXp, bool isAffectedByFocusFactor = true, bool shouldNotify = true)
		{
			if (rawXp <= 0f)
			{
				return;
			}
			if (isAffectedByFocusFactor)
			{
				this.GainRawXp(rawXp, shouldNotify);
			}
			float num = rawXp * Campaign.Current.Models.GenericXpModel.GetXpMultiplier(this.Hero);
			if (num <= 0f)
			{
				return;
			}
			float propertyValue = base.GetPropertyValue(skill);
			float focusFactor = this.GetFocusFactor(skill);
			float num2 = (isAffectedByFocusFactor ? (num * focusFactor) : num);
			float num3 = propertyValue + num2;
			int skillLevelChange = Campaign.Current.Models.CharacterDevelopmentModel.GetSkillLevelChange(this.Hero, skill, num3);
			base.SetPropertyValue(skill, num3);
			if (skillLevelChange > 0)
			{
				this.ChangeSkillLevelFromXpChange(skill, skillLevelChange, shouldNotify);
			}
		}

		private void GainRawXp(float rawXp, bool shouldNotify)
		{
			if ((long)this._totalXp + (long)MathF.Round(rawXp) < (long)Campaign.Current.Models.CharacterDevelopmentModel.GetMaxSkillPoint())
			{
				this._totalXp += MathF.Round(rawXp);
				this.CheckLevel(shouldNotify);
				return;
			}
			this._totalXp = Campaign.Current.Models.CharacterDevelopmentModel.GetMaxSkillPoint();
		}

		public float GetFocusFactor(SkillObject skill)
		{
			return Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningRate(this.Hero, skill);
		}

		private void ChangeSkillLevelFromXpChange(SkillObject skill, int changeAmount, bool shouldNotify = false)
		{
			if (changeAmount != 0)
			{
				int num = this.Hero.GetSkillValue(skill) + changeAmount;
				this.Hero.SetSkillValue(skill, num);
				CampaignEventDispatcher.Instance.OnHeroGainedSkill(this.Hero, skill, changeAmount, shouldNotify);
			}
		}

		void IHeroDeveloper.AfterLoad()
		{
			base.AfterLoadInternal();
		}

		internal void CheckLevel(bool shouldNotify = false)
		{
			bool flag = false;
			int totalXp = this._totalXp;
			while (!flag)
			{
				int xpRequiredForLevel = this.GetXpRequiredForLevel(this.Hero.Level + 1);
				if (xpRequiredForLevel != Campaign.Current.Models.CharacterDevelopmentModel.GetMaxSkillPoint() && totalXp >= xpRequiredForLevel)
				{
					this.Hero.Level++;
					this.OnGainLevel(shouldNotify);
				}
				else
				{
					flag = true;
				}
			}
		}

		public void SetInitialLevel(int level)
		{
			int xpRequiredForLevel = this.GetXpRequiredForLevel(level);
			this.TotalXp = xpRequiredForLevel + 1;
		}

		public void CheckInitialLevel()
		{
			this.SetupDefaultPoints();
			this.SetInitialLevelFromSkills();
			this.CheckLevel(false);
			this.SetInitialFocusAndAttributePoints();
		}

		private void SetupDefaultPoints()
		{
			this.UnspentFocusPoints = (this.Hero.Level - 1) * Campaign.Current.Models.CharacterDevelopmentModel.FocusPointsPerLevel + Campaign.Current.Models.CharacterDevelopmentModel.FocusPointsAtStart;
			this.UnspentAttributePoints = (this.Hero.Level - 1) / Campaign.Current.Models.CharacterDevelopmentModel.LevelsPerAttributePoint + Campaign.Current.Models.CharacterDevelopmentModel.AttributePointsAtStart;
		}

		private void SetInitialLevelFromSkills()
		{
			int num = (int)Skills.All.Sum((SkillObject s) => 2f * MathF.Pow((float)this.Hero.GetSkillValue(s), 2.2f)) - 2000;
			this.TotalXp = MathF.Max(1, num);
		}

		private void SetInitialFocusAndAttributePoints()
		{
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				int attributeValue = this.Hero.GetAttributeValue(characterAttribute);
				this.UnspentAttributePoints -= attributeValue;
				if (attributeValue == 0)
				{
					this.AddAttribute(characterAttribute, 1, true);
				}
			}
			foreach (SkillObject skillObject in Skills.All)
			{
				this.UnspentFocusPoints -= this.GetFocus(skillObject);
				this.InitializeSkillXp(skillObject);
			}
		}

		public MBReadOnlyList<PerkObject> GetOneAvailablePerkForEachPerkPair()
		{
			MBList<PerkObject> mblist = new MBList<PerkObject>();
			foreach (PerkObject perkObject in PerkObject.All)
			{
				SkillObject skill = perkObject.Skill;
				if ((float)this.Hero.GetSkillValue(skill) >= perkObject.RequiredSkillValue && !this.Hero.GetPerkValue(perkObject) && (perkObject.AlternativePerk == null || !this.Hero.GetPerkValue(perkObject.AlternativePerk)) && !mblist.Contains(perkObject.AlternativePerk))
				{
					mblist.Add(perkObject);
				}
			}
			return mblist;
		}

		private void ClearHeroLevel()
		{
			this.Hero.Level = 0;
		}

		public void AddPerk(PerkObject perk)
		{
			this.Hero.SetPerkValueInternal(perk, true);
		}

		private void OnGainLevel(bool shouldNotify = true)
		{
			this.UnspentFocusPoints += Campaign.Current.Models.CharacterDevelopmentModel.FocusPointsPerLevel;
			if (this.Hero.Level % Campaign.Current.Models.CharacterDevelopmentModel.LevelsPerAttributePoint == 0)
			{
				this.UnspentAttributePoints++;
			}
			CampaignEventDispatcher.Instance.OnHeroLevelledUp(this.Hero, shouldNotify);
		}

		public int GetXpRequiredForLevel(int level)
		{
			return Campaign.Current.Models.CharacterDevelopmentModel.SkillsRequiredForLevel(level);
		}

		public void RemoveAttribute(CharacterAttribute attrib, int changeAmount)
		{
			if (changeAmount == 0)
			{
				return;
			}
			int num = this.Hero.GetAttributeValue(attrib) - changeAmount;
			this.Hero.SetAttributeValueInternal(attrib, num);
		}

		public void AddAttribute(CharacterAttribute attrib, int changeAmount, bool checkUnspentPoints = true)
		{
			if (changeAmount == 0)
			{
				return;
			}
			int attributeValue = this.Hero.GetAttributeValue(attrib);
			if (attributeValue + changeAmount <= Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute && (this.UnspentAttributePoints >= 1 || !checkUnspentPoints))
			{
				int num = attributeValue + changeAmount;
				this.Hero.SetAttributeValueInternal(attrib, num);
				if (checkUnspentPoints)
				{
					this.UnspentAttributePoints--;
				}
			}
		}

		private void ClearFocuses()
		{
			this._newFocuses.ClearAllProperty();
		}

		public void AddFocus(SkillObject skill, int changeAmount, bool checkUnspentFocusPoints = true)
		{
			int focus = this.GetFocus(skill);
			int requiredFocusPointsToAddFocus = this.GetRequiredFocusPointsToAddFocus(skill);
			int num = focus + changeAmount;
			this.SetFocus(skill, num);
			this.UnspentFocusPoints = (checkUnspentFocusPoints ? (this.UnspentFocusPoints - requiredFocusPointsToAddFocus) : this.UnspentFocusPoints);
		}

		public void RemoveFocus(SkillObject skill, int changeAmount)
		{
			int num = this.GetFocus(skill) - changeAmount;
			this.SetFocus(skill, num);
		}

		public bool CanAddFocusToSkill(SkillObject skill)
		{
			return this.GetFocus(skill) < Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill && this.Hero.HeroDeveloper.UnspentFocusPoints >= this.GetRequiredFocusPointsToAddFocus(skill);
		}

		public int GetRequiredFocusPointsToAddFocus(SkillObject skill)
		{
			return 1;
		}

		private void SetFocus(SkillObject focus, int newAmount)
		{
			this._newFocuses.SetPropertyValue(focus, newAmount);
		}

		public int GetFocus(SkillObject skill)
		{
			return this._newFocuses.GetPropertyValue(skill);
		}

		public bool GetPerkValue(PerkObject perk)
		{
			return this.Hero.GetPerkValue(perk);
		}

		public void InitializeSkillXp(SkillObject skill)
		{
			int xpRequiredForSkillLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(this.Hero.GetSkillValue(skill));
			base.SetPropertyValue(skill, (float)xpRequiredForSkillLevel);
		}

		protected override void AfterLoad()
		{
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.0", 27066))
			{
				if (this.Hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && Campaign.Current.Models.CharacterDevelopmentModel.SkillsRequiredForLevel(this.Hero.Level) > this.TotalXp)
				{
					this.TotalXp = Campaign.Current.Models.CharacterDevelopmentModel.SkillsRequiredForLevel(this.Hero.Level);
					this.CheckLevel(false);
				}
				foreach (SkillObject skillObject in Skills.All)
				{
					if (this.GetSkillXpProgress(skillObject) < 0)
					{
						this.InitializeSkillXp(skillObject);
					}
				}
			}
		}

		[SaveableField(100)]
		private CharacterSkills _newFocuses;

		[SaveableField(130)]
		private int _totalXp;
	}
}
