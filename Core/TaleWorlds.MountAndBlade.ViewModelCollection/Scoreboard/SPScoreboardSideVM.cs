using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardSideVM : ViewModel
	{
		public SPScoreboardSideVM(TextObject name, Banner sideFlag)
		{
			this._nameTextObject = name;
			this.Parties = new MBBindingList<SPScoreboardPartyVM>();
			this.Score = new SPScoreboardStatsVM(this._nameTextObject);
			MBBindingList<SPScoreboardPartyVM> parties = this.Parties;
			this.SortController = new SPScoreboardSortControllerVM(ref parties);
			this.Parties = parties;
			if (sideFlag != null)
			{
				BannerCode bannerCode = BannerCode.CreateFrom(sideFlag);
				this.BannerVisual = new ImageIdentifierVM(bannerCode, true);
				this.BannerVisualSmall = new ImageIdentifierVM(bannerCode, false);
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Score.RefreshValues();
			this.Parties.ApplyActionOnAllItems(delegate(SPScoreboardPartyVM x)
			{
				x.RefreshValues();
			});
		}

		public void UpdateScores(IBattleCombatant battleCombatant, bool isPlayerParty, BasicCharacterObject character, int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.GetPartyAddIfNotExists(battleCombatant, isPlayerParty).UpdateScores(character, numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			this.Score.UpdateScores(numberRemaining, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			this.RefreshPower();
		}

		public void UpdateHeroSkills(IBattleCombatant battleCombatant, bool isPlayerParty, BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			this.GetPartyAddIfNotExists(battleCombatant, isPlayerParty).UpdateHeroSkills(heroCharacter, upgradedSkill);
		}

		public SPScoreboardPartyVM GetPartyAddIfNotExists(IBattleCombatant battleCombatant, bool isPlayerParty)
		{
			SPScoreboardPartyVM spscoreboardPartyVM = this.Parties.FirstOrDefault((SPScoreboardPartyVM p) => p.BattleCombatant == battleCombatant);
			if (spscoreboardPartyVM == null)
			{
				spscoreboardPartyVM = new SPScoreboardPartyVM(battleCombatant);
				if (isPlayerParty)
				{
					this.Parties.Insert(0, spscoreboardPartyVM);
				}
				else
				{
					this.Parties.Add(spscoreboardPartyVM);
				}
			}
			return spscoreboardPartyVM;
		}

		public SPScoreboardPartyVM GetParty(IBattleCombatant battleCombatant)
		{
			return this.Parties.FirstOrDefault((SPScoreboardPartyVM p) => p.BattleCombatant == battleCombatant);
		}

		public SPScoreboardStatsVM RemoveTroop(IBattleCombatant battleCombatant, BasicCharacterObject troop)
		{
			SPScoreboardPartyVM spscoreboardPartyVM = this.Parties.FirstOrDefault((SPScoreboardPartyVM p) => p.BattleCombatant == battleCombatant);
			SPScoreboardStatsVM spscoreboardStatsVM = spscoreboardPartyVM.RemoveUnit(troop);
			if (spscoreboardPartyVM.Members.Count == 0)
			{
				this.Parties.Remove(spscoreboardPartyVM);
			}
			this.Score.UpdateScores(-spscoreboardStatsVM.Remaining, -spscoreboardStatsVM.Dead, -spscoreboardStatsVM.Wounded, -spscoreboardStatsVM.Routed, -spscoreboardStatsVM.Kill, -spscoreboardStatsVM.ReadyToUpgrade);
			return spscoreboardStatsVM;
		}

		public void AddTroop(IBattleCombatant battleCombatant, BasicCharacterObject currentTroop, SPScoreboardStatsVM scoreToBringOver)
		{
			this.Parties.FirstOrDefault((SPScoreboardPartyVM p) => p.BattleCombatant == battleCombatant).AddUnit(currentTroop, scoreToBringOver);
			this.Score.UpdateScores(scoreToBringOver.Remaining, scoreToBringOver.Dead, scoreToBringOver.Wounded, scoreToBringOver.Routed, scoreToBringOver.Kill, scoreToBringOver.ReadyToUpgrade);
		}

		private void RefreshPower()
		{
			this.CurrentPower = 0f;
			this.InitialPower = 0f;
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._parties)
			{
				this.InitialPower += spscoreboardPartyVM.InitialPower;
				this.CurrentPower += spscoreboardPartyVM.CurrentPower;
			}
		}

		public float CurrentPower { get; private set; }

		public float InitialPower { get; private set; }

		[DataSourceProperty]
		public ImageIdentifierVM BannerVisual
		{
			get
			{
				return this._bannerVisual;
			}
			set
			{
				if (value != this._bannerVisual)
				{
					this._bannerVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "BannerVisual");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM BannerVisualSmall
		{
			get
			{
				return this._bannerVisualSmall;
			}
			set
			{
				if (value != this._bannerVisualSmall)
				{
					this._bannerVisualSmall = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "BannerVisualSmall");
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

		[DataSourceProperty]
		public MBBindingList<SPScoreboardPartyVM> Parties
		{
			get
			{
				return this._parties;
			}
			set
			{
				if (value != this._parties)
				{
					this._parties = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPScoreboardPartyVM>>(value, "Parties");
				}
			}
		}

		[DataSourceProperty]
		public SPScoreboardSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChanged("SortController");
				}
			}
		}

		private TextObject _nameTextObject;

		private MBBindingList<SPScoreboardPartyVM> _parties;

		private SPScoreboardStatsVM _score;

		private ImageIdentifierVM _bannerVisual;

		private ImageIdentifierVM _bannerVisualSmall;

		private SPScoreboardSortControllerVM _sortController;
	}
}
