using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardUnitVM : ViewModel
	{
		public SPScoreboardUnitVM(BasicCharacterObject character)
		{
			this.Character = character;
			this.GainedSkills = new MBBindingList<SPScoreboardSkillItemVM>();
			this._skills = new List<SPScoreboardSkillItemVM>();
			this.Score = new SPScoreboardStatsVM(character.Name);
			CharacterCode.CreateFrom(character);
			this.IsHero = character.IsHero;
			this.Score.IsMainHero = character == Game.Current.PlayerTroop;
			this.IsGainedAnySkills = false;
			if (character.IsHero)
			{
				foreach (SkillObject skillObject in Game.Current.ObjectManager.GetObjectTypeList<SkillObject>())
				{
					this._skills.Add(new SPScoreboardSkillItemVM(skillObject, character.GetSkillValue(skillObject)));
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Score.RefreshValues();
			this.GainedSkills.ApplyActionOnAllItems(delegate(SPScoreboardSkillItemVM x)
			{
				x.RefreshValues();
			});
		}

		private void ExecuteActivateGainedSkills()
		{
		}

		private void ExecuteDeactivateGainedSkills()
		{
		}

		public void UpdateScores(int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.Score.UpdateScores(numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
		}

		public void UpdateHeroSkills(SkillObject gainedSkill, int currentSkill)
		{
			SPScoreboardSkillItemVM spscoreboardSkillItemVM = this._skills.First((SPScoreboardSkillItemVM s) => s.Skill == gainedSkill);
			spscoreboardSkillItemVM.UpdateSkill(currentSkill);
			if (!this.GainedSkills.Contains(spscoreboardSkillItemVM))
			{
				this.GainedSkills.Add(spscoreboardSkillItemVM);
			}
			this.IsGainedAnySkills = this.GainedSkills.Count > 0;
		}

		[DataSourceProperty]
		public bool IsGainedAnySkills
		{
			get
			{
				return this._isGainedAnySkills;
			}
			set
			{
				if (value != this._isGainedAnySkills)
				{
					this._isGainedAnySkills = value;
					base.OnPropertyChangedWithValue(value, "IsGainedAnySkills");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SPScoreboardSkillItemVM> GainedSkills
		{
			get
			{
				return this._gainedSkills;
			}
			set
			{
				if (value != this._gainedSkills)
				{
					this._gainedSkills = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPScoreboardSkillItemVM>>(value, "GainedSkills");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHero
		{
			get
			{
				return this._isHero;
			}
			set
			{
				if (value != this._isHero)
				{
					this._isHero = value;
					base.OnPropertyChangedWithValue(value, "IsHero");
				}
			}
		}

		[DataSourceProperty]
		public SPScoreboardStatsVM Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue<SPScoreboardStatsVM>(value, "Score");
				}
			}
		}

		public readonly BasicCharacterObject Character;

		private readonly List<SPScoreboardSkillItemVM> _skills;

		private SPScoreboardStatsVM _score;

		private bool _isHero;

		private bool _isGainedAnySkills;

		private MBBindingList<SPScoreboardSkillItemVM> _gainedSkills;
	}
}
