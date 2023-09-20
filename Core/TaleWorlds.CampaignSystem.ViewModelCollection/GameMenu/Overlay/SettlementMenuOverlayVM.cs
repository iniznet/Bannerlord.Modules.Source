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
	// Token: 0x020000A5 RID: 165
	[MenuOverlay("SettlementMenuOverlay")]
	public class SettlementMenuOverlayVM : GameMenuOverlay
	{
		// Token: 0x0600105E RID: 4190 RVA: 0x00040BFC File Offset: 0x0003EDFC
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

		// Token: 0x0600105F RID: 4191 RVA: 0x00040F31 File Offset: 0x0003F131
		private List<TooltipProperty> GetCrimeTooltip()
		{
			Game game = Game.Current;
			if (game != null)
			{
				game.EventManager.TriggerEvent<SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent>(new SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent());
			}
			return CampaignUIHelper.GetCrimeTooltip(Settlement.CurrentSettlement);
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x00040F57 File Offset: 0x0003F157
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PartyFilterHint = new HintViewModel(GameTexts.FindText("str_parties", null), null);
			this.CharacterFilterHint = new HintViewModel(GameTexts.FindText("str_characters", null), null);
			this.Refresh();
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00040F94 File Offset: 0x0003F194
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
				if (this._contextMenuItem.Party.Owner.Clan == Hero.MainHero.Clan)
				{
					MobileParty mobileParty = this._contextMenuItem.Party.MobileParty;
					if (mobileParty != null && !mobileParty.IsMainParty)
					{
						MobileParty mobileParty2 = this._contextMenuItem.Party.MobileParty;
						if (mobileParty2 != null && mobileParty2.IsGarrison)
						{
							this._overlayTalkItem = new GameMenuOverlayActionVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", "ManageGarrison").ToString(), true, GameMenuOverlay.MenuOverlayContextList.ManageGarrison, null);
							base.ContextList.Add(this._overlayTalkItem);
							goto IL_654;
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
				IL_654:
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

		// Token: 0x06001062 RID: 4194 RVA: 0x000416C0 File Offset: 0x0003F8C0
		private void OnSettlementOverlayTalkPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayTalkPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x000416CF File Offset: 0x0003F8CF
		private void OnSettlementOverlayQuickTalkPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayQuickTalkPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x000416DE File Offset: 0x0003F8DE
		private void OnSettlementOverlayLeaveCharacterPermissionResult(bool isAvailable, TextObject reasonStr)
		{
			this._mostRecentOverlayLeaveCharacterPermission = new Tuple<bool, TextObject>(isAvailable, reasonStr);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x000416ED File Offset: 0x0003F8ED
		public override void ExecuteOnOverlayClosed()
		{
			base.ExecuteOnOverlayClosed();
			this.InitLists();
			base.ContextList.Clear();
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x00041706 File Offset: 0x0003F906
		private void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0004170D File Offset: 0x0003F90D
		private void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[]
			{
				Settlement.CurrentSettlement,
				true
			});
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x00041735 File Offset: 0x0003F935
		private void ExecuteSettlementLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Settlement.CurrentSettlement.EncyclopediaLink);
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x00041750 File Offset: 0x0003F950
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

		// Token: 0x0600106A RID: 4202 RVA: 0x000417A0 File Offset: 0x0003F9A0
		public override void UpdateOverlayType(GameOverlays.MenuOverlayType newType)
		{
			this._type = newType;
			base.UpdateOverlayType(newType);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000417B0 File Offset: 0x0003F9B0
		private void InitLists()
		{
			this.InitCharacterList();
			this.InitPartyList();
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x000417C0 File Offset: 0x0003F9C0
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

		// Token: 0x0600106D RID: 4205 RVA: 0x00041914 File Offset: 0x0003FB14
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

		// Token: 0x0600106E RID: 4206 RVA: 0x00041A10 File Offset: 0x0003FC10
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

		// Token: 0x0600106F RID: 4207 RVA: 0x00041AC8 File Offset: 0x0003FCC8
		private bool WillBeListed(MobileParty mobileParty)
		{
			return mobileParty != null && mobileParty.IsActive;
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x00041AD8 File Offset: 0x0003FCD8
		private bool WillBeListed(LocationCharacter locationCharacter)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			return locationCharacter.Character.IsHero && !locationCharacter.IsHidden && locationCharacter.Character.HeroObject.PartyBelongedTo != MobileParty.MainParty && locationCharacter.Character.HeroObject.CurrentSettlement == settlement;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x00041B4C File Offset: 0x0003FD4C
		private bool WillBeListed(CharacterObject character)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			return character.IsHero && character.HeroObject.PartyBelongedTo != MobileParty.MainParty && character.HeroObject.CurrentSettlement == settlement;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00041BA8 File Offset: 0x0003FDA8
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

		// Token: 0x06001073 RID: 4211 RVA: 0x00041C50 File Offset: 0x0003FE50
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
				this.GarrisonAmount = currentSettlement.Town.GetNumberOfTroops();
				this.IsNoGarrisonWarning = false;
				if (this.GarrisonAmount < 1)
				{
					this.IsNoGarrisonWarning = true;
				}
			}
			if (currentSettlement.IsFortification)
			{
				MobileParty garrisonParty = currentSettlement.Town.GarrisonParty;
				this.GarrisonLbl = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers.ToString() : null) ?? "0";
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

		// Token: 0x06001074 RID: 4212 RVA: 0x00042150 File Offset: 0x00040350
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

		// Token: 0x06001075 RID: 4213 RVA: 0x00042470 File Offset: 0x00040670
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

		// Token: 0x06001076 RID: 4214 RVA: 0x00042550 File Offset: 0x00040750
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

		// Token: 0x06001077 RID: 4215 RVA: 0x000425F4 File Offset: 0x000407F4
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
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=aGFxIvqx}Leave Member(s)", null).ToString(), string.Empty, list, true, -1, new TextObject("{=FBYFcrWo}Leave in settlement", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action<List<InquiryElement>>(this.OnLeaveMembersInSettlement), new Action<List<InquiryElement>>(this.OnLeaveMembersInSettlement), ""), false, false);
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x00042714 File Offset: 0x00040914
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

		// Token: 0x06001079 RID: 4217 RVA: 0x000427F0 File Offset: 0x000409F0
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

		// Token: 0x0600107A RID: 4218 RVA: 0x0004286C File Offset: 0x00040A6C
		private void OnSettlementEntered(MobileParty arg1, Settlement arg2, Hero arg3)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			if (arg2 == settlement)
			{
				this.InitLists();
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x000428A8 File Offset: 0x00040AA8
		private void OnSettlementLeft(MobileParty arg1, Settlement arg2)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			if (arg2 == settlement)
			{
				this.InitLists();
			}
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x000428E4 File Offset: 0x00040AE4
		private void OnQuestCompleted(QuestBase arg1, QuestBase.QuestCompleteDetails arg2)
		{
			Settlement settlement = ((MobileParty.MainParty.CurrentSettlement != null) ? MobileParty.MainParty.CurrentSettlement : MobileParty.MainParty.LastVisitedSettlement);
			Hero questGiver = arg1.QuestGiver;
			if (((questGiver != null) ? questGiver.CurrentSettlement : null) != null && arg1.QuestGiver.CurrentSettlement == settlement)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0004293C File Offset: 0x00040B3C
		private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x00042946 File Offset: 0x00040B46
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail reason)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x00042950 File Offset: 0x00040B50
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

		// Token: 0x06001080 RID: 4224 RVA: 0x000429EA File Offset: 0x00040BEA
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

		// Token: 0x06001081 RID: 4225 RVA: 0x00042A28 File Offset: 0x00040C28
		private void OnTownRebelliousStateChanged(Town town, bool isRebellious)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.Town == town)
			{
				this.IsLoyaltyRebellionWarning = isRebellious || town.Loyalty < (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06001082 RID: 4226 RVA: 0x00042A7E File Offset: 0x00040C7E
		// (set) Token: 0x06001083 RID: 4227 RVA: 0x00042A86 File Offset: 0x00040C86
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

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x00042AA9 File Offset: 0x00040CA9
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x00042AB1 File Offset: 0x00040CB1
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

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x00042ACF File Offset: 0x00040CCF
		// (set) Token: 0x06001087 RID: 4231 RVA: 0x00042AD7 File Offset: 0x00040CD7
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

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x00042AF5 File Offset: 0x00040CF5
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x00042AFD File Offset: 0x00040CFD
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

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x00042B1B File Offset: 0x00040D1B
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x00042B23 File Offset: 0x00040D23
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

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x00042B41 File Offset: 0x00040D41
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x00042B49 File Offset: 0x00040D49
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

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x00042B67 File Offset: 0x00040D67
		// (set) Token: 0x0600108F RID: 4239 RVA: 0x00042B6F File Offset: 0x00040D6F
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

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x00042B8D File Offset: 0x00040D8D
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x00042B95 File Offset: 0x00040D95
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

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x00042BB3 File Offset: 0x00040DB3
		// (set) Token: 0x06001093 RID: 4243 RVA: 0x00042BBB File Offset: 0x00040DBB
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

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x00042BD9 File Offset: 0x00040DD9
		// (set) Token: 0x06001095 RID: 4245 RVA: 0x00042BE1 File Offset: 0x00040DE1
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

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x00042BFF File Offset: 0x00040DFF
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x00042C07 File Offset: 0x00040E07
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

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x00042C25 File Offset: 0x00040E25
		// (set) Token: 0x06001099 RID: 4249 RVA: 0x00042C2D File Offset: 0x00040E2D
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

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x0600109A RID: 4250 RVA: 0x00042C4B File Offset: 0x00040E4B
		// (set) Token: 0x0600109B RID: 4251 RVA: 0x00042C53 File Offset: 0x00040E53
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

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x0600109C RID: 4252 RVA: 0x00042C71 File Offset: 0x00040E71
		// (set) Token: 0x0600109D RID: 4253 RVA: 0x00042C79 File Offset: 0x00040E79
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

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600109E RID: 4254 RVA: 0x00042C97 File Offset: 0x00040E97
		// (set) Token: 0x0600109F RID: 4255 RVA: 0x00042C9F File Offset: 0x00040E9F
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

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x060010A0 RID: 4256 RVA: 0x00042CBD File Offset: 0x00040EBD
		// (set) Token: 0x060010A1 RID: 4257 RVA: 0x00042CC5 File Offset: 0x00040EC5
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

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x060010A2 RID: 4258 RVA: 0x00042CE3 File Offset: 0x00040EE3
		// (set) Token: 0x060010A3 RID: 4259 RVA: 0x00042CEB File Offset: 0x00040EEB
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

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x060010A4 RID: 4260 RVA: 0x00042D09 File Offset: 0x00040F09
		// (set) Token: 0x060010A5 RID: 4261 RVA: 0x00042D11 File Offset: 0x00040F11
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

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x00042D2F File Offset: 0x00040F2F
		// (set) Token: 0x060010A7 RID: 4263 RVA: 0x00042D37 File Offset: 0x00040F37
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

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x060010A8 RID: 4264 RVA: 0x00042D55 File Offset: 0x00040F55
		// (set) Token: 0x060010A9 RID: 4265 RVA: 0x00042D5D File Offset: 0x00040F5D
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

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x060010AA RID: 4266 RVA: 0x00042D7B File Offset: 0x00040F7B
		// (set) Token: 0x060010AB RID: 4267 RVA: 0x00042D83 File Offset: 0x00040F83
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

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x060010AC RID: 4268 RVA: 0x00042DA1 File Offset: 0x00040FA1
		// (set) Token: 0x060010AD RID: 4269 RVA: 0x00042DA9 File Offset: 0x00040FA9
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

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x060010AE RID: 4270 RVA: 0x00042DC7 File Offset: 0x00040FC7
		// (set) Token: 0x060010AF RID: 4271 RVA: 0x00042DCF File Offset: 0x00040FCF
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

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x060010B0 RID: 4272 RVA: 0x00042DED File Offset: 0x00040FED
		// (set) Token: 0x060010B1 RID: 4273 RVA: 0x00042DF5 File Offset: 0x00040FF5
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

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x060010B2 RID: 4274 RVA: 0x00042E13 File Offset: 0x00041013
		// (set) Token: 0x060010B3 RID: 4275 RVA: 0x00042E1B File Offset: 0x0004101B
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

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x060010B4 RID: 4276 RVA: 0x00042E3E File Offset: 0x0004103E
		// (set) Token: 0x060010B5 RID: 4277 RVA: 0x00042E46 File Offset: 0x00041046
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

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x060010B6 RID: 4278 RVA: 0x00042E69 File Offset: 0x00041069
		// (set) Token: 0x060010B7 RID: 4279 RVA: 0x00042E71 File Offset: 0x00041071
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

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x060010B8 RID: 4280 RVA: 0x00042E94 File Offset: 0x00041094
		// (set) Token: 0x060010B9 RID: 4281 RVA: 0x00042E9C File Offset: 0x0004109C
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

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x00042EBA File Offset: 0x000410BA
		// (set) Token: 0x060010BB RID: 4283 RVA: 0x00042EC2 File Offset: 0x000410C2
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

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x00042EE5 File Offset: 0x000410E5
		// (set) Token: 0x060010BD RID: 4285 RVA: 0x00042EED File Offset: 0x000410ED
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

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x060010BE RID: 4286 RVA: 0x00042F10 File Offset: 0x00041110
		// (set) Token: 0x060010BF RID: 4287 RVA: 0x00042F18 File Offset: 0x00041118
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

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x00042F3B File Offset: 0x0004113B
		// (set) Token: 0x060010C1 RID: 4289 RVA: 0x00042F43 File Offset: 0x00041143
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

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x060010C2 RID: 4290 RVA: 0x00042F66 File Offset: 0x00041166
		// (set) Token: 0x060010C3 RID: 4291 RVA: 0x00042F6E File Offset: 0x0004116E
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

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x00042F8C File Offset: 0x0004118C
		// (set) Token: 0x060010C5 RID: 4293 RVA: 0x00042F94 File Offset: 0x00041194
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

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x00042FB7 File Offset: 0x000411B7
		// (set) Token: 0x060010C7 RID: 4295 RVA: 0x00042FBF File Offset: 0x000411BF
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

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x060010C8 RID: 4296 RVA: 0x00042FDD File Offset: 0x000411DD
		// (set) Token: 0x060010C9 RID: 4297 RVA: 0x00042FE5 File Offset: 0x000411E5
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

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x060010CA RID: 4298 RVA: 0x00043003 File Offset: 0x00041203
		// (set) Token: 0x060010CB RID: 4299 RVA: 0x0004300B File Offset: 0x0004120B
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

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060010CC RID: 4300 RVA: 0x00043029 File Offset: 0x00041229
		// (set) Token: 0x060010CD RID: 4301 RVA: 0x00043031 File Offset: 0x00041231
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

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060010CE RID: 4302 RVA: 0x0004304F File Offset: 0x0004124F
		// (set) Token: 0x060010CF RID: 4303 RVA: 0x00043057 File Offset: 0x00041257
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

		// Token: 0x0400079A RID: 1946
		private readonly SettlementComponent _settlementComponent;

		// Token: 0x0400079B RID: 1947
		private GameOverlays.MenuOverlayType _type;

		// Token: 0x0400079C RID: 1948
		private GameMenuOverlayActionVM _overlayTalkItem;

		// Token: 0x0400079D RID: 1949
		private GameMenuOverlayActionVM _overlayQuickTalkItem;

		// Token: 0x0400079E RID: 1950
		private Tuple<bool, TextObject> _mostRecentOverlayTalkPermission;

		// Token: 0x0400079F RID: 1951
		private Tuple<bool, TextObject> _mostRecentOverlayQuickTalkPermission;

		// Token: 0x040007A0 RID: 1952
		private Tuple<bool, TextObject> _mostRecentOverlayLeaveCharacterPermission;

		// Token: 0x040007A1 RID: 1953
		private string _latestTutorialElementID;

		// Token: 0x040007A2 RID: 1954
		private bool _isCompanionHighlightApplied;

		// Token: 0x040007A3 RID: 1955
		private bool _isQuestGiversHighlightApplied;

		// Token: 0x040007A4 RID: 1956
		private bool _isNotableHighlightApplied;

		// Token: 0x040007A5 RID: 1957
		private bool _isTalkItemHighlightApplied;

		// Token: 0x040007A6 RID: 1958
		private string _militasLbl;

		// Token: 0x040007A7 RID: 1959
		private string _garrisonLbl;

		// Token: 0x040007A8 RID: 1960
		private bool _isNoGarrisonWarning;

		// Token: 0x040007A9 RID: 1961
		private bool _isLoyaltyRebellionWarning;

		// Token: 0x040007AA RID: 1962
		private bool _isCrimeLabelHighlightEnabled;

		// Token: 0x040007AB RID: 1963
		private string _crimeLbl;

		// Token: 0x040007AC RID: 1964
		private string _prosperityLbl;

		// Token: 0x040007AD RID: 1965
		private string _loyaltyLbl;

		// Token: 0x040007AE RID: 1966
		private string _securityLbl;

		// Token: 0x040007AF RID: 1967
		private string _wallsLbl;

		// Token: 0x040007B0 RID: 1968
		private string _settlementNameLbl;

		// Token: 0x040007B1 RID: 1969
		private string _remainingFoodText = "";

		// Token: 0x040007B2 RID: 1970
		private int _wallsLevel;

		// Token: 0x040007B3 RID: 1971
		private int _prosperityChangeAmount;

		// Token: 0x040007B4 RID: 1972
		private int _militiaChangeAmount;

		// Token: 0x040007B5 RID: 1973
		private int _garrisonChangeAmount;

		// Token: 0x040007B6 RID: 1974
		private int _garrisonAmount;

		// Token: 0x040007B7 RID: 1975
		private int _crimeChangeAmount;

		// Token: 0x040007B8 RID: 1976
		private int _loyaltyChangeAmount;

		// Token: 0x040007B9 RID: 1977
		private int _securityChangeAmount;

		// Token: 0x040007BA RID: 1978
		private int _foodChangeAmount;

		// Token: 0x040007BB RID: 1979
		private MBBindingList<GameMenuPartyItemVM> _characterList;

		// Token: 0x040007BC RID: 1980
		private MBBindingList<GameMenuPartyItemVM> _partyList;

		// Token: 0x040007BD RID: 1981
		private MBBindingList<StringItemWithHintVM> _issueList;

		// Token: 0x040007BE RID: 1982
		private bool _isFortification;

		// Token: 0x040007BF RID: 1983
		private bool _isCrimeEnabled;

		// Token: 0x040007C0 RID: 1984
		private bool _canLeaveMembers;

		// Token: 0x040007C1 RID: 1985
		private BasicTooltipViewModel _remainingFoodHint;

		// Token: 0x040007C2 RID: 1986
		private BasicTooltipViewModel _militasHint;

		// Token: 0x040007C3 RID: 1987
		private BasicTooltipViewModel _garrisonHint;

		// Token: 0x040007C4 RID: 1988
		private BasicTooltipViewModel _prosperityHint;

		// Token: 0x040007C5 RID: 1989
		private BasicTooltipViewModel _loyaltyHint;

		// Token: 0x040007C6 RID: 1990
		private BasicTooltipViewModel _securityHint;

		// Token: 0x040007C7 RID: 1991
		private BasicTooltipViewModel _wallsHint;

		// Token: 0x040007C8 RID: 1992
		private BasicTooltipViewModel _crimeHint;

		// Token: 0x040007C9 RID: 1993
		private HintViewModel _characterFilterHint;

		// Token: 0x040007CA RID: 1994
		private HintViewModel _partyFilterHint;

		// Token: 0x040007CB RID: 1995
		private HintViewModel _leaveMembersHint;

		// Token: 0x040007CC RID: 1996
		private ImageIdentifierVM _settlementOwnerBanner;

		// Token: 0x020001E5 RID: 485
		public class SettlementOverlayTalkPermissionEvent : EventBase
		{
			// Token: 0x17000A6B RID: 2667
			// (get) Token: 0x06002068 RID: 8296 RVA: 0x0006FADE File Offset: 0x0006DCDE
			// (set) Token: 0x06002069 RID: 8297 RVA: 0x0006FAE6 File Offset: 0x0006DCE6
			public Action<bool, TextObject> IsTalkAvailable { get; private set; }

			// Token: 0x0600206A RID: 8298 RVA: 0x0006FAEF File Offset: 0x0006DCEF
			public SettlementOverlayTalkPermissionEvent(Hero heroToTalkTo, Action<bool, TextObject> isTalkAvailable)
			{
				this.HeroToTalkTo = heroToTalkTo;
				this.IsTalkAvailable = isTalkAvailable;
			}

			// Token: 0x04001017 RID: 4119
			public Hero HeroToTalkTo;
		}

		// Token: 0x020001E6 RID: 486
		public class SettlementOverylayQuickTalkPermissionEvent : EventBase
		{
			// Token: 0x17000A6C RID: 2668
			// (get) Token: 0x0600206B RID: 8299 RVA: 0x0006FB05 File Offset: 0x0006DD05
			// (set) Token: 0x0600206C RID: 8300 RVA: 0x0006FB0D File Offset: 0x0006DD0D
			public Action<bool, TextObject> IsTalkAvailable { get; private set; }

			// Token: 0x0600206D RID: 8301 RVA: 0x0006FB16 File Offset: 0x0006DD16
			public SettlementOverylayQuickTalkPermissionEvent(Action<bool, TextObject> isTalkAvailable)
			{
				this.IsTalkAvailable = isTalkAvailable;
			}
		}

		// Token: 0x020001E7 RID: 487
		public class SettlementOverlayLeaveCharacterPermissionEvent : EventBase
		{
			// Token: 0x17000A6D RID: 2669
			// (get) Token: 0x0600206E RID: 8302 RVA: 0x0006FB25 File Offset: 0x0006DD25
			// (set) Token: 0x0600206F RID: 8303 RVA: 0x0006FB2D File Offset: 0x0006DD2D
			public Action<bool, TextObject> IsLeaveAvailable { get; private set; }

			// Token: 0x06002070 RID: 8304 RVA: 0x0006FB36 File Offset: 0x0006DD36
			public SettlementOverlayLeaveCharacterPermissionEvent(Action<bool, TextObject> isLeaveAvailable)
			{
				this.IsLeaveAvailable = isLeaveAvailable;
			}
		}

		// Token: 0x020001E8 RID: 488
		public class CrimeValueInspectedInSettlementOverlayEvent : EventBase
		{
		}

		// Token: 0x020001E9 RID: 489
		private class CharacterComparer : IComparer<GameMenuPartyItemVM>
		{
			// Token: 0x06002072 RID: 8306 RVA: 0x0006FB4D File Offset: 0x0006DD4D
			public int Compare(GameMenuPartyItemVM x, GameMenuPartyItemVM y)
			{
				return CampaignUIHelper.GetHeroCompareSortIndex(x.Character.HeroObject, y.Character.HeroObject);
			}
		}

		// Token: 0x020001EA RID: 490
		private class PartyComparer : IComparer<GameMenuPartyItemVM>
		{
			// Token: 0x06002074 RID: 8308 RVA: 0x0006FB72 File Offset: 0x0006DD72
			public int Compare(GameMenuPartyItemVM x, GameMenuPartyItemVM y)
			{
				return CampaignUIHelper.MobilePartyPrecedenceComparerInstance.Compare(x.Party.MobileParty, y.Party.MobileParty);
			}
		}
	}
}
