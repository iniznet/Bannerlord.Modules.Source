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
	// Token: 0x02000105 RID: 261
	public class ClanPartyItemVM : ViewModel
	{
		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x00059506 File Offset: 0x00057706
		// (set) Token: 0x0600189B RID: 6299 RVA: 0x0005950E File Offset: 0x0005770E
		public int Expense { get; private set; }

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x0600189C RID: 6300 RVA: 0x00059517 File Offset: 0x00057717
		// (set) Token: 0x0600189D RID: 6301 RVA: 0x0005951F File Offset: 0x0005771F
		public int Income { get; private set; }

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x0600189E RID: 6302 RVA: 0x00059528 File Offset: 0x00057728
		public PartyBase Party { get; }

		// Token: 0x0600189F RID: 6303 RVA: 0x00059530 File Offset: 0x00057730
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

		// Token: 0x060018A0 RID: 6304 RVA: 0x0005991B File Offset: 0x00057B1B
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateProperties();
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0005992C File Offset: 0x00057B2C
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

		// Token: 0x060018A2 RID: 6306 RVA: 0x0005A190 File Offset: 0x00058390
		private ValueTuple<bool, TextObject> GetCanUseActions()
		{
			if (Hero.MainHero.IsPrisoner)
			{
				return new ValueTuple<bool, TextObject>(false, GameTexts.FindText("str_action_disabled_reason_prisoner", null));
			}
			return new ValueTuple<bool, TextObject>(true, TextObject.Empty);
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0005A1BB File Offset: 0x000583BB
		private void OnExpenseChange()
		{
			this._onExpenseChange();
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0005A1C8 File Offset: 0x000583C8
		public void OnPartySelection()
		{
			int num = (this.IsPartyBehaviorEnabled ? this.PartyBehaviorSelector.SelectedIndex : (-1));
			this._onAssignment(this);
			if (this.IsPartyBehaviorEnabled)
			{
				this.PartyBehaviorSelector.SelectedIndex = num;
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0005A20C File Offset: 0x0005840C
		public void ExecuteChangeLeader()
		{
			Action onShowChangeLeaderPopup = this._onShowChangeLeaderPopup;
			if (onShowChangeLeaderPopup == null)
			{
				return;
			}
			onShowChangeLeaderPopup();
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0005A21E File Offset: 0x0005841E
		private void OnRoleAssigned()
		{
			this.Roles.ApplyActionOnAllItems(delegate(ClanRoleItemVM x)
			{
				x.Refresh();
			});
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0005A24A File Offset: 0x0005844A
		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0005A25C File Offset: 0x0005845C
		private void UpdatePartyBehaviorSelectionUpdate(SelectorVM<SelectorItemVM> s)
		{
			if (s.SelectedIndex != (int)this.Party.MobileParty.Objective)
			{
				this.Party.MobileParty.SetPartyObjective((MobileParty.PartyObjective)s.SelectedIndex);
			}
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0005A28C File Offset: 0x0005848C
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

		// Token: 0x060018AA RID: 6314 RVA: 0x0005A2F1 File Offset: 0x000584F1
		private IEnumerable<SkillEffect.PerkRole> GetAssignablePartyRoles()
		{
			yield return SkillEffect.PerkRole.Quartermaster;
			yield return SkillEffect.PerkRole.Scout;
			yield return SkillEffect.PerkRole.Surgeon;
			yield return SkillEffect.PerkRole.Engineer;
			yield break;
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0005A2FA File Offset: 0x000584FA
		private void OnRoleSelectionToggled(ClanRoleItemVM role)
		{
			this.LastOpenedRoleSelection = role;
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0005A304 File Offset: 0x00058504
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

		// Token: 0x060018AD RID: 6317 RVA: 0x0005A384 File Offset: 0x00058584
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

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x060018AE RID: 6318 RVA: 0x0005A3EB File Offset: 0x000585EB
		// (set) Token: 0x060018AF RID: 6319 RVA: 0x0005A3F3 File Offset: 0x000585F3
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

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x060018B0 RID: 6320 RVA: 0x0005A411 File Offset: 0x00058611
		// (set) Token: 0x060018B1 RID: 6321 RVA: 0x0005A419 File Offset: 0x00058619
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

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x060018B2 RID: 6322 RVA: 0x0005A437 File Offset: 0x00058637
		// (set) Token: 0x060018B3 RID: 6323 RVA: 0x0005A43F File Offset: 0x0005863F
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

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x060018B4 RID: 6324 RVA: 0x0005A45D File Offset: 0x0005865D
		// (set) Token: 0x060018B5 RID: 6325 RVA: 0x0005A465 File Offset: 0x00058665
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

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x0005A483 File Offset: 0x00058683
		// (set) Token: 0x060018B7 RID: 6327 RVA: 0x0005A48B File Offset: 0x0005868B
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

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x060018B8 RID: 6328 RVA: 0x0005A4A9 File Offset: 0x000586A9
		// (set) Token: 0x060018B9 RID: 6329 RVA: 0x0005A4B1 File Offset: 0x000586B1
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

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x060018BA RID: 6330 RVA: 0x0005A4CF File Offset: 0x000586CF
		// (set) Token: 0x060018BB RID: 6331 RVA: 0x0005A4D7 File Offset: 0x000586D7
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

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x060018BC RID: 6332 RVA: 0x0005A4F5 File Offset: 0x000586F5
		// (set) Token: 0x060018BD RID: 6333 RVA: 0x0005A4FD File Offset: 0x000586FD
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

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x060018BE RID: 6334 RVA: 0x0005A51B File Offset: 0x0005871B
		// (set) Token: 0x060018BF RID: 6335 RVA: 0x0005A523 File Offset: 0x00058723
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

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x060018C0 RID: 6336 RVA: 0x0005A541 File Offset: 0x00058741
		// (set) Token: 0x060018C1 RID: 6337 RVA: 0x0005A549 File Offset: 0x00058749
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

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x0005A567 File Offset: 0x00058767
		// (set) Token: 0x060018C3 RID: 6339 RVA: 0x0005A56F File Offset: 0x0005876F
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

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x060018C4 RID: 6340 RVA: 0x0005A58D File Offset: 0x0005878D
		// (set) Token: 0x060018C5 RID: 6341 RVA: 0x0005A595 File Offset: 0x00058795
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

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x060018C6 RID: 6342 RVA: 0x0005A5B3 File Offset: 0x000587B3
		// (set) Token: 0x060018C7 RID: 6343 RVA: 0x0005A5BB File Offset: 0x000587BB
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

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x060018C8 RID: 6344 RVA: 0x0005A5D9 File Offset: 0x000587D9
		// (set) Token: 0x060018C9 RID: 6345 RVA: 0x0005A5E1 File Offset: 0x000587E1
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

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x060018CA RID: 6346 RVA: 0x0005A5FF File Offset: 0x000587FF
		// (set) Token: 0x060018CB RID: 6347 RVA: 0x0005A607 File Offset: 0x00058807
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

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x060018CC RID: 6348 RVA: 0x0005A625 File Offset: 0x00058825
		// (set) Token: 0x060018CD RID: 6349 RVA: 0x0005A62D File Offset: 0x0005882D
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

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x060018CE RID: 6350 RVA: 0x0005A64B File Offset: 0x0005884B
		// (set) Token: 0x060018CF RID: 6351 RVA: 0x0005A653 File Offset: 0x00058853
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

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x0005A678 File Offset: 0x00058878
		// (set) Token: 0x060018D1 RID: 6353 RVA: 0x0005A680 File Offset: 0x00058880
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

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x060018D2 RID: 6354 RVA: 0x0005A69E File Offset: 0x0005889E
		// (set) Token: 0x060018D3 RID: 6355 RVA: 0x0005A6A6 File Offset: 0x000588A6
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

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x060018D4 RID: 6356 RVA: 0x0005A6C4 File Offset: 0x000588C4
		// (set) Token: 0x060018D5 RID: 6357 RVA: 0x0005A6CC File Offset: 0x000588CC
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

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x060018D6 RID: 6358 RVA: 0x0005A6EA File Offset: 0x000588EA
		// (set) Token: 0x060018D7 RID: 6359 RVA: 0x0005A6F2 File Offset: 0x000588F2
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

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x060018D8 RID: 6360 RVA: 0x0005A710 File Offset: 0x00058910
		// (set) Token: 0x060018D9 RID: 6361 RVA: 0x0005A718 File Offset: 0x00058918
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

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x060018DA RID: 6362 RVA: 0x0005A736 File Offset: 0x00058936
		// (set) Token: 0x060018DB RID: 6363 RVA: 0x0005A73E File Offset: 0x0005893E
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

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x060018DC RID: 6364 RVA: 0x0005A75C File Offset: 0x0005895C
		// (set) Token: 0x060018DD RID: 6365 RVA: 0x0005A764 File Offset: 0x00058964
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

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x060018DE RID: 6366 RVA: 0x0005A786 File Offset: 0x00058986
		// (set) Token: 0x060018DF RID: 6367 RVA: 0x0005A78E File Offset: 0x0005898E
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

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x060018E0 RID: 6368 RVA: 0x0005A7A6 File Offset: 0x000589A6
		// (set) Token: 0x060018E1 RID: 6369 RVA: 0x0005A7AE File Offset: 0x000589AE
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

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x060018E2 RID: 6370 RVA: 0x0005A7D1 File Offset: 0x000589D1
		// (set) Token: 0x060018E3 RID: 6371 RVA: 0x0005A7D9 File Offset: 0x000589D9
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

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x060018E4 RID: 6372 RVA: 0x0005A7FC File Offset: 0x000589FC
		// (set) Token: 0x060018E5 RID: 6373 RVA: 0x0005A804 File Offset: 0x00058A04
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

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x060018E6 RID: 6374 RVA: 0x0005A827 File Offset: 0x00058A27
		// (set) Token: 0x060018E7 RID: 6375 RVA: 0x0005A82F File Offset: 0x00058A2F
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

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x060018E8 RID: 6376 RVA: 0x0005A852 File Offset: 0x00058A52
		// (set) Token: 0x060018E9 RID: 6377 RVA: 0x0005A85A File Offset: 0x00058A5A
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

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x060018EA RID: 6378 RVA: 0x0005A87D File Offset: 0x00058A7D
		// (set) Token: 0x060018EB RID: 6379 RVA: 0x0005A885 File Offset: 0x00058A85
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

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x060018EC RID: 6380 RVA: 0x0005A8A8 File Offset: 0x00058AA8
		// (set) Token: 0x060018ED RID: 6381 RVA: 0x0005A8B0 File Offset: 0x00058AB0
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

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x060018EE RID: 6382 RVA: 0x0005A8D3 File Offset: 0x00058AD3
		// (set) Token: 0x060018EF RID: 6383 RVA: 0x0005A8DB File Offset: 0x00058ADB
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

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x060018F0 RID: 6384 RVA: 0x0005A8FE File Offset: 0x00058AFE
		// (set) Token: 0x060018F1 RID: 6385 RVA: 0x0005A906 File Offset: 0x00058B06
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

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x060018F2 RID: 6386 RVA: 0x0005A924 File Offset: 0x00058B24
		// (set) Token: 0x060018F3 RID: 6387 RVA: 0x0005A92C File Offset: 0x00058B2C
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

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x060018F4 RID: 6388 RVA: 0x0005A94A File Offset: 0x00058B4A
		// (set) Token: 0x060018F5 RID: 6389 RVA: 0x0005A952 File Offset: 0x00058B52
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

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x060018F6 RID: 6390 RVA: 0x0005A970 File Offset: 0x00058B70
		// (set) Token: 0x060018F7 RID: 6391 RVA: 0x0005A978 File Offset: 0x00058B78
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

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060018F8 RID: 6392 RVA: 0x0005A996 File Offset: 0x00058B96
		// (set) Token: 0x060018F9 RID: 6393 RVA: 0x0005A99E File Offset: 0x00058B9E
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

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x060018FA RID: 6394 RVA: 0x0005A9C1 File Offset: 0x00058BC1
		// (set) Token: 0x060018FB RID: 6395 RVA: 0x0005A9C9 File Offset: 0x00058BC9
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

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x060018FC RID: 6396 RVA: 0x0005A9EC File Offset: 0x00058BEC
		// (set) Token: 0x060018FD RID: 6397 RVA: 0x0005A9F4 File Offset: 0x00058BF4
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

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x060018FE RID: 6398 RVA: 0x0005AA17 File Offset: 0x00058C17
		// (set) Token: 0x060018FF RID: 6399 RVA: 0x0005AA1F File Offset: 0x00058C1F
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

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06001900 RID: 6400 RVA: 0x0005AA3D File Offset: 0x00058C3D
		// (set) Token: 0x06001901 RID: 6401 RVA: 0x0005AA45 File Offset: 0x00058C45
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

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06001902 RID: 6402 RVA: 0x0005AA63 File Offset: 0x00058C63
		// (set) Token: 0x06001903 RID: 6403 RVA: 0x0005AA6B File Offset: 0x00058C6B
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

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06001904 RID: 6404 RVA: 0x0005AA89 File Offset: 0x00058C89
		// (set) Token: 0x06001905 RID: 6405 RVA: 0x0005AA91 File Offset: 0x00058C91
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

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x06001906 RID: 6406 RVA: 0x0005AAAF File Offset: 0x00058CAF
		// (set) Token: 0x06001907 RID: 6407 RVA: 0x0005AAB7 File Offset: 0x00058CB7
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

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06001908 RID: 6408 RVA: 0x0005AAD5 File Offset: 0x00058CD5
		// (set) Token: 0x06001909 RID: 6409 RVA: 0x0005AADD File Offset: 0x00058CDD
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

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x0005AAFB File Offset: 0x00058CFB
		// (set) Token: 0x0600190B RID: 6411 RVA: 0x0005AB03 File Offset: 0x00058D03
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

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x0600190C RID: 6412 RVA: 0x0005AB21 File Offset: 0x00058D21
		// (set) Token: 0x0600190D RID: 6413 RVA: 0x0005AB29 File Offset: 0x00058D29
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

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x0600190E RID: 6414 RVA: 0x0005AB47 File Offset: 0x00058D47
		// (set) Token: 0x0600190F RID: 6415 RVA: 0x0005AB4F File Offset: 0x00058D4F
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

		// Token: 0x06001910 RID: 6416 RVA: 0x0005AB70 File Offset: 0x00058D70
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

		// Token: 0x04000BB3 RID: 2995
		private readonly Action<ClanPartyItemVM> _onAssignment;

		// Token: 0x04000BB4 RID: 2996
		private readonly Action _onExpenseChange;

		// Token: 0x04000BB5 RID: 2997
		private readonly Action _onShowChangeLeaderPopup;

		// Token: 0x04000BB6 RID: 2998
		private readonly ClanPartyItemVM.ClanPartyType _type;

		// Token: 0x04000BB7 RID: 2999
		private readonly TextObject _changeLeaderHintText = GameTexts.FindText("str_change_party_leader", null);

		// Token: 0x04000BB8 RID: 3000
		private readonly IDisbandPartyCampaignBehavior _disbandBehavior;

		// Token: 0x04000BB9 RID: 3001
		private readonly bool _isLeaderTeleporting;

		// Token: 0x04000BBB RID: 3003
		private readonly CharacterObject _leader;

		// Token: 0x04000BBC RID: 3004
		private SelectorVM<SelectorItemVM> _partyBehaviorSelector;

		// Token: 0x04000BBD RID: 3005
		private ClanFinanceExpenseItemVM _expenseItem;

		// Token: 0x04000BBE RID: 3006
		private ClanRoleItemVM _lastOpenedRoleSelection;

		// Token: 0x04000BBF RID: 3007
		private ClanPartyMemberItemVM _leaderMember;

		// Token: 0x04000BC0 RID: 3008
		private ImageIdentifierVM _leaderVisual;

		// Token: 0x04000BC1 RID: 3009
		private bool _isMainHeroParty;

		// Token: 0x04000BC2 RID: 3010
		private bool _isSelected;

		// Token: 0x04000BC3 RID: 3011
		private bool _hasHeroMembers;

		// Token: 0x04000BC4 RID: 3012
		private string _partyLocationText;

		// Token: 0x04000BC5 RID: 3013
		private string _partySizeText;

		// Token: 0x04000BC6 RID: 3014
		private string _membersText;

		// Token: 0x04000BC7 RID: 3015
		private string _assigneesText;

		// Token: 0x04000BC8 RID: 3016
		private string _rolesText;

		// Token: 0x04000BC9 RID: 3017
		private string _partyLeaderRoleEffectsText;

		// Token: 0x04000BCA RID: 3018
		private string _name;

		// Token: 0x04000BCB RID: 3019
		private string _partySizeSubTitleText;

		// Token: 0x04000BCC RID: 3020
		private string _partyWageSubTitleText;

		// Token: 0x04000BCD RID: 3021
		private string _partyBehaviorText;

		// Token: 0x04000BCE RID: 3022
		private int _infantryCount;

		// Token: 0x04000BCF RID: 3023
		private int _rangedCount;

		// Token: 0x04000BD0 RID: 3024
		private int _cavalryCount;

		// Token: 0x04000BD1 RID: 3025
		private int _horseArcherCount;

		// Token: 0x04000BD2 RID: 3026
		private string _inArmyText;

		// Token: 0x04000BD3 RID: 3027
		private string _disbandingText;

		// Token: 0x04000BD4 RID: 3028
		private string _autoRecruitmentText;

		// Token: 0x04000BD5 RID: 3029
		private bool _autoRecruitmentValue;

		// Token: 0x04000BD6 RID: 3030
		private bool _isAutoRecruitmentVisible;

		// Token: 0x04000BD7 RID: 3031
		private bool _shouldPartyHaveExpense;

		// Token: 0x04000BD8 RID: 3032
		private bool _hasCompanion;

		// Token: 0x04000BD9 RID: 3033
		private bool _isPartyBehaviorEnabled;

		// Token: 0x04000BDA RID: 3034
		private bool _isMembersAndRolesVisible;

		// Token: 0x04000BDB RID: 3035
		private bool _isCaravan;

		// Token: 0x04000BDC RID: 3036
		private bool _isDisbanding;

		// Token: 0x04000BDD RID: 3037
		private bool _isInArmy;

		// Token: 0x04000BDE RID: 3038
		private bool _canUseActions;

		// Token: 0x04000BDF RID: 3039
		private bool _isChangeLeaderVisible;

		// Token: 0x04000BE0 RID: 3040
		private bool _isChangeLeaderEnabled;

		// Token: 0x04000BE1 RID: 3041
		private bool _isClanRoleSelectionHighlightEnabled;

		// Token: 0x04000BE2 RID: 3042
		private HintViewModel _actionsDisabledHint;

		// Token: 0x04000BE3 RID: 3043
		private CharacterViewModel _characterModel;

		// Token: 0x04000BE4 RID: 3044
		private HintViewModel _autoRecruitmentHint;

		// Token: 0x04000BE5 RID: 3045
		private HintViewModel _inArmyHint;

		// Token: 0x04000BE6 RID: 3046
		private HintViewModel _changeLeaderHint;

		// Token: 0x04000BE7 RID: 3047
		private BasicTooltipViewModel _infantryHint;

		// Token: 0x04000BE8 RID: 3048
		private BasicTooltipViewModel _rangedHint;

		// Token: 0x04000BE9 RID: 3049
		private BasicTooltipViewModel _cavalryHint;

		// Token: 0x04000BEA RID: 3050
		private BasicTooltipViewModel _horseArcherHint;

		// Token: 0x04000BEB RID: 3051
		private MBBindingList<ClanPartyMemberItemVM> _heroMembers;

		// Token: 0x04000BEC RID: 3052
		private MBBindingList<ClanRoleItemVM> _roles;

		// Token: 0x02000238 RID: 568
		public enum ClanPartyType
		{
			// Token: 0x040010CA RID: 4298
			Main,
			// Token: 0x040010CB RID: 4299
			Member,
			// Token: 0x040010CC RID: 4300
			Caravan,
			// Token: 0x040010CD RID: 4301
			Garrison
		}
	}
}
