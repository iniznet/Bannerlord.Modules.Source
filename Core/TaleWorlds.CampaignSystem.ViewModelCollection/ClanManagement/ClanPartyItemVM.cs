using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanPartyItemVM : ViewModel
	{
		public int Expense { get; private set; }

		public int Income { get; private set; }

		public PartyBase Party { get; }

		public ClanPartyItemVM(PartyBase party, Action<ClanPartyItemVM> onAssignment, Action onExpenseChange, Action onShowChangeLeaderPopup, ClanPartyItemVM.ClanPartyType type, IDisbandPartyCampaignBehavior disbandBehavior, ITeleportationCampaignBehavior teleportationBehavior)
		{
			this.Party = party;
			this._type = type;
			this._disbandBehavior = disbandBehavior;
			this._leader = CampaignUIHelper.GetVisualPartyLeader(this.Party);
			this.HasHeroMembers = party.IsMobile;
			if (this._leader == null)
			{
				TroopRosterElement troopRosterElement = this.Party.MemberRoster.GetTroopRoster().FirstOrDefault<TroopRosterElement>();
				if (!troopRosterElement.Equals(default(TroopRosterElement)))
				{
					this._leader = troopRosterElement.Character;
				}
				else
				{
					IFaction mapFaction = this.Party.MapFaction;
					this._leader = ((mapFaction != null) ? mapFaction.BasicTroop : null);
				}
			}
			CharacterObject leader = this._leader;
			if ((leader == null || !leader.IsHero) && party.IsMobile && (this._type == ClanPartyItemVM.ClanPartyType.Member || this._type == ClanPartyItemVM.ClanPartyType.Caravan))
			{
				Hero teleportingLeaderHero = CampaignUIHelper.GetTeleportingLeaderHero(party.MobileParty, teleportationBehavior);
				this._leader = ((teleportingLeaderHero != null) ? teleportingLeaderHero.CharacterObject : null);
				this._isLeaderTeleporting = this._leader != null;
			}
			if (this._leader != null)
			{
				CharacterCode characterCode = ClanPartyItemVM.GetCharacterCode(this._leader);
				this.LeaderVisual = new ImageIdentifierVM(characterCode);
				this.CharacterModel = new CharacterViewModel(CharacterViewModel.StanceTypes.None);
				this.CharacterModel.FillFrom(this._leader, -1);
				CharacterViewModel characterModel = this.CharacterModel;
				IFaction mapFaction2 = this.Party.MapFaction;
				characterModel.ArmorColor1 = ((mapFaction2 != null) ? mapFaction2.Color : 0U);
				CharacterViewModel characterModel2 = this.CharacterModel;
				IFaction mapFaction3 = this.Party.MapFaction;
				characterModel2.ArmorColor2 = ((mapFaction3 != null) ? mapFaction3.Color2 : 0U);
			}
			else
			{
				this.LeaderVisual = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.CharacterModel = new CharacterViewModel();
			}
			this._onAssignment = onAssignment;
			this._onExpenseChange = onExpenseChange;
			this._onShowChangeLeaderPopup = onShowChangeLeaderPopup;
			bool flag;
			if (!this.Party.MobileParty.IsDisbanding)
			{
				IDisbandPartyCampaignBehavior disbandBehavior2 = this._disbandBehavior;
				flag = disbandBehavior2 != null && disbandBehavior2.IsPartyWaitingForDisband(party.MobileParty);
			}
			else
			{
				flag = true;
			}
			this.IsDisbanding = flag;
			bool flag2 = !party.MobileParty.IsMilitia && !party.MobileParty.IsVillager && party.MobileParty.IsActive && !this.IsDisbanding;
			this.ShouldPartyHaveExpense = flag2 && (type == ClanPartyItemVM.ClanPartyType.Garrison || type == ClanPartyItemVM.ClanPartyType.Member);
			this.IsCaravan = type == ClanPartyItemVM.ClanPartyType.Caravan;
			TextObject empty = TextObject.Empty;
			this.IsChangeLeaderVisible = type == ClanPartyItemVM.ClanPartyType.Caravan || type == ClanPartyItemVM.ClanPartyType.Member;
			this.IsChangeLeaderEnabled = this.IsChangeLeaderVisible && CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out empty);
			this.ChangeLeaderHint = new HintViewModel(this.IsChangeLeaderEnabled ? this._changeLeaderHintText : empty, null);
			if (this.ShouldPartyHaveExpense)
			{
				if (party.MobileParty != null)
				{
					this.ExpenseItem = new ClanFinanceExpenseItemVM(party.MobileParty);
					this.OnExpenseChange();
				}
				else
				{
					Debug.FailedAssert("This party should have expense info but it doesn't", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\ClanManagement\\ClanPartyItemVM.cs", ".ctor", 114);
				}
			}
			if (this.IsCaravan)
			{
				this.Income = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromCaravan(party.MobileParty);
			}
			this.AutoRecruitmentHint = new HintViewModel(GameTexts.FindText("str_clan_auto_recruitment_hint", null), null);
			this.IsAutoRecruitmentVisible = party.MobileParty.IsGarrison;
			this.AutoRecruitmentValue = party.MobileParty.IsGarrison && this.Party.MobileParty.CurrentSettlement.Town.GarrisonAutoRecruitmentIsEnabled;
			this.HeroMembers = new MBBindingList<ClanPartyMemberItemVM>();
			this.Roles = new MBBindingList<ClanRoleItemVM>();
			this.InfantryHint = new BasicTooltipViewModel(() => this.GetPartyTroopInfo(this.Party, FormationClass.Infantry));
			this.CavalryHint = new BasicTooltipViewModel(() => this.GetPartyTroopInfo(this.Party, FormationClass.Cavalry));
			this.RangedHint = new BasicTooltipViewModel(() => this.GetPartyTroopInfo(this.Party, FormationClass.Ranged));
			this.HorseArcherHint = new BasicTooltipViewModel(() => this.GetPartyTroopInfo(this.Party, FormationClass.HorseArcher));
			this.ActionsDisabledHint = new HintViewModel();
			this.InArmyHint = new HintViewModel();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateProperties();
		}

		public void UpdateProperties()
		{
			this.MembersText = GameTexts.FindText("str_members", null).ToString();
			this.AssigneesText = GameTexts.FindText("str_clan_assignee_title", null).ToString();
			this.RolesText = GameTexts.FindText("str_clan_role_title", null).ToString();
			this.PartyLeaderRoleEffectsText = GameTexts.FindText("str_clan_party_leader_roles_and_effects", null).ToString();
			this.AutoRecruitmentText = GameTexts.FindText("str_clan_auto_recruitment", null).ToString();
			PartyBase party = this.Party;
			this.IsPartyBehaviorEnabled = ((party != null) ? party.LeaderHero : null) != null && this.Party.LeaderHero.Clan.Leader != this.Party.LeaderHero && !this.Party.MobileParty.IsCaravan && !this.IsDisbanding;
			if (this.Party == PartyBase.MainParty && Hero.MainHero.IsPrisoner)
			{
				TextObject textObject = new TextObject("{=shL0WElC}{TROOP.NAME}{.o} Party", null);
				textObject.SetCharacterProperties("TROOP", Hero.MainHero.CharacterObject, false);
				this.Name = textObject.ToString();
			}
			else if (this._isLeaderTeleporting)
			{
				TextObject textObject2 = new TextObject("{=P5YtNXHR}{LEADER.NAME}{.o} Party", null);
				StringHelpers.SetCharacterProperties("LEADER", this._leader, textObject2, false);
				this.Name = textObject2.ToString();
			}
			else
			{
				this.Name = this.Party.Name.ToString();
			}
			this.IsMainHeroParty = this._type == ClanPartyItemVM.ClanPartyType.Main;
			if (this._type == ClanPartyItemVM.ClanPartyType.Garrison)
			{
				this.PartyLocationText = this.Party.MobileParty.CurrentSettlement.Name.ToString();
			}
			else
			{
				this.PartyLocationText = ((this.Party.LeaderHero != null && !this.IsMainHeroParty && this.Party.LeaderHero.LastKnownClosestSettlement != null) ? this.Party.LeaderHero.LastKnownClosestSettlement.Name.ToString() : " ");
			}
			GameTexts.SetVariable("LEFT", this.Party.MobileParty.MemberRoster.TotalManCount);
			GameTexts.SetVariable("RIGHT", this.Party.MobileParty.Party.PartySizeLimit);
			string text = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			string text2 = GameTexts.FindText("str_party_morale_party_size", null).ToString();
			this.PartySizeText = text;
			GameTexts.SetVariable("LEFT", text2);
			GameTexts.SetVariable("RIGHT", text);
			this.PartySizeSubTitleText = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_party_wage", null));
			GameTexts.SetVariable("RIGHT", this.Party.MobileParty.TotalWage);
			this.PartyWageSubTitleText = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
			this.InArmyText = "";
			if (this.Party.MobileParty.Army != null)
			{
				this.IsInArmy = true;
				TextObject textObject3 = GameTexts.FindText("str_clan_in_army_hint", null);
				TextObject textObject4 = textObject3;
				string text3 = "ARMY_LEADER";
				MobileParty leaderParty = this.Party.MobileParty.Army.LeaderParty;
				string text4;
				if (leaderParty == null)
				{
					text4 = null;
				}
				else
				{
					Hero leaderHero = leaderParty.LeaderHero;
					text4 = ((leaderHero != null) ? leaderHero.Name.ToString() : null);
				}
				textObject4.SetTextVariable(text3, text4 ?? string.Empty);
				this.InArmyHint = new HintViewModel(textObject3, null);
				this.InArmyText = GameTexts.FindText("str_in_army", null).ToString();
			}
			this.DisbandingText = "";
			this.IsMembersAndRolesVisible = !this.IsDisbanding && this._type != ClanPartyItemVM.ClanPartyType.Garrison;
			if (this.IsDisbanding)
			{
				this.DisbandingText = GameTexts.FindText("str_disbanding", null).ToString();
			}
			this.PartyBehaviorText = "";
			if (this.IsPartyBehaviorEnabled)
			{
				this.PartyBehaviorSelector = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.UpdatePartyBehaviorSelectionUpdate));
				for (int i = 0; i < 3; i++)
				{
					string text5 = GameTexts.FindText("str_clan_party_objective", i.ToString()).ToString();
					TextObject textObject5 = GameTexts.FindText("str_clan_party_objective_hint", i.ToString());
					this.PartyBehaviorSelector.AddItem(new SelectorItemVM(text5, textObject5));
				}
				this.PartyBehaviorSelector.SelectedIndex = (int)this.Party.MobileParty.Objective;
				this.PartyBehaviorText = GameTexts.FindText("str_clan_party_behavior", null).ToString();
			}
			if (this._leader != null)
			{
				this.CharacterModel.FillFrom(this._leader, -1);
				CharacterViewModel characterModel = this.CharacterModel;
				IFaction mapFaction = this.Party.MapFaction;
				characterModel.ArmorColor1 = ((mapFaction != null) ? mapFaction.Color : 0U);
				CharacterViewModel characterModel2 = this.CharacterModel;
				IFaction mapFaction2 = this.Party.MapFaction;
				characterModel2.ArmorColor2 = ((mapFaction2 != null) ? mapFaction2.Color2 : 0U);
			}
			this.HeroMembers.Clear();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (TroopRosterElement troopRosterElement in this.Party.MemberRoster.GetTroopRoster())
			{
				Hero heroObject = troopRosterElement.Character.HeroObject;
				if (heroObject != null && heroObject.Clan == Clan.PlayerClan && heroObject.GovernorOf == null)
				{
					ClanPartyMemberItemVM clanPartyMemberItemVM = new ClanPartyMemberItemVM(troopRosterElement.Character.HeroObject, this.Party.MobileParty);
					this.HeroMembers.Add(clanPartyMemberItemVM);
					if (clanPartyMemberItemVM.IsLeader)
					{
						this.LeaderMember = clanPartyMemberItemVM;
					}
				}
				else if (troopRosterElement.Character.DefaultFormationClass.Equals(FormationClass.Infantry))
				{
					num += troopRosterElement.Number;
				}
				else if (troopRosterElement.Character.DefaultFormationClass.Equals(FormationClass.Ranged))
				{
					num2 += troopRosterElement.Number;
				}
				else if (troopRosterElement.Character.DefaultFormationClass.Equals(FormationClass.Cavalry))
				{
					num3 += troopRosterElement.Number;
				}
				else if (troopRosterElement.Character.DefaultFormationClass.Equals(FormationClass.HorseArcher))
				{
					num4 += troopRosterElement.Number;
				}
			}
			if (this._isLeaderTeleporting)
			{
				ClanPartyMemberItemVM clanPartyMemberItemVM2 = new ClanPartyMemberItemVM(this._leader.HeroObject, this.Party.MobileParty);
				this.LeaderMember = clanPartyMemberItemVM2;
				this.HeroMembers.Insert(0, clanPartyMemberItemVM2);
			}
			this.HasCompanion = this.HeroMembers.Count > 1;
			if (this.IsMembersAndRolesVisible)
			{
				this.Roles.ApplyActionOnAllItems(delegate(ClanRoleItemVM x)
				{
					x.OnFinalize();
				});
				this.Roles.Clear();
				foreach (SkillEffect.PerkRole perkRole in this.GetAssignablePartyRoles())
				{
					this.Roles.Add(new ClanRoleItemVM(this.Party.MobileParty, perkRole, this.HeroMembers, new Action<ClanRoleItemVM>(this.OnRoleSelectionToggled), new Action(this.OnRoleAssigned)));
				}
			}
			this.InfantryCount = num;
			this.RangedCount = num2;
			this.CavalryCount = num3;
			this.HorseArcherCount = num4;
			ValueTuple<bool, TextObject> canUseActions = this.GetCanUseActions();
			this.CanUseActions = canUseActions.Item1;
			this.ActionsDisabledHint.HintText = (this.CanUseActions ? TextObject.Empty : canUseActions.Item2);
			if (!this.CanUseActions)
			{
				this.AutoRecruitmentHint.HintText = canUseActions.Item2;
				if (this.ExpenseItem != null)
				{
					this.ExpenseItem.IsEnabled = this.CanUseActions;
					this.ExpenseItem.WageLimitHint.HintText = canUseActions.Item2;
				}
				foreach (ClanRoleItemVM clanRoleItemVM in this.Roles)
				{
					clanRoleItemVM.SetEnabled(false, canUseActions.Item2);
				}
			}
		}

		private ValueTuple<bool, TextObject> GetCanUseActions()
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return new ValueTuple<bool, TextObject>(false, GameTexts.FindText("str_action_disabled_reason_prisoner", null));
			}
			return new ValueTuple<bool, TextObject>(true, TextObject.Empty);
		}

		private void OnExpenseChange()
		{
			this._onExpenseChange();
		}

		public void OnPartySelection()
		{
			int num = (this.IsPartyBehaviorEnabled ? this.PartyBehaviorSelector.SelectedIndex : (-1));
			this._onAssignment(this);
			if (this.IsPartyBehaviorEnabled)
			{
				this.PartyBehaviorSelector.SelectedIndex = num;
			}
		}

		public void ExecuteChangeLeader()
		{
			Action onShowChangeLeaderPopup = this._onShowChangeLeaderPopup;
			if (onShowChangeLeaderPopup == null)
			{
				return;
			}
			onShowChangeLeaderPopup();
		}

		private void OnRoleAssigned()
		{
			this.Roles.ApplyActionOnAllItems(delegate(ClanRoleItemVM x)
			{
				x.Refresh();
			});
		}

		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		private void UpdatePartyBehaviorSelectionUpdate(SelectorVM<SelectorItemVM> s)
		{
			if (s.SelectedIndex != (int)this.Party.MobileParty.Objective)
			{
				this.Party.MobileParty.SetPartyObjective((MobileParty.PartyObjective)s.SelectedIndex);
			}
		}

		private void OnAutoRecruitChanged(bool value)
		{
			if (this.Party.IsMobile && this.Party.MobileParty.IsGarrison)
			{
				Settlement homeSettlement = this.Party.MobileParty.HomeSettlement;
				if (((homeSettlement != null) ? homeSettlement.Town : null) != null)
				{
					this.Party.MobileParty.HomeSettlement.Town.GarrisonAutoRecruitmentIsEnabled = value;
				}
			}
		}

		private IEnumerable<SkillEffect.PerkRole> GetAssignablePartyRoles()
		{
			yield return SkillEffect.PerkRole.Quartermaster;
			yield return SkillEffect.PerkRole.Scout;
			yield return SkillEffect.PerkRole.Surgeon;
			yield return SkillEffect.PerkRole.Engineer;
			yield break;
		}

		private void OnRoleSelectionToggled(ClanRoleItemVM role)
		{
			this.LastOpenedRoleSelection = role;
		}

		private static CharacterCode GetCharacterCode(CharacterObject character)
		{
			if (character.IsHero)
			{
				return CampaignUIHelper.GetCharacterCode(character, false);
			}
			uint color = Hero.MainHero.MapFaction.Color;
			uint color2 = Hero.MainHero.MapFaction.Color2;
			Equipment equipment = character.Equipment;
			string text = ((equipment != null) ? equipment.CalculateEquipmentCode() : null);
			BodyProperties bodyProperties = character.GetBodyProperties(character.Equipment, -1);
			return CharacterCode.CreateFrom(text, bodyProperties, character.IsFemale, character.IsHero, color, color2, character.DefaultFormationClass, character.Race);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroMembers.ApplyActionOnAllItems(delegate(ClanPartyMemberItemVM h)
			{
				h.OnFinalize();
			});
			this.Roles.ApplyActionOnAllItems(delegate(ClanRoleItemVM x)
			{
				x.OnFinalize();
			});
		}

		[DataSourceProperty]
		public CharacterViewModel CharacterModel
		{
			get
			{
				return this._characterModel;
			}
			set
			{
				if (value != this._characterModel)
				{
					this._characterModel = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CharacterModel");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> PartyBehaviorSelector
		{
			get
			{
				return this._partyBehaviorSelector;
			}
			set
			{
				if (value != this._partyBehaviorSelector)
				{
					this._partyBehaviorSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "PartyBehaviorSelector");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM LeaderVisual
		{
			get
			{
				return this._leaderVisual;
			}
			set
			{
				if (value != this._leaderVisual)
				{
					this._leaderVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "LeaderVisual");
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
		public bool HasHeroMembers
		{
			get
			{
				return this._hasHeroMembers;
			}
			set
			{
				if (value != this._hasHeroMembers)
				{
					this._hasHeroMembers = value;
					base.OnPropertyChangedWithValue(value, "HasHeroMembers");
				}
			}
		}

		[DataSourceProperty]
		public bool IsClanRoleSelectionHighlightEnabled
		{
			get
			{
				return this._isClanRoleSelectionHighlightEnabled;
			}
			set
			{
				if (value != this._isClanRoleSelectionHighlightEnabled)
				{
					this._isClanRoleSelectionHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsClanRoleSelectionHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisbanding
		{
			get
			{
				return this._isDisbanding;
			}
			set
			{
				if (value != this._isDisbanding)
				{
					this._isDisbanding = value;
					base.OnPropertyChangedWithValue(value, "IsDisbanding");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInArmy
		{
			get
			{
				return this._isInArmy;
			}
			set
			{
				if (value != this._isInArmy)
				{
					this._isInArmy = value;
					base.OnPropertyChangedWithValue(value, "IsInArmy");
				}
			}
		}

		[DataSourceProperty]
		public bool CanUseActions
		{
			get
			{
				return this._canUseActions;
			}
			set
			{
				if (value != this._canUseActions)
				{
					this._canUseActions = value;
					base.OnPropertyChangedWithValue(value, "CanUseActions");
				}
			}
		}

		[DataSourceProperty]
		public bool IsChangeLeaderVisible
		{
			get
			{
				return this._isChangeLeaderVisible;
			}
			set
			{
				if (value != this._isChangeLeaderVisible)
				{
					this._isChangeLeaderVisible = value;
					base.OnPropertyChangedWithValue(value, "IsChangeLeaderVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsChangeLeaderEnabled
		{
			get
			{
				return this._isChangeLeaderEnabled;
			}
			set
			{
				if (value != this._isChangeLeaderEnabled)
				{
					this._isChangeLeaderEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsChangeLeaderEnabled");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ActionsDisabledHint
		{
			get
			{
				return this._actionsDisabledHint;
			}
			set
			{
				if (value != this._actionsDisabledHint)
				{
					this._actionsDisabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ActionsDisabledHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCaravan
		{
			get
			{
				return this._isCaravan;
			}
			set
			{
				if (value != this._isCaravan)
				{
					this._isCaravan = value;
					base.OnPropertyChangedWithValue(value, "IsCaravan");
				}
			}
		}

		[DataSourceProperty]
		public bool ShouldPartyHaveExpense
		{
			get
			{
				return this._shouldPartyHaveExpense;
			}
			set
			{
				if (value != this._shouldPartyHaveExpense)
				{
					this._shouldPartyHaveExpense = value;
					base.OnPropertyChangedWithValue(value, "ShouldPartyHaveExpense");
				}
			}
		}

		[DataSourceProperty]
		public bool HasCompanion
		{
			get
			{
				return this._hasCompanion;
			}
			set
			{
				if (value != this._hasCompanion)
				{
					this._hasCompanion = value;
					base.OnPropertyChangedWithValue(value, "HasCompanion");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAutoRecruitmentVisible
		{
			get
			{
				return this._isAutoRecruitmentVisible;
			}
			set
			{
				if (value != this._isAutoRecruitmentVisible)
				{
					this._isAutoRecruitmentVisible = value;
					base.OnPropertyChangedWithValue(value, "IsAutoRecruitmentVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool AutoRecruitmentValue
		{
			get
			{
				return this._autoRecruitmentValue;
			}
			set
			{
				if (value != this._autoRecruitmentValue)
				{
					this._autoRecruitmentValue = value;
					base.OnPropertyChangedWithValue(value, "AutoRecruitmentValue");
					this.OnAutoRecruitChanged(value);
				}
			}
		}

		[DataSourceProperty]
		public bool IsPartyBehaviorEnabled
		{
			get
			{
				return this._isPartyBehaviorEnabled;
			}
			set
			{
				if (value != this._isPartyBehaviorEnabled)
				{
					this._isPartyBehaviorEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPartyBehaviorEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMembersAndRolesVisible
		{
			get
			{
				return this._isMembersAndRolesVisible;
			}
			set
			{
				if (value != this._isMembersAndRolesVisible)
				{
					this._isMembersAndRolesVisible = value;
					base.OnPropertyChangedWithValue(value, "IsMembersAndRolesVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHeroParty
		{
			get
			{
				return this._isMainHeroParty;
			}
			set
			{
				if (value != this._isMainHeroParty)
				{
					this._isMainHeroParty = value;
					base.OnPropertyChangedWithValue(value, "IsMainHeroParty");
				}
			}
		}

		[DataSourceProperty]
		public ClanFinanceExpenseItemVM ExpenseItem
		{
			get
			{
				return this._expenseItem;
			}
			set
			{
				if (value != this._expenseItem)
				{
					this._expenseItem = value;
					base.OnPropertyChangedWithValue<ClanFinanceExpenseItemVM>(value, "ExpenseItem");
				}
			}
		}

		[DataSourceProperty]
		public ClanRoleItemVM LastOpenedRoleSelection
		{
			get
			{
				return this._lastOpenedRoleSelection;
			}
			set
			{
				if (value != this._lastOpenedRoleSelection)
				{
					this._lastOpenedRoleSelection = value;
					base.OnPropertyChangedWithValue<ClanRoleItemVM>(value, "LastOpenedRoleSelection");
				}
			}
		}

		[DataSourceProperty]
		public ClanPartyMemberItemVM LeaderMember
		{
			get
			{
				return this._leaderMember;
			}
			set
			{
				if (value != this._leaderMember)
				{
					this._leaderMember = value;
					base.OnPropertyChangedWithValue<ClanPartyMemberItemVM>(value, "LeaderMember");
				}
			}
		}

		[DataSourceProperty]
		public string PartySizeText
		{
			get
			{
				return this._partySizeText;
			}
			set
			{
				if (value != this._partySizeText)
				{
					this._partySizeText = value;
					base.OnPropertyChanged("PartyStrengthText");
				}
			}
		}

		[DataSourceProperty]
		public string MembersText
		{
			get
			{
				return this._membersText;
			}
			set
			{
				if (value != null)
				{
					this._membersText = value;
					base.OnPropertyChangedWithValue<string>(value, "MembersText");
				}
			}
		}

		[DataSourceProperty]
		public string AssigneesText
		{
			get
			{
				return this._assigneesText;
			}
			set
			{
				if (value != this._assigneesText)
				{
					this._assigneesText = value;
					base.OnPropertyChangedWithValue<string>(value, "AssigneesText");
				}
			}
		}

		[DataSourceProperty]
		public string RolesText
		{
			get
			{
				return this._rolesText;
			}
			set
			{
				if (value != this._rolesText)
				{
					this._rolesText = value;
					base.OnPropertyChangedWithValue<string>(value, "RolesText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyLeaderRoleEffectsText
		{
			get
			{
				return this._partyLeaderRoleEffectsText;
			}
			set
			{
				if (value != this._partyLeaderRoleEffectsText)
				{
					this._partyLeaderRoleEffectsText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyLeaderRoleEffectsText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyLocationText
		{
			get
			{
				return this._partyLocationText;
			}
			set
			{
				if (value != this._partyLocationText)
				{
					this._partyLocationText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyLocationText");
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
		public string PartySizeSubTitleText
		{
			get
			{
				return this._partySizeSubTitleText;
			}
			set
			{
				if (value != this._partySizeSubTitleText)
				{
					this._partySizeSubTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartySizeSubTitleText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyWageSubTitleText
		{
			get
			{
				return this._partyWageSubTitleText;
			}
			set
			{
				if (value != this._partyWageSubTitleText)
				{
					this._partyWageSubTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyWageSubTitleText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyBehaviorText
		{
			get
			{
				return this._partyBehaviorText;
			}
			set
			{
				if (value != this._partyBehaviorText)
				{
					this._partyBehaviorText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyBehaviorText");
				}
			}
		}

		[DataSourceProperty]
		public int InfantryCount
		{
			get
			{
				return this._infantryCount;
			}
			set
			{
				if (value != this._infantryCount)
				{
					this._infantryCount = value;
					base.OnPropertyChangedWithValue(value, "InfantryCount");
				}
			}
		}

		[DataSourceProperty]
		public int RangedCount
		{
			get
			{
				return this._rangedCount;
			}
			set
			{
				if (value != this._rangedCount)
				{
					this._rangedCount = value;
					base.OnPropertyChangedWithValue(value, "RangedCount");
				}
			}
		}

		[DataSourceProperty]
		public int CavalryCount
		{
			get
			{
				return this._cavalryCount;
			}
			set
			{
				if (value != this._cavalryCount)
				{
					this._cavalryCount = value;
					base.OnPropertyChangedWithValue(value, "CavalryCount");
				}
			}
		}

		[DataSourceProperty]
		public int HorseArcherCount
		{
			get
			{
				return this._horseArcherCount;
			}
			set
			{
				if (value != this._horseArcherCount)
				{
					this._horseArcherCount = value;
					base.OnPropertyChangedWithValue(value, "HorseArcherCount");
				}
			}
		}

		[DataSourceProperty]
		public string InArmyText
		{
			get
			{
				return this._inArmyText;
			}
			set
			{
				if (value != this._inArmyText)
				{
					this._inArmyText = value;
					base.OnPropertyChangedWithValue<string>(value, "InArmyText");
				}
			}
		}

		[DataSourceProperty]
		public string DisbandingText
		{
			get
			{
				return this._disbandingText;
			}
			set
			{
				if (value != this._disbandingText)
				{
					this._disbandingText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisbandingText");
				}
			}
		}

		[DataSourceProperty]
		public string AutoRecruitmentText
		{
			get
			{
				return this._autoRecruitmentText;
			}
			set
			{
				if (value != this._autoRecruitmentText)
				{
					this._autoRecruitmentText = value;
					base.OnPropertyChangedWithValue<string>(value, "AutoRecruitmentText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel AutoRecruitmentHint
		{
			get
			{
				return this._autoRecruitmentHint;
			}
			set
			{
				if (value != this._autoRecruitmentHint)
				{
					this._autoRecruitmentHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AutoRecruitmentHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel InArmyHint
		{
			get
			{
				return this._inArmyHint;
			}
			set
			{
				if (value != this._inArmyHint)
				{
					this._inArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InArmyHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ChangeLeaderHint
		{
			get
			{
				return this._changeLeaderHint;
			}
			set
			{
				if (value != this._changeLeaderHint)
				{
					this._changeLeaderHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ChangeLeaderHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel InfantryHint
		{
			get
			{
				return this._infantryHint;
			}
			set
			{
				if (value != this._infantryHint)
				{
					this._infantryHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "InfantryHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel RangedHint
		{
			get
			{
				return this._rangedHint;
			}
			set
			{
				if (value != this._rangedHint)
				{
					this._rangedHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "RangedHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel CavalryHint
		{
			get
			{
				return this._cavalryHint;
			}
			set
			{
				if (value != this._cavalryHint)
				{
					this._cavalryHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CavalryHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel HorseArcherHint
		{
			get
			{
				return this._horseArcherHint;
			}
			set
			{
				if (value != this._horseArcherHint)
				{
					this._horseArcherHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "HorseArcherHint");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanPartyMemberItemVM> HeroMembers
		{
			get
			{
				return this._heroMembers;
			}
			set
			{
				if (value != this._heroMembers)
				{
					this._heroMembers = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanPartyMemberItemVM>>(value, "HeroMembers");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanRoleItemVM> Roles
		{
			get
			{
				return this._roles;
			}
			set
			{
				if (value != this._roles)
				{
					this._roles = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanRoleItemVM>>(value, "Roles");
				}
			}
		}

		private List<TooltipProperty> GetPartyTroopInfo(PartyBase party, FormationClass formationClass)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", GameTexts.FindText("str_formation_class_string", formationClass.GetName()).ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			foreach (TroopRosterElement troopRosterElement in this.Party.MemberRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsHero && troopRosterElement.Character.DefaultFormationClass.Equals(formationClass))
				{
					list.Add(new TooltipProperty(troopRosterElement.Character.Name.ToString(), troopRosterElement.Number.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		private readonly Action<ClanPartyItemVM> _onAssignment;

		private readonly Action _onExpenseChange;

		private readonly Action _onShowChangeLeaderPopup;

		private readonly ClanPartyItemVM.ClanPartyType _type;

		private readonly TextObject _changeLeaderHintText = GameTexts.FindText("str_change_party_leader", null);

		private readonly IDisbandPartyCampaignBehavior _disbandBehavior;

		private readonly bool _isLeaderTeleporting;

		private readonly CharacterObject _leader;

		private SelectorVM<SelectorItemVM> _partyBehaviorSelector;

		private ClanFinanceExpenseItemVM _expenseItem;

		private ClanRoleItemVM _lastOpenedRoleSelection;

		private ClanPartyMemberItemVM _leaderMember;

		private ImageIdentifierVM _leaderVisual;

		private bool _isMainHeroParty;

		private bool _isSelected;

		private bool _hasHeroMembers;

		private string _partyLocationText;

		private string _partySizeText;

		private string _membersText;

		private string _assigneesText;

		private string _rolesText;

		private string _partyLeaderRoleEffectsText;

		private string _name;

		private string _partySizeSubTitleText;

		private string _partyWageSubTitleText;

		private string _partyBehaviorText;

		private int _infantryCount;

		private int _rangedCount;

		private int _cavalryCount;

		private int _horseArcherCount;

		private string _inArmyText;

		private string _disbandingText;

		private string _autoRecruitmentText;

		private bool _autoRecruitmentValue;

		private bool _isAutoRecruitmentVisible;

		private bool _shouldPartyHaveExpense;

		private bool _hasCompanion;

		private bool _isPartyBehaviorEnabled;

		private bool _isMembersAndRolesVisible;

		private bool _isCaravan;

		private bool _isDisbanding;

		private bool _isInArmy;

		private bool _canUseActions;

		private bool _isChangeLeaderVisible;

		private bool _isChangeLeaderEnabled;

		private bool _isClanRoleSelectionHighlightEnabled;

		private HintViewModel _actionsDisabledHint;

		private CharacterViewModel _characterModel;

		private HintViewModel _autoRecruitmentHint;

		private HintViewModel _inArmyHint;

		private HintViewModel _changeLeaderHint;

		private BasicTooltipViewModel _infantryHint;

		private BasicTooltipViewModel _rangedHint;

		private BasicTooltipViewModel _cavalryHint;

		private BasicTooltipViewModel _horseArcherHint;

		private MBBindingList<ClanPartyMemberItemVM> _heroMembers;

		private MBBindingList<ClanRoleItemVM> _roles;

		public enum ClanPartyType
		{
			Main,
			Member,
			Caravan,
			Garrison
		}
	}
}
