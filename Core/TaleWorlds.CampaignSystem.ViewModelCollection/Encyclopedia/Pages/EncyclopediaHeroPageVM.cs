using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B4 RID: 180
	[EncyclopediaViewModel(typeof(Hero))]
	public class EncyclopediaHeroPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x060011AE RID: 4526 RVA: 0x00045AAC File Offset: 0x00043CAC
		public EncyclopediaHeroPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._hero = base.Obj as Hero;
			this._relationAscendingComparer = new HeroRelationComparer(this._hero, true);
			this._relationDescendingComparer = new HeroRelationComparer(this._hero, false);
			TextObject textObject;
			this.IsInformationHidden = CampaignUIHelper.IsHeroInformationHidden(this._hero, out textObject);
			this._infoHiddenReasonText = textObject;
			this._allRelatedHeroes = new List<Hero>
			{
				this._hero.Father,
				this._hero.Mother,
				this._hero.Spouse
			};
			this._allRelatedHeroes.AddRange(this._hero.Children);
			this._allRelatedHeroes.AddRange(this._hero.Siblings);
			this._allRelatedHeroes.AddRange(this._hero.ExSpouses);
			StringHelpers.SetCharacterProperties("NPC", this._hero.CharacterObject, null, false);
			this.Settlements = new MBBindingList<EncyclopediaSettlementVM>();
			this.Dwellings = new MBBindingList<EncyclopediaDwellingVM>();
			this.Allies = new MBBindingList<HeroVM>();
			this.Enemies = new MBBindingList<HeroVM>();
			this.Family = new MBBindingList<EncyclopediaFamilyMemberVM>();
			this.Companions = new MBBindingList<HeroVM>();
			this.History = new MBBindingList<EncyclopediaHistoryEventVM>();
			this.Skills = new MBBindingList<EncyclopediaSkillVM>();
			this.Stats = new MBBindingList<StringPairItemVM>();
			this.Traits = new MBBindingList<EncyclopediaTraitItemVM>();
			this.HeroCharacter = new HeroViewModel(CharacterViewModel.StanceTypes.EmphasizeFace);
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._hero);
			this.Faction = new EncyclopediaFactionVM(this._hero.Clan);
			this.RefreshValues();
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00045C60 File Offset: 0x00043E60
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ClanText = GameTexts.FindText("str_clan", null).ToString();
			this.AlliesText = GameTexts.FindText("str_friends", null).ToString();
			this.EnemiesText = GameTexts.FindText("str_enemies", null).ToString();
			this.FamilyText = GameTexts.FindText("str_family_group", null).ToString();
			this.CompanionsText = GameTexts.FindText("str_companions", null).ToString();
			this.DwellingsText = GameTexts.FindText("str_dwellings", null).ToString();
			this.SettlementsText = GameTexts.FindText("str_settlements", null).ToString();
			this.DeceasedText = GameTexts.FindText("str_encyclopedia_deceased", null).ToString();
			this.TraitsText = GameTexts.FindText("str_traits_group", null).ToString();
			this.SkillsText = GameTexts.FindText("str_skills", null).ToString();
			this.InfoText = GameTexts.FindText("str_info", null).ToString();
			this.PregnantHint = new HintViewModel(GameTexts.FindText("str_pregnant", null), null);
			base.UpdateBookmarkHintText();
			this.UpdateInformationText();
			this.Refresh();
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00045D90 File Offset: 0x00043F90
		public override void Refresh()
		{
			base.IsLoadingOver = false;
			this.Settlements.Clear();
			this.Dwellings.Clear();
			this.Allies.Clear();
			this.Enemies.Clear();
			this.Companions.Clear();
			this.Family.Clear();
			this.History.Clear();
			this.Skills.Clear();
			this.Stats.Clear();
			this.Traits.Clear();
			this.NameText = this._hero.Name.ToString();
			string text = GameTexts.FindText("str_missing_info_indicator", null).ToString();
			EncyclopediaPage pageOf = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero));
			this.HasNeutralClan = this._hero.Clan == null || this._hero.Clan == CampaignData.NeutralFaction;
			if (!this.IsInformationHidden)
			{
				for (int i = 0; i < TaleWorlds.CampaignSystem.Extensions.Skills.All.Count; i++)
				{
					SkillObject skillObject = TaleWorlds.CampaignSystem.Extensions.Skills.All[i];
					if (this._hero.GetSkillValue(skillObject) > 0 && this._hero.GetSkillValue(skillObject) > 50)
					{
						this.Skills.Add(new EncyclopediaSkillVM(skillObject, this._hero.GetSkillValue(skillObject)));
					}
				}
				foreach (TraitObject traitObject in CampaignUIHelper.GetHeroTraits())
				{
					if (this._hero.GetTraitLevel(traitObject) != 0)
					{
						this.Traits.Add(new EncyclopediaTraitItemVM(traitObject, this._hero));
					}
				}
				if (this._hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
				{
					for (int j = 0; j < Hero.AllAliveHeroes.Count; j++)
					{
						this.AddHeroToRelatedVMList(Hero.AllAliveHeroes[j]);
					}
					for (int k = 0; k < Hero.DeadOrDisabledHeroes.Count; k++)
					{
						this.AddHeroToRelatedVMList(Hero.DeadOrDisabledHeroes[k]);
					}
					this.Allies.Sort(this._relationDescendingComparer);
					this.Enemies.Sort(this._relationAscendingComparer);
				}
				if (this._hero.Clan != null && this._hero == this._hero.Clan.Leader)
				{
					for (int l = 0; l < this._hero.Clan.Companions.Count; l++)
					{
						Hero hero = this._hero.Clan.Companions[l];
						this.Companions.Add(new HeroVM(hero, false));
					}
				}
				for (int m = 0; m < this._allRelatedHeroes.Count; m++)
				{
					Hero hero2 = this._allRelatedHeroes[m];
					if (hero2 != null && pageOf.IsValidEncyclopediaItem(hero2))
					{
						this.Family.Add(new EncyclopediaFamilyMemberVM(hero2, this._hero));
					}
				}
				for (int n = 0; n < this._hero.OwnedWorkshops.Count; n++)
				{
					this.Dwellings.Add(new EncyclopediaDwellingVM(this._hero.OwnedWorkshops[n].WorkshopType));
				}
				EncyclopediaPage pageOf2 = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Settlement));
				for (int num = 0; num < Settlement.All.Count; num++)
				{
					Settlement settlement = Settlement.All[num];
					if (settlement.OwnerClan != null && settlement.OwnerClan.Leader == this._hero && pageOf2.IsValidEncyclopediaItem(settlement))
					{
						this.Settlements.Add(new EncyclopediaSettlementVM(settlement));
					}
				}
			}
			if (this._hero.Culture != null)
			{
				string text2 = GameTexts.FindText("str_enc_sf_culture", null).ToString();
				this.Stats.Add(new StringPairItemVM(text2, this._hero.Culture.Name.ToString(), null));
			}
			string text3 = GameTexts.FindText("str_enc_sf_age", null).ToString();
			this.Stats.Add(new StringPairItemVM(text3, this.IsInformationHidden ? text : ((int)this._hero.Age).ToString(), null));
			for (int num2 = Campaign.Current.LogEntryHistory.GameActionLogs.Count - 1; num2 >= 0; num2--)
			{
				IEncyclopediaLog encyclopediaLog;
				if ((encyclopediaLog = Campaign.Current.LogEntryHistory.GameActionLogs[num2] as IEncyclopediaLog) != null && encyclopediaLog.IsVisibleInEncyclopediaPageOf<Hero>(this._hero))
				{
					this.History.Add(new EncyclopediaHistoryEventVM(encyclopediaLog));
				}
			}
			if (!this._hero.IsNotable && !this._hero.IsWanderer)
			{
				Clan clan = this._hero.Clan;
				if (((clan != null) ? clan.Kingdom : null) != null)
				{
					this.KingdomRankText = CampaignUIHelper.GetHeroKingdomRank(this._hero);
				}
			}
			string heroOccupationName = CampaignUIHelper.GetHeroOccupationName(this._hero);
			if (!string.IsNullOrEmpty(heroOccupationName))
			{
				string text4 = GameTexts.FindText("str_enc_sf_occupation", null).ToString();
				this.Stats.Add(new StringPairItemVM(text4, this.IsInformationHidden ? text : heroOccupationName, null));
			}
			if (this._hero != Hero.MainHero)
			{
				string text5 = GameTexts.FindText("str_enc_sf_relation", null).ToString();
				this.Stats.Add(new StringPairItemVM(text5, this.IsInformationHidden ? text : this._hero.GetRelationWithPlayer().ToString(), null));
			}
			this.LastSeenText = ((this._hero == Hero.MainHero) ? "" : HeroHelper.GetLastSeenText(this._hero).ToString());
			this.HeroCharacter.FillFrom(this._hero, -1, this._hero.IsNotable, true);
			this.HeroCharacter.SetEquipment(EquipmentIndex.ArmorItemEndSlot, default(EquipmentElement));
			this.HeroCharacter.SetEquipment(EquipmentIndex.HorseHarness, default(EquipmentElement));
			this.HeroCharacter.SetEquipment(EquipmentIndex.NumAllWeaponSlots, default(EquipmentElement));
			this.IsCompanion = this._hero.CompanionOf != null;
			if (this.IsCompanion)
			{
				this.MasterText = GameTexts.FindText("str_companion_of", null).ToString();
				Clan companionOf = this._hero.CompanionOf;
				this.Master = new HeroVM((companionOf != null) ? companionOf.Leader : null, false);
			}
			this.IsPregnant = this._hero.IsPregnant;
			this.IsDead = !this._hero.IsAlive;
			base.IsLoadingOver = true;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0004644C File Offset: 0x0004464C
		private void AddHeroToRelatedVMList(Hero hero)
		{
			if (!Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero)).IsValidEncyclopediaItem(hero) || hero.IsNotable)
			{
				return;
			}
			if (hero != this._hero && hero.IsAlive && hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && !this._allRelatedHeroes.Contains(hero))
			{
				if (EncyclopediaHeroPageVM.IsFriend(this._hero, hero))
				{
					this.Allies.Add(new HeroVM(hero, false));
					return;
				}
				if (EncyclopediaHeroPageVM.IsEnemy(this._hero, hero))
				{
					this.Enemies.Add(new HeroVM(hero, false));
				}
			}
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00046500 File Offset: 0x00044700
		private static bool IsFriend(Hero h1, Hero h2)
		{
			return CharacterRelationManager.GetHeroRelation(h1, h2) >= 40;
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00046510 File Offset: 0x00044710
		public static bool IsEnemy(Hero h1, Hero h2)
		{
			return CharacterRelationManager.GetHeroRelation(h1, h2) <= -30;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00046520 File Offset: 0x00044720
		public override string GetName()
		{
			return this._hero.Name.ToString();
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00046534 File Offset: 0x00044734
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Heroes", GameTexts.FindText("str_encyclopedia_heroes", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00046599 File Offset: 0x00044799
		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x000465AC File Offset: 0x000447AC
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._hero);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._hero);
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x000465FC File Offset: 0x000447FC
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroCharacter.OnFinalize();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00046610 File Offset: 0x00044810
		private void UpdateInformationText()
		{
			this.InformationText = "";
			if (this._hero.EncyclopediaText != null)
			{
				this.InformationText = this._hero.EncyclopediaText.ToString();
				return;
			}
			if (this._hero.CharacterObject.Occupation == Occupation.Lord)
			{
				this.InformationText = Hero.SetHeroEncyclopediaTextAndLinks(this._hero).ToString();
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x00046675 File Offset: 0x00044875
		// (set) Token: 0x060011BB RID: 4539 RVA: 0x0004667D File Offset: 0x0004487D
		[DataSourceProperty]
		public EncyclopediaFactionVM Faction
		{
			get
			{
				return this._faction;
			}
			set
			{
				if (value != this._faction)
				{
					this._faction = value;
					base.OnPropertyChangedWithValue<EncyclopediaFactionVM>(value, "Faction");
				}
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x0004669B File Offset: 0x0004489B
		// (set) Token: 0x060011BD RID: 4541 RVA: 0x000466A3 File Offset: 0x000448A3
		[DataSourceProperty]
		public bool IsCompanion
		{
			get
			{
				return this._isCompanion;
			}
			set
			{
				if (value != this._isCompanion)
				{
					this._isCompanion = value;
					base.OnPropertyChangedWithValue(value, "IsCompanion");
				}
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x000466C1 File Offset: 0x000448C1
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x000466C9 File Offset: 0x000448C9
		[DataSourceProperty]
		public bool IsPregnant
		{
			get
			{
				return this._isPregnant;
			}
			set
			{
				if (value != this._isPregnant)
				{
					this._isPregnant = value;
					base.OnPropertyChangedWithValue(value, "IsPregnant");
				}
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x000466E7 File Offset: 0x000448E7
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x000466EF File Offset: 0x000448EF
		[DataSourceProperty]
		public HeroVM Master
		{
			get
			{
				return this._master;
			}
			set
			{
				if (value != this._master)
				{
					this._master = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "Master");
				}
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0004670D File Offset: 0x0004490D
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x00046715 File Offset: 0x00044915
		[DataSourceProperty]
		public string ClanText
		{
			get
			{
				return this._clanText;
			}
			set
			{
				if (value != this._clanText)
				{
					this._clanText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanText");
				}
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x00046738 File Offset: 0x00044938
		// (set) Token: 0x060011C5 RID: 4549 RVA: 0x00046740 File Offset: 0x00044940
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

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x060011C6 RID: 4550 RVA: 0x00046763 File Offset: 0x00044963
		// (set) Token: 0x060011C7 RID: 4551 RVA: 0x0004676B File Offset: 0x0004496B
		[DataSourceProperty]
		public string TraitsText
		{
			get
			{
				return this._traitsText;
			}
			set
			{
				if (value != this._traitsText)
				{
					this._traitsText = value;
					base.OnPropertyChangedWithValue<string>(value, "TraitsText");
				}
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x060011C8 RID: 4552 RVA: 0x0004678E File Offset: 0x0004498E
		// (set) Token: 0x060011C9 RID: 4553 RVA: 0x00046796 File Offset: 0x00044996
		[DataSourceProperty]
		public string MasterText
		{
			get
			{
				return this._masterText;
			}
			set
			{
				if (value != this._masterText)
				{
					this._masterText = value;
					base.OnPropertyChangedWithValue<string>(value, "MasterText");
				}
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x000467B9 File Offset: 0x000449B9
		// (set) Token: 0x060011CB RID: 4555 RVA: 0x000467C1 File Offset: 0x000449C1
		[DataSourceProperty]
		public string KingdomRankText
		{
			get
			{
				return this._kingdomRankText;
			}
			set
			{
				if (value != this._kingdomRankText)
				{
					this._kingdomRankText = value;
					base.OnPropertyChangedWithValue<string>(value, "KingdomRankText");
				}
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x060011CC RID: 4556 RVA: 0x000467E4 File Offset: 0x000449E4
		[DataSourceProperty]
		public string InfoHiddenReasonText
		{
			get
			{
				return this._infoHiddenReasonText.ToString();
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x060011CD RID: 4557 RVA: 0x000467F1 File Offset: 0x000449F1
		// (set) Token: 0x060011CE RID: 4558 RVA: 0x000467F9 File Offset: 0x000449F9
		[DataSourceProperty]
		public string SkillsText
		{
			get
			{
				return this._skillsText;
			}
			set
			{
				if (value != this._skillsText)
				{
					this._skillsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillsText");
				}
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x060011CF RID: 4559 RVA: 0x0004681C File Offset: 0x00044A1C
		// (set) Token: 0x060011D0 RID: 4560 RVA: 0x00046824 File Offset: 0x00044A24
		[DataSourceProperty]
		public HeroViewModel HeroCharacter
		{
			get
			{
				return this._heroCharacter;
			}
			set
			{
				if (value != this._heroCharacter)
				{
					this._heroCharacter = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "HeroCharacter");
				}
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x060011D1 RID: 4561 RVA: 0x00046842 File Offset: 0x00044A42
		// (set) Token: 0x060011D2 RID: 4562 RVA: 0x0004684A File Offset: 0x00044A4A
		[DataSourceProperty]
		public string LastSeenText
		{
			get
			{
				return this._lastSeenText;
			}
			set
			{
				if (value != this._lastSeenText)
				{
					this._lastSeenText = value;
					base.OnPropertyChangedWithValue<string>(value, "LastSeenText");
				}
			}
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x060011D3 RID: 4563 RVA: 0x0004686D File Offset: 0x00044A6D
		// (set) Token: 0x060011D4 RID: 4564 RVA: 0x00046875 File Offset: 0x00044A75
		[DataSourceProperty]
		public string DeceasedText
		{
			get
			{
				return this._deceasedText;
			}
			set
			{
				if (value != this._deceasedText)
				{
					this._deceasedText = value;
					base.OnPropertyChangedWithValue<string>(value, "DeceasedText");
				}
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x060011D5 RID: 4565 RVA: 0x00046898 File Offset: 0x00044A98
		// (set) Token: 0x060011D6 RID: 4566 RVA: 0x000468A0 File Offset: 0x00044AA0
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

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x060011D7 RID: 4567 RVA: 0x000468C3 File Offset: 0x00044AC3
		// (set) Token: 0x060011D8 RID: 4568 RVA: 0x000468CB File Offset: 0x00044ACB
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

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x060011D9 RID: 4569 RVA: 0x000468EE File Offset: 0x00044AEE
		// (set) Token: 0x060011DA RID: 4570 RVA: 0x000468F6 File Offset: 0x00044AF6
		[DataSourceProperty]
		public string DwellingsText
		{
			get
			{
				return this._dwellingsText;
			}
			set
			{
				if (value != this._dwellingsText)
				{
					this._dwellingsText = value;
					base.OnPropertyChangedWithValue<string>(value, "DwellingsText");
				}
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x060011DB RID: 4571 RVA: 0x00046919 File Offset: 0x00044B19
		// (set) Token: 0x060011DC RID: 4572 RVA: 0x00046921 File Offset: 0x00044B21
		[DataSourceProperty]
		public string CompanionsText
		{
			get
			{
				return this._companionsText;
			}
			set
			{
				if (value != this._companionsText)
				{
					this._companionsText = value;
					base.OnPropertyChangedWithValue<string>(value, "CompanionsText");
				}
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x060011DD RID: 4573 RVA: 0x00046944 File Offset: 0x00044B44
		// (set) Token: 0x060011DE RID: 4574 RVA: 0x0004694C File Offset: 0x00044B4C
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

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x060011DF RID: 4575 RVA: 0x0004696F File Offset: 0x00044B6F
		// (set) Token: 0x060011E0 RID: 4576 RVA: 0x00046977 File Offset: 0x00044B77
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

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x060011E1 RID: 4577 RVA: 0x0004699A File Offset: 0x00044B9A
		// (set) Token: 0x060011E2 RID: 4578 RVA: 0x000469A2 File Offset: 0x00044BA2
		[DataSourceProperty]
		public string FamilyText
		{
			get
			{
				return this._familyText;
			}
			set
			{
				if (value != this._familyText)
				{
					this._familyText = value;
					base.OnPropertyChangedWithValue<string>(value, "FamilyText");
				}
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x060011E3 RID: 4579 RVA: 0x000469C5 File Offset: 0x00044BC5
		// (set) Token: 0x060011E4 RID: 4580 RVA: 0x000469CD File Offset: 0x00044BCD
		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> Stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (value != this._stats)
				{
					this._stats = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "Stats");
				}
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x000469EB File Offset: 0x00044BEB
		// (set) Token: 0x060011E6 RID: 4582 RVA: 0x000469F3 File Offset: 0x00044BF3
		[DataSourceProperty]
		public MBBindingList<EncyclopediaTraitItemVM> Traits
		{
			get
			{
				return this._traits;
			}
			set
			{
				if (value != this._traits)
				{
					this._traits = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTraitItemVM>>(value, "Traits");
				}
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x060011E7 RID: 4583 RVA: 0x00046A11 File Offset: 0x00044C11
		// (set) Token: 0x060011E8 RID: 4584 RVA: 0x00046A19 File Offset: 0x00044C19
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSkillVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSkillVM>>(value, "Skills");
				}
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x00046A37 File Offset: 0x00044C37
		// (set) Token: 0x060011EA RID: 4586 RVA: 0x00046A3F File Offset: 0x00044C3F
		[DataSourceProperty]
		public MBBindingList<EncyclopediaDwellingVM> Dwellings
		{
			get
			{
				return this._dwellings;
			}
			set
			{
				if (value != this._dwellings)
				{
					this._dwellings = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaDwellingVM>>(value, "Dwellings");
				}
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x00046A5D File Offset: 0x00044C5D
		// (set) Token: 0x060011EC RID: 4588 RVA: 0x00046A65 File Offset: 0x00044C65
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

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x00046A83 File Offset: 0x00044C83
		// (set) Token: 0x060011EE RID: 4590 RVA: 0x00046A8B File Offset: 0x00044C8B
		[DataSourceProperty]
		public MBBindingList<EncyclopediaFamilyMemberVM> Family
		{
			get
			{
				return this._family;
			}
			set
			{
				if (value != this._family)
				{
					this._family = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaFamilyMemberVM>>(value, "Family");
				}
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x00046AA9 File Offset: 0x00044CA9
		// (set) Token: 0x060011F0 RID: 4592 RVA: 0x00046AB1 File Offset: 0x00044CB1
		[DataSourceProperty]
		public MBBindingList<HeroVM> Companions
		{
			get
			{
				return this._companions;
			}
			set
			{
				if (value != this._companions)
				{
					this._companions = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Companions");
				}
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x060011F1 RID: 4593 RVA: 0x00046ACF File Offset: 0x00044CCF
		// (set) Token: 0x060011F2 RID: 4594 RVA: 0x00046AD7 File Offset: 0x00044CD7
		[DataSourceProperty]
		public MBBindingList<HeroVM> Enemies
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
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Enemies");
				}
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x00046AF5 File Offset: 0x00044CF5
		// (set) Token: 0x060011F4 RID: 4596 RVA: 0x00046AFD File Offset: 0x00044CFD
		[DataSourceProperty]
		public MBBindingList<HeroVM> Allies
		{
			get
			{
				return this._allies;
			}
			set
			{
				if (value != this._allies)
				{
					this._allies = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroVM>>(value, "Allies");
				}
			}
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x00046B1B File Offset: 0x00044D1B
		// (set) Token: 0x060011F6 RID: 4598 RVA: 0x00046B23 File Offset: 0x00044D23
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

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x00046B41 File Offset: 0x00044D41
		// (set) Token: 0x060011F8 RID: 4600 RVA: 0x00046B49 File Offset: 0x00044D49
		[DataSourceProperty]
		public bool HasNeutralClan
		{
			get
			{
				return this._hasNeutralClan;
			}
			set
			{
				if (value != this._hasNeutralClan)
				{
					this._hasNeutralClan = value;
					base.OnPropertyChangedWithValue(value, "HasNeutralClan");
				}
			}
		}

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x00046B67 File Offset: 0x00044D67
		// (set) Token: 0x060011FA RID: 4602 RVA: 0x00046B6F File Offset: 0x00044D6F
		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChanged("IsAlive");
					base.OnPropertyChangedWithValue(value, "IsDead");
				}
			}
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x00046B98 File Offset: 0x00044D98
		// (set) Token: 0x060011FC RID: 4604 RVA: 0x00046BA0 File Offset: 0x00044DA0
		[DataSourceProperty]
		public bool IsInformationHidden
		{
			get
			{
				return this._isInformationHidden;
			}
			set
			{
				if (value != this._isInformationHidden)
				{
					this._isInformationHidden = value;
					base.OnPropertyChangedWithValue(value, "IsInformationHidden");
				}
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x060011FD RID: 4605 RVA: 0x00046BBE File Offset: 0x00044DBE
		// (set) Token: 0x060011FE RID: 4606 RVA: 0x00046BC6 File Offset: 0x00044DC6
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

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060011FF RID: 4607 RVA: 0x00046BE9 File Offset: 0x00044DE9
		// (set) Token: 0x06001200 RID: 4608 RVA: 0x00046BF1 File Offset: 0x00044DF1
		[DataSourceProperty]
		public HintViewModel PregnantHint
		{
			get
			{
				return this._pregnantHint;
			}
			set
			{
				if (value != this._pregnantHint)
				{
					this._pregnantHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PregnantHint");
				}
			}
		}

		// Token: 0x0400083C RID: 2108
		private readonly Hero _hero;

		// Token: 0x0400083D RID: 2109
		private readonly TextObject _infoHiddenReasonText;

		// Token: 0x0400083E RID: 2110
		private List<Hero> _allRelatedHeroes;

		// Token: 0x0400083F RID: 2111
		private readonly HeroRelationComparer _relationAscendingComparer;

		// Token: 0x04000840 RID: 2112
		private readonly HeroRelationComparer _relationDescendingComparer;

		// Token: 0x04000841 RID: 2113
		private const int _friendLimit = 40;

		// Token: 0x04000842 RID: 2114
		private const int _enemyLimit = -30;

		// Token: 0x04000843 RID: 2115
		private MBBindingList<HeroVM> _enemies;

		// Token: 0x04000844 RID: 2116
		private MBBindingList<HeroVM> _allies;

		// Token: 0x04000845 RID: 2117
		private MBBindingList<EncyclopediaFamilyMemberVM> _family;

		// Token: 0x04000846 RID: 2118
		private MBBindingList<HeroVM> _companions;

		// Token: 0x04000847 RID: 2119
		private MBBindingList<EncyclopediaSettlementVM> _settlements;

		// Token: 0x04000848 RID: 2120
		private MBBindingList<EncyclopediaDwellingVM> _dwellings;

		// Token: 0x04000849 RID: 2121
		private MBBindingList<EncyclopediaHistoryEventVM> _history;

		// Token: 0x0400084A RID: 2122
		private MBBindingList<EncyclopediaSkillVM> _skills;

		// Token: 0x0400084B RID: 2123
		private MBBindingList<StringPairItemVM> _stats;

		// Token: 0x0400084C RID: 2124
		private MBBindingList<EncyclopediaTraitItemVM> _traits;

		// Token: 0x0400084D RID: 2125
		private string _clanText;

		// Token: 0x0400084E RID: 2126
		private string _settlementsText;

		// Token: 0x0400084F RID: 2127
		private string _dwellingsText;

		// Token: 0x04000850 RID: 2128
		private string _alliesText;

		// Token: 0x04000851 RID: 2129
		private string _enemiesText;

		// Token: 0x04000852 RID: 2130
		private string _companionsText;

		// Token: 0x04000853 RID: 2131
		private string _lastSeenText;

		// Token: 0x04000854 RID: 2132
		private string _nameText;

		// Token: 0x04000855 RID: 2133
		private string _informationText;

		// Token: 0x04000856 RID: 2134
		private string _deceasedText;

		// Token: 0x04000857 RID: 2135
		private string _traitsText;

		// Token: 0x04000858 RID: 2136
		private string _skillsText;

		// Token: 0x04000859 RID: 2137
		private string _infoText;

		// Token: 0x0400085A RID: 2138
		private string _kingdomRankText;

		// Token: 0x0400085B RID: 2139
		private string _familyText;

		// Token: 0x0400085C RID: 2140
		private HeroViewModel _heroCharacter;

		// Token: 0x0400085D RID: 2141
		private bool _isCompanion;

		// Token: 0x0400085E RID: 2142
		private bool _isPregnant;

		// Token: 0x0400085F RID: 2143
		private bool _hasNeutralClan;

		// Token: 0x04000860 RID: 2144
		private bool _isDead;

		// Token: 0x04000861 RID: 2145
		private bool _isInformationHidden;

		// Token: 0x04000862 RID: 2146
		private HeroVM _master;

		// Token: 0x04000863 RID: 2147
		private EncyclopediaFactionVM _faction;

		// Token: 0x04000864 RID: 2148
		private string _masterText;

		// Token: 0x04000865 RID: 2149
		private HintViewModel _pregnantHint;
	}
}
