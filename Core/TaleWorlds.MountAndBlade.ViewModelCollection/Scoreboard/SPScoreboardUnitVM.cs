using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	// Token: 0x02000014 RID: 20
	public class SPScoreboardUnitVM : ViewModel
	{
		// Token: 0x0600018F RID: 399 RVA: 0x00006908 File Offset: 0x00004B08
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

		// Token: 0x06000190 RID: 400 RVA: 0x000069E4 File Offset: 0x00004BE4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Score.RefreshValues();
			this.GainedSkills.ApplyActionOnAllItems(delegate(SPScoreboardSkillItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00006A21 File Offset: 0x00004C21
		private void ExecuteActivateGainedSkills()
		{
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00006A23 File Offset: 0x00004C23
		private void ExecuteDeactivateGainedSkills()
		{
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00006A25 File Offset: 0x00004C25
		public void UpdateScores(int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.Score.UpdateScores(numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00006A3C File Offset: 0x00004C3C
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

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00006AA3 File Offset: 0x00004CA3
		// (set) Token: 0x06000196 RID: 406 RVA: 0x00006AAB File Offset: 0x00004CAB
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00006AC9 File Offset: 0x00004CC9
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00006AD1 File Offset: 0x00004CD1
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00006AEF File Offset: 0x00004CEF
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00006AF7 File Offset: 0x00004CF7
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00006B15 File Offset: 0x00004D15
		// (set) Token: 0x0600019C RID: 412 RVA: 0x00006B1D File Offset: 0x00004D1D
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

		// Token: 0x040000B7 RID: 183
		public readonly BasicCharacterObject Character;

		// Token: 0x040000B8 RID: 184
		private readonly List<SPScoreboardSkillItemVM> _skills;

		// Token: 0x040000B9 RID: 185
		private SPScoreboardStatsVM _score;

		// Token: 0x040000BA RID: 186
		private bool _isHero;

		// Token: 0x040000BB RID: 187
		private bool _isGainedAnySkills;

		// Token: 0x040000BC RID: 188
		private MBBindingList<SPScoreboardSkillItemVM> _gainedSkills;
	}
}
