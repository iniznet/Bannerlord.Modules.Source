using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B0 RID: 176
	[EncyclopediaViewModel(typeof(Clan))]
	public class EncyclopediaClanPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x06001128 RID: 4392 RVA: 0x00043F10 File Offset: 0x00042110
		public EncyclopediaClanPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._faction = base.Obj as IFaction;
			this._clan = this._faction as Clan;
			this.Members = new MBBindingList<HeroVM>();
			this.Enemies = new MBBindingList<EncyclopediaFactionVM>();
			this.Settlements = new MBBindingList<EncyclopediaSettlementVM>();
			this.History = new MBBindingList<EncyclopediaHistoryEventVM>();
			this.ClanInfo = new MBBindingList<StringPairItemVM>();
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._clan);
			this.RefreshValues();
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00043FA4 File Offset: 0x000421A4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StrengthHint = new HintViewModel(GameTexts.FindText("str_strength", null), null);
			this.ProsperityHint = new HintViewModel(GameTexts.FindText("str_prosperity", null), null);
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.AlliesText = new TextObject("{=bfQLwMUp}Clans", null).ToString();
			this.EnemiesText = new TextObject("{=zZlWRZjO}Wars", null).ToString();
			this.SettlementsText = GameTexts.FindText("str_settlements", null).ToString();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			this.DestroyedText = new TextObject("{=w8Yzf0F0}Destroyed", null).ToString();
			this.PartOfText = GameTexts.FindText("str_encyclopedia_clan_part_of_kingdom", null).ToString();
			this.LeaderText = GameTexts.FindText("str_leader", null).ToString();
			this.InfoText = GameTexts.FindText("str_info", null).ToString();
			base.UpdateBookmarkHintText();
			this.Refresh();
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x000440B8 File Offset: 0x000422B8
		public override void Refresh()
		{
			this.Members.Clear();
			this.Enemies.Clear();
			this.Settlements.Clear();
			this.History.Clear();
			this.ClanInfo.Clear();
			TextObject encyclopediaText = this._faction.EncyclopediaText;
			this.InformationText = ((encyclopediaText != null) ? encyclopediaText.ToString() : null);
			this.Leader = new HeroVM(this._faction.Leader, true);
			this.NameText = this._clan.Name.ToString();
			this.HasParentKingdom = this._clan.Kingdom != null;
			this.ParentKingdom = (this.HasParentKingdom ? new EncyclopediaFactionVM(((Clan)this._faction).Kingdom) : null);
			if (this._faction.IsKingdomFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_kingdom_faction", null).ToString();
			}
			else if (this._faction.IsBanditFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_bandit_faction", null).ToString();
			}
			else if (this._faction.IsMinorFaction)
			{
				this.DescriptorText = GameTexts.FindText("str_minor_faction", null).ToString();
			}
			int num = 0;
			float num2 = 0f;
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			IEnumerable<Hero> heroes = this._faction.Heroes;
			Clan clan = this._clan;
			foreach (Hero hero in heroes.Union((clan != null) ? clan.Companions : null))
			{
				if (pageOf.IsValidEncyclopediaItem(hero))
				{
					if (hero != this.Leader.Hero)
					{
						this.Members.Add(new HeroVM(hero, true));
					}
					num += hero.Gold;
				}
			}
			this.Banner = new ImageIdentifierVM(BannerCode.CreateFrom(this._faction.Banner), true);
			foreach (MobileParty mobileParty in MobileParty.AllLordParties)
			{
				if (mobileParty.MapFaction == this._faction && !mobileParty.IsDisbanding)
				{
					num2 += mobileParty.Party.TotalStrength;
				}
			}
			this.ProsperityText = num.ToString();
			this.StrengthText = num2.ToString();
			for (int i = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; i >= 0; i--)
			{
				IEncyclopediaLog encyclopediaLog;
				if ((encyclopediaLog = Campaign.Current.LogEntryHistory.GameActionLogs[i] as IEncyclopediaLog) != null && ((this._faction.IsKingdomFaction && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Kingdom>((Kingdom)this._faction)) || (this._faction.IsClan && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Clan>((Clan)this._faction))))
				{
					this.History.Add(new EncyclopediaHistoryEventVM(encyclopediaLog));
				}
			}
			EncyclopediaPage pageOf2 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Clan));
			foreach (IFaction faction in Campaign.Current.Factions.OrderBy((IFaction x) => !x.IsKingdomFaction).ThenBy((IFaction f) => f.Name.ToString()))
			{
				IFaction mapFaction = faction.MapFaction;
				if (pageOf2.IsValidEncyclopediaItem(mapFaction) && mapFaction != this._faction.MapFaction && mapFaction != this._faction && !mapFaction.IsBanditFaction && FactionManager.IsAtWarAgainstFaction(this._faction.MapFaction, mapFaction) && !this.Enemies.Any((EncyclopediaFactionVM x) => x.Faction == mapFaction))
				{
					this.Enemies.Add(new EncyclopediaFactionVM(mapFaction));
				}
			}
			EncyclopediaPage pageOf3 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Settlement));
			foreach (Settlement settlement in from s in Settlement.All
				orderby s.IsVillage, s.IsCastle, s.IsTown
				select s)
			{
				if ((settlement.MapFaction == this._faction || (settlement.OwnerClan == this._faction && settlement.OwnerClan.Leader != null)) && pageOf3.IsValidEncyclopediaItem(settlement) && (settlement.IsTown || settlement.IsCastle))
				{
					this.Settlements.Add(new EncyclopediaSettlementVM(settlement));
				}
			}
			GameTexts.SetVariable("LEFT", new TextObject("{=tTLvo8sM}Clan Tier", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), this._clan.Tier.ToString(), null));
			GameTexts.SetVariable("LEFT", new TextObject("{=ODEnkg0o}Clan Strength", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), this._clan.TotalStrength.ToString("F0"), null));
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_wealth", null).ToString());
			this.ClanInfo.Add(new StringPairItemVM(GameTexts.FindText("str_LEFT_colon", null).ToString(), CampaignUIHelper.GetClanWealthStatusText(this._clan), null));
			this.IsClanDestroyed = this._clan.IsEliminated;
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00044730 File Offset: 0x00042930
		public override string GetName()
		{
			return this._clan.Name.ToString();
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00044744 File Offset: 0x00042944
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Clans", GameTexts.FindText("str_encyclopedia_clans", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x000447AC File Offset: 0x000429AC
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._clan);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._clan);
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x0600112E RID: 4398 RVA: 0x000447FC File Offset: 0x000429FC
		// (set) Token: 0x0600112F RID: 4399 RVA: 0x00044804 File Offset: 0x00042A04
		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> ClanInfo
		{
			get
			{
				return this._clanInfo;
			}
			set
			{
				if (value != this._clanInfo)
				{
					this._clanInfo = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "ClanInfo");
				}
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x00044822 File Offset: 0x00042A22
		// (set) Token: 0x06001131 RID: 4401 RVA: 0x0004482A File Offset: 0x00042A2A
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

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x00044848 File Offset: 0x00042A48
		// (set) Token: 0x06001133 RID: 4403 RVA: 0x00044850 File Offset: 0x00042A50
		[DataSourceProperty]
		public MBBindingList<EncyclopediaFactionVM> Enemies
		{
			get
			{
				return this._enemies;
			}
			set
			{
				if (value != this._enemies)
				{
					this._enemies = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaFactionVM>>(value, "Enemies");
				}
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x0004486E File Offset: 0x00042A6E
		// (set) Token: 0x06001135 RID: 4405 RVA: 0x00044876 File Offset: 0x00042A76
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSettlementVM> Settlements
		{
			get
			{
				return this._settlements;
			}
			set
			{
				if (value != this._settlements)
				{
					this._settlements = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSettlementVM>>(value, "Settlements");
				}
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x00044894 File Offset: 0x00042A94
		// (set) Token: 0x06001137 RID: 4407 RVA: 0x0004489C File Offset: 0x00042A9C
		[DataSourceProperty]
		public MBBindingList<EncyclopediaHistoryEventVM> History
		{
			get
			{
				return this._history;
			}
			set
			{
				if (value != this._history)
				{
					this._history = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaHistoryEventVM>>(value, "History");
				}
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001138 RID: 4408 RVA: 0x000448BA File Offset: 0x00042ABA
		// (set) Token: 0x06001139 RID: 4409 RVA: 0x000448C2 File Offset: 0x00042AC2
		[DataSourceProperty]
		public EncyclopediaFactionVM ParentKingdom
		{
			get
			{
				return this._parentKingdom;
			}
			set
			{
				if (value != this._parentKingdom)
				{
					this._parentKingdom = value;
					base.OnPropertyChangedWithValue<EncyclopediaFactionVM>(value, "ParentKingdom");
				}
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x0600113A RID: 4410 RVA: 0x000448E0 File Offset: 0x00042AE0
		// (set) Token: 0x0600113B RID: 4411 RVA: 0x000448E8 File Offset: 0x00042AE8
		[DataSourceProperty]
		public bool HasParentKingdom
		{
			get
			{
				return this._hasParentKingdom;
			}
			set
			{
				if (value != this._hasParentKingdom)
				{
					this._hasParentKingdom = value;
					base.OnPropertyChangedWithValue(value, "HasParentKingdom");
				}
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x00044906 File Offset: 0x00042B06
		// (set) Token: 0x0600113D RID: 4413 RVA: 0x0004490E File Offset: 0x00042B0E
		[DataSourceProperty]
		public bool IsClanDestroyed
		{
			get
			{
				return this._isClanDestroyed;
			}
			set
			{
				if (value != this._isClanDestroyed)
				{
					this._isClanDestroyed = value;
					base.OnPropertyChangedWithValue(value, "IsClanDestroyed");
				}
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x0600113E RID: 4414 RVA: 0x0004492C File Offset: 0x00042B2C
		// (set) Token: 0x0600113F RID: 4415 RVA: 0x00044934 File Offset: 0x00042B34
		[DataSourceProperty]
		public string DestroyedText
		{
			get
			{
				return this._destroyedText;
			}
			set
			{
				if (value != this._destroyedText)
				{
					this._destroyedText = value;
					base.OnPropertyChangedWithValue<string>(value, "DestroyedText");
				}
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x00044957 File Offset: 0x00042B57
		// (set) Token: 0x06001141 RID: 4417 RVA: 0x0004495F File Offset: 0x00042B5F
		[DataSourceProperty]
		public string PartOfText
		{
			get
			{
				return this._partOfText;
			}
			set
			{
				if (value != this._partOfText)
				{
					this._partOfText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartOfText");
				}
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x00044982 File Offset: 0x00042B82
		// (set) Token: 0x06001143 RID: 4419 RVA: 0x0004498A File Offset: 0x00042B8A
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

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x000449AD File Offset: 0x00042BAD
		// (set) Token: 0x06001145 RID: 4421 RVA: 0x000449B5 File Offset: 0x00042BB5
		[DataSourceProperty]
		public string InfoText
		{
			get
			{
				return this._infoText;
			}
			set
			{
				if (value != this._infoText)
				{
					this._infoText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfoText");
				}
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x000449D8 File Offset: 0x00042BD8
		// (set) Token: 0x06001147 RID: 4423 RVA: 0x000449E0 File Offset: 0x00042BE0
		[DataSourceProperty]
		public HeroVM Leader
		{
			get
			{
				return this._leader;
			}
			set
			{
				if (value != this._leader)
				{
					this._leader = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Leader");
				}
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001148 RID: 4424 RVA: 0x000449FE File Offset: 0x00042BFE
		// (set) Token: 0x06001149 RID: 4425 RVA: 0x00044A06 File Offset: 0x00042C06
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

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x00044A24 File Offset: 0x00042C24
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x00044A2C File Offset: 0x00042C2C
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x00044A4F File Offset: 0x00042C4F
		// (set) Token: 0x0600114D RID: 4429 RVA: 0x00044A57 File Offset: 0x00042C57
		[DataSourceProperty]
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != this._membersText)
				{
					this._membersText = value;
					base.OnPropertyChangedWithValue<string>(value, "MembersText");
				}
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x0600114E RID: 4430 RVA: 0x00044A7A File Offset: 0x00042C7A
		// (set) Token: 0x0600114F RID: 4431 RVA: 0x00044A82 File Offset: 0x00042C82
		[DataSourceProperty]
		public string EnemiesText
		{
			get
			{
				return this._enemiesText;
			}
			set
			{
				if (value != this._enemiesText)
				{
					this._enemiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemiesText");
				}
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06001150 RID: 4432 RVA: 0x00044AA5 File Offset: 0x00042CA5
		// (set) Token: 0x06001151 RID: 4433 RVA: 0x00044AAD File Offset: 0x00042CAD
		[DataSourceProperty]
		public string AlliesText
		{
			get
			{
				return this._alliesText;
			}
			set
			{
				if (value != this._alliesText)
				{
					this._alliesText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlliesText");
				}
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06001152 RID: 4434 RVA: 0x00044AD0 File Offset: 0x00042CD0
		// (set) Token: 0x06001153 RID: 4435 RVA: 0x00044AD8 File Offset: 0x00042CD8
		[DataSourceProperty]
		public string SettlementsText
		{
			get
			{
				return this._settlementsText;
			}
			set
			{
				if (value != this._settlementsText)
				{
					this._settlementsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementsText");
				}
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06001154 RID: 4436 RVA: 0x00044AFB File Offset: 0x00042CFB
		// (set) Token: 0x06001155 RID: 4437 RVA: 0x00044B03 File Offset: 0x00042D03
		[DataSourceProperty]
		public string VillagesText
		{
			get
			{
				return this._villagesText;
			}
			set
			{
				if (value != this._villagesText)
				{
					this._villagesText = value;
					base.OnPropertyChangedWithValue<string>(value, "VillagesText");
				}
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06001156 RID: 4438 RVA: 0x00044B26 File Offset: 0x00042D26
		// (set) Token: 0x06001157 RID: 4439 RVA: 0x00044B2E File Offset: 0x00042D2E
		[DataSourceProperty]
		public string InformationText
		{
			get
			{
				return this._informationText;
			}
			set
			{
				if (value != this._informationText)
				{
					this._informationText = value;
					base.OnPropertyChangedWithValue<string>(value, "InformationText");
				}
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06001158 RID: 4440 RVA: 0x00044B51 File Offset: 0x00042D51
		// (set) Token: 0x06001159 RID: 4441 RVA: 0x00044B59 File Offset: 0x00042D59
		[DataSourceProperty]
		public string LeaderText
		{
			get
			{
				return this._leaderText;
			}
			set
			{
				if (value != this._leaderText)
				{
					this._leaderText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderText");
				}
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x0600115A RID: 4442 RVA: 0x00044B7C File Offset: 0x00042D7C
		// (set) Token: 0x0600115B RID: 4443 RVA: 0x00044B84 File Offset: 0x00042D84
		[DataSourceProperty]
		public string DescriptorText
		{
			get
			{
				return this._descriptorText;
			}
			set
			{
				if (value != this._descriptorText)
				{
					this._descriptorText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptorText");
				}
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x0600115C RID: 4444 RVA: 0x00044BA7 File Offset: 0x00042DA7
		// (set) Token: 0x0600115D RID: 4445 RVA: 0x00044BAF File Offset: 0x00042DAF
		[DataSourceProperty]
		public string ProsperityText
		{
			get
			{
				return this._prosperityText;
			}
			set
			{
				if (value != this._prosperityText)
				{
					this._prosperityText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProsperityText");
				}
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x0600115E RID: 4446 RVA: 0x00044BD2 File Offset: 0x00042DD2
		// (set) Token: 0x0600115F RID: 4447 RVA: 0x00044BDA File Offset: 0x00042DDA
		[DataSourceProperty]
		public string StrengthText
		{
			get
			{
				return this._strengthText;
			}
			set
			{
				if (value != this._strengthText)
				{
					this._strengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "StrengthText");
				}
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06001160 RID: 4448 RVA: 0x00044BFD File Offset: 0x00042DFD
		// (set) Token: 0x06001161 RID: 4449 RVA: 0x00044C05 File Offset: 0x00042E05
		[DataSourceProperty]
		public HintViewModel ProsperityHint
		{
			get
			{
				return this._prosperityHint;
			}
			set
			{
				if (value != this._prosperityHint)
				{
					this._prosperityHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ProsperityHint");
				}
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06001162 RID: 4450 RVA: 0x00044C23 File Offset: 0x00042E23
		// (set) Token: 0x06001163 RID: 4451 RVA: 0x00044C2B File Offset: 0x00042E2B
		[DataSourceProperty]
		public HintViewModel StrengthHint
		{
			get
			{
				return this._strengthHint;
			}
			set
			{
				if (value != this._strengthHint)
				{
					this._strengthHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "StrengthHint");
				}
			}
		}

		// Token: 0x040007FE RID: 2046
		private readonly IFaction _faction;

		// Token: 0x040007FF RID: 2047
		private readonly Clan _clan;

		// Token: 0x04000800 RID: 2048
		private MBBindingList<StringPairItemVM> _clanInfo;

		// Token: 0x04000801 RID: 2049
		private MBBindingList<HeroVM> _members;

		// Token: 0x04000802 RID: 2050
		private MBBindingList<EncyclopediaFactionVM> _enemies;

		// Token: 0x04000803 RID: 2051
		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		// Token: 0x04000804 RID: 2052
		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		// Token: 0x04000805 RID: 2053
		private HeroVM _leader;

		// Token: 0x04000806 RID: 2054
		private ImageIdentifierVM _banner;

		// Token: 0x04000807 RID: 2055
		private string _membersText;

		// Token: 0x04000808 RID: 2056
		private string _enemiesText;

		// Token: 0x04000809 RID: 2057
		private string _alliesText;

		// Token: 0x0400080A RID: 2058
		private string _settlementsText;

		// Token: 0x0400080B RID: 2059
		private string _villagesText;

		// Token: 0x0400080C RID: 2060
		private string _leaderText;

		// Token: 0x0400080D RID: 2061
		private string _descriptorText;

		// Token: 0x0400080E RID: 2062
		private string _informationText;

		// Token: 0x0400080F RID: 2063
		private string _prosperityText;

		// Token: 0x04000810 RID: 2064
		private string _strengthText;

		// Token: 0x04000811 RID: 2065
		private string _destroyedText;

		// Token: 0x04000812 RID: 2066
		private string _partOfText;

		// Token: 0x04000813 RID: 2067
		private string _tierText;

		// Token: 0x04000814 RID: 2068
		private string _infoText;

		// Token: 0x04000815 RID: 2069
		private HintViewModel _prosperityHint;

		// Token: 0x04000816 RID: 2070
		private HintViewModel _strengthHint;

		// Token: 0x04000817 RID: 2071
		private EncyclopediaFactionVM _parentKingdom;

		// Token: 0x04000818 RID: 2072
		private string _nameText;

		// Token: 0x04000819 RID: 2073
		private bool _hasParentKingdom;

		// Token: 0x0400081A RID: 2074
		private bool _isClanDestroyed;
	}
}
