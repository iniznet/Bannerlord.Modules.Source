using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	[MenuOverlay("SettlementMenuOverlay")]
	public class SettlementMenuOverlayVM : GameMenuOverlay
	{
		public SettlementMenuOverlayVM(GameOverlays.MenuOverlayType type)
		{
			this._type = type;
			this._overlayTalkItem = null;
			base.IsInitializationOver = false;
			this.CharacterList = new MBBindingList<GameMenuPartyItemVM>();
			this.PartyList = new MBBindingList<GameMenuPartyItemVM>();
			this.IssueList = new MBBindingList<StringItemWithHintVM>();
			base.CurrentOverlayType = 0;
			this.CrimeHint = new BasicTooltipViewModel(() => this.GetCrimeTooltip());
			if (Settlement.CurrentSettlement.IsFortification)
			{
				this.RemainingFoodHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownFoodTooltip(Settlement.CurrentSettlement.Town));
				this.LoyaltyHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownLoyaltyTooltip(Settlement.CurrentSettlement.Town));
				this.MilitasHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownMilitiaTooltip(Settlement.CurrentSettlement.Town));
				this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownProsperityTooltip(Settlement.CurrentSettlement.Town));
				this.WallsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownWallsTooltip(Settlement.CurrentSettlement.Town));
				this.GarrisonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownGarrisonTooltip(Settlement.CurrentSettlement.Town));
				this.SecurityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTownSecurityTooltip(Settlement.CurrentSettlement.Town));
			}
			else
			{
				this.MilitasHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetVillageMilitiaTooltip(Settlement.CurrentSettlement.Village));
				this.LoyaltyHint = new BasicTooltipViewModel();
				this.WallsHint = new BasicTooltipViewModel();
				this.ProsperityHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetVillageProsperityTooltip(Settlement.CurrentSettlement.Village));
			}
			if (Settlement.CurrentSettlement.IsFortification)
			{
				this._settlementComponent = Settlement.CurrentSettlement.SettlementComponent;
			}
			else if (Settlement.CurrentSettlement.IsVillage)
			{
				this._settlementComponent = Settlement.CurrentSettlement.Village;
			}
			else
			{
				this._settlementComponent = Settlement.CurrentSettlement.SettlementComponent;
			}
			this.UpdateSettlementOwnerBanner();
			this._contextMenuItem = null;
			base.IsInitializationOver = true;
			CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnPeaceDeclared));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.TownRebelliosStateChanged.AddNonSerializedListener(this, new Action<Town, bool>(this.OnTownRebelliousStateChanged));
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		private List<TooltipProperty> GetCrimeTooltip()
		{
			Game game = Game.Current;
			if (game != null)
			{
				game.EventManager.TriggerEvent<SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent>(new SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent());
			}
			return CampaignUIHelper.GetCrimeTooltip(Settlement.CurrentSettlement);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PartyFilterHint = new HintViewModel(GameTexts.FindText("str_parties", null), null);
			this.CharacterFilterHint = new HintViewModel(GameTexts.FindText("str_characters", null), null);
			this.Refresh();
		}

		protected override void ExecuteOnSetAsActiveContextMenuItem(GameMenuPartyItemVM troop)
		{
			base.ExecuteOnSetAsActiveContextMenuItem(troop);
			base.ContextList.Clear();
			this.IssueList.Clear();
			if (this._contextMenuItem.Character != null && (!this._contextMenuItem.Character.IsHero || !this._contextMenuItem.Character.HeroObject.IsPrisoner))
			{
				bool flag = true;
				TextObject textObject = TextObject.Empty;
				this._mostRecentOverlayTalkPermission = null;
				Game.Current.EventManager.TriggerEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(new SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent(this._contextMenuItem.Character.HeroObject, new Action<bool, TextObject>(this.OnSettlementOverlayTalkPermissionResult)));
				if (this._mostRecentOverlayTalkPermission != null)
				{
					flag = this._mostRecentOverlayTalkPermission.Item1;
					textObject = this._mostRecentOverlayTalkPermission.Item2;
				}
				this._overlayTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "Conversation").ToString(), flag, GameMenuOverlay.MenuOverlayContextList.Conversation, textObject);
				base.ContextList.Add(this._overlayTalkItem);
				bool flag2 = true;
				TextObject textObject2 = TextObject.Empty;
				this._mostRecentOverlayQuickTalkPermission = null;
				Game.Current.EventManager.TriggerEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(new SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent(new Action<bool, TextObject>(this.OnSettlementOverlayQuickTalkPermissionResult)));
				if (this._mostRecentOverlayQuickTalkPermission != null)
				{
					flag2 = this._mostRecentOverlayQuickTalkPermission.Item1;
					textObject2 = this._mostRecentOverlayQuickTalkPermission.Item2;
				}
				this._overlayQuickTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "QuickConversation").ToString(), flag2, GameMenuOverlay.MenuOverlayContextList.QuickConversation, textObject2);
				base.ContextList.Add(this._overlayQuickTalkItem);
				foreach (QuestMarkerVM questMarkerVM in troop.Quests)
				{
					if (questMarkerVM.IssueQuestFlag != CampaignUIHelper.IssueQuestFlags.None)
					{
						GameTexts.SetVariable("STR2", questMarkerVM.QuestTitle);
						string text = string.Empty;
						if (questMarkerVM.IssueQuestFlag == CampaignUIHelper.IssueQuestFlags.ActiveIssue)
						{
							text = "{=!}<img src=\"General\\Icons\\icon_issue_active_square\" extend=\"4\">";
						}
						else if (questMarkerVM.IssueQuestFlag == CampaignUIHelper.IssueQuestFlags.AvailableIssue)
						{
							text = "{=!}<img src=\"General\\Icons\\icon_issue_available_square\" extend=\"4\">";
						}
						else if (questMarkerVM.IssueQuestFlag == CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest)
						{
							text = "{=!}<img src=\"General\\Icons\\icon_story_quest_active_square\" extend=\"4\">";
						}
						else if (questMarkerVM.IssueQuestFlag == CampaignUIHelper.IssueQuestFlags.TrackedIssue)
						{
							text = "{=!}<img src=\"General\\Icons\\issue_target_icon\" extend=\"4\">";
						}
						else if (questMarkerVM.IssueQuestFlag == CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest)
						{
							text = "{=!}<img src=\"General\\Icons\\quest_target_icon\" extend=\"4\">";
						}
						GameTexts.SetVariable("STR1", text);
						string text2 = GameTexts.FindText("str_STR1_STR2", null).ToString();
						this.IssueList.Add(new StringItemWithHintVM(text2, questMarkerVM.QuestHint.HintText));
					}
				}
				if (this._contextMenuItem.Character.IsHero)
				{
					MobileParty partyBelongedTo = this._contextMenuItem.Character.HeroObject.PartyBelongedTo;
					if (((partyBelongedTo != null) ? partyBelongedTo.Army : null) != null && this._contextMenuItem.Character.HeroObject.PartyBelongedTo.Army.LeaderParty == this._contextMenuItem.Character.HeroObject.PartyBelongedTo && MobileParty.MainParty.Army == null && FactionManager.IsAlliedWithFaction(this._contextMenuItem.Character.HeroObject.MapFaction, Hero.MainHero.MapFaction))
					{
						GameMenuOverlayActionVM gameMenuOverlayActionVM = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "JoinArmy").ToString(), true, GameMenuOverlay.MenuOverlayContextList.JoinArmy, null);
						base.ContextList.Add(gameMenuOverlayActionVM);
					}
				}
				if (this._contextMenuItem.Character.IsHero && this._contextMenuItem.Character.HeroObject.PartyBelongedTo == null && this._contextMenuItem.Character.HeroObject.Clan == Clan.PlayerClan && this._contextMenuItem.Character.HeroObject.Age > (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && !Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>().IsHeroAlleyLeaderOfAnyPlayerAlley(this._contextMenuItem.Character.HeroObject))
				{
					GameMenuOverlayActionVM gameMenuOverlayActionVM2 = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "TakeToParty").ToString(), true, GameMenuOverlay.MenuOverlayContextList.TakeToParty, null);
					base.ContextList.Add(gameMenuOverlayActionVM2);
				}
				CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpOpened(this._contextMenuItem.Character);
				return;
			}
			if (this._contextMenuItem.Party != null)
			{
				Hero owner = this._contextMenuItem.Party.Owner;
				if (((owner != null) ? owner.Clan : null) == Hero.MainHero.Clan)
				{
					MobileParty mobileParty = this._contextMenuItem.Party.MobileParty;
					if (mobileParty != null && !mobileParty.IsMainParty)
					{
						MobileParty mobileParty2 = this._contextMenuItem.Party.MobileParty;
						if (mobileParty2 != null && mobileParty2.IsGarrison)
						{
							this._overlayTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "ManageGarrison").ToString(), true, GameMenuOverlay.MenuOverlayContextList.ManageGarrison, null);
							base.ContextList.Add(this._overlayTalkItem);
							goto IL_65B;
						}
					}
				}
				if (this._contextMenuItem.Party.MapFaction == Hero.MainHero.MapFaction)
				{
					MobileParty mobileParty3 = this._contextMenuItem.Party.MobileParty;
					if (mobileParty3 != null && !mobileParty3.IsMainParty && (this._contextMenuItem.Party.MobileParty == null || (!this._contextMenuItem.Party.MobileParty.IsVillager && !this._contextMenuItem.Party.MobileParty.IsCaravan && !this._contextMenuItem.Party.MobileParty.IsMilitia)))
					{
						if (this._contextMenuItem.Party.MobileParty.ActualClan == Clan.PlayerClan)
						{
							this._overlayTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "ManageTroops").ToString(), true, GameMenuOverlay.MenuOverlayContextList.ManageTroops, null);
							base.ContextList.Add(this._overlayTalkItem);
						}
						else
						{
							this._overlayTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "DonateTroops").ToString(), true, GameMenuOverlay.MenuOverlayContextList.DonateTroops, null);
							base.ContextList.Add(this._overlayTalkItem);
						}
					}
				}
				IL_65B:
				if (this._contextMenuItem.Party.LeaderHero != null && this._contextMenuItem.Party.LeaderHero != Hero.MainHero)
				{
					bool flag3 = this.CharacterList.Any((GameMenuPartyItemVM c) => c.Character == this._contextMenuItem.Party.LeaderHero.CharacterObject);
					TextObject textObject3 = ((!flag3) ? GameTexts.FindText("str_menu_overlay_cant_talk_to_party_leader", null) : TextObject.Empty);
					base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "ConverseWithLeader").ToString(), flag3, GameMenuOverlay.MenuOverlayContextList.ConverseWithLeader, textObject3));
				}
				CharacterObject visualPartyLeader = CampaignUIHelper.GetVisualPartyLeader(this._contextMenuItem.Party);
				if (visualPartyLeader != null)
				{
					CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpOpened(visualPartyLeader);
				}
			}
		}

		private void OnSettlementOverlayTalkPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayTalkPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		private void OnSettlementOverlayQuickTalkPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayQuickTalkPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		private void OnSettlementOverlayLeaveCharacterPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayLeaveCharacterPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		public override void ExecuteOnOverlayClosed()
		{
			base.ExecuteOnOverlayClosed();
			this.InitLists();
			base.ContextList.Clear();
		}

		private void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		private void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[]
			{
				Settlement.CurrentSettlement,
				true
			});
		}

		private void ExecuteSettlementLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Settlement.CurrentSettlement.EncyclopediaLink);
		}

		private bool Contains(MBBindingList<GameMenuPartyItemVM> list, CharacterObject character)
		{
			using (IEnumerator<GameMenuPartyItemVM> enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Character == character)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void UpdateOverlayType(GameOverlays.MenuOverlayType newType)
		{
			this._type = newType;
			base.UpdateOverlayType(newType);
		}

		private void InitLists()
		{
			this.InitCharacterList();
			this.InitPartyList();
		}

		private void InitCharacterList()
		{
			if (this._type == GameOverlays.MenuOverlayType.SettlementWithCharacters || this._type == GameOverlays.MenuOverlayType.SettlementWithBoth)
			{
				Dictionary<Hero, bool> heroesInSettlement = new Dictionary<Hero, bool>();
				foreach (Location location in Campaign.Current.GameMenuManager.MenuLocations)
				{
					foreach (LocationCharacter locationCharacter in location.GetCharacterList())
					{
						if (this.WillBeListed(locationCharacter) && !heroesInSettlement.ContainsKey(locationCharacter.Character.HeroObject))
						{
							heroesInSettlement.Add(locationCharacter.Character.HeroObject, locationCharacter.UseCivilianEquipment);
						}
					}
				}
				this.UpdateList<GameMenuPartyItemVM, Hero>(this.CharacterList, heroesInSettlement.Keys, new SettlementMenuOverlayVM.CharacterComparer(), delegate(GameMenuPartyItemVM x)
				{
					if (x == null)
					{
						return null;
					}
					CharacterObject character = x.Character;
					if (character == null)
					{
						return null;
					}
					return character.HeroObject;
				}, (Hero x) => heroesInSettlement.ContainsKey(x), (Hero x) => new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), x.CharacterObject, heroesInSettlement[x]));
				return;
			}
			this.CharacterList.Clear();
		}

		private void InitPartyList()
		{
			if (this._type == GameOverlays.MenuOverlayType.SettlementWithBoth || this._type == GameOverlays.MenuOverlayType.SettlementWithParties)
			{
				Settlement settlement = MobileParty.MainParty.CurrentSettlement ?? MobileParty.MainParty.LastVisitedSettlement;
				HashSet<MobileParty> partiesInSettlement = new HashSet<MobileParty>();
				foreach (MobileParty mobileParty in settlement.Parties)
				{
					if (this.WillBeListed(mobileParty))
					{
						partiesInSettlement.Add(mobileParty);
					}
				}
				this.UpdateList<GameMenuPartyItemVM, MobileParty>(this.PartyList, partiesInSettlement, new SettlementMenuOverlayVM.PartyComparer(), delegate(GameMenuPartyItemVM x)
				{
					if (x == null)
					{
						return null;
					}
					PartyBase party = x.Party;
					if (party == null)
					{
						return null;
					}
					return party.MobileParty;
				}, (MobileParty x) => partiesInSettlement.Contains(x), (MobileParty x) => new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), x.Party, false));
				return;
			}
			this.PartyList.Clear();
		}

		private void UpdateList<TListItem, TElement>(MBBindingList<TListItem> listToUpdate, IEnumerable<TElement> listInSettlement, IComparer<TListItem> comparer, Func<TListItem, TElement> getElementFromListItem, Func<TElement, bool> doesSettlementHasElement, Func<TElement, TListItem> createListItem)
		{
			HashSet<TElement> hashSet = new HashSet<TElement>();
			for (int i = 0; i < listToUpdate.Count; i++)
			{
				TListItem tlistItem = listToUpdate[i];
				TElement telement = getElementFromListItem(tlistItem);
				if (doesSettlementHasElement(telement))
				{
					hashSet.Add(telement);
				}
				else
				{
					listToUpdate.RemoveAt(i);
					i--;
				}
			}
			foreach (TElement telement2 in listInSettlement)
			{
				if (!hashSet.Contains(telement2))
				{
					listToUpdate.Add(createListItem(telement2));
					hashSet.Add(telement2);
				}
			}
			listToUpdate.Sort(comparer);
		}

		private bool WillBeListed(MobileParty mobileParty)
		{
			return mobileParty != null && mobileParty.IsActive;
		}

		private bool WillBeListed(LocationCharacter locationCharacter)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			return locationCharacter.Character.IsHero && !locationCharacter.IsHidden && locationCharacter.Character.HeroObject.PartyBelongedTo != MobileParty.MainParty && locationCharacter.Character.HeroObject.CurrentSettlement == settlement;
		}

		private bool WillBeListed(CharacterObject character)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			return character.IsHero && character.HeroObject.PartyBelongedTo != MobileParty.MainParty && character.HeroObject.CurrentSettlement == settlement;
		}

		private void UpdateSettlementOwnerBanner()
		{
			Banner banner = null;
			IFaction mapFaction = Settlement.CurrentSettlement.MapFaction;
			if (mapFaction != null && mapFaction.IsKingdomFaction && ((Kingdom)Settlement.CurrentSettlement.MapFaction).RulingClan == Settlement.CurrentSettlement.OwnerClan)
			{
				banner = Settlement.CurrentSettlement.OwnerClan.Kingdom.Banner;
			}
			else
			{
				Clan ownerClan = Settlement.CurrentSettlement.OwnerClan;
				if (((ownerClan != null) ? ownerClan.Banner : null) != null)
				{
					banner = Settlement.CurrentSettlement.OwnerClan.Banner;
				}
			}
			if (banner != null)
			{
				this.SettlementOwnerBanner = new ImageIdentifierVM(BannerCode.CreateFrom(banner), true);
				return;
			}
			this.SettlementOwnerBanner = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		private void UpdateProperties()
		{
			Settlement currentSettlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			this.IsFortification = currentSettlement.IsFortification;
			IFaction mapFaction = currentSettlement.MapFaction;
			this.IsCrimeEnabled = mapFaction != null && mapFaction.MainHeroCrimeRating > 0f;
			IFaction mapFaction2 = currentSettlement.MapFaction;
			this.CrimeLbl = ((int)((mapFaction2 != null) ? new float?(mapFaction2.MainHeroCrimeRating) : null).Value).ToString();
			IFaction mapFaction3 = currentSettlement.MapFaction;
			this.CrimeChangeAmount = (int)((mapFaction3 != null) ? new float?(mapFaction3.DailyCrimeRatingChange) : null).Value;
			this.RemainingFoodText = (currentSettlement.IsFortification ? ((int)currentSettlement.Town.FoodStocks).ToString() : "-");
			this.FoodChangeAmount = ((currentSettlement.Town != null) ? ((int)currentSettlement.Town.FoodChange) : 0);
			this.MilitasLbl = ((int)currentSettlement.Militia).ToString();
			this.MilitiaChangeAmount = ((currentSettlement.Town != null) ? ((int)currentSettlement.Town.MilitiaChange) : ((int)currentSettlement.Village.MilitiaChange));
			this.IsLoyaltyRebellionWarning = currentSettlement.IsTown && currentSettlement.Town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
			if (currentSettlement.IsFortification)
			{
				MobileParty garrisonParty = currentSettlement.Town.GarrisonParty;
				this.GarrisonAmount = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers : 0);
				this.IsNoGarrisonWarning = this.GarrisonAmount < 1;
			}
			if (currentSettlement.IsFortification)
			{
				MobileParty garrisonParty2 = currentSettlement.Town.GarrisonParty;
				this.GarrisonLbl = ((garrisonParty2 != null) ? garrisonParty2.Party.NumberOfAllMembers.ToString() : null) ?? "0";
				this.GarrisonChangeAmount = currentSettlement.Town.GarrisonChange;
				this.WallsLbl = currentSettlement.Town.GetWallLevel().ToString();
				this.WallsLevel = currentSettlement.Town.GetWallLevel();
			}
			else
			{
				this.GarrisonChangeAmount = 0;
				this.WallsLbl = "-";
				this.GarrisonLbl = "-";
				this.WallsLevel = 1;
			}
			if (currentSettlement.IsFortification)
			{
				this.ProsperityLbl = ((int)currentSettlement.Town.Prosperity).ToString();
				this.ProsperityChangeAmount = (int)currentSettlement.Town.ProsperityChange;
			}
			else
			{
				this.ProsperityLbl = ((int)currentSettlement.Village.Hearth).ToString();
				this.ProsperityChangeAmount = (int)currentSettlement.Village.HearthChange;
			}
			this.SettlementNameLbl = currentSettlement.Name + ((currentSettlement.IsVillage && currentSettlement.Village.VillageState != Village.VillageStates.Normal) ? ("(" + currentSettlement.Village.VillageState.ToString() + ")") : "");
			if (currentSettlement.IsFortification)
			{
				this.LoyaltyLbl = ((int)currentSettlement.Town.Loyalty).ToString();
				this.LoyaltyChangeAmount = (int)currentSettlement.Town.LoyaltyChange;
				this.SecurityLbl = ((int)currentSettlement.Town.Security).ToString();
				this.SecurityChangeAmount = (int)currentSettlement.Town.SecurityChange;
			}
			else
			{
				this.LoyaltyChangeAmount = 0;
				this.LoyaltyLbl = "-";
				this.SecurityChangeAmount = 0;
				this.SecurityLbl = "-";
			}
			Game.Current.EventManager.TriggerEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(new SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent(new Action<bool, TextObject>(this.OnSettlementOverlayLeaveCharacterPermissionResult)));
			if (this._mostRecentOverlayLeaveCharacterPermission != null)
			{
				this.CanLeaveMembers = this._mostRecentOverlayLeaveCharacterPermission.Item1;
				this.LeaveMembersHint = (this.CanLeaveMembers ? new HintViewModel(new TextObject("{=aGFxIvqx}Leave Member(s)", null), null) : new HintViewModel(this._mostRecentOverlayLeaveCharacterPermission.Item2, null));
				return;
			}
			this.CanLeaveMembers = Clan.PlayerClan.Heroes.Any((Hero hero) => currentSettlement == hero.StayingInSettlement || (!hero.CharacterObject.IsPlayerCharacter && MobileParty.MainParty.MemberRoster.Contains(hero.CharacterObject)));
			if (!this.CanLeaveMembers)
			{
				this.LeaveMembersHint = new HintViewModel(new TextObject("{=d2K6gMsZ}Leave members. Need at least 1 companion.", null), null);
				return;
			}
			this.LeaveMembersHint = new HintViewModel(new TextObject("{=aGFxIvqx}Leave Member(s)", null), null);
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			this._latestTutorialElementID = obj.NewNotificationElementID;
			if (this._latestTutorialElementID != null)
			{
				if (this._latestTutorialElementID != "")
				{
					if (this._latestTutorialElementID == "ApplicapleCompanion" && !this._isCompanionHighlightApplied)
					{
						this._isCompanionHighlightApplied = this.SetPartyItemHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "ApplicapleCompanion" && this._isCompanionHighlightApplied)
					{
						this._isCompanionHighlightApplied = this.SetPartyItemHighlightState("ApplicapleCompanion", false);
					}
					if (this._latestTutorialElementID == "ApplicableQuestGivers" && !this._isQuestGiversHighlightApplied)
					{
						this._isQuestGiversHighlightApplied = this.SetPartyItemHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "ApplicableQuestGivers" && this._isQuestGiversHighlightApplied)
					{
						this._isCompanionHighlightApplied = this.SetPartyItemHighlightState("ApplicableQuestGivers", false);
					}
					if (this._latestTutorialElementID == "ApplicableNotable" && !this._isNotableHighlightApplied)
					{
						this._isNotableHighlightApplied = this.SetPartyItemHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "ApplicableNotable" && this._isNotableHighlightApplied)
					{
						this._isNotableHighlightApplied = this.SetPartyItemHighlightState("ApplicableNotable", false);
					}
					if (this._latestTutorialElementID == "CrimeLabel" && !this.IsCrimeLabelHighlightEnabled)
					{
						this.IsCrimeLabelHighlightEnabled = true;
					}
					else if (this._latestTutorialElementID != "CrimeLabel" && this.IsCrimeLabelHighlightEnabled)
					{
						this.IsCrimeLabelHighlightEnabled = false;
					}
					if (this._latestTutorialElementID == "OverlayTalkButton" && !this._isTalkItemHighlightApplied)
					{
						if (this._overlayTalkItem != null)
						{
							this._overlayTalkItem.IsHiglightEnabled = true;
							this._isTalkItemHighlightApplied = true;
							return;
						}
					}
					else if (this._latestTutorialElementID != "OverlayTalkButton" && this._isTalkItemHighlightApplied && this._overlayTalkItem != null)
					{
						this._overlayTalkItem.IsHiglightEnabled = false;
						this._isTalkItemHighlightApplied = true;
						return;
					}
				}
				else
				{
					if (this._isCompanionHighlightApplied)
					{
						this._isCompanionHighlightApplied = !this.SetPartyItemHighlightState("ApplicapleCompanion", false);
					}
					if (this._isNotableHighlightApplied)
					{
						this._isNotableHighlightApplied = !this.SetPartyItemHighlightState("ApplicableNotable", false);
					}
					if (this._isQuestGiversHighlightApplied)
					{
						this._isQuestGiversHighlightApplied = !this.SetPartyItemHighlightState("ApplicableQuestGivers", false);
					}
					if (this.IsCrimeLabelHighlightEnabled)
					{
						this.IsCrimeLabelHighlightEnabled = false;
					}
					if (this._isTalkItemHighlightApplied && this._overlayTalkItem != null)
					{
						this._overlayTalkItem.IsHiglightEnabled = false;
						this._isTalkItemHighlightApplied = false;
						return;
					}
				}
			}
			else
			{
				if (this._isCompanionHighlightApplied)
				{
					this._isCompanionHighlightApplied = !this.SetPartyItemHighlightState("ApplicapleCompanion", false);
				}
				if (this._isNotableHighlightApplied)
				{
					this._isNotableHighlightApplied = !this.SetPartyItemHighlightState("ApplicableNotable", false);
				}
				if (this._isQuestGiversHighlightApplied)
				{
					this._isQuestGiversHighlightApplied = !this.SetPartyItemHighlightState("ApplicableQuestGivers", false);
				}
				if (this._isTalkItemHighlightApplied && this._overlayTalkItem != null)
				{
					this._overlayTalkItem.IsHiglightEnabled = false;
					this._isTalkItemHighlightApplied = false;
				}
				if (this.IsCrimeLabelHighlightEnabled)
				{
					this.IsCrimeLabelHighlightEnabled = false;
				}
			}
		}

		private bool SetPartyItemHighlightState(string condition, bool state)
		{
			bool flag = false;
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM in this.CharacterList)
			{
				if (condition == "ApplicapleCompanion" && gameMenuPartyItemVM.Character.IsHero && gameMenuPartyItemVM.Character.HeroObject.IsWanderer && !gameMenuPartyItemVM.Character.HeroObject.IsPlayerCompanion)
				{
					gameMenuPartyItemVM.IsHighlightEnabled = state;
					flag = true;
				}
				else if (condition == "ApplicableNotable" && gameMenuPartyItemVM.Character.IsHero && gameMenuPartyItemVM.Character.HeroObject.IsNotable && !gameMenuPartyItemVM.Character.HeroObject.IsPlayerCompanion)
				{
					gameMenuPartyItemVM.IsHighlightEnabled = state;
					flag = true;
				}
			}
			return flag;
		}

		public override void Refresh()
		{
			base.IsInitializationOver = false;
			this.InitLists();
			this.UpdateProperties();
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM in this.CharacterList)
			{
				gameMenuPartyItemVM.RefreshProperties();
			}
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM2 in this.PartyList)
			{
				gameMenuPartyItemVM2.RefreshProperties();
			}
			base.IsInitializationOver = true;
			base.Refresh();
		}

		public void ExecuteAddCompanion()
		{
			List<InquiryElement> list = new List<InquiryElement>();
			foreach (TroopRosterElement troopRosterElement in from m in MobileParty.MainParty.MemberRoster.GetTroopRoster()
				where m.Character.IsHero && m.Character.HeroObject.CanMoveToSettlement()
				select m)
			{
				if (!troopRosterElement.Character.IsPlayerCharacter)
				{
					list.Add(new InquiryElement(troopRosterElement.Character.HeroObject, troopRosterElement.Character.Name.ToString(), new ImageIdentifier(CampaignUIHelper.GetCharacterCode(troopRosterElement.Character, false))));
				}
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=aGFxIvqx}Leave Member(s)", null).ToString(), string.Empty, list, true, 1, 0, new TextObject("{=FBYFcrWo}Leave in settlement", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action<List<InquiryElement>>(this.OnLeaveMembersInSettlement), new Action<List<InquiryElement>>(this.OnLeaveMembersInSettlement), ""), false, false);
		}

		private void OnLeaveMembersInSettlement(List<InquiryElement> leftMembers)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			foreach (InquiryElement inquiryElement in leftMembers)
			{
				Hero hero = inquiryElement.Identifier as Hero;
				PartyBase.MainParty.MemberRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				if (hero.CharacterObject.IsHero && !settlement.HeroesWithoutParty.Contains(hero.CharacterObject.HeroObject))
				{
					EnterSettlementAction.ApplyForCharacterOnly(hero.CharacterObject.HeroObject, settlement);
				}
			}
			if (leftMembers.Count > 0)
			{
				this.InitLists();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.AfterSettlementEntered.ClearListeners(this);
			CampaignEvents.OnSettlementLeftEvent.ClearListeners(this);
			CampaignEvents.OnQuestCompletedEvent.ClearListeners(this);
			CampaignEvents.WarDeclared.ClearListeners(this);
			CampaignEvents.MakePeace.ClearListeners(this);
			CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
			CampaignEvents.TownRebelliosStateChanged.ClearListeners(this);
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		private void OnSettlementEntered(MobileParty arg1, Settlement arg2, Hero arg3)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			if (arg2 == settlement)
			{
				this.InitLists();
			}
		}

		private void OnSettlementLeft(MobileParty arg1, Settlement arg2)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			if (arg2 == settlement)
			{
				this.InitLists();
			}
		}

		private void OnQuestCompleted(QuestBase arg1, QuestBase.QuestCompleteDetails arg2)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			Hero questGiver = arg1.QuestGiver;
			if (((questGiver != null) ? questGiver.CurrentSettlement : null) != null && arg1.QuestGiver.CurrentSettlement == settlement)
			{
				this.Refresh();
			}
		}

		private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail reason)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		private void OnPeaceOrWarDeclared(IFaction arg1, IFaction arg2)
		{
			Hero mainHero = Hero.MainHero;
			bool flag;
			if (mainHero == null)
			{
				flag = null != null;
			}
			else
			{
				Settlement currentSettlement = mainHero.CurrentSettlement;
				flag = ((currentSettlement != null) ? currentSettlement.MapFaction : null) != null;
			}
			bool flag2;
			if (flag)
			{
				Hero mainHero2 = Hero.MainHero;
				if (((mainHero2 != null) ? mainHero2.CurrentSettlement.MapFaction : null) != arg1)
				{
					Hero mainHero3 = Hero.MainHero;
					flag2 = ((mainHero3 != null) ? mainHero3.CurrentSettlement.MapFaction : null) == arg2;
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = false;
			}
			Hero mainHero4 = Hero.MainHero;
			bool flag3;
			if (((mainHero4 != null) ? mainHero4.MapFaction : null) != arg1)
			{
				Hero mainHero5 = Hero.MainHero;
				flag3 = ((mainHero5 != null) ? mainHero5.MapFaction : null) == arg2;
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			if (flag2 || flag4)
			{
				this.InitLists();
			}
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero previousOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (Settlement.CurrentSettlement == null)
			{
				return;
			}
			if (settlement == Settlement.CurrentSettlement || (Settlement.CurrentSettlement.IsVillage && settlement.BoundVillages.Contains(Settlement.CurrentSettlement.Village)))
			{
				this.UpdateSettlementOwnerBanner();
			}
		}

		private void OnTownRebelliousStateChanged(Town town, bool isRebellious)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.Town == town)
			{
				this.IsLoyaltyRebellionWarning = isRebellious || town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
			}
		}

		[DataSourceProperty]
		public string RemainingFoodText
		{
			get
			{
				return this._remainingFoodText;
			}
			set
			{
				if (value != this._remainingFoodText)
				{
					this._remainingFoodText = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingFoodText");
				}
			}
		}

		[DataSourceProperty]
		public int ProsperityChangeAmount
		{
			get
			{
				return this._prosperityChangeAmount;
			}
			set
			{
				if (value != this._prosperityChangeAmount)
				{
					this._prosperityChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "ProsperityChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int MilitiaChangeAmount
		{
			get
			{
				return this._militiaChangeAmount;
			}
			set
			{
				if (value != this._militiaChangeAmount)
				{
					this._militiaChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "MilitiaChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int GarrisonChangeAmount
		{
			get
			{
				return this._garrisonChangeAmount;
			}
			set
			{
				if (value != this._garrisonChangeAmount)
				{
					this._garrisonChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "GarrisonChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int GarrisonAmount
		{
			get
			{
				return this._garrisonAmount;
			}
			set
			{
				if (value != this._garrisonAmount)
				{
					this._garrisonAmount = value;
					base.OnPropertyChangedWithValue(value, "GarrisonAmount");
				}
			}
		}

		[DataSourceProperty]
		public int CrimeChangeAmount
		{
			get
			{
				return this._crimeChangeAmount;
			}
			set
			{
				if (value != this._crimeChangeAmount)
				{
					this._crimeChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "CrimeChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int LoyaltyChangeAmount
		{
			get
			{
				return this._loyaltyChangeAmount;
			}
			set
			{
				if (value != this._loyaltyChangeAmount)
				{
					this._loyaltyChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "LoyaltyChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int SecurityChangeAmount
		{
			get
			{
				return this._securityChangeAmount;
			}
			set
			{
				if (value != this._securityChangeAmount)
				{
					this._securityChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "SecurityChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public int FoodChangeAmount
		{
			get
			{
				return this._foodChangeAmount;
			}
			set
			{
				if (value != this._foodChangeAmount)
				{
					this._foodChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "FoodChangeAmount");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel RemainingFoodHint
		{
			get
			{
				return this._remainingFoodHint;
			}
			set
			{
				if (value != this._remainingFoodHint)
				{
					this._remainingFoodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "RemainingFoodHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SecurityHint
		{
			get
			{
				return this._securityHint;
			}
			set
			{
				if (value != this._securityHint)
				{
					this._securityHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SecurityHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PartyFilterHint
		{
			get
			{
				return this._partyFilterHint;
			}
			set
			{
				if (value != this._partyFilterHint)
				{
					this._partyFilterHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PartyFilterHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CharacterFilterHint
		{
			get
			{
				return this._characterFilterHint;
			}
			set
			{
				if (value != this._characterFilterHint)
				{
					this._characterFilterHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CharacterFilterHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel MilitasHint
		{
			get
			{
				return this._militasHint;
			}
			set
			{
				if (value != this._militasHint)
				{
					this._militasHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MilitasHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel GarrisonHint
		{
			get
			{
				return this._garrisonHint;
			}
			set
			{
				if (value != this._garrisonHint)
				{
					this._garrisonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GarrisonHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel ProsperityHint
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ProsperityHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel LoyaltyHint
		{
			get
			{
				return this._loyaltyHint;
			}
			set
			{
				if (value != this._loyaltyHint)
				{
					this._loyaltyHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LoyaltyHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel WallsHint
		{
			get
			{
				return this._wallsHint;
			}
			set
			{
				if (value != this._wallsHint)
				{
					this._wallsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "WallsHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel CrimeHint
		{
			get
			{
				return this._crimeHint;
			}
			set
			{
				if (value != this._crimeHint)
				{
					this._crimeHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CrimeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LeaveMembersHint
		{
			get
			{
				return this._leaveMembersHint;
			}
			set
			{
				if (value != this._leaveMembersHint)
				{
					this._leaveMembersHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LeaveMembersHint");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM SettlementOwnerBanner
		{
			get
			{
				return this._settlementOwnerBanner;
			}
			set
			{
				if (value != this._settlementOwnerBanner)
				{
					this._settlementOwnerBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "SettlementOwnerBanner");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameMenuPartyItemVM> CharacterList
		{
			get
			{
				return this._characterList;
			}
			set
			{
				if (value != this._characterList)
				{
					this._characterList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPartyItemVM>>(value, "CharacterList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameMenuPartyItemVM> PartyList
		{
			get
			{
				return this._partyList;
			}
			set
			{
				if (value != this._partyList)
				{
					this._partyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPartyItemVM>>(value, "PartyList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringItemWithHintVM> IssueList
		{
			get
			{
				return this._issueList;
			}
			set
			{
				if (value != this._issueList)
				{
					this._issueList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithHintVM>>(value, "IssueList");
				}
			}
		}

		[DataSourceProperty]
		public string MilitasLbl
		{
			get
			{
				return this._militasLbl;
			}
			set
			{
				if (value != this._militasLbl)
				{
					this._militasLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "MilitasLbl");
				}
			}
		}

		[DataSourceProperty]
		public string GarrisonLbl
		{
			get
			{
				return this._garrisonLbl;
			}
			set
			{
				if (value != this._garrisonLbl)
				{
					this._garrisonLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "GarrisonLbl");
				}
			}
		}

		[DataSourceProperty]
		public string CrimeLbl
		{
			get
			{
				return this._crimeLbl;
			}
			set
			{
				if (value != this._crimeLbl)
				{
					this._crimeLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CrimeLbl");
				}
			}
		}

		[DataSourceProperty]
		public bool CanLeaveMembers
		{
			get
			{
				return this._canLeaveMembers;
			}
			set
			{
				if (value != this._canLeaveMembers)
				{
					this._canLeaveMembers = value;
					base.OnPropertyChangedWithValue(value, "CanLeaveMembers");
				}
			}
		}

		[DataSourceProperty]
		public string ProsperityLbl
		{
			get
			{
				return this._prosperityLbl;
			}
			set
			{
				if (value != this._prosperityLbl)
				{
					this._prosperityLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ProsperityLbl");
				}
			}
		}

		[DataSourceProperty]
		public string LoyaltyLbl
		{
			get
			{
				return this._loyaltyLbl;
			}
			set
			{
				if (value != this._loyaltyLbl)
				{
					this._loyaltyLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "LoyaltyLbl");
				}
			}
		}

		[DataSourceProperty]
		public string SecurityLbl
		{
			get
			{
				return this._securityLbl;
			}
			set
			{
				if (value != this._securityLbl)
				{
					this._securityLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "SecurityLbl");
				}
			}
		}

		[DataSourceProperty]
		public string WallsLbl
		{
			get
			{
				return this._wallsLbl;
			}
			set
			{
				if (value != this._wallsLbl)
				{
					this._wallsLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "WallsLbl");
				}
			}
		}

		[DataSourceProperty]
		public int WallsLevel
		{
			get
			{
				return this._wallsLevel;
			}
			set
			{
				if (value != this._wallsLevel)
				{
					this._wallsLevel = value;
					base.OnPropertyChangedWithValue(value, "WallsLevel");
				}
			}
		}

		[DataSourceProperty]
		public string SettlementNameLbl
		{
			get
			{
				return this._settlementNameLbl;
			}
			set
			{
				if (value != this._settlementNameLbl)
				{
					this._settlementNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementNameLbl");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFortification
		{
			get
			{
				return this._isFortification;
			}
			set
			{
				if (value != this._isFortification)
				{
					this._isFortification = value;
					base.OnPropertyChangedWithValue(value, "IsFortification");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCrimeEnabled
		{
			get
			{
				return this._isCrimeEnabled;
			}
			set
			{
				if (value != this._isCrimeEnabled)
				{
					this._isCrimeEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCrimeEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNoGarrisonWarning
		{
			get
			{
				return this._isNoGarrisonWarning;
			}
			set
			{
				if (value != this._isNoGarrisonWarning)
				{
					this._isNoGarrisonWarning = value;
					base.OnPropertyChangedWithValue(value, "IsNoGarrisonWarning");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCrimeLabelHighlightEnabled
		{
			get
			{
				return this._isCrimeLabelHighlightEnabled;
			}
			set
			{
				if (value != this._isCrimeLabelHighlightEnabled)
				{
					this._isCrimeLabelHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCrimeLabelHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLoyaltyRebellionWarning
		{
			get
			{
				return this._isLoyaltyRebellionWarning;
			}
			set
			{
				if (value != this._isLoyaltyRebellionWarning)
				{
					this._isLoyaltyRebellionWarning = value;
					base.OnPropertyChangedWithValue(value, "IsLoyaltyRebellionWarning");
				}
			}
		}

		private readonly SettlementComponent _settlementComponent;

		private GameOverlays.MenuOverlayType _type;

		private GameMenuOverlayActionVM _overlayTalkItem;

		private GameMenuOverlayActionVM _overlayQuickTalkItem;

		private Tuple<bool, TextObject> _mostRecentOverlayTalkPermission;

		private Tuple<bool, TextObject> _mostRecentOverlayQuickTalkPermission;

		private Tuple<bool, TextObject> _mostRecentOverlayLeaveCharacterPermission;

		private string _latestTutorialElementID;

		private bool _isCompanionHighlightApplied;

		private bool _isQuestGiversHighlightApplied;

		private bool _isNotableHighlightApplied;

		private bool _isTalkItemHighlightApplied;

		private string _militasLbl;

		private string _garrisonLbl;

		private bool _isNoGarrisonWarning;

		private bool _isLoyaltyRebellionWarning;

		private bool _isCrimeLabelHighlightEnabled;

		private string _crimeLbl;

		private string _prosperityLbl;

		private string _loyaltyLbl;

		private string _securityLbl;

		private string _wallsLbl;

		private string _settlementNameLbl;

		private string _remainingFoodText = "";

		private int _wallsLevel;

		private int _prosperityChangeAmount;

		private int _militiaChangeAmount;

		private int _garrisonChangeAmount;

		private int _garrisonAmount;

		private int _crimeChangeAmount;

		private int _loyaltyChangeAmount;

		private int _securityChangeAmount;

		private int _foodChangeAmount;

		private MBBindingList<GameMenuPartyItemVM> _characterList;

		private MBBindingList<GameMenuPartyItemVM> _partyList;

		private MBBindingList<StringItemWithHintVM> _issueList;

		private bool _isFortification;

		private bool _isCrimeEnabled;

		private bool _canLeaveMembers;

		private BasicTooltipViewModel _remainingFoodHint;

		private BasicTooltipViewModel _militasHint;

		private BasicTooltipViewModel _garrisonHint;

		private BasicTooltipViewModel _prosperityHint;

		private BasicTooltipViewModel _loyaltyHint;

		private BasicTooltipViewModel _securityHint;

		private BasicTooltipViewModel _wallsHint;

		private BasicTooltipViewModel _crimeHint;

		private HintViewModel _characterFilterHint;

		private HintViewModel _partyFilterHint;

		private HintViewModel _leaveMembersHint;

		private ImageIdentifierVM _settlementOwnerBanner;

		public class SettlementOverlayTalkPermissionEvent : EventBase
		{
			public Action<bool, TextObject> IsTalkAvailable { get; private set; }

			public SettlementOverlayTalkPermissionEvent(Hero heroToTalkTo, Action<bool, TextObject> isTalkAvailable)
			{
				this.HeroToTalkTo = heroToTalkTo;
				this.IsTalkAvailable = isTalkAvailable;
			}

			public Hero HeroToTalkTo;
		}

		public class SettlementOverylayQuickTalkPermissionEvent : EventBase
		{
			public Action<bool, TextObject> IsTalkAvailable { get; private set; }

			public SettlementOverylayQuickTalkPermissionEvent(Action<bool, TextObject> isTalkAvailable)
			{
				this.IsTalkAvailable = isTalkAvailable;
			}
		}

		public class SettlementOverlayLeaveCharacterPermissionEvent : EventBase
		{
			public Action<bool, TextObject> IsLeaveAvailable { get; private set; }

			public SettlementOverlayLeaveCharacterPermissionEvent(Action<bool, TextObject> isLeaveAvailable)
			{
				this.IsLeaveAvailable = isLeaveAvailable;
			}
		}

		public class CrimeValueInspectedInSettlementOverlayEvent : EventBase
		{
		}

		private class CharacterComparer : IComparer<GameMenuPartyItemVM>
		{
			public int Compare(GameMenuPartyItemVM x, GameMenuPartyItemVM y)
			{
				return CampaignUIHelper.GetHeroCompareSortIndex(x.Character.HeroObject, y.Character.HeroObject);
			}
		}

		private class PartyComparer : IComparer<GameMenuPartyItemVM>
		{
			public int Compare(GameMenuPartyItemVM x, GameMenuPartyItemVM y)
			{
				return CampaignUIHelper.MobilePartyPrecedenceComparerInstance.Compare(x.Party.MobileParty, y.Party.MobileParty);
			}
		}
	}
}
