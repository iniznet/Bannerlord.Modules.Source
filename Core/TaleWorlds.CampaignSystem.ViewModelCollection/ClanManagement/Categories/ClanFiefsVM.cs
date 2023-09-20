using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	// Token: 0x02000112 RID: 274
	public class ClanFiefsVM : ViewModel
	{
		// Token: 0x060019ED RID: 6637 RVA: 0x0005DCC4 File Offset: 0x0005BEC4
		public ClanFiefsVM(Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
		{
			this._onRefresh = onRefresh;
			this._faction = Hero.MainHero.Clan;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this.Settlements = new MBBindingList<ClanSettlementItemVM>();
			this.Castles = new MBBindingList<ClanSettlementItemVM>();
			this.RefreshAllLists();
			this.RefreshValues();
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0005DD38 File Offset: 0x0005BF38
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TaxText = GameTexts.FindText("str_tax", null).ToString();
			this.GovernorText = GameTexts.FindText("str_notable_governor", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.TownsText = GameTexts.FindText("str_towns", null).ToString();
			this.CastlesText = GameTexts.FindText("str_castles", null).ToString();
			this.NoFiefsText = GameTexts.FindText("str_clan_no_fiefs", null).ToString();
			this.NoGovernorText = this._noGovernorTextSource.ToString();
			this.Settlements.ApplyActionOnAllItems(delegate(ClanSettlementItemVM x)
			{
				x.RefreshValues();
			});
			this.Castles.ApplyActionOnAllItems(delegate(ClanSettlementItemVM x)
			{
				x.RefreshValues();
			});
			ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
			if (currentSelectedFief == null)
			{
				return;
			}
			currentSelectedFief.RefreshValues();
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0005DE44 File Offset: 0x0005C044
		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0005DE4C File Offset: 0x0005C04C
		public void RefreshAllLists()
		{
			this.Settlements.Clear();
			this.Castles.Clear();
			foreach (Settlement settlement in this._faction.Settlements)
			{
				if (settlement.IsTown)
				{
					this.Settlements.Add(new ClanSettlementItemVM(settlement, new Action<ClanSettlementItemVM>(this.OnFiefSelection), new Action(this.OnShowSendMembers), this._teleportationBehavior));
				}
				else if (settlement.IsCastle)
				{
					this.Castles.Add(new ClanSettlementItemVM(settlement, new Action<ClanSettlementItemVM>(this.OnFiefSelection), new Action(this.OnShowSendMembers), this._teleportationBehavior));
				}
			}
			this.OnFiefSelection(this.GetDefaultMember());
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x0005DF34 File Offset: 0x0005C134
		private ClanSettlementItemVM GetDefaultMember()
		{
			if (!this.Settlements.IsEmpty<ClanSettlementItemVM>())
			{
				return this.Settlements.FirstOrDefault<ClanSettlementItemVM>();
			}
			return this.Castles.FirstOrDefault<ClanSettlementItemVM>();
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0005DF5C File Offset: 0x0005C15C
		public void SelectFief(Settlement settlement)
		{
			foreach (ClanSettlementItemVM clanSettlementItemVM in this.Settlements)
			{
				if (clanSettlementItemVM.Settlement == settlement)
				{
					this.OnFiefSelection(clanSettlementItemVM);
					break;
				}
			}
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
		private void OnFiefSelection(ClanSettlementItemVM fief)
		{
			if (this.CurrentSelectedFief != null)
			{
				this.CurrentSelectedFief.IsSelected = false;
			}
			this.CurrentSelectedFief = fief;
			TextObject textObject;
			this.CanChangeGovernorOfCurrentFief = this.GetCanChangeGovernor(out textObject);
			this.GovernorActionHint = new HintViewModel(textObject, null);
			if (fief != null)
			{
				fief.IsSelected = true;
				this.GovernorActionText = (fief.HasGovernor ? GameTexts.FindText("str_clan_change_governor", null).ToString() : GameTexts.FindText("str_clan_assign_governor", null).ToString());
			}
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0005E034 File Offset: 0x0005C234
		private bool GetCanChangeGovernor(out TextObject disabledReason)
		{
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
			bool flag;
			if (currentSelectedFief == null)
			{
				flag = false;
			}
			else
			{
				HeroVM governor = currentSelectedFief.Governor;
				bool? flag2;
				if (governor == null)
				{
					flag2 = null;
				}
				else
				{
					Hero hero = governor.Hero;
					flag2 = ((hero != null) ? new bool?(hero.IsTraveling) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag)
			{
				disabledReason = new TextObject("{=qbqimqMb}{GOVERNOR.NAME} is on the way to be the new governor of {SETTLEMENT_NAME}", null);
				if (this.CurrentSelectedFief.Governor.Hero.CharacterObject != null)
				{
					StringHelpers.SetCharacterProperties("GOVERNOR", this.CurrentSelectedFief.Governor.Hero.CharacterObject, disabledReason, false);
				}
				TextObject textObject2 = disabledReason;
				string text = "SETTLEMENT_NAME";
				Settlement settlement = this.CurrentSelectedFief.Settlement;
				string text2;
				if (settlement == null)
				{
					text2 = null;
				}
				else
				{
					TextObject name = settlement.Name;
					text2 = ((name != null) ? name.ToString() : null);
				}
				textObject2.SetTextVariable(text, text2 ?? string.Empty);
				return false;
			}
			ClanSettlementItemVM currentSelectedFief2 = this.CurrentSelectedFief;
			if (((currentSelectedFief2 != null) ? currentSelectedFief2.Settlement.Town : null) == null)
			{
				disabledReason = TextObject.Empty;
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x0005E154 File Offset: 0x0005C354
		public void ExecuteAssignGovernor()
		{
			ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
			bool flag;
			if (currentSelectedFief == null)
			{
				flag = null != null;
			}
			else
			{
				Settlement settlement = currentSelectedFief.Settlement;
				flag = ((settlement != null) ? settlement.Town : null) != null;
			}
			if (flag)
			{
				ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(GameTexts.FindText("str_clan_assign_governor", null).CopyTextObject(), this.GetGovernorCandidates(), new Action<List<object>, Action>(this.OnGovernorSelectionOver), false);
				Action<ClanCardSelectionInfo> openCardSelectionPopup = this._openCardSelectionPopup;
				if (openCardSelectionPopup == null)
				{
					return;
				}
				openCardSelectionPopup(clanCardSelectionInfo);
			}
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x0005E1BC File Offset: 0x0005C3BC
		private IEnumerable<ClanCardSelectionItemInfo> GetGovernorCandidates()
		{
			yield return new ClanCardSelectionItemInfo(this._noGovernorTextSource.CopyTextObject(), false, null, null);
			foreach (Hero hero in this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions))
			{
				if ((hero.IsActive || hero.IsTraveling) && !hero.IsChild && hero != Hero.MainHero)
				{
					Hero hero2 = hero;
					HeroVM governor = this.CurrentSelectedFief.Governor;
					if (hero2 != ((governor != null) ? governor.Hero : null))
					{
						TextObject textObject;
						bool flag = FactionHelper.IsMainClanMemberAvailableForSendingSettlementAsGovernor(hero, this.GetSettlementOfGovernor(hero), out textObject);
						SkillObject charm = DefaultSkills.Charm;
						int skillValue = hero.GetSkillValue(charm);
						ImageIdentifier imageIdentifier = new ImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
						yield return new ClanCardSelectionItemInfo(hero, hero.Name, imageIdentifier, CardSelectionItemSpriteType.Skill, charm.StringId.ToLower(), skillValue.ToString(), this.GetGovernorCandidateProperties(hero), !flag, textObject, null);
					}
				}
			}
			IEnumerator<Hero> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0005E1CC File Offset: 0x0005C3CC
		private IEnumerable<ClanCardSelectionItemPropertyInfo> GetGovernorCandidateProperties(Hero hero)
		{
			GameTexts.SetVariable("newline", "\n");
			TextObject teleportationDelayText = CampaignUIHelper.GetTeleportationDelayText(hero, this.CurrentSelectedFief.Settlement.Party);
			yield return new ClanCardSelectionItemPropertyInfo(teleportationDelayText);
			ValueTuple<TextObject, TextObject> governorEngineeringSkillEffectForHero = PerkHelper.GetGovernorEngineeringSkillEffectForHero(hero);
			yield return new ClanCardSelectionItemPropertyInfo(new TextObject("{=J8ddrAOf}Governor Effects", null), governorEngineeringSkillEffectForHero.Item2);
			List<PerkObject> governorPerksForHero = PerkHelper.GetGovernorPerksForHero(hero);
			TextObject textObject = new TextObject("{=oSfsqBwJ}No perks", null);
			int num = 0;
			foreach (PerkObject perkObject in governorPerksForHero)
			{
				TextObject textObject2 = ((perkObject.PrimaryRole == SkillEffect.PerkRole.Governor) ? perkObject.PrimaryDescription : perkObject.SecondaryDescription);
				TextObject textObject3 = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(perkObject.Name, textObject2);
				if (num == 0)
				{
					textObject = textObject3;
				}
				else
				{
					TextObject textObject4 = GameTexts.FindText("str_string_newline_newline_string", null);
					textObject4.SetTextVariable("STR1", textObject);
					textObject4.SetTextVariable("STR2", textObject3);
					textObject = textObject4;
				}
				num++;
			}
			yield return new ClanCardSelectionItemPropertyInfo(GameTexts.FindText("str_clan_governor_perks", null), textObject);
			yield break;
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x0005E1E4 File Offset: 0x0005C3E4
		private void OnGovernorSelectionOver(List<object> selectedItems, Action closePopup)
		{
			if (selectedItems.Count == 1)
			{
				ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
				Hero hero;
				if (currentSelectedFief == null)
				{
					hero = null;
				}
				else
				{
					HeroVM governor = currentSelectedFief.Governor;
					hero = ((governor != null) ? governor.Hero : null);
				}
				Hero hero2 = hero;
				Hero newGovernor = selectedItems.FirstOrDefault<object>() as Hero;
				if (newGovernor != null || hero2 != null)
				{
					ValueTuple<TextObject, TextObject> governorSelectionConfirmationPopupTexts = CampaignUIHelper.GetGovernorSelectionConfirmationPopupTexts(hero2, newGovernor, this.CurrentSelectedFief.Settlement);
					InformationManager.ShowInquiry(new InquiryData(governorSelectionConfirmationPopupTexts.Item1.ToString(), governorSelectionConfirmationPopupTexts.Item2.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						Action closePopup4 = closePopup;
						if (closePopup4 != null)
						{
							closePopup4();
						}
						ChangeGovernorAction.Apply(this.CurrentSelectedFief.Settlement.Town, newGovernor);
						Hero newGovernor2 = newGovernor;
						if (newGovernor2 != null)
						{
							MobileParty partyBelongedTo = newGovernor2.PartyBelongedTo;
							if (partyBelongedTo != null)
							{
								partyBelongedTo.RemoveHeroPerkRole(newGovernor);
							}
						}
						Action onRefresh = this._onRefresh;
						if (onRefresh == null)
						{
							return;
						}
						onRefresh();
					}, null, "", 0f, null, null, null), false, false);
					return;
				}
				Action closePopup2 = closePopup;
				if (closePopup2 == null)
				{
					return;
				}
				closePopup2();
				return;
			}
			else
			{
				Action closePopup3 = closePopup;
				if (closePopup3 == null)
				{
					return;
				}
				closePopup3();
				return;
			}
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x0005E2FC File Offset: 0x0005C4FC
		private Settlement GetSettlementOfGovernor(Hero hero)
		{
			foreach (ClanSettlementItemVM clanSettlementItemVM in this.Settlements)
			{
				Hero hero2;
				if (clanSettlementItemVM == null)
				{
					hero2 = null;
				}
				else
				{
					HeroVM governor = clanSettlementItemVM.Governor;
					hero2 = ((governor != null) ? governor.Hero : null);
				}
				if (hero2 == hero)
				{
					return clanSettlementItemVM.Settlement;
				}
			}
			foreach (ClanSettlementItemVM clanSettlementItemVM2 in this.Castles)
			{
				Hero hero3;
				if (clanSettlementItemVM2 == null)
				{
					hero3 = null;
				}
				else
				{
					HeroVM governor2 = clanSettlementItemVM2.Governor;
					hero3 = ((governor2 != null) ? governor2.Hero : null);
				}
				if (hero3 == hero)
				{
					return clanSettlementItemVM2.Settlement;
				}
			}
			return null;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x0005E3C4 File Offset: 0x0005C5C4
		private void OnShowSendMembers()
		{
			ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
			Settlement settlement = ((currentSelectedFief != null) ? currentSelectedFief.Settlement : null);
			if (settlement != null)
			{
				TextObject textObject = GameTexts.FindText("str_send_members", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", settlement.Name);
				ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(textObject, this.GetSendMembersCandidates(), new Action<List<object>, Action>(this.OnSendMembersSelectionOver), true);
				Action<ClanCardSelectionInfo> openCardSelectionPopup = this._openCardSelectionPopup;
				if (openCardSelectionPopup == null)
				{
					return;
				}
				openCardSelectionPopup(clanCardSelectionInfo);
			}
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x0005E431 File Offset: 0x0005C631
		private IEnumerable<ClanCardSelectionItemInfo> GetSendMembersCandidates()
		{
			foreach (Hero hero in this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions))
			{
				if ((hero.IsActive || hero.IsTraveling) && (hero.CurrentSettlement != this.CurrentSelectedFief.Settlement || hero.PartyBelongedTo != null) && !hero.IsChild && hero != Hero.MainHero)
				{
					TextObject textObject;
					bool flag = FactionHelper.IsMainClanMemberAvailableForSendingSettlement(hero, this.CurrentSelectedFief.Settlement, out textObject);
					SkillObject charm = DefaultSkills.Charm;
					int skillValue = hero.GetSkillValue(charm);
					ImageIdentifier imageIdentifier = new ImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
					yield return new ClanCardSelectionItemInfo(hero, hero.Name, imageIdentifier, CardSelectionItemSpriteType.Skill, charm.StringId.ToLower(), skillValue.ToString(), this.GetSendMembersCandidateProperties(hero), !flag, textObject, null);
				}
			}
			IEnumerator<Hero> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x0005E441 File Offset: 0x0005C641
		private IEnumerable<ClanCardSelectionItemPropertyInfo> GetSendMembersCandidateProperties(Hero hero)
		{
			TextObject teleportationDelayText = CampaignUIHelper.GetTeleportationDelayText(hero, this.CurrentSelectedFief.Settlement.Party);
			yield return new ClanCardSelectionItemPropertyInfo(teleportationDelayText);
			TextObject textObject = new TextObject("{=otaUtXMX}+{AMOUNT} relation chance with notables per day.", null);
			int emissaryRelationBonusForMainClan = Campaign.Current.Models.EmissaryModel.EmissaryRelationBonusForMainClan;
			textObject.SetTextVariable("AMOUNT", emissaryRelationBonusForMainClan);
			yield return new ClanCardSelectionItemPropertyInfo(textObject);
			yield break;
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x0005E458 File Offset: 0x0005C658
		private void OnSendMembersSelectionOver(List<object> selectedItems, Action closePopup)
		{
			if (selectedItems.Count > 0)
			{
				string text = "SETTLEMENT_NAME";
				ClanSettlementItemVM currentSelectedFief = this.CurrentSelectedFief;
				string text2;
				if (currentSelectedFief == null)
				{
					text2 = null;
				}
				else
				{
					Settlement settlement = currentSelectedFief.Settlement;
					if (settlement == null)
					{
						text2 = null;
					}
					else
					{
						TextObject name = settlement.Name;
						text2 = ((name != null) ? name.ToString() : null);
					}
				}
				MBTextManager.SetTextVariable(text, text2 ?? string.Empty, false);
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_send_members", null).ToString(), GameTexts.FindText("str_send_members_inquiry", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					Action closePopup3 = closePopup;
					if (closePopup3 != null)
					{
						closePopup3();
					}
					using (List<object>.Enumerator enumerator = selectedItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Hero hero;
							if ((hero = enumerator.Current as Hero) != null)
							{
								TeleportHeroAction.ApplyDelayedTeleportToSettlement(hero, this.CurrentSelectedFief.Settlement);
							}
						}
					}
					Action onRefresh = this._onRefresh;
					if (onRefresh == null)
					{
						return;
					}
					onRefresh();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			Action closePopup2 = closePopup;
			if (closePopup2 == null)
			{
				return;
			}
			closePopup2();
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x060019FE RID: 6654 RVA: 0x0005E548 File Offset: 0x0005C748
		// (set) Token: 0x060019FF RID: 6655 RVA: 0x0005E550 File Offset: 0x0005C750
		[DataSourceProperty]
		public string GovernorActionText
		{
			get
			{
				return this._governorActionText;
			}
			set
			{
				if (value != this._governorActionText)
				{
					this._governorActionText = value;
					base.OnPropertyChangedWithValue<string>(value, "GovernorActionText");
				}
			}
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06001A00 RID: 6656 RVA: 0x0005E573 File Offset: 0x0005C773
		// (set) Token: 0x06001A01 RID: 6657 RVA: 0x0005E57B File Offset: 0x0005C77B
		[DataSourceProperty]
		public bool CanChangeGovernorOfCurrentFief
		{
			get
			{
				return this._canChangeGovernorOfCurrentFief;
			}
			set
			{
				if (value != this._canChangeGovernorOfCurrentFief)
				{
					this._canChangeGovernorOfCurrentFief = value;
					base.OnPropertyChangedWithValue(value, "CanChangeGovernorOfCurrentFief");
				}
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06001A02 RID: 6658 RVA: 0x0005E599 File Offset: 0x0005C799
		// (set) Token: 0x06001A03 RID: 6659 RVA: 0x0005E5A1 File Offset: 0x0005C7A1
		[DataSourceProperty]
		public HintViewModel GovernorActionHint
		{
			get
			{
				return this._governorActionHint;
			}
			set
			{
				if (value != this._governorActionHint)
				{
					this._governorActionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "GovernorActionHint");
				}
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06001A04 RID: 6660 RVA: 0x0005E5BF File Offset: 0x0005C7BF
		// (set) Token: 0x06001A05 RID: 6661 RVA: 0x0005E5C7 File Offset: 0x0005C7C7
		[DataSourceProperty]
		public bool IsAnyValidFiefSelected
		{
			get
			{
				return this._isAnyValidFiefSelected;
			}
			set
			{
				if (value != this._isAnyValidFiefSelected)
				{
					this._isAnyValidFiefSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidFiefSelected");
				}
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06001A06 RID: 6662 RVA: 0x0005E5E5 File Offset: 0x0005C7E5
		// (set) Token: 0x06001A07 RID: 6663 RVA: 0x0005E5ED File Offset: 0x0005C7ED
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

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06001A08 RID: 6664 RVA: 0x0005E610 File Offset: 0x0005C810
		// (set) Token: 0x06001A09 RID: 6665 RVA: 0x0005E618 File Offset: 0x0005C818
		[DataSourceProperty]
		public string TaxText
		{
			get
			{
				return this._taxText;
			}
			set
			{
				if (value != this._taxText)
				{
					this._taxText = value;
					base.OnPropertyChangedWithValue<string>(value, "TaxText");
				}
			}
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06001A0A RID: 6666 RVA: 0x0005E63B File Offset: 0x0005C83B
		// (set) Token: 0x06001A0B RID: 6667 RVA: 0x0005E643 File Offset: 0x0005C843
		[DataSourceProperty]
		public string GovernorText
		{
			get
			{
				return this._governorText;
			}
			set
			{
				if (value != this._governorText)
				{
					this._governorText = value;
					base.OnPropertyChangedWithValue<string>(value, "GovernorText");
				}
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06001A0C RID: 6668 RVA: 0x0005E666 File Offset: 0x0005C866
		// (set) Token: 0x06001A0D RID: 6669 RVA: 0x0005E66E File Offset: 0x0005C86E
		[DataSourceProperty]
		public string TownsText
		{
			get
			{
				return this._townsText;
			}
			set
			{
				if (value != this._townsText)
				{
					this._townsText = value;
					base.OnPropertyChangedWithValue<string>(value, "TownsText");
				}
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06001A0E RID: 6670 RVA: 0x0005E691 File Offset: 0x0005C891
		// (set) Token: 0x06001A0F RID: 6671 RVA: 0x0005E699 File Offset: 0x0005C899
		[DataSourceProperty]
		public string CastlesText
		{
			get
			{
				return this._castlesText;
			}
			set
			{
				if (value != this._castlesText)
				{
					this._castlesText = value;
					base.OnPropertyChangedWithValue<string>(value, "CastlesText");
				}
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06001A10 RID: 6672 RVA: 0x0005E6BC File Offset: 0x0005C8BC
		// (set) Token: 0x06001A11 RID: 6673 RVA: 0x0005E6C4 File Offset: 0x0005C8C4
		[DataSourceProperty]
		public string NoFiefsText
		{
			get
			{
				return this._noFiefsText;
			}
			set
			{
				if (value != this._noFiefsText)
				{
					this._noFiefsText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoFiefsText");
				}
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x0005E6E7 File Offset: 0x0005C8E7
		// (set) Token: 0x06001A13 RID: 6675 RVA: 0x0005E6EF File Offset: 0x0005C8EF
		[DataSourceProperty]
		public string NoGovernorText
		{
			get
			{
				return this._noGovernorText;
			}
			set
			{
				if (value != this._noGovernorText)
				{
					this._noGovernorText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoGovernorText");
				}
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06001A14 RID: 6676 RVA: 0x0005E712 File Offset: 0x0005C912
		// (set) Token: 0x06001A15 RID: 6677 RVA: 0x0005E71A File Offset: 0x0005C91A
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06001A16 RID: 6678 RVA: 0x0005E738 File Offset: 0x0005C938
		// (set) Token: 0x06001A17 RID: 6679 RVA: 0x0005E740 File Offset: 0x0005C940
		[DataSourceProperty]
		public MBBindingList<ClanSettlementItemVM> Settlements
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
					base.OnPropertyChangedWithValue<MBBindingList<ClanSettlementItemVM>>(value, "Settlements");
				}
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06001A18 RID: 6680 RVA: 0x0005E75E File Offset: 0x0005C95E
		// (set) Token: 0x06001A19 RID: 6681 RVA: 0x0005E766 File Offset: 0x0005C966
		[DataSourceProperty]
		public MBBindingList<ClanSettlementItemVM> Castles
		{
			get
			{
				return this._castles;
			}
			set
			{
				if (value != this._castles)
				{
					this._castles = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanSettlementItemVM>>(value, "Castles");
				}
			}
		}

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06001A1A RID: 6682 RVA: 0x0005E784 File Offset: 0x0005C984
		// (set) Token: 0x06001A1B RID: 6683 RVA: 0x0005E78C File Offset: 0x0005C98C
		[DataSourceProperty]
		public ClanSettlementItemVM CurrentSelectedFief
		{
			get
			{
				return this._currentSelectedFief;
			}
			set
			{
				if (value != this._currentSelectedFief)
				{
					this._currentSelectedFief = value;
					base.OnPropertyChangedWithValue<ClanSettlementItemVM>(value, "CurrentSelectedFief");
					this.IsAnyValidFiefSelected = value != null;
				}
			}
		}

		// Token: 0x04000C4B RID: 3147
		private readonly Clan _faction;

		// Token: 0x04000C4C RID: 3148
		private readonly Action _onRefresh;

		// Token: 0x04000C4D RID: 3149
		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		// Token: 0x04000C4E RID: 3150
		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x04000C4F RID: 3151
		private readonly TextObject _noGovernorTextSource = new TextObject("{=zLFsnaqR}No Governor", null);

		// Token: 0x04000C50 RID: 3152
		private MBBindingList<ClanSettlementItemVM> _settlements;

		// Token: 0x04000C51 RID: 3153
		private MBBindingList<ClanSettlementItemVM> _castles;

		// Token: 0x04000C52 RID: 3154
		private ClanSettlementItemVM _currentSelectedFief;

		// Token: 0x04000C53 RID: 3155
		private bool _isSelected;

		// Token: 0x04000C54 RID: 3156
		private string _nameText;

		// Token: 0x04000C55 RID: 3157
		private string _taxText;

		// Token: 0x04000C56 RID: 3158
		private string _governorText;

		// Token: 0x04000C57 RID: 3159
		private string _townsText;

		// Token: 0x04000C58 RID: 3160
		private string _castlesText;

		// Token: 0x04000C59 RID: 3161
		private string _noFiefsText;

		// Token: 0x04000C5A RID: 3162
		private string _noGovernorText;

		// Token: 0x04000C5B RID: 3163
		private bool _isAnyValidFiefSelected;

		// Token: 0x04000C5C RID: 3164
		private bool _canChangeGovernorOfCurrentFief;

		// Token: 0x04000C5D RID: 3165
		private HintViewModel _governorActionHint;

		// Token: 0x04000C5E RID: 3166
		private string _governorActionText;
	}
}
