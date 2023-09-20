using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanPartiesVM : ViewModel
	{
		public int TotalExpense { get; private set; }

		public int TotalIncome { get; private set; }

		public ClanPartiesVM(Action onExpenseChange, Action<Hero> openPartyAsManage, Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
		{
			this._onExpenseChange = onExpenseChange;
			this._onRefresh = onRefresh;
			this._disbandBehavior = Campaign.Current.GetCampaignBehavior<IDisbandPartyCampaignBehavior>();
			this._teleportationBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			this._openPartyAsManage = openPartyAsManage;
			this._openCardSelectionPopup = openCardSelectionPopup;
			this._faction = Hero.MainHero.Clan;
			this.Parties = new MBBindingList<ClanPartyItemVM>();
			this.Garrisons = new MBBindingList<ClanPartyItemVM>();
			this.Caravans = new MBBindingList<ClanPartyItemVM>();
			this.CreateNewPartyActionHint = new HintViewModel();
			this.RefreshPartiesList();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SizeText = GameTexts.FindText("str_clan_party_size", null).ToString();
			this.MoraleText = GameTexts.FindText("str_morale", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.CreateNewPartyText = GameTexts.FindText("str_clan_create_new_party", null).ToString();
			this.GarrisonsText = GameTexts.FindText("str_garrisons", null).ToString();
			this.CaravansText = GameTexts.FindText("str_caravans", null).ToString();
			this.RefreshPartiesList();
			this.Parties.ApplyActionOnAllItems(delegate(ClanPartyItemVM x)
			{
				x.RefreshValues();
			});
			this.Garrisons.ApplyActionOnAllItems(delegate(ClanPartyItemVM x)
			{
				x.RefreshValues();
			});
			this.Caravans.ApplyActionOnAllItems(delegate(ClanPartyItemVM x)
			{
				x.RefreshValues();
			});
		}

		public void RefreshTotalExpense()
		{
			this.TotalExpense = (from p in this.Parties.Union(this.Garrisons).Union(this.Caravans)
				where p.ShouldPartyHaveExpense
				select p).Sum((ClanPartyItemVM p) => p.Expense);
			this.TotalIncome = this.Caravans.Sum((ClanPartyItemVM p) => p.Income);
		}

		public void RefreshPartiesList()
		{
			this.Parties.Clear();
			this.Garrisons.Clear();
			this.Caravans.Clear();
			foreach (WarPartyComponent warPartyComponent in this._faction.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty == MobileParty.MainParty)
				{
					this.Parties.Insert(0, new ClanPartyItemVM(warPartyComponent.Party, new Action<ClanPartyItemVM>(this.OnPartySelection), new Action(this.OnAnyExpenseChange), new Action(this.OnShowChangeLeaderPopup), ClanPartyItemVM.ClanPartyType.Main, this._disbandBehavior, this._teleportationBehavior));
				}
				else
				{
					this.Parties.Add(new ClanPartyItemVM(warPartyComponent.Party, new Action<ClanPartyItemVM>(this.OnPartySelection), new Action(this.OnAnyExpenseChange), new Action(this.OnShowChangeLeaderPopup), ClanPartyItemVM.ClanPartyType.Member, this._disbandBehavior, this._teleportationBehavior));
				}
			}
			using (IEnumerator<CaravanPartyComponent> enumerator2 = this._faction.Heroes.SelectMany((Hero h) => h.OwnedCaravans).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CaravanPartyComponent party = enumerator2.Current;
					if (!this.Caravans.Any((ClanPartyItemVM c) => c.Party.MobileParty == party.MobileParty))
					{
						this.Caravans.Add(new ClanPartyItemVM(party.Party, new Action<ClanPartyItemVM>(this.OnPartySelection), new Action(this.OnAnyExpenseChange), new Action(this.OnShowChangeLeaderPopup), ClanPartyItemVM.ClanPartyType.Caravan, this._disbandBehavior, this._teleportationBehavior));
					}
				}
			}
			using (IEnumerator<MobileParty> enumerator3 = (from a in this._faction.Settlements
				where a.Town != null
				select a into s
				select s.Town.GarrisonParty).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					MobileParty garrison = enumerator3.Current;
					if (garrison != null && !this.Garrisons.Any((ClanPartyItemVM c) => c.Party == garrison.Party))
					{
						this.Garrisons.Add(new ClanPartyItemVM(garrison.Party, new Action<ClanPartyItemVM>(this.OnPartySelection), new Action(this.OnAnyExpenseChange), new Action(this.OnShowChangeLeaderPopup), ClanPartyItemVM.ClanPartyType.Garrison, this._disbandBehavior, this._teleportationBehavior));
					}
				}
			}
			int count = this._faction.WarPartyComponents.Count;
			this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions).Any((Hero h) => h.IsActive && h.PartyBelongedToAsPrisoner == null && !h.IsChild && h.CanLeadParty() && (h.PartyBelongedTo == null || h.PartyBelongedTo.LeaderHero != h));
			TextObject textObject;
			this.CanCreateNewParty = this.GetCanCreateNewParty(out textObject);
			this.CreateNewPartyActionHint.HintText = textObject;
			GameTexts.SetVariable("CURRENT", count);
			GameTexts.SetVariable("LIMIT", this._faction.CommanderLimit);
			this.PartiesText = GameTexts.FindText("str_clan_parties", null).ToString();
			this.OnPartySelection(this.GetDefaultMember());
		}

		private bool GetCanCreateNewParty(out TextObject disabledReason)
		{
			bool flag = this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions).Any((Hero h) => h.IsActive && h.PartyBelongedToAsPrisoner == null && !h.IsChild && h.CanLeadParty() && (h.PartyBelongedTo == null || h.PartyBelongedTo.LeaderHero != h));
			TextObject textObject;
			if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
			{
				disabledReason = textObject;
				return false;
			}
			if (this._faction.CommanderLimit - this._faction.WarPartyComponents.Count <= 0)
			{
				disabledReason = GameTexts.FindText("str_clan_doesnt_have_empty_party_slots", null);
				return false;
			}
			if (!flag)
			{
				disabledReason = GameTexts.FindText("str_clan_doesnt_have_available_heroes", null);
				return false;
			}
			disabledReason = TextObject.Empty;
			return true;
		}

		private void OnAnyExpenseChange()
		{
			this.RefreshTotalExpense();
			this._onExpenseChange();
		}

		private ClanPartyItemVM GetDefaultMember()
		{
			return this.Parties.FirstOrDefault<ClanPartyItemVM>();
		}

		public void ExecuteCreateNewParty()
		{
			if (this.CanCreateNewParty)
			{
				List<InquiryElement> list = new List<InquiryElement>();
				foreach (Hero hero in this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions))
				{
					if ((hero.IsActive || hero.IsReleased || hero.IsFugitive) && !hero.IsChild && hero != Hero.MainHero && hero.CanLeadParty())
					{
						bool flag = false;
						string text = this.GetPartyLeaderAssignmentSkillsHint(hero);
						if (hero.PartyBelongedToAsPrisoner != null)
						{
							text = new TextObject("{=vOojEcIf}You cannot assign a prisoner member as a new party leader", null).ToString();
						}
						else if (hero.IsReleased)
						{
							text = new TextObject("{=OhNYkblK}This hero has just escaped from captors and will be available after some time.", null).ToString();
						}
						else if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero == hero)
						{
							text = new TextObject("{=aFYwbosi}This hero is already leading a party.", null).ToString();
						}
						else if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero != Hero.MainHero)
						{
							text = new TextObject("{=FjJi1DJb}This hero is already a part of an another party.", null).ToString();
						}
						else if (hero.GovernorOf != null)
						{
							text = new TextObject("{=Hz8XO8wk}Governors cannot lead a mobile party and be a governor at the same time.", null).ToString();
						}
						else if (hero.HeroState == Hero.CharacterStates.Disabled)
						{
							text = new TextObject("{=slzfQzl3}This hero is lost", null).ToString();
						}
						else if (hero.HeroState == Hero.CharacterStates.Fugitive)
						{
							text = new TextObject("{=dD3kRDHi}This hero is a fugitive and running from their captors. They will be available after some time.", null).ToString();
						}
						else
						{
							flag = true;
						}
						list.Add(new InquiryElement(hero, hero.Name.ToString(), new ImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false)), flag, text));
					}
				}
				if (list.Count > 0)
				{
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=0Q4Xo2BQ}Select the Leader of the New Party", null).ToString(), string.Empty, list, true, 1, GameTexts.FindText("str_done", null).ToString(), "", new Action<List<InquiryElement>>(this.OnNewPartySelectionOver), new Action<List<InquiryElement>>(this.OnNewPartySelectionOver), ""), false, false);
					return;
				}
				MBInformationManager.AddQuickInformation(new TextObject("{=qZvNIVGV}There is no one available in your clan who can lead a party right now.", null), 0, null, "");
			}
		}

		private void OnNewPartySelectionOver(List<InquiryElement> element)
		{
			if (element.Count == 0)
			{
				return;
			}
			Hero hero = (Hero)element[0].Identifier;
			bool flag = hero.PartyBelongedTo == MobileParty.MainParty;
			if (flag)
			{
				this._openPartyAsManage(hero);
				return;
			}
			MobilePartyHelper.CreateNewClanMobileParty(hero, this._faction, out flag);
			this._onRefresh();
		}

		public void SelectParty(PartyBase party)
		{
			foreach (ClanPartyItemVM clanPartyItemVM in this.Parties)
			{
				if (clanPartyItemVM.Party == party)
				{
					this.OnPartySelection(clanPartyItemVM);
					break;
				}
			}
			foreach (ClanPartyItemVM clanPartyItemVM2 in this.Caravans)
			{
				if (clanPartyItemVM2.Party == party)
				{
					this.OnPartySelection(clanPartyItemVM2);
					break;
				}
			}
		}

		private void OnPartySelection(ClanPartyItemVM party)
		{
			if (this.CurrentSelectedParty != null)
			{
				this.CurrentSelectedParty.IsSelected = false;
			}
			this.CurrentSelectedParty = party;
			if (party != null)
			{
				party.IsSelected = true;
			}
		}

		private string GetPartyLeaderAssignmentSkillsHint(Hero hero)
		{
			string text = "";
			int num = 0;
			foreach (SkillObject skillObject in this._leaderAssignmentRelevantSkills)
			{
				int skillValue = hero.GetSkillValue(skillObject);
				GameTexts.SetVariable("LEFT", skillObject.Name.ToString());
				GameTexts.SetVariable("RIGHT", skillValue);
				string text2 = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				if (num == 0)
				{
					text = text2;
				}
				else
				{
					GameTexts.SetVariable("STR1", text);
					GameTexts.SetVariable("STR2", text2);
					text = GameTexts.FindText("str_string_newline_string", null).ToString();
				}
				num++;
			}
			return text;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Parties.ApplyActionOnAllItems(delegate(ClanPartyItemVM p)
			{
				p.OnFinalize();
			});
			this.Garrisons.ApplyActionOnAllItems(delegate(ClanPartyItemVM p)
			{
				p.OnFinalize();
			});
			this.Caravans.ApplyActionOnAllItems(delegate(ClanPartyItemVM p)
			{
				p.OnFinalize();
			});
		}

		public void OnShowChangeLeaderPopup()
		{
			ClanPartyItemVM currentSelectedParty = this.CurrentSelectedParty;
			bool flag;
			if (currentSelectedParty == null)
			{
				flag = null != null;
			}
			else
			{
				PartyBase party = currentSelectedParty.Party;
				flag = ((party != null) ? party.MobileParty : null) != null;
			}
			if (flag)
			{
				ClanCardSelectionInfo clanCardSelectionInfo = new ClanCardSelectionInfo(GameTexts.FindText("str_change_party_leader", null), this.GetChangeLeaderCandidates(), new Action<List<object>, Action>(this.OnChangeLeaderOver), false);
				Action<ClanCardSelectionInfo> openCardSelectionPopup = this._openCardSelectionPopup;
				if (openCardSelectionPopup == null)
				{
					return;
				}
				openCardSelectionPopup(clanCardSelectionInfo);
			}
		}

		private IEnumerable<ClanCardSelectionItemInfo> GetChangeLeaderCandidates()
		{
			TextObject textObject;
			bool canDisbandParty = this.GetCanDisbandParty(out textObject);
			yield return new ClanCardSelectionItemInfo(GameTexts.FindText("str_disband_party", null), !canDisbandParty, textObject, null);
			foreach (Hero hero in this._faction.Heroes.Where((Hero h) => !h.IsDisabled).Union(this._faction.Companions))
			{
				if ((hero.IsActive || hero.IsReleased || hero.IsFugitive || hero.IsTraveling) && !hero.IsChild && hero != Hero.MainHero && hero.CanLeadParty())
				{
					Hero hero2 = hero;
					ClanPartyMemberItemVM leaderMember = this.CurrentSelectedParty.LeaderMember;
					if (hero2 != ((leaderMember != null) ? leaderMember.HeroObject : null))
					{
						TextObject textObject2;
						bool flag = FactionHelper.IsMainClanMemberAvailableForPartyLeaderChange(hero, true, this.CurrentSelectedParty.Party.MobileParty, out textObject2);
						ImageIdentifier imageIdentifier = new ImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
						yield return new ClanCardSelectionItemInfo(hero, hero.Name, imageIdentifier, CardSelectionItemSpriteType.None, null, null, this.GetChangeLeaderCandidateProperties(hero), !flag, textObject2, null);
					}
				}
			}
			IEnumerator<Hero> enumerator = null;
			yield break;
			yield break;
		}

		private IEnumerable<ClanCardSelectionItemPropertyInfo> GetChangeLeaderCandidateProperties(Hero hero)
		{
			TextObject teleportationDelayText = CampaignUIHelper.GetTeleportationDelayText(hero, this.CurrentSelectedParty.Party);
			yield return new ClanCardSelectionItemPropertyInfo(teleportationDelayText);
			TextObject textObject = new TextObject("{=hwrQqWir}No Skills", null);
			int num = 0;
			foreach (SkillObject skillObject in this._leaderAssignmentRelevantSkills)
			{
				TextObject textObject2 = new TextObject("{=!}{SKILL_VALUE}", null);
				textObject2.SetTextVariable("SKILL_VALUE", hero.GetSkillValue(skillObject));
				TextObject textObject3 = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(skillObject.Name, textObject2);
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
			yield return new ClanCardSelectionItemPropertyInfo(GameTexts.FindText("str_skills", null), textObject);
			yield break;
		}

		private void OnChangeLeaderOver(List<object> selectedItems, Action closePopup)
		{
			if (selectedItems.Count == 1)
			{
				Hero newLeader = selectedItems.FirstOrDefault<object>() as Hero;
				bool isDisband = newLeader == null;
				ClanPartyItemVM currentSelectedParty = this.CurrentSelectedParty;
				MobileParty mobileParty;
				if (currentSelectedParty == null)
				{
					mobileParty = null;
				}
				else
				{
					PartyBase party = currentSelectedParty.Party;
					mobileParty = ((party != null) ? party.MobileParty : null);
				}
				MobileParty mobileParty2 = mobileParty;
				DelayedTeleportationModel delayedTeleportationModel = Campaign.Current.Models.DelayedTeleportationModel;
				int num = ((!isDisband && mobileParty2 != null) ? ((int)Math.Ceiling((double)delayedTeleportationModel.GetTeleportationDelayAsHours(newLeader, mobileParty2.Party).ResultNumber)) : 0);
				MBTextManager.SetTextVariable("TRAVEL_DURATION", CampaignUIHelper.GetHoursAndDaysTextFromHourValue(num).ToString(), false);
				Hero newLeader2 = newLeader;
				if (((newLeader2 != null) ? newLeader2.CharacterObject : null) != null)
				{
					StringHelpers.SetCharacterProperties("LEADER", newLeader.CharacterObject, null, false);
				}
				object obj = GameTexts.FindText(isDisband ? "str_disband_party" : "str_change_clan_party_leader", null);
				TextObject textObject = GameTexts.FindText(isDisband ? "str_disband_party_inquiry" : ((num == 0) ? "str_change_clan_party_leader_instantly_inquiry" : "str_change_clan_party_leader_inquiry"), null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					Action closePopup3 = closePopup;
					if (closePopup3 != null)
					{
						closePopup3();
					}
					if (isDisband)
					{
						this.OnDisbandCurrentParty();
					}
					else
					{
						this.OnPartyLeaderChanged(newLeader);
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

		private void OnPartyLeaderChanged(Hero newLeader)
		{
			ClanPartyItemVM currentSelectedParty = this.CurrentSelectedParty;
			bool flag;
			if (currentSelectedParty == null)
			{
				flag = null != null;
			}
			else
			{
				PartyBase party = currentSelectedParty.Party;
				flag = ((party != null) ? party.LeaderHero : null) != null;
			}
			if (flag)
			{
				TeleportHeroAction.ApplyDelayedTeleportToParty(this.CurrentSelectedParty.Party.LeaderHero, MobileParty.MainParty);
			}
			TeleportHeroAction.ApplyDelayedTeleportToPartyAsPartyLeader(newLeader, this.CurrentSelectedParty.Party.MobileParty);
		}

		private void OnDisbandCurrentParty()
		{
			DisbandPartyAction.StartDisband(this.CurrentSelectedParty.Party.MobileParty);
		}

		private bool GetCanDisbandParty(out TextObject cannotDisbandReason)
		{
			bool flag = false;
			cannotDisbandReason = TextObject.Empty;
			ClanPartyItemVM currentSelectedParty = this.CurrentSelectedParty;
			MobileParty mobileParty;
			if (currentSelectedParty == null)
			{
				mobileParty = null;
			}
			else
			{
				PartyBase party = currentSelectedParty.Party;
				mobileParty = ((party != null) ? party.MobileParty : null);
			}
			MobileParty mobileParty2 = mobileParty;
			if (mobileParty2 != null)
			{
				TextObject textObject;
				if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject))
				{
					cannotDisbandReason = textObject;
				}
				else if (mobileParty2.IsMilitia)
				{
					cannotDisbandReason = GameTexts.FindText("str_cannot_disband_milita_party", null);
				}
				else if (mobileParty2.IsGarrison)
				{
					cannotDisbandReason = GameTexts.FindText("str_cannot_disband_garrison_party", null);
				}
				else if (mobileParty2.IsMainParty)
				{
					cannotDisbandReason = GameTexts.FindText("str_cannot_disband_main_party", null);
				}
				else if (this.CurrentSelectedParty.IsDisbanding)
				{
					cannotDisbandReason = GameTexts.FindText("str_cannot_disband_already_disbanding_party", null);
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		[DataSourceProperty]
		public HintViewModel CreateNewPartyActionHint
		{
			get
			{
				return this._createNewPartyActionHint;
			}
			set
			{
				if (value != this._createNewPartyActionHint)
				{
					this._createNewPartyActionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CreateNewPartyActionHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAnyValidPartySelected
		{
			get
			{
				return this._isAnyValidPartySelected;
			}
			set
			{
				if (value != this._isAnyValidPartySelected)
				{
					this._isAnyValidPartySelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyValidPartySelected");
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
		public string CaravansText
		{
			get
			{
				return this._caravansText;
			}
			set
			{
				if (value != this._caravansText)
				{
					this._caravansText = value;
					base.OnPropertyChangedWithValue<string>(value, "CaravansText");
				}
			}
		}

		[DataSourceProperty]
		public string GarrisonsText
		{
			get
			{
				return this._garrisonsText;
			}
			set
			{
				if (value != this._garrisonsText)
				{
					this._garrisonsText = value;
					base.OnPropertyChangedWithValue<string>(value, "GarrisonsText");
				}
			}
		}

		[DataSourceProperty]
		public string PartiesText
		{
			get
			{
				return this._partiesText;
			}
			set
			{
				if (value != this._partiesText)
				{
					this._partiesText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartiesText");
				}
			}
		}

		[DataSourceProperty]
		public string MoraleText
		{
			get
			{
				return this._moraleText;
			}
			set
			{
				if (value != this._moraleText)
				{
					this._moraleText = value;
					base.OnPropertyChangedWithValue<string>(value, "MoraleText");
				}
			}
		}

		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		[DataSourceProperty]
		public string CreateNewPartyText
		{
			get
			{
				return this._createNewPartyText;
			}
			set
			{
				if (value != this._createNewPartyText)
				{
					this._createNewPartyText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateNewPartyText");
				}
			}
		}

		[DataSourceProperty]
		public string SizeText
		{
			get
			{
				return this._sizeText;
			}
			set
			{
				if (value != this._sizeText)
				{
					this._sizeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SizeText");
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
		public bool CanCreateNewParty
		{
			get
			{
				return this._canCreateNewParty;
			}
			set
			{
				if (value != this._canCreateNewParty)
				{
					this._canCreateNewParty = value;
					base.OnPropertyChangedWithValue(value, "CanCreateNewParty");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanPartyItemVM> Parties
		{
			get
			{
				return this._parties;
			}
			set
			{
				if (value != this._parties)
				{
					this._parties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanPartyItemVM>>(value, "Parties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanPartyItemVM> Caravans
		{
			get
			{
				return this._caravans;
			}
			set
			{
				if (value != this._caravans)
				{
					this._caravans = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanPartyItemVM>>(value, "Caravans");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanPartyItemVM> Garrisons
		{
			get
			{
				return this._garrisons;
			}
			set
			{
				if (value != this._garrisons)
				{
					this._garrisons = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanPartyItemVM>>(value, "Garrisons");
				}
			}
		}

		[DataSourceProperty]
		public ClanPartyItemVM CurrentSelectedParty
		{
			get
			{
				return this._currentSelectedParty;
			}
			set
			{
				if (value != this._currentSelectedParty)
				{
					this._currentSelectedParty = value;
					base.OnPropertyChangedWithValue<ClanPartyItemVM>(value, "CurrentSelectedParty");
					this.IsAnyValidPartySelected = value != null;
				}
			}
		}

		private Action _onExpenseChange;

		private Action<Hero> _openPartyAsManage;

		private Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		private readonly IDisbandPartyCampaignBehavior _disbandBehavior;

		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		private readonly Action _onRefresh;

		private readonly Clan _faction;

		private readonly IEnumerable<SkillObject> _leaderAssignmentRelevantSkills = new List<SkillObject>
		{
			DefaultSkills.Engineering,
			DefaultSkills.Steward,
			DefaultSkills.Scouting,
			DefaultSkills.Medicine
		};

		private MBBindingList<ClanPartyItemVM> _parties;

		private MBBindingList<ClanPartyItemVM> _garrisons;

		private MBBindingList<ClanPartyItemVM> _caravans;

		private ClanPartyItemVM _currentSelectedParty;

		private HintViewModel _createNewPartyActionHint;

		private bool _canCreateNewParty;

		private bool _isSelected;

		private string _nameText;

		private string _moraleText;

		private string _locationText;

		private string _sizeText;

		private string _createNewPartyText;

		private string _partiesText;

		private string _caravansText;

		private string _garrisonsText;

		private bool _isAnyValidPartySelected;
	}
}
