using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanFiefsVM : ViewModel
	{
		public ClanFiefsVM(Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
		{
			this._onRefresh = onRefresh;
			this._clan = Hero.MainHero.Clan;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this.Settlements = new MBBindingList<ClanSettlementItemVM>();
			this.Castles = new MBBindingList<ClanSettlementItemVM>();
			List<MBBindingList<ClanSettlementItemVM>> list = new List<MBBindingList<ClanSettlementItemVM>> { this.Settlements, this.Castles };
			this.SortController = new ClanFiefsSortControllerVM(list);
			this.RefreshAllLists();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TaxText = GameTexts.FindText("str_tax", null).ToString();
			this.GovernorText = GameTexts.FindText("str_notable_governor", null).ToString();
			this.ProfitText = GameTexts.FindText("str_profit", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
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
			if (currentSelectedFief != null)
			{
				currentSelectedFief.RefreshValues();
			}
			this.SortController.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		public void RefreshAllLists()
		{
			this.Settlements.Clear();
			this.Castles.Clear();
			this.SortController.ResetAllStates();
			foreach (Settlement settlement in this._clan.Settlements)
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
			GameTexts.SetVariable("RANK", GameTexts.FindText("str_towns", null));
			GameTexts.SetVariable("NUMBER", this.Settlements.Count);
			this.TownsText = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
			GameTexts.SetVariable("RANK", GameTexts.FindText("str_castles", null));
			GameTexts.SetVariable("NUMBER", this.Castles.Count);
			this.CastlesText = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
			this.OnFiefSelection(this.GetDefaultMember());
		}

		private ClanSettlementItemVM GetDefaultMember()
		{
			if (!this.Settlements.IsEmpty<ClanSettlementItemVM>())
			{
				return this.Settlements.FirstOrDefault<ClanSettlementItemVM>();
			}
			return this.Castles.FirstOrDefault<ClanSettlementItemVM>();
		}

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

		private IEnumerable<ClanCardSelectionItemInfo> GetGovernorCandidates()
		{
			yield return new ClanCardSelectionItemInfo(this._noGovernorTextSource.CopyTextObject(), false, null, null);
			foreach (Hero hero in this._clan.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._clan.Companions))
			{
				if ((hero.IsActive || hero.IsTraveling) && !hero.IsChild && hero != Hero.MainHero)
				{
					Hero hero2 = hero;
					HeroVM governor = this.CurrentSelectedFief.Governor;
					if (hero2 != ((governor != null) ? governor.Hero : null) && hero.CanBeGovernorOrHavePartyRole())
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
				bool flag = perkObject.PrimaryRole == SkillEffect.PerkRole.Governor;
				bool flag2 = perkObject.SecondaryRole == SkillEffect.PerkRole.Governor;
				if (flag)
				{
					TextObject textObject2 = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(perkObject.Name, perkObject.PrimaryDescription);
					this.SetPerksPropertyText(textObject2, ref textObject, ref num);
				}
				if (flag2)
				{
					TextObject textObject3 = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(perkObject.Name, perkObject.SecondaryDescription);
					this.SetPerksPropertyText(textObject3, ref textObject, ref num);
				}
			}
			yield return new ClanCardSelectionItemPropertyInfo(GameTexts.FindText("str_clan_governor_perks", null), textObject);
			yield break;
		}

		private void SetPerksPropertyText(TextObject perkText, ref TextObject perksPropertyText, ref int addedPerkCount)
		{
			if (addedPerkCount == 0)
			{
				perksPropertyText = perkText;
			}
			else
			{
				TextObject textObject = GameTexts.FindText("str_string_newline_newline_string", null);
				textObject.SetTextVariable("STR1", perksPropertyText);
				textObject.SetTextVariable("STR2", perkText);
				perksPropertyText = textObject;
			}
			addedPerkCount++;
		}

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
				bool isRemoveGovernor = newGovernor == null;
				if (!isRemoveGovernor || hero2 != null)
				{
					ValueTuple<TextObject, TextObject> governorSelectionConfirmationPopupTexts = CampaignUIHelper.GetGovernorSelectionConfirmationPopupTexts(hero2, newGovernor, this.CurrentSelectedFief.Settlement);
					InformationManager.ShowInquiry(new InquiryData(governorSelectionConfirmationPopupTexts.Item1.ToString(), governorSelectionConfirmationPopupTexts.Item2.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						Action closePopup4 = closePopup;
						if (closePopup4 != null)
						{
							closePopup4();
						}
						if (isRemoveGovernor)
						{
							ChangeGovernorAction.RemoveGovernorOfIfExists(this.CurrentSelectedFief.Settlement.Town);
						}
						else
						{
							ChangeGovernorAction.Apply(this.CurrentSelectedFief.Settlement.Town, newGovernor);
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

		private IEnumerable<ClanCardSelectionItemInfo> GetSendMembersCandidates()
		{
			foreach (Hero hero in this._clan.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._clan.Companions))
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

		[DataSourceProperty]
		public string ProfitText
		{
			get
			{
				return this._profitText;
			}
			set
			{
				if (value != this._profitText)
				{
					this._profitText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProfitText");
				}
			}
		}

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

		[DataSourceProperty]
		public ClanFiefsSortControllerVM SortController
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
					base.OnPropertyChangedWithValue<ClanFiefsSortControllerVM>(value, "SortController");
				}
			}
		}

		private readonly Clan _clan;

		private readonly Action _onRefresh;

		private readonly Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		private readonly TextObject _noGovernorTextSource = new TextObject("{=zLFsnaqR}No Governor", null);

		private MBBindingList<ClanSettlementItemVM> _settlements;

		private MBBindingList<ClanSettlementItemVM> _castles;

		private ClanSettlementItemVM _currentSelectedFief;

		private bool _isSelected;

		private string _nameText;

		private string _taxText;

		private string _governorText;

		private string _profitText;

		private string _townsText;

		private string _castlesText;

		private string _noFiefsText;

		private string _noGovernorText;

		private bool _isAnyValidFiefSelected;

		private bool _canChangeGovernorOfCurrentFief;

		private HintViewModel _governorActionHint;

		private string _governorActionText;

		private ClanFiefsSortControllerVM _sortController;
	}
}
