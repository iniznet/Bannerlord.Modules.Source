using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements
{
	// Token: 0x02000058 RID: 88
	public class KingdomSettlementVM : KingdomCategoryVM
	{
		// Token: 0x06000745 RID: 1861 RVA: 0x0001FB78 File Offset: 0x0001DD78
		public KingdomSettlementVM(Action<KingdomDecision> forceDecision, Action<Settlement> onGrantFief)
		{
			this._forceDecision = forceDecision;
			this._onGrantFief = onGrantFief;
			this._kingdom = Hero.MainHero.MapFaction as Kingdom;
			this.AnnexCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAnnexation(this._kingdom);
			this.AnnexHint = new HintViewModel();
			base.IsAcceptableItemSelected = false;
			this.Settlements = new MBBindingList<KingdomSettlementItemVM>();
			this.RefreshSettlementList();
			base.NotificationCount = 0;
			this.SettlementSortController = new KingdomSettlementSortControllerVM(ref this._settlements);
			this.RefreshValues();
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x0001FC10 File Offset: 0x0001DE10
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OwnerText = GameTexts.FindText("str_owner", null).ToString();
			this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
			this.TypeText = GameTexts.FindText("str_sort_by_type_label", null).ToString();
			this.ProsperityText = GameTexts.FindText("str_prosperity_abbr", null).ToString();
			this.FoodText = GameTexts.FindText("str_inventory_category_tooltip", "6").ToString();
			this.GarrisonText = GameTexts.FindText("str_map_tooltip_garrison", null).ToString();
			this.MilitiaText = GameTexts.FindText("str_militia", null).ToString();
			this.ClanText = GameTexts.FindText("str_clans", null).ToString();
			this.VillagesText = GameTexts.FindText("str_villages", null).ToString();
			base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_settlement_selected", null).ToString();
			this.ProposeText = GameTexts.FindText("str_policy_propose", null).ToString();
			this.DefendersText = GameTexts.FindText("str_sort_by_defenders_label", null).ToString();
			base.CategoryNameText = new TextObject("{=qKUjgS6r}Settlement", null).ToString();
			this.Settlements.ApplyActionOnAllItems(delegate(KingdomSettlementItemVM x)
			{
				x.RefreshValues();
			});
			KingdomSettlementItemVM currentSelectedSettlement = this.CurrentSelectedSettlement;
			if (currentSelectedSettlement == null)
			{
				return;
			}
			currentSelectedSettlement.RefreshValues();
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x0001FD84 File Offset: 0x0001DF84
		public void RefreshSettlementList()
		{
			this.Settlements.Clear();
			if (this._kingdom != null)
			{
				foreach (Settlement settlement in this._kingdom.Settlements.Where((Settlement S) => S.IsCastle || S.IsTown))
				{
					this._settlements.Add(new KingdomSettlementItemVM(settlement, new Action<KingdomSettlementItemVM>(this.OnSettlementSelection)));
				}
			}
			if (this.Settlements.Count > 0)
			{
				this.SetCurrentSelectedSettlement(this.Settlements.FirstOrDefault<KingdomSettlementItemVM>());
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0001FE44 File Offset: 0x0001E044
		private void SetCurrentSelectedSettlement(KingdomSettlementItemVM settlementItem)
		{
			if (this.CurrentSelectedSettlement != settlementItem)
			{
				if (this.CurrentSelectedSettlement != null)
				{
					this.CurrentSelectedSettlement.IsSelected = false;
				}
				this.CurrentSelectedSettlement = settlementItem;
				this.CurrentSelectedSettlement.IsSelected = true;
				if (settlementItem != null)
				{
					this._currenItemsUnresolvedDecision = this.GetSettlementsAnyWaitingDecision(settlementItem.Settlement);
					if (this._currenItemsUnresolvedDecision != null)
					{
						base.IsAcceptableItemSelected = true;
						this.AnnexCost = 0;
						this.AnnexText = GameTexts.FindText("str_resolve", null).ToString();
						this.AnnexActionExplanationText = GameTexts.FindText("str_resolve_explanation", null).ToString();
						this.AnnexHint.HintText = TextObject.Empty;
					}
					else if (settlementItem.Owner.Hero == Hero.MainHero)
					{
						if (Hero.MainHero.IsFactionLeader)
						{
							this.AnnexActionExplanationText = new TextObject("{=G2h0V10w}Gift this settlement to a clan in your kingdom.", null).ToString();
							this.AnnexText = new TextObject("{=sffGeQ1g}Gift", null).ToString();
						}
						else
						{
							this.AnnexActionExplanationText = new TextObject("{=1UbocG5B}Denounce your rights and responsibilities from this fief by giving it back to the realm.", null).ToString();
							this.AnnexText = new TextObject("{=U3ksQXD3}Give Away", null).ToString();
						}
						if (Hero.MainHero.IsPrisoner)
						{
							this.CanAnnexCurrentSettlement = false;
							this.HasCost = true;
							this.AnnexHint.HintText = GameTexts.FindText("str_action_disabled_reason_prisoner", null);
						}
						else if (!Campaign.Current.Models.DiplomacyModel.CanSettlementBeGifted(this._currentSelectedSettlement.Settlement))
						{
							this.CanAnnexCurrentSettlement = false;
							this.HasCost = true;
							this.AnnexHint.HintText = GameTexts.FindText("str_cannot_annex_waiting_for_ruler_decision", null);
						}
						else if (PlayerEncounter.Current != null && PlayerEncounter.EncounterSettlement == null)
						{
							this.CanAnnexCurrentSettlement = false;
							this.HasCost = true;
							this.AnnexHint.HintText = GameTexts.FindText("str_action_disabled_reason_encounter", null);
						}
						else if (PlayerSiege.PlayerSiegeEvent != null)
						{
							this.CanAnnexCurrentSettlement = false;
							this.HasCost = true;
							this.AnnexHint.HintText = GameTexts.FindText("str_action_disabled_reason_siege", null);
						}
						else
						{
							this.CanAnnexCurrentSettlement = true;
							this.HasCost = false;
							this.AnnexHint.HintText = TextObject.Empty;
						}
					}
					else
					{
						this.AnnexText = GameTexts.FindText("str_policy_propose", null).ToString();
						this.AnnexCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAnnexation(this._kingdom);
						string text = GameTexts.FindText("str_annex_fief_action_explanation", null).ToString();
						int num = KingdomSettlementVM.CalculateLikelihood(settlementItem.Settlement);
						text = string.Format("{0} ({1}%)", text, num);
						this.AnnexActionExplanationText = text;
						TextObject textObject;
						this.CanAnnexCurrentSettlement = this.GetCanAnnexSettlementWithReason(this.AnnexCost, out textObject);
						this.AnnexHint.HintText = textObject;
						this.HasCost = true;
					}
				}
				base.IsAcceptableItemSelected = this.CurrentSelectedSettlement != null;
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0002011C File Offset: 0x0001E31C
		private bool GetCanAnnexSettlementWithReason(int annexCost, out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (Hero.MainHero.Clan.Influence < (float)annexCost)
			{
				disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence", null);
				return false;
			}
			if (this.CurrentSelectedSettlement.Settlement.OwnerClan == this._kingdom.RulingClan)
			{
				disabledReason = GameTexts.FindText("str_cannot_annex_ruling_clan_settlement", null);
				return false;
			}
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				disabledReason = GameTexts.FindText("str_cannot_annex_while_mercenary", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x000201A8 File Offset: 0x0001E3A8
		public void SelectSettlement(Settlement settlement)
		{
			foreach (KingdomSettlementItemVM kingdomSettlementItemVM in this.Settlements)
			{
				if (kingdomSettlementItemVM.Settlement == settlement)
				{
					this.OnSettlementSelection(kingdomSettlementItemVM);
					break;
				}
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00020200 File Offset: 0x0001E400
		private void OnSettlementSelection(KingdomSettlementItemVM settlement)
		{
			if (this._currentSelectedSettlement != settlement)
			{
				this.SetCurrentSelectedSettlement(settlement);
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00020214 File Offset: 0x0001E414
		private void ExecuteAnnex()
		{
			if (this._currentSelectedSettlement != null)
			{
				if (this._currenItemsUnresolvedDecision != null)
				{
					this._forceDecision(this._currenItemsUnresolvedDecision);
					return;
				}
				Settlement settlement = this._currentSelectedSettlement.Settlement;
				if (settlement.OwnerClan.Leader == Hero.MainHero)
				{
					this._onGrantFief(settlement);
					return;
				}
				if (Hero.MainHero.Clan.Influence >= (float)this.AnnexCost)
				{
					SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision = new SettlementClaimantPreliminaryDecision(Clan.PlayerClan, settlement);
					Clan.PlayerClan.Kingdom.AddDecision(settlementClaimantPreliminaryDecision, false);
					this._forceDecision(settlementClaimantPreliminaryDecision);
				}
			}
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000202B4 File Offset: 0x0001E4B4
		private KingdomDecision GetSettlementsAnyWaitingDecision(Settlement settlement)
		{
			KingdomDecision kingdomDecision = Clan.PlayerClan.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision d)
			{
				SettlementClaimantDecision settlementClaimantDecision;
				return (settlementClaimantDecision = d as SettlementClaimantDecision) != null && settlementClaimantDecision.Settlement == settlement && !d.ShouldBeCancelled();
			});
			KingdomDecision kingdomDecision2 = Clan.PlayerClan.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision d)
			{
				SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision;
				return (settlementClaimantPreliminaryDecision = d as SettlementClaimantPreliminaryDecision) != null && settlementClaimantPreliminaryDecision.Settlement == settlement && !d.ShouldBeCancelled();
			});
			return kingdomDecision ?? kingdomDecision2;
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x0600074E RID: 1870 RVA: 0x00020314 File Offset: 0x0001E514
		// (set) Token: 0x0600074F RID: 1871 RVA: 0x0002031C File Offset: 0x0001E51C
		[DataSourceProperty]
		public KingdomSettlementItemVM CurrentSelectedSettlement
		{
			get
			{
				return this._currentSelectedSettlement;
			}
			set
			{
				if (value != this._currentSelectedSettlement)
				{
					this._currentSelectedSettlement = value;
					base.OnPropertyChangedWithValue<KingdomSettlementItemVM>(value, "CurrentSelectedSettlement");
				}
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000750 RID: 1872 RVA: 0x0002033A File Offset: 0x0001E53A
		// (set) Token: 0x06000751 RID: 1873 RVA: 0x00020342 File Offset: 0x0001E542
		[DataSourceProperty]
		public KingdomSettlementSortControllerVM SettlementSortController
		{
			get
			{
				return this._settlementSortController;
			}
			set
			{
				if (value != this._settlementSortController)
				{
					this._settlementSortController = value;
					base.OnPropertyChangedWithValue<KingdomSettlementSortControllerVM>(value, "SettlementSortController");
				}
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x00020360 File Offset: 0x0001E560
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x00020368 File Offset: 0x0001E568
		[DataSourceProperty]
		public HintViewModel AnnexHint
		{
			get
			{
				return this._annexHint;
			}
			set
			{
				if (value != this._annexHint)
				{
					this._annexHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AnnexHint");
				}
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00020386 File Offset: 0x0001E586
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x0002038E File Offset: 0x0001E58E
		[DataSourceProperty]
		public string ProposeText
		{
			get
			{
				return this._proposeText;
			}
			set
			{
				if (value != this._proposeText)
				{
					this._proposeText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProposeText");
				}
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x000203B1 File Offset: 0x0001E5B1
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x000203B9 File Offset: 0x0001E5B9
		[DataSourceProperty]
		public string AnnexActionExplanationText
		{
			get
			{
				return this._annexActionExplanationText;
			}
			set
			{
				if (value != this._annexActionExplanationText)
				{
					this._annexActionExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "AnnexActionExplanationText");
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x000203DC File Offset: 0x0001E5DC
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x000203E4 File Offset: 0x0001E5E4
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

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x00020407 File Offset: 0x0001E607
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x0002040F File Offset: 0x0001E60F
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

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x00020432 File Offset: 0x0001E632
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x0002043A File Offset: 0x0001E63A
		[DataSourceProperty]
		public string OwnerText
		{
			get
			{
				return this._ownerText;
			}
			set
			{
				if (value != this._ownerText)
				{
					this._ownerText = value;
					base.OnPropertyChangedWithValue<string>(value, "OwnerText");
				}
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x0002045D File Offset: 0x0001E65D
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x00020465 File Offset: 0x0001E665
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

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00020488 File Offset: 0x0001E688
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x00020490 File Offset: 0x0001E690
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

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x000204B3 File Offset: 0x0001E6B3
		// (set) Token: 0x06000763 RID: 1891 RVA: 0x000204BB File Offset: 0x0001E6BB
		[DataSourceProperty]
		public string FoodText
		{
			get
			{
				return this._foodText;
			}
			set
			{
				if (value != this._foodText)
				{
					this._foodText = value;
					base.OnPropertyChangedWithValue<string>(value, "FoodText");
				}
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x000204DE File Offset: 0x0001E6DE
		// (set) Token: 0x06000765 RID: 1893 RVA: 0x000204E6 File Offset: 0x0001E6E6
		[DataSourceProperty]
		public string GarrisonText
		{
			get
			{
				return this._garrisonText;
			}
			set
			{
				if (value != this._garrisonText)
				{
					this._garrisonText = value;
					base.OnPropertyChangedWithValue<string>(value, "GarrisonText");
				}
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x00020509 File Offset: 0x0001E709
		// (set) Token: 0x06000767 RID: 1895 RVA: 0x00020511 File Offset: 0x0001E711
		[DataSourceProperty]
		public string MilitiaText
		{
			get
			{
				return this._militiaText;
			}
			set
			{
				if (value != this._militiaText)
				{
					this._militiaText = value;
					base.OnPropertyChangedWithValue<string>(value, "MilitiaText");
				}
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x00020534 File Offset: 0x0001E734
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x0002053C File Offset: 0x0001E73C
		[DataSourceProperty]
		public string AnnexText
		{
			get
			{
				return this._annexText;
			}
			set
			{
				if (value != this._annexText)
				{
					this._annexText = value;
					base.OnPropertyChangedWithValue<string>(value, "AnnexText");
				}
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x0002055F File Offset: 0x0001E75F
		// (set) Token: 0x0600076B RID: 1899 RVA: 0x00020567 File Offset: 0x0001E767
		[DataSourceProperty]
		public string TypeText
		{
			get
			{
				return this._typeText;
			}
			set
			{
				if (value != this._typeText)
				{
					this._typeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeText");
				}
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0002058A File Offset: 0x0001E78A
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x00020592 File Offset: 0x0001E792
		[DataSourceProperty]
		public int AnnexCost
		{
			get
			{
				return this._annexCost;
			}
			set
			{
				if (value != this._annexCost)
				{
					this._annexCost = value;
					base.OnPropertyChangedWithValue(value, "AnnexCost");
				}
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x000205B0 File Offset: 0x0001E7B0
		// (set) Token: 0x0600076F RID: 1903 RVA: 0x000205B8 File Offset: 0x0001E7B8
		[DataSourceProperty]
		public string DefendersText
		{
			get
			{
				return this._defendersText;
			}
			set
			{
				if (value != this._defendersText)
				{
					this._defendersText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefendersText");
				}
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x000205DB File Offset: 0x0001E7DB
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x000205E3 File Offset: 0x0001E7E3
		[DataSourceProperty]
		public MBBindingList<KingdomSettlementItemVM> Settlements
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
					base.OnPropertyChangedWithValue<MBBindingList<KingdomSettlementItemVM>>(value, "Settlements");
				}
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x00020601 File Offset: 0x0001E801
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x00020609 File Offset: 0x0001E809
		[DataSourceProperty]
		public bool CanAnnexCurrentSettlement
		{
			get
			{
				return this._canAnnexCurrentSettlement;
			}
			set
			{
				if (value != this._canAnnexCurrentSettlement)
				{
					this._canAnnexCurrentSettlement = value;
					base.OnPropertyChangedWithValue(value, "CanAnnexCurrentSettlement");
				}
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x00020627 File Offset: 0x0001E827
		// (set) Token: 0x06000775 RID: 1909 RVA: 0x0002062F File Offset: 0x0001E82F
		[DataSourceProperty]
		public bool HasCost
		{
			get
			{
				return this._hasCost;
			}
			set
			{
				if (value != this._hasCost)
				{
					this._hasCost = value;
					base.OnPropertyChangedWithValue(value, "HasCost");
				}
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x0002064D File Offset: 0x0001E84D
		private static int CalculateLikelihood(Settlement settlement)
		{
			return MathF.Round(new KingdomElection(new SettlementClaimantPreliminaryDecision(Clan.PlayerClan, settlement)).GetLikelihoodForSponsor(Clan.PlayerClan) * 100f);
		}

		// Token: 0x04000334 RID: 820
		private readonly Action<KingdomDecision> _forceDecision;

		// Token: 0x04000335 RID: 821
		private readonly Action<Settlement> _onGrantFief;

		// Token: 0x04000336 RID: 822
		private readonly Kingdom _kingdom;

		// Token: 0x04000337 RID: 823
		private KingdomDecision _currenItemsUnresolvedDecision;

		// Token: 0x04000338 RID: 824
		private MBBindingList<KingdomSettlementItemVM> _settlements;

		// Token: 0x04000339 RID: 825
		private KingdomSettlementItemVM _currentSelectedSettlement;

		// Token: 0x0400033A RID: 826
		private HintViewModel _annexHint;

		// Token: 0x0400033B RID: 827
		private string _ownerText;

		// Token: 0x0400033C RID: 828
		private string _nameText;

		// Token: 0x0400033D RID: 829
		private string _typeText;

		// Token: 0x0400033E RID: 830
		private string _prosperityText;

		// Token: 0x0400033F RID: 831
		private string _foodText;

		// Token: 0x04000340 RID: 832
		private string _garrisonText;

		// Token: 0x04000341 RID: 833
		private string _militiaText;

		// Token: 0x04000342 RID: 834
		private string _annexText;

		// Token: 0x04000343 RID: 835
		private string _clanText;

		// Token: 0x04000344 RID: 836
		private string _villagesText;

		// Token: 0x04000345 RID: 837
		private string _annexActionExplanationText;

		// Token: 0x04000346 RID: 838
		private string _proposeText;

		// Token: 0x04000347 RID: 839
		private string _defendersText;

		// Token: 0x04000348 RID: 840
		private int _annexCost;

		// Token: 0x04000349 RID: 841
		private bool _canAnnexCurrentSettlement;

		// Token: 0x0400034A RID: 842
		private bool _hasCost;

		// Token: 0x0400034B RID: 843
		private KingdomSettlementSortControllerVM _settlementSortController;
	}
}
