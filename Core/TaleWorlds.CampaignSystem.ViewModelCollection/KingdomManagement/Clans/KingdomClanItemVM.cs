using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	// Token: 0x02000070 RID: 112
	public class KingdomClanItemVM : KingdomItemVM
	{
		// Token: 0x060009CB RID: 2507 RVA: 0x00027C08 File Offset: 0x00025E08
		public KingdomClanItemVM(Clan clan, Action<KingdomClanItemVM> onSelect)
		{
			this.Clan = clan;
			this._onSelect = onSelect;
			this.Banner = new ImageIdentifierVM(clan.Banner);
			this.Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(clan.Banner), true);
			this.RefreshValues();
			this.Refresh();
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00027C64 File Offset: 0x00025E64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Clan.Name.ToString();
			GameTexts.SetVariable("TIER", this.Clan.Tier);
			this.TierText = GameTexts.FindText("str_clan_tier", null).ToString();
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00027CB8 File Offset: 0x00025EB8
		public void Refresh()
		{
			this.Members = new MBBindingList<HeroVM>();
			this.ClanType = 0;
			if (this.Clan.IsUnderMercenaryService)
			{
				this.ClanType = 2;
			}
			else if (this.Clan.Kingdom.RulingClan == this.Clan)
			{
				this.ClanType = 1;
			}
			foreach (Hero hero in this.Clan.Heroes.Where((Hero h) => !h.IsDisabled && !h.IsNotSpawned && h.IsAlive && !h.IsChild))
			{
				this.Members.Add(new HeroVM(hero, false));
			}
			this.NumOfMembers = this.Members.Count;
			this.Fiefs = new MBBindingList<KingdomClanFiefItemVM>();
			foreach (Settlement settlement in this.Clan.Settlements.Where((Settlement s) => s.IsTown || s.IsCastle))
			{
				this.Fiefs.Add(new KingdomClanFiefItemVM(settlement));
			}
			this.NumOfFiefs = this.Fiefs.Count;
			this.Influence = (int)this.Clan.Influence;
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00027E30 File Offset: 0x00026030
		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060009CF RID: 2511 RVA: 0x00027E44 File Offset: 0x00026044
		// (set) Token: 0x060009D0 RID: 2512 RVA: 0x00027E4C File Offset: 0x0002604C
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

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x00027E6F File Offset: 0x0002606F
		// (set) Token: 0x060009D2 RID: 2514 RVA: 0x00027E77 File Offset: 0x00026077
		[DataSourceProperty]
		public int ClanType
		{
			get
			{
				return this._clanType;
			}
			set
			{
				if (value != this._clanType)
				{
					this._clanType = value;
					base.OnPropertyChangedWithValue(value, "ClanType");
				}
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060009D3 RID: 2515 RVA: 0x00027E95 File Offset: 0x00026095
		// (set) Token: 0x060009D4 RID: 2516 RVA: 0x00027E9D File Offset: 0x0002609D
		[DataSourceProperty]
		public int NumOfMembers
		{
			get
			{
				return this._numOfMembers;
			}
			set
			{
				if (value != this._numOfMembers)
				{
					this._numOfMembers = value;
					base.OnPropertyChangedWithValue(value, "NumOfMembers");
				}
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060009D5 RID: 2517 RVA: 0x00027EBB File Offset: 0x000260BB
		// (set) Token: 0x060009D6 RID: 2518 RVA: 0x00027EC3 File Offset: 0x000260C3
		[DataSourceProperty]
		public int NumOfFiefs
		{
			get
			{
				return this._numOfFiefs;
			}
			set
			{
				if (value != this._numOfFiefs)
				{
					this._numOfFiefs = value;
					base.OnPropertyChangedWithValue(value, "NumOfFiefs");
				}
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x060009D7 RID: 2519 RVA: 0x00027EE1 File Offset: 0x000260E1
		// (set) Token: 0x060009D8 RID: 2520 RVA: 0x00027EE9 File Offset: 0x000260E9
		[DataSourceProperty]
		public string TierText
		{
			get
			{
				return this._tierText;
			}
			set
			{
				if (value != this._tierText)
				{
					this._tierText = value;
					base.OnPropertyChangedWithValue<string>(value, "TierText");
				}
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x060009D9 RID: 2521 RVA: 0x00027F0C File Offset: 0x0002610C
		// (set) Token: 0x060009DA RID: 2522 RVA: 0x00027F14 File Offset: 0x00026114
		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner)
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x060009DB RID: 2523 RVA: 0x00027F32 File Offset: 0x00026132
		// (set) Token: 0x060009DC RID: 2524 RVA: 0x00027F3A File Offset: 0x0002613A
		[DataSourceProperty]
		public ImageIdentifierVM Banner_9
		{
			get
			{
				return this._banner_9;
			}
			set
			{
				if (value != this._banner_9)
				{
					this._banner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner_9");
				}
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x060009DD RID: 2525 RVA: 0x00027F58 File Offset: 0x00026158
		// (set) Token: 0x060009DE RID: 2526 RVA: 0x00027F60 File Offset: 0x00026160
		[DataSourceProperty]
		public MBBindingList<HeroVM> Members
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
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Members");
				}
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x00027F7E File Offset: 0x0002617E
		// (set) Token: 0x060009E0 RID: 2528 RVA: 0x00027F86 File Offset: 0x00026186
		[DataSourceProperty]
		public MBBindingList<KingdomClanFiefItemVM> Fiefs
		{
			get
			{
				return this._fiefs;
			}
			set
			{
				if (value != this._fiefs)
				{
					this._fiefs = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomClanFiefItemVM>>(value, "Fiefs");
				}
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x00027FA4 File Offset: 0x000261A4
		// (set) Token: 0x060009E2 RID: 2530 RVA: 0x00027FAC File Offset: 0x000261AC
		[DataSourceProperty]
		public int Influence
		{
			get
			{
				return this._influence;
			}
			set
			{
				if (value != this._influence)
				{
					this._influence = value;
					base.OnPropertyChangedWithValue(value, "Influence");
				}
			}
		}

		// Token: 0x04000468 RID: 1128
		private readonly Action<KingdomClanItemVM> _onSelect;

		// Token: 0x04000469 RID: 1129
		public readonly Clan Clan;

		// Token: 0x0400046A RID: 1130
		private string _name;

		// Token: 0x0400046B RID: 1131
		private ImageIdentifierVM _banner;

		// Token: 0x0400046C RID: 1132
		private ImageIdentifierVM _banner_9;

		// Token: 0x0400046D RID: 1133
		private MBBindingList<HeroVM> _members;

		// Token: 0x0400046E RID: 1134
		private MBBindingList<KingdomClanFiefItemVM> _fiefs;

		// Token: 0x0400046F RID: 1135
		private int _influence;

		// Token: 0x04000470 RID: 1136
		private int _numOfMembers;

		// Token: 0x04000471 RID: 1137
		private int _numOfFiefs;

		// Token: 0x04000472 RID: 1138
		private string _tierText;

		// Token: 0x04000473 RID: 1139
		private int _clanType = -1;

		// Token: 0x020001A4 RID: 420
		private enum ClanTypes
		{
			// Token: 0x04000F5F RID: 3935
			Normal,
			// Token: 0x04000F60 RID: 3936
			Leader,
			// Token: 0x04000F61 RID: 3937
			Mercenary
		}
	}
}
