using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard
{
	// Token: 0x02000097 RID: 151
	public class TournamentLeaderboardEntryItemVM : ViewModel
	{
		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x0003A1A4 File Offset: 0x000383A4
		// (set) Token: 0x06000EAA RID: 3754 RVA: 0x0003A1AC File Offset: 0x000383AC
		public int Rank { get; private set; }

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x0003A1B5 File Offset: 0x000383B5
		// (set) Token: 0x06000EAC RID: 3756 RVA: 0x0003A1BD File Offset: 0x000383BD
		public float PrizeValue { get; private set; }

		// Token: 0x06000EAD RID: 3757 RVA: 0x0003A1C8 File Offset: 0x000383C8
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

		// Token: 0x06000EAE RID: 3758 RVA: 0x0003A278 File Offset: 0x00038478
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

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0003A2D7 File Offset: 0x000384D7
		// (set) Token: 0x06000EB0 RID: 3760 RVA: 0x0003A2DF File Offset: 0x000384DF
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06000EB1 RID: 3761 RVA: 0x0003A2FD File Offset: 0x000384FD
		// (set) Token: 0x06000EB2 RID: 3762 RVA: 0x0003A305 File Offset: 0x00038505
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

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06000EB3 RID: 3763 RVA: 0x0003A328 File Offset: 0x00038528
		// (set) Token: 0x06000EB4 RID: 3764 RVA: 0x0003A330 File Offset: 0x00038530
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

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0003A353 File Offset: 0x00038553
		// (set) Token: 0x06000EB6 RID: 3766 RVA: 0x0003A35B File Offset: 0x0003855B
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

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06000EB7 RID: 3767 RVA: 0x0003A379 File Offset: 0x00038579
		// (set) Token: 0x06000EB8 RID: 3768 RVA: 0x0003A381 File Offset: 0x00038581
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

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x0003A39F File Offset: 0x0003859F
		// (set) Token: 0x06000EBA RID: 3770 RVA: 0x0003A3A7 File Offset: 0x000385A7
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

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0003A3C5 File Offset: 0x000385C5
		// (set) Token: 0x06000EBC RID: 3772 RVA: 0x0003A3CD File Offset: 0x000385CD
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

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06000EBD RID: 3773 RVA: 0x0003A3EB File Offset: 0x000385EB
		// (set) Token: 0x06000EBE RID: 3774 RVA: 0x0003A3F3 File Offset: 0x000385F3
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

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0003A416 File Offset: 0x00038616
		// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x0003A41E File Offset: 0x0003861E
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

		// Token: 0x040006CE RID: 1742
		private readonly Hero _heroObj;

		// Token: 0x040006CF RID: 1743
		private int _placementOnLeaderboard;

		// Token: 0x040006D0 RID: 1744
		private int _victories;

		// Token: 0x040006D1 RID: 1745
		private bool _isMainHero;

		// Token: 0x040006D2 RID: 1746
		private bool _isChampion;

		// Token: 0x040006D3 RID: 1747
		private string _name;

		// Token: 0x040006D4 RID: 1748
		private string _rankText;

		// Token: 0x040006D5 RID: 1749
		private string _prizeStr;

		// Token: 0x040006D6 RID: 1750
		private HeroVM _hero;

		// Token: 0x040006D7 RID: 1751
		private BasicTooltipViewModel _championRewardsHint;
	}
}
