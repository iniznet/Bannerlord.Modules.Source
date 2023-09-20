using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard
{
	public class TournamentLeaderboardEntryItemVM : ViewModel
	{
		public int Rank { get; private set; }

		public float PrizeValue { get; private set; }

		public TournamentLeaderboardEntryItemVM(Hero hero, int victories, int placement)
		{
			this._heroObj = hero;
			this.PrizeStr = "-";
			this.Rank = placement;
			this.PlacementOnLeaderboard = placement;
			this.IsChampion = placement == 1;
			this.Victories = victories;
			float num;
			if (float.TryParse(this.PrizeStr, out num))
			{
				this.PrizeValue = num;
			}
			this.IsMainHero = hero == TaleWorlds.CampaignSystem.Hero.MainHero;
			this.Hero = new HeroVM(hero, false);
			this.ChampionRewardsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTournamentChampionRewardsTooltip(hero, null));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._heroObj.Name.ToString();
			GameTexts.SetVariable("RANK", this.Rank);
			this.RankText = GameTexts.FindText("str_leaderboard_rank", null).ToString();
			HeroVM hero = this.Hero;
			if (hero == null)
			{
				return;
			}
			hero.RefreshValues();
		}

		[DataSourceProperty]
		public BasicTooltipViewModel ChampionRewardsHint
		{
			get
			{
				return this._championRewardsHint;
			}
			set
			{
				if (value != this._championRewardsHint)
				{
					this._championRewardsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ChampionRewardsHint");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string RankText
		{
			get
			{
				return this._rankText;
			}
			set
			{
				if (value != this._rankText)
				{
					this._rankText = value;
					base.OnPropertyChangedWithValue<string>(value, "RankText");
				}
			}
		}

		[DataSourceProperty]
		public int Victories
		{
			get
			{
				return this._victories;
			}
			set
			{
				if (value != this._victories)
				{
					this._victories = value;
					base.OnPropertyChangedWithValue(value, "Victories");
				}
			}
		}

		[DataSourceProperty]
		public bool IsChampion
		{
			get
			{
				return this._isChampion;
			}
			set
			{
				if (value != this._isChampion)
				{
					this._isChampion = value;
					base.OnPropertyChangedWithValue(value, "IsChampion");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM Hero
		{
			get
			{
				return this._hero;
			}
			set
			{
				if (value != this._hero)
				{
					this._hero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Hero");
				}
			}
		}

		[DataSourceProperty]
		public string PrizeStr
		{
			get
			{
				return this._prizeStr;
			}
			set
			{
				if (value != this._prizeStr)
				{
					this._prizeStr = value;
					base.OnPropertyChangedWithValue<string>(value, "PrizeStr");
				}
			}
		}

		[DataSourceProperty]
		public int PlacementOnLeaderboard
		{
			get
			{
				return this._placementOnLeaderboard;
			}
			set
			{
				if (value != this._placementOnLeaderboard)
				{
					this._placementOnLeaderboard = value;
					base.OnPropertyChangedWithValue(value, "PlacementOnLeaderboard");
				}
			}
		}

		private readonly Hero _heroObj;

		private int _placementOnLeaderboard;

		private int _victories;

		private bool _isMainHero;

		private bool _isChampion;

		private string _name;

		private string _rankText;

		private string _prizeStr;

		private HeroVM _hero;

		private BasicTooltipViewModel _championRewardsHint;
	}
}
