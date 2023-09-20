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
	// Token: 0x02000115 RID: 277
	public class ClanPartiesVM : ViewModel
	{
		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06001A6E RID: 6766 RVA: 0x0005FA91 File Offset: 0x0005DC91
		// (set) Token: 0x06001A6F RID: 6767 RVA: 0x0005FA99 File Offset: 0x0005DC99
		public int TotalExpense { get; private set; }

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06001A70 RID: 6768 RVA: 0x0005FAA2 File Offset: 0x0005DCA2
		// (set) Token: 0x06001A71 RID: 6769 RVA: 0x0005FAAA File Offset: 0x0005DCAA
		public int TotalIncome { get; private set; }

		// Token: 0x06001A72 RID: 6770 RVA: 0x0005FAB4 File Offset: 0x0005DCB4
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

		// Token: 0x06001A73 RID: 6771 RVA: 0x0005FB84 File Offset: 0x0005DD84
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

		// Token: 0x06001A74 RID: 6772 RVA: 0x0005FCB8 File Offset: 0x0005DEB8
		public void RefreshTotalExpense()
		{
			this.TotalExpense = (from p in this.Parties.Union(this.Garrisons).Union(this.Caravans)
				where p.ShouldPartyHaveExpense
				select p).Sum((ClanPartyItemVM p) => p.Expense);
			this.TotalIncome = this.Caravans.Sum((ClanPartyItemVM p) => p.Income);
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0005FD60 File Offset: 0x0005DF60
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

		// Token: 0x06001A76 RID: 6774 RVA: 0x0006012C File Offset: 0x0005E32C
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

		// Token: 0x06001A77 RID: 6775 RVA: 0x000601F3 File Offset: 0x0005E3F3
		private void OnAnyExpenseChange()
		{
			this.RefreshTotalExpense();
			this._onExpenseChange();
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x00060206 File Offset: 0x0005E406
		private ClanPartyItemVM GetDefaultMember()
		{
			return this.Parties.FirstOrDefault<ClanPartyItemVM>();
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x00060214 File Offset: 0x0005E414
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

		// Token: 0x06001A7A RID: 6778 RVA: 0x00060490 File Offset: 0x0005E690
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

		// Token: 0x06001A7B RID: 6779 RVA: 0x000604F0 File Offset: 0x0005E6F0
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

		// Token: 0x06001A7C RID: 6780 RVA: 0x00060590 File Offset: 0x0005E790
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

		// Token: 0x06001A7D RID: 6781 RVA: 0x000605B8 File Offset: 0x0005E7B8
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

		// Token: 0x06001A7E RID: 6782 RVA: 0x00060678 File Offset: 0x0005E878
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

		// Token: 0x06001A7F RID: 6783 RVA: 0x0006070C File Offset: 0x0005E90C
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

		// Token: 0x06001A80 RID: 6784 RVA: 0x0006076F File Offset: 0x0005E96F
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

		// Token: 0x06001A81 RID: 6785 RVA: 0x0006077F File Offset: 0x0005E97F
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

		// Token: 0x06001A82 RID: 6786 RVA: 0x00060798 File Offset: 0x0005E998
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

		// Token: 0x06001A83 RID: 6787 RVA: 0x0006093C File Offset: 0x0005EB3C
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

		// Token: 0x06001A84 RID: 6788 RVA: 0x00060999 File Offset: 0x0005EB99
		private void OnDisbandCurrentParty()
		{
			DisbandPartyAction.StartDisband(this.CurrentSelectedParty.Party.MobileParty);
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x000609B0 File Offset: 0x0005EBB0
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

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06001A86 RID: 6790 RVA: 0x00060A5A File Offset: 0x0005EC5A
		// (set) Token: 0x06001A87 RID: 6791 RVA: 0x00060A62 File Offset: 0x0005EC62
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

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06001A88 RID: 6792 RVA: 0x00060A80 File Offset: 0x0005EC80
		// (set) Token: 0x06001A89 RID: 6793 RVA: 0x00060A88 File Offset: 0x0005EC88
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

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06001A8A RID: 6794 RVA: 0x00060AA6 File Offset: 0x0005ECA6
		// (set) Token: 0x06001A8B RID: 6795 RVA: 0x00060AAE File Offset: 0x0005ECAE
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

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06001A8C RID: 6796 RVA: 0x00060AD1 File Offset: 0x0005ECD1
		// (set) Token: 0x06001A8D RID: 6797 RVA: 0x00060AD9 File Offset: 0x0005ECD9
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

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06001A8E RID: 6798 RVA: 0x00060AFC File Offset: 0x0005ECFC
		// (set) Token: 0x06001A8F RID: 6799 RVA: 0x00060B04 File Offset: 0x0005ED04
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

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06001A90 RID: 6800 RVA: 0x00060B27 File Offset: 0x0005ED27
		// (set) Token: 0x06001A91 RID: 6801 RVA: 0x00060B2F File Offset: 0x0005ED2F
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

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06001A92 RID: 6802 RVA: 0x00060B52 File Offset: 0x0005ED52
		// (set) Token: 0x06001A93 RID: 6803 RVA: 0x00060B5A File Offset: 0x0005ED5A
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

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06001A94 RID: 6804 RVA: 0x00060B7D File Offset: 0x0005ED7D
		// (set) Token: 0x06001A95 RID: 6805 RVA: 0x00060B85 File Offset: 0x0005ED85
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

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06001A96 RID: 6806 RVA: 0x00060BA8 File Offset: 0x0005EDA8
		// (set) Token: 0x06001A97 RID: 6807 RVA: 0x00060BB0 File Offset: 0x0005EDB0
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

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x00060BD3 File Offset: 0x0005EDD3
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x00060BDB File Offset: 0x0005EDDB
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

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x00060BFE File Offset: 0x0005EDFE
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x00060C06 File Offset: 0x0005EE06
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

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06001A9C RID: 6812 RVA: 0x00060C24 File Offset: 0x0005EE24
		// (set) Token: 0x06001A9D RID: 6813 RVA: 0x00060C2C File Offset: 0x0005EE2C
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

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06001A9E RID: 6814 RVA: 0x00060C4A File Offset: 0x0005EE4A
		// (set) Token: 0x06001A9F RID: 6815 RVA: 0x00060C52 File Offset: 0x0005EE52
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

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x00060C70 File Offset: 0x0005EE70
		// (set) Token: 0x06001AA1 RID: 6817 RVA: 0x00060C78 File Offset: 0x0005EE78
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

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06001AA2 RID: 6818 RVA: 0x00060C96 File Offset: 0x0005EE96
		// (set) Token: 0x06001AA3 RID: 6819 RVA: 0x00060C9E File Offset: 0x0005EE9E
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

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x00060CBC File Offset: 0x0005EEBC
		// (set) Token: 0x06001AA5 RID: 6821 RVA: 0x00060CC4 File Offset: 0x0005EEC4
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

		// Token: 0x04000C85 RID: 3205
		private Action _onExpenseChange;

		// Token: 0x04000C86 RID: 3206
		private Action<Hero> _openPartyAsManage;

		// Token: 0x04000C87 RID: 3207
		private Action<ClanCardSelectionInfo> _openCardSelectionPopup;

		// Token: 0x04000C88 RID: 3208
		private readonly IDisbandPartyCampaignBehavior _disbandBehavior;

		// Token: 0x04000C89 RID: 3209
		private readonly ITeleportationCampaignBehavior _teleportationBehavior;

		// Token: 0x04000C8A RID: 3210
		private readonly Action _onRefresh;

		// Token: 0x04000C8B RID: 3211
		private readonly Clan _faction;

		// Token: 0x04000C8C RID: 3212
		private readonly IEnumerable<SkillObject> _leaderAssignmentRelevantSkills = new List<SkillObject>
		{
			DefaultSkills.Engineering,
			DefaultSkills.Steward,
			DefaultSkills.Scouting,
			DefaultSkills.Medicine
		};

		// Token: 0x04000C8D RID: 3213
		private MBBindingList<ClanPartyItemVM> _parties;

		// Token: 0x04000C8E RID: 3214
		private MBBindingList<ClanPartyItemVM> _garrisons;

		// Token: 0x04000C8F RID: 3215
		private MBBindingList<ClanPartyItemVM> _caravans;

		// Token: 0x04000C90 RID: 3216
		private ClanPartyItemVM _currentSelectedParty;

		// Token: 0x04000C91 RID: 3217
		private HintViewModel _createNewPartyActionHint;

		// Token: 0x04000C92 RID: 3218
		private bool _canCreateNewParty;

		// Token: 0x04000C93 RID: 3219
		private bool _isSelected;

		// Token: 0x04000C94 RID: 3220
		private string _nameText;

		// Token: 0x04000C95 RID: 3221
		private string _moraleText;

		// Token: 0x04000C96 RID: 3222
		private string _locationText;

		// Token: 0x04000C97 RID: 3223
		private string _sizeText;

		// Token: 0x04000C98 RID: 3224
		private string _createNewPartyText;

		// Token: 0x04000C99 RID: 3225
		private string _partiesText;

		// Token: 0x04000C9A RID: 3226
		private string _caravansText;

		// Token: 0x04000C9B RID: 3227
		private string _garrisonsText;

		// Token: 0x04000C9C RID: 3228
		private bool _isAnyValidPartySelected;
	}
}
