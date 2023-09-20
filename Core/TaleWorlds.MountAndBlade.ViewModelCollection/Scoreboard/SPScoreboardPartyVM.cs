using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardPartyVM : ViewModel
	{
		public IBattleCombatant BattleCombatant { get; private set; }

		public float CurrentPower { get; private set; }

		public float InitialPower { get; private set; }

		public SPScoreboardPartyVM(IBattleCombatant battleCombatant)
		{
			this.BattleCombatant = battleCombatant;
			this.Members = new MBBindingList<SPScoreboardUnitVM>();
			this.Score = new SPScoreboardStatsVM((battleCombatant != null) ? battleCombatant.Name : new TextObject("{=qnxJYAs7}Party", null));
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Score.RefreshValues();
			this.Members.ApplyActionOnAllItems(delegate(SPScoreboardUnitVM x)
			{
				x.RefreshValues();
			});
		}

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

		public void UpdateHeroSkills(BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			this.GetUnitAddIfNotExists(heroCharacter).UpdateHeroSkills(upgradedSkill, heroCharacter.GetSkillValue(upgradedSkill));
		}

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

		public SPScoreboardUnitVM GetUnit(BasicCharacterObject character)
		{
			return this.Members.FirstOrDefault((SPScoreboardUnitVM p) => p.Character == character);
		}

		internal SPScoreboardStatsVM RemoveUnit(BasicCharacterObject troop)
		{
			SPScoreboardUnitVM spscoreboardUnitVM = this.Members.FirstOrDefault((SPScoreboardUnitVM m) => m.Character == troop);
			SPScoreboardStatsVM spscoreboardStatsVM = null;
			if (spscoreboardUnitVM != null)
			{
				spscoreboardStatsVM = spscoreboardUnitVM.Score.GetScoreForOneAliveMember();
				this.UpdateScores(troop, -spscoreboardStatsVM.Remaining, -spscoreboardStatsVM.Dead, -spscoreboardStatsVM.Wounded, -spscoreboardStatsVM.Routed, -spscoreboardStatsVM.Kill, -spscoreboardStatsVM.ReadyToUpgrade);
				if (!spscoreboardUnitVM.Score.IsAnyStatRelevant())
				{
					this.Members.Remove(spscoreboardUnitVM);
					if (troop == Game.Current.PlayerTroop)
					{
						this.Score.IsMainParty = false;
					}
				}
			}
			return spscoreboardStatsVM;
		}

		internal void AddUnit(BasicCharacterObject unit, SPScoreboardStatsVM scoreToBringOver)
		{
			this.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
			SPScoreboardUnitVM spscoreboardUnitVM = this.Members.FirstOrDefault((SPScoreboardUnitVM m) => m.Character == unit);
			if (spscoreboardUnitVM == null)
			{
				spscoreboardUnitVM = new SPScoreboardUnitVM(unit);
				this.Members.Add(spscoreboardUnitVM);
				if (unit == Game.Current.PlayerTroop)
				{
					this.Score.IsMainParty = true;
				}
			}
			spscoreboardUnitVM.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
		}

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

		private MBBindingList<SPScoreboardUnitVM> _members;

		private SPScoreboardStatsVM _score;
	}
}
