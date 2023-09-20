using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	// Token: 0x0200000F RID: 15
	public class SPScoreboardPartyVM : ViewModel
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000567C File Offset: 0x0000387C
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00005684 File Offset: 0x00003884
		public IBattleCombatant BattleCombatant { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000126 RID: 294 RVA: 0x0000568D File Offset: 0x0000388D
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00005695 File Offset: 0x00003895
		public float CurrentPower { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000569E File Offset: 0x0000389E
		// (set) Token: 0x06000129 RID: 297 RVA: 0x000056A6 File Offset: 0x000038A6
		public float InitialPower { get; private set; }

		// Token: 0x0600012A RID: 298 RVA: 0x000056AF File Offset: 0x000038AF
		public SPScoreboardPartyVM(IBattleCombatant battleCombatant)
		{
			this.BattleCombatant = battleCombatant;
			this.Members = new MBBindingList<SPScoreboardUnitVM>();
			this.Score = new SPScoreboardStatsVM((battleCombatant != null) ? battleCombatant.Name : new TextObject("{=qnxJYAs7}Party", null));
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000056EA File Offset: 0x000038EA
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Score.RefreshValues();
			this.Members.ApplyActionOnAllItems(delegate(SPScoreboardUnitVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005727 File Offset: 0x00003927
		public void UpdateScores(BasicCharacterObject character, int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.GetUnitAddIfNotExists(character).UpdateScores(numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			SPScoreboardStatsVM score = this.Score;
			if (score != null)
			{
				score.UpdateScores(numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			}
			this.RefreshPower();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00005760 File Offset: 0x00003960
		public void UpdateHeroSkills(BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			this.GetUnitAddIfNotExists(heroCharacter).UpdateHeroSkills(upgradedSkill, heroCharacter.GetSkillValue(upgradedSkill));
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005778 File Offset: 0x00003978
		public SPScoreboardUnitVM GetUnitAddIfNotExists(BasicCharacterObject character)
		{
			if (character == Game.Current.PlayerTroop)
			{
				this.Score.IsMainParty = true;
			}
			SPScoreboardUnitVM spscoreboardUnitVM = this.Members.FirstOrDefault((SPScoreboardUnitVM p) => p.Character == character);
			if (spscoreboardUnitVM == null)
			{
				spscoreboardUnitVM = new SPScoreboardUnitVM(character);
				this.Members.Add(spscoreboardUnitVM);
				this.Members.Sort(new SPScoreboardSortControllerVM.ItemMemberComparer());
			}
			return spscoreboardUnitVM;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000057F4 File Offset: 0x000039F4
		public SPScoreboardUnitVM GetUnit(BasicCharacterObject character)
		{
			return this.Members.FirstOrDefault((SPScoreboardUnitVM p) => p.Character == character);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005828 File Offset: 0x00003A28
		internal SPScoreboardStatsVM RemoveUnit(BasicCharacterObject troop)
		{
			SPScoreboardUnitVM spscoreboardUnitVM = this.Members.FirstOrDefault((SPScoreboardUnitVM m) => m.Character == troop);
			SPScoreboardStatsVM spscoreboardStatsVM = null;
			if (spscoreboardUnitVM != null)
			{
				spscoreboardStatsVM = spscoreboardUnitVM.Score.GetScoreForOneAliveMember();
				if (!spscoreboardUnitVM.Score.IsAnyStatRelevant())
				{
					this.Members.Remove(spscoreboardUnitVM);
				}
				this.Score.UpdateScores(-spscoreboardStatsVM.Remaining, -spscoreboardStatsVM.Dead, -spscoreboardStatsVM.Wounded, -spscoreboardStatsVM.Routed, -spscoreboardStatsVM.Kill, -spscoreboardStatsVM.ReadyToUpgrade);
			}
			return spscoreboardStatsVM;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000058BC File Offset: 0x00003ABC
		internal void AddUnit(BasicCharacterObject unit, SPScoreboardStatsVM scoreToBringOver)
		{
			this.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
			SPScoreboardUnitVM spscoreboardUnitVM = this.Members.FirstOrDefault((SPScoreboardUnitVM m) => m.Character == unit);
			if (spscoreboardUnitVM == null)
			{
				SPScoreboardUnitVM spscoreboardUnitVM2 = new SPScoreboardUnitVM(unit);
				this.Members.Add(spscoreboardUnitVM2);
				spscoreboardUnitVM2.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
				return;
			}
			spscoreboardUnitVM.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005998 File Offset: 0x00003B98
		private void RefreshPower()
		{
			this.CurrentPower = 0f;
			this.InitialPower = 0f;
			foreach (SPScoreboardUnitVM spscoreboardUnitVM in this.Members)
			{
				this.CurrentPower += (float)spscoreboardUnitVM.Score.Remaining * spscoreboardUnitVM.Character.GetPower();
				this.InitialPower += (float)(spscoreboardUnitVM.Score.Dead + spscoreboardUnitVM.Score.Routed + spscoreboardUnitVM.Score.Wounded + spscoreboardUnitVM.Score.Remaining) * spscoreboardUnitVM.Character.GetPower();
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00005A64 File Offset: 0x00003C64
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00005A6C File Offset: 0x00003C6C
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

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00005A8A File Offset: 0x00003C8A
		// (set) Token: 0x06000136 RID: 310 RVA: 0x00005A92 File Offset: 0x00003C92
		[DataSourceProperty]
		public MBBindingList<SPScoreboardUnitVM> Members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != this._members)
				{
					this._members = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPScoreboardUnitVM>>(value, "Members");
				}
			}
		}

		// Token: 0x04000089 RID: 137
		private MBBindingList<SPScoreboardUnitVM> _members;

		// Token: 0x0400008A RID: 138
		private SPScoreboardStatsVM _score;
	}
}
