using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Policies;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement
{
	// Token: 0x02000055 RID: 85
	public class KingdomManagementVM : ViewModel
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0001DFD8 File Offset: 0x0001C1D8
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
		public Kingdom Kingdom { get; private set; }

		// Token: 0x060006AC RID: 1708 RVA: 0x0001DFEC File Offset: 0x0001C1EC
		public KingdomManagementVM(Action onClose, Action onManageArmy, Action<Army> onShowArmyOnMap)
		{
			this._onClose = onClose;
			this._onShowArmyOnMap = onShowArmyOnMap;
			this.Army = new KingdomArmyVM(onManageArmy, new Action(this.OnRefreshDecision), this._onShowArmyOnMap);
			this.Settlement = new KingdomSettlementVM(new Action<KingdomDecision>(this.ForceDecideDecision), new Action<Settlement>(this.OnGrantFief));
			this.Clan = new KingdomClanVM(new Action<KingdomDecision>(this.ForceDecideDecision));
			this.Policy = new KingdomPoliciesVM(new Action<KingdomDecision>(this.ForceDecideDecision));
			this.Diplomacy = new KingdomDiplomacyVM(new Action<KingdomDecision>(this.ForceDecideDecision));
			this.GiftFief = new KingdomGiftFiefPopupVM(new Action(this.OnSettlementGranted));
			this.Decision = new KingdomDecisionsVM(new Action(this.OnRefresh));
			this._categoryCount = 5;
			this.SetSelectedCategory(1);
			this.RefreshValues();
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001E0D8 File Offset: 0x0001C2D8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LeaderText = GameTexts.FindText("str_sort_by_leader_name_label", null).ToString();
			this.ClansText = GameTexts.FindText("str_encyclopedia_clans", null).ToString();
			this.FiefsText = GameTexts.FindText("str_fiefs", null).ToString();
			this.PoliciesText = GameTexts.FindText("str_policies", null).ToString();
			this.ArmiesText = GameTexts.FindText("str_armies", null).ToString();
			this.DiplomacyText = GameTexts.FindText("str_diplomatic_group", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.ChangeKingdomNameHint = new HintViewModel();
			this.RefreshDynamicKingdomProperties();
			this.Army.RefreshValues();
			this.Policy.RefreshValues();
			this.Clan.RefreshValues();
			this.Settlement.RefreshValues();
			this.Diplomacy.RefreshValues();
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0001E1D0 File Offset: 0x0001C3D0
		private void RefreshDynamicKingdomProperties()
		{
			this.Name = ((Hero.MainHero.MapFaction == null) ? new TextObject("{=kQsXUvgO}You are not under a kingdom.", null).ToString() : Hero.MainHero.MapFaction.Name.ToString());
			this.PlayerHasKingdom = Hero.MainHero.MapFaction is Kingdom;
			if (this.PlayerHasKingdom)
			{
				this.Kingdom = Hero.MainHero.MapFaction as Kingdom;
				this.Leader = new HeroVM(this.Kingdom.Leader, false);
				this.KingdomBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.Kingdom.Banner), true);
				Kingdom kingdom = this.Kingdom;
				this._isPlayerTheRuler = ((kingdom != null) ? kingdom.Leader : null) == Hero.MainHero;
				TextObject textObject;
				this.PlayerCanChangeKingdomName = this.GetCanChangeKingdomNameWithReason(out textObject);
				this.ChangeKingdomNameHint.HintText = textObject;
			}
			this.KingdomActionText = (this._isPlayerTheRuler ? GameTexts.FindText("str_abdicate_leadership", null).ToString() : GameTexts.FindText("str_leave_kingdom", null).ToString());
			List<TextObject> kingdomActionDisabledReasons;
			this.IsKingdomActionEnabled = this.GetIsKingdomActionEnabledWithReason(this._isPlayerTheRuler, out kingdomActionDisabledReasons);
			this.KingdomActionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetHintTextFromReasons(kingdomActionDisabledReasons));
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001E320 File Offset: 0x0001C520
		private bool GetCanChangeKingdomNameWithReason(out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (!this._isPlayerTheRuler)
			{
				disabledReason = new TextObject("{=HFZdseH9}Only the ruler of the kingdom can change it's name.", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001E35C File Offset: 0x0001C55C
		private bool GetIsKingdomActionEnabledWithReason(bool isPlayerTheRuler, out List<TextObject> disabledReasons)
		{
			disabledReasons = new List<TextObject>();
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReasons.Add(textObject);
				return false;
			}
			List<TextObject> list;
			if (isPlayerTheRuler && !Campaign.Current.Models.KingdomCreationModel.IsPlayerKingdomAbdicationPossible(out list))
			{
				disabledReasons.AddRange(list);
				return false;
			}
			if (!isPlayerTheRuler && MobileParty.MainParty.Army != null)
			{
				disabledReasons.Add(new TextObject("{=4Y8u4JKO}You can't leave the kingdom while in an army", null));
				return false;
			}
			return true;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001E3CC File Offset: 0x0001C5CC
		public void OnRefresh()
		{
			this.RefreshDynamicKingdomProperties();
			this.Army.RefreshArmyList();
			this.Policy.RefreshPolicyList();
			this.Clan.RefreshClan();
			this.Settlement.RefreshSettlementList();
			this.Diplomacy.RefreshDiplomacyList();
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0001E40B File Offset: 0x0001C60B
		public void OnFrameTick()
		{
			KingdomDecisionsVM decision = this.Decision;
			if (decision == null)
			{
				return;
			}
			decision.OnFrameTick();
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001E41D File Offset: 0x0001C61D
		private void OnRefreshDecision()
		{
			this.Decision.QueryForNextDecision();
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0001E42A File Offset: 0x0001C62A
		private void ForceDecideDecision(KingdomDecision decision)
		{
			this.Decision.RefreshWith(decision);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0001E438 File Offset: 0x0001C638
		private void OnGrantFief(Settlement settlement)
		{
			if (this.Kingdom.Leader == Hero.MainHero)
			{
				this.GiftFief.OpenWith(settlement);
				return;
			}
			string text = new TextObject("{=eIGFuGOx}Give Settlement", null).ToString();
			string text2 = new TextObject("{=rkubGa4K}Are you sure want to give this settlement back to your kingdom?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				Campaign.Current.KingdomManager.RelinquishSettlementOwnership(settlement);
				this.ForceDecideDecision(this.Kingdom.UnresolvedDecisions[this.Kingdom.UnresolvedDecisions.Count - 1]);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0001E4E7 File Offset: 0x0001C6E7
		private void OnSettlementGranted()
		{
			this.Settlement.RefreshSettlementList();
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001E4F4 File Offset: 0x0001C6F4
		public void ExecuteClose()
		{
			this._onClose();
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0001E501 File Offset: 0x0001C701
		private void ExecuteShowClan()
		{
			this.SetSelectedCategory(0);
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0001E50A File Offset: 0x0001C70A
		private void ExecuteShowFiefs()
		{
			this.SetSelectedCategory(1);
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0001E513 File Offset: 0x0001C713
		private void ExecuteShowPolicies()
		{
			if (this.PlayerHasKingdom)
			{
				this.SetSelectedCategory(2);
			}
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0001E524 File Offset: 0x0001C724
		private void ExecuteShowDiplomacy()
		{
			if (this.PlayerHasKingdom)
			{
				this.SetSelectedCategory(4);
			}
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0001E535 File Offset: 0x0001C735
		private void ExecuteShowArmy()
		{
			this.SetSelectedCategory(3);
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0001E540 File Offset: 0x0001C740
		private void ExecuteKingdomAction()
		{
			if (this.IsKingdomActionEnabled)
			{
				if (this._isPlayerTheRuler)
				{
					GameTexts.SetVariable("WILL_DESTROY", (this.Kingdom.Clans.Count == 1) ? 1 : 0);
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_abdicate_leadership", null).ToString(), GameTexts.FindText("str_abdicate_leadership_question", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.OnConfirmAbdicateLeadership), null, "", 0f, null, null, null), false, false);
					return;
				}
				if (TaleWorlds.CampaignSystem.Clan.PlayerClan.Settlements.Count == 0)
				{
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=3sxtCWPe}Leaving Kingdom", null).ToString(), new TextObject("{=BgqZWbga}The nobles of the realm will dislike you for abandoning your fealty. Are you sure you want to leave the Kingdom?", null).ToString(), true, true, new TextObject("{=5Unqsx3N}Confirm", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnConfirmLeaveKingdom), null, "", 0f, null, null, null), false, false);
					return;
				}
				List<InquiryElement> list = new List<InquiryElement>
				{
					new InquiryElement("keep", new TextObject("{=z8h0BRAb}Keep all holdings", null).ToString(), null, true, new TextObject("{=lkJfq1ap}Owned settlements remain under your control but nobles will dislike this dishonorable act and the kingdom will declare war on you.", null).ToString()),
					new InquiryElement("dontkeep", new TextObject("{=JIr3Jc7b}Relinquish all holdings", null).ToString(), null, true, new TextObject("{=ZjaSde0X}Owned settlements are returned to the kingdom. This will avert a war and nobles will dislike you less for abandoning your fealty.", null).ToString())
				};
				MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=3sxtCWPe}Leaving Kingdom", null).ToString(), new TextObject("{=xtlIFKaa}Are you sure you want to leave the Kingdom?{newline}If so, choose how you want to leave the kingdom.", null).ToString(), list, true, 1, new TextObject("{=5Unqsx3N}Confirm", null).ToString(), string.Empty, new Action<List<InquiryElement>>(this.OnConfirmLeaveKingdomWithOption), null, ""), false, false);
			}
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0001E728 File Offset: 0x0001C928
		private void OnConfirmAbdicateLeadership()
		{
			Campaign.Current.KingdomManager.AbdicateTheThrone(this.Kingdom);
			KingdomDecision kingdomDecision = this.Kingdom.UnresolvedDecisions.LastOrDefault<KingdomDecision>();
			if (kingdomDecision != null)
			{
				this.ForceDecideDecision(kingdomDecision);
				return;
			}
			this.ExecuteClose();
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0001E76C File Offset: 0x0001C96C
		private void OnConfirmLeaveKingdomWithOption(List<InquiryElement> obj)
		{
			InquiryElement inquiryElement = obj.FirstOrDefault<InquiryElement>();
			if (inquiryElement != null)
			{
				string text = inquiryElement.Identifier as string;
				if (text == "keep")
				{
					ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(TaleWorlds.CampaignSystem.Clan.PlayerClan, true);
				}
				else if (text == "dontkeep")
				{
					ChangeKingdomAction.ApplyByLeaveKingdom(TaleWorlds.CampaignSystem.Clan.PlayerClan, true);
				}
				this.ExecuteClose();
			}
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0001E7C7 File Offset: 0x0001C9C7
		private void OnConfirmLeaveKingdom()
		{
			if (TaleWorlds.CampaignSystem.Clan.PlayerClan.IsUnderMercenaryService)
			{
				ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(TaleWorlds.CampaignSystem.Clan.PlayerClan, true);
			}
			else
			{
				ChangeKingdomAction.ApplyByLeaveKingdom(TaleWorlds.CampaignSystem.Clan.PlayerClan, true);
			}
			this.ExecuteClose();
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0001E7F4 File Offset: 0x0001C9F4
		private void ExecuteChangeKingdomName()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(GameTexts.FindText("str_change_kingdom_name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnChangeKingdomNameDone), null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsKingdomNameApplicable), "", ""), false, false);
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0001E868 File Offset: 0x0001CA68
		private void OnChangeKingdomNameDone(string newKingdomName)
		{
			TextObject textObject = new TextObject(newKingdomName, null);
			TextObject textObject2 = GameTexts.FindText("str_generic_kingdom_name", null);
			TextObject textObject3 = GameTexts.FindText("str_generic_kingdom_short_name", null);
			textObject2.SetTextVariable("KINGDOM_NAME", textObject);
			textObject3.SetTextVariable("KINGDOM_SHORT_NAME", textObject);
			this.Kingdom.ChangeKingdomName(textObject2, textObject3);
			this.OnRefresh();
			this.RefreshValues();
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0001E8C8 File Offset: 0x0001CAC8
		public void SelectArmy(Army army)
		{
			this.SetSelectedCategory(3);
			this.Army.SelectArmy(army);
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0001E8DD File Offset: 0x0001CADD
		public void SelectSettlement(Settlement settlement)
		{
			this.SetSelectedCategory(1);
			this.Settlement.SelectSettlement(settlement);
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0001E8F2 File Offset: 0x0001CAF2
		public void SelectClan(Clan clan)
		{
			this.SetSelectedCategory(0);
			this.Clan.SelectClan(clan);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0001E907 File Offset: 0x0001CB07
		public void SelectPolicy(PolicyObject policy)
		{
			this.SetSelectedCategory(2);
			this.Policy.SelectPolicy(policy);
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0001E91C File Offset: 0x0001CB1C
		public void SelectKingdom(Kingdom kingdom)
		{
			this.SetSelectedCategory(4);
			this.Diplomacy.SelectKingdom(kingdom);
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0001E934 File Offset: 0x0001CB34
		public void SelectPreviousCategory()
		{
			int num = ((this._currentCategory == 0) ? (this._categoryCount - 1) : (this._currentCategory - 1));
			this.SetSelectedCategory(num);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0001E964 File Offset: 0x0001CB64
		public void SelectNextCategory()
		{
			int num = (this._currentCategory + 1) % this._categoryCount;
			this.SetSelectedCategory(num);
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0001E988 File Offset: 0x0001CB88
		private void SetSelectedCategory(int index)
		{
			this.Clan.Show = false;
			this.Settlement.Show = false;
			this.Policy.Show = false;
			this.Army.Show = false;
			this.Diplomacy.Show = false;
			this._currentCategory = index;
			if (index == 0)
			{
				this.Clan.Show = true;
				return;
			}
			if (index == 1)
			{
				this.Settlement.Show = true;
				return;
			}
			if (index == 2)
			{
				this.Policy.Show = true;
				return;
			}
			if (index == 3)
			{
				this.Army.Show = true;
				return;
			}
			this._currentCategory = 4;
			this.Diplomacy.Show = true;
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0001EA2E File Offset: 0x0001CC2E
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.PreviousTabInputKey.OnFinalize();
			this.NextTabInputKey.OnFinalize();
			this.Decision.OnFinalize();
			this.Clan.OnFinalize();
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x0001EA6D File Offset: 0x0001CC6D
		// (set) Token: 0x060006CD RID: 1741 RVA: 0x0001EA75 File Offset: 0x0001CC75
		[DataSourceProperty]
		public BasicTooltipViewModel KingdomActionHint
		{
			get
			{
				return this._kingdomActionHint;
			}
			set
			{
				if (value != this._kingdomActionHint)
				{
					this._kingdomActionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "KingdomActionHint");
				}
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060006CE RID: 1742 RVA: 0x0001EA93 File Offset: 0x0001CC93
		// (set) Token: 0x060006CF RID: 1743 RVA: 0x0001EA9B File Offset: 0x0001CC9B
		[DataSourceProperty]
		public ImageIdentifierVM KingdomBanner
		{
			get
			{
				return this._kingdomBanner;
			}
			set
			{
				if (value != this._kingdomBanner)
				{
					this._kingdomBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "KingdomBanner");
				}
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x0001EAB9 File Offset: 0x0001CCB9
		// (set) Token: 0x060006D1 RID: 1745 RVA: 0x0001EAC1 File Offset: 0x0001CCC1
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

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x0001EADF File Offset: 0x0001CCDF
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x0001EAE7 File Offset: 0x0001CCE7
		[DataSourceProperty]
		public KingdomArmyVM Army
		{
			get
			{
				return this._army;
			}
			set
			{
				if (value != this._army)
				{
					this._army = value;
					base.OnPropertyChangedWithValue<KingdomArmyVM>(value, "Army");
				}
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x0001EB05 File Offset: 0x0001CD05
		// (set) Token: 0x060006D5 RID: 1749 RVA: 0x0001EB0D File Offset: 0x0001CD0D
		[DataSourceProperty]
		public KingdomSettlementVM Settlement
		{
			get
			{
				return this._settlement;
			}
			set
			{
				if (value != this._settlement)
				{
					this._settlement = value;
					base.OnPropertyChangedWithValue<KingdomSettlementVM>(value, "Settlement");
				}
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x0001EB2B File Offset: 0x0001CD2B
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x0001EB33 File Offset: 0x0001CD33
		[DataSourceProperty]
		public KingdomClanVM Clan
		{
			get
			{
				return this._clan;
			}
			set
			{
				if (value != this._clan)
				{
					this._clan = value;
					base.OnPropertyChangedWithValue<KingdomClanVM>(value, "Clan");
				}
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x0001EB51 File Offset: 0x0001CD51
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x0001EB59 File Offset: 0x0001CD59
		[DataSourceProperty]
		public KingdomPoliciesVM Policy
		{
			get
			{
				return this._policy;
			}
			set
			{
				if (value != this._policy)
				{
					this._policy = value;
					base.OnPropertyChangedWithValue<KingdomPoliciesVM>(value, "Policy");
				}
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x0001EB77 File Offset: 0x0001CD77
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x0001EB7F File Offset: 0x0001CD7F
		[DataSourceProperty]
		public KingdomDiplomacyVM Diplomacy
		{
			get
			{
				return this._diplomacy;
			}
			set
			{
				if (value != this._diplomacy)
				{
					this._diplomacy = value;
					base.OnPropertyChangedWithValue<KingdomDiplomacyVM>(value, "Diplomacy");
				}
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0001EB9D File Offset: 0x0001CD9D
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x0001EBA5 File Offset: 0x0001CDA5
		[DataSourceProperty]
		public KingdomGiftFiefPopupVM GiftFief
		{
			get
			{
				return this._giftFief;
			}
			set
			{
				if (value != this._giftFief)
				{
					this._giftFief = value;
					base.OnPropertyChangedWithValue<KingdomGiftFiefPopupVM>(value, "GiftFief");
				}
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x0001EBC3 File Offset: 0x0001CDC3
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x0001EBCB File Offset: 0x0001CDCB
		[DataSourceProperty]
		public KingdomDecisionsVM Decision
		{
			get
			{
				return this._decision;
			}
			set
			{
				if (value != this._decision)
				{
					this._decision = value;
					base.OnPropertyChangedWithValue<KingdomDecisionsVM>(value, "Decision");
				}
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0001EBE9 File Offset: 0x0001CDE9
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0001EBF1 File Offset: 0x0001CDF1
		[DataSourceProperty]
		public HintViewModel ChangeKingdomNameHint
		{
			get
			{
				return this._changeKingdomNameHint;
			}
			set
			{
				if (value != this._changeKingdomNameHint)
				{
					this._changeKingdomNameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ChangeKingdomNameHint");
				}
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x0001EC0F File Offset: 0x0001CE0F
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x0001EC17 File Offset: 0x0001CE17
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

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0001EC3A File Offset: 0x0001CE3A
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0001EC42 File Offset: 0x0001CE42
		[DataSourceProperty]
		public bool CanSwitchTabs
		{
			get
			{
				return this._canSwitchTabs;
			}
			set
			{
				if (value != this._canSwitchTabs)
				{
					this._canSwitchTabs = value;
					base.OnPropertyChangedWithValue(value, "CanSwitchTabs");
				}
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001EC60 File Offset: 0x0001CE60
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0001EC68 File Offset: 0x0001CE68
		[DataSourceProperty]
		public bool PlayerHasKingdom
		{
			get
			{
				return this._playerHasKingdom;
			}
			set
			{
				if (value != this._playerHasKingdom)
				{
					this._playerHasKingdom = value;
					base.OnPropertyChangedWithValue(value, "PlayerHasKingdom");
				}
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0001EC86 File Offset: 0x0001CE86
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x0001EC8E File Offset: 0x0001CE8E
		[DataSourceProperty]
		public bool IsKingdomActionEnabled
		{
			get
			{
				return this._isKingdomActionEnabled;
			}
			set
			{
				if (value != this._isKingdomActionEnabled)
				{
					this._isKingdomActionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsKingdomActionEnabled");
				}
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x0001ECAC File Offset: 0x0001CEAC
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x0001ECB4 File Offset: 0x0001CEB4
		[DataSourceProperty]
		public bool PlayerCanChangeKingdomName
		{
			get
			{
				return this._playerCanChangeKingdomName;
			}
			set
			{
				if (value != this._playerCanChangeKingdomName)
				{
					this._playerCanChangeKingdomName = value;
					base.OnPropertyChangedWithValue(value, "PlayerCanChangeKingdomName");
				}
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0001ECD2 File Offset: 0x0001CED2
		// (set) Token: 0x060006ED RID: 1773 RVA: 0x0001ECDA File Offset: 0x0001CEDA
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

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0001ECFD File Offset: 0x0001CEFD
		// (set) Token: 0x060006EF RID: 1775 RVA: 0x0001ED05 File Offset: 0x0001CF05
		[DataSourceProperty]
		public string KingdomActionText
		{
			get
			{
				return this._kingdomActionText;
			}
			set
			{
				if (value != this._kingdomActionText)
				{
					this._kingdomActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "KingdomActionText");
				}
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x0001ED28 File Offset: 0x0001CF28
		// (set) Token: 0x060006F1 RID: 1777 RVA: 0x0001ED30 File Offset: 0x0001CF30
		[DataSourceProperty]
		public string ClansText
		{
			get
			{
				return this._clansText;
			}
			set
			{
				if (value != this._clansText)
				{
					this._clansText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClansText");
				}
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0001ED53 File Offset: 0x0001CF53
		// (set) Token: 0x060006F3 RID: 1779 RVA: 0x0001ED5B File Offset: 0x0001CF5B
		[DataSourceProperty]
		public string DiplomacyText
		{
			get
			{
				return this._diplomacyText;
			}
			set
			{
				if (value != this._diplomacyText)
				{
					this._diplomacyText = value;
					base.OnPropertyChangedWithValue<string>(value, "DiplomacyText");
				}
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0001ED7E File Offset: 0x0001CF7E
		// (set) Token: 0x060006F5 RID: 1781 RVA: 0x0001ED86 File Offset: 0x0001CF86
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0001EDA9 File Offset: 0x0001CFA9
		// (set) Token: 0x060006F7 RID: 1783 RVA: 0x0001EDB1 File Offset: 0x0001CFB1
		[DataSourceProperty]
		public string FiefsText
		{
			get
			{
				return this._fiefsText;
			}
			set
			{
				if (value != this._fiefsText)
				{
					this._fiefsText = value;
					base.OnPropertyChangedWithValue<string>(value, "FiefsText");
				}
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060006F8 RID: 1784 RVA: 0x0001EDD4 File Offset: 0x0001CFD4
		// (set) Token: 0x060006F9 RID: 1785 RVA: 0x0001EDDC File Offset: 0x0001CFDC
		[DataSourceProperty]
		public string PoliciesText
		{
			get
			{
				return this._policiesText;
			}
			set
			{
				if (value != this._policiesText)
				{
					this._policiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "PoliciesText");
				}
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060006FA RID: 1786 RVA: 0x0001EDFF File Offset: 0x0001CFFF
		// (set) Token: 0x060006FB RID: 1787 RVA: 0x0001EE07 File Offset: 0x0001D007
		[DataSourceProperty]
		public string ArmiesText
		{
			get
			{
				return this._armiesText;
			}
			set
			{
				if (value != this._armiesText)
				{
					this._armiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "ArmiesText");
				}
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0001EE2A File Offset: 0x0001D02A
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			this.Decision.SetDoneInputKey(hotkey);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0001EE45 File Offset: 0x0001D045
		public void SetPreviousTabInputKey(HotKey hotkey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0001EE54 File Offset: 0x0001D054
		public void SetNextTabInputKey(HotKey hotkey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x0001EE63 File Offset: 0x0001D063
		// (set) Token: 0x06000700 RID: 1792 RVA: 0x0001EE6B File Offset: 0x0001D06B
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x0001EE89 File Offset: 0x0001D089
		// (set) Token: 0x06000702 RID: 1794 RVA: 0x0001EE91 File Offset: 0x0001D091
		public InputKeyItemVM PreviousTabInputKey
		{
			get
			{
				return this._previousTabInputKey;
			}
			set
			{
				if (value != this._previousTabInputKey)
				{
					this._previousTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousTabInputKey");
				}
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x0001EEAF File Offset: 0x0001D0AF
		// (set) Token: 0x06000704 RID: 1796 RVA: 0x0001EEB7 File Offset: 0x0001D0B7
		public InputKeyItemVM NextTabInputKey
		{
			get
			{
				return this._nextTabInputKey;
			}
			set
			{
				if (value != this._nextTabInputKey)
				{
					this._nextTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextTabInputKey");
				}
			}
		}

		// Token: 0x040002F2 RID: 754
		private readonly Action _onClose;

		// Token: 0x040002F3 RID: 755
		private readonly Action<Army> _onShowArmyOnMap;

		// Token: 0x040002F4 RID: 756
		private readonly int _categoryCount;

		// Token: 0x040002F5 RID: 757
		private int _currentCategory;

		// Token: 0x040002F6 RID: 758
		private bool _isPlayerTheRuler;

		// Token: 0x040002F8 RID: 760
		private KingdomArmyVM _army;

		// Token: 0x040002F9 RID: 761
		private KingdomSettlementVM _settlement;

		// Token: 0x040002FA RID: 762
		private KingdomClanVM _clan;

		// Token: 0x040002FB RID: 763
		private KingdomPoliciesVM _policy;

		// Token: 0x040002FC RID: 764
		private KingdomDiplomacyVM _diplomacy;

		// Token: 0x040002FD RID: 765
		private KingdomGiftFiefPopupVM _giftFief;

		// Token: 0x040002FE RID: 766
		private ImageIdentifierVM _kingdomBanner;

		// Token: 0x040002FF RID: 767
		private HeroVM _leader;

		// Token: 0x04000300 RID: 768
		private KingdomDecisionsVM _decision;

		// Token: 0x04000301 RID: 769
		private HintViewModel _changeKingdomNameHint;

		// Token: 0x04000302 RID: 770
		private string _name;

		// Token: 0x04000303 RID: 771
		private bool _canSwitchTabs;

		// Token: 0x04000304 RID: 772
		private bool _playerHasKingdom;

		// Token: 0x04000305 RID: 773
		private bool _isKingdomActionEnabled;

		// Token: 0x04000306 RID: 774
		private bool _playerCanChangeKingdomName;

		// Token: 0x04000307 RID: 775
		private string _kingdomActionText;

		// Token: 0x04000308 RID: 776
		private string _leaderText;

		// Token: 0x04000309 RID: 777
		private string _clansText;

		// Token: 0x0400030A RID: 778
		private string _fiefsText;

		// Token: 0x0400030B RID: 779
		private string _policiesText;

		// Token: 0x0400030C RID: 780
		private string _armiesText;

		// Token: 0x0400030D RID: 781
		private string _diplomacyText;

		// Token: 0x0400030E RID: 782
		private string _doneText;

		// Token: 0x0400030F RID: 783
		private BasicTooltipViewModel _kingdomActionHint;

		// Token: 0x04000310 RID: 784
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000311 RID: 785
		private InputKeyItemVM _previousTabInputKey;

		// Token: 0x04000312 RID: 786
		private InputKeyItemVM _nextTabInputKey;
	}
}
