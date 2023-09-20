using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Missions.MissionLogics;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.PlayerClanQuests
{
	// Token: 0x0200002E RID: 46
	public class RescueFamilyQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000270 RID: 624 RVA: 0x0000D3B3 File Offset: 0x0000B5B3
		internal RescueFamilyQuestBehavior()
		{
			this._rescueFamilyQuestReadyToStart = false;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000D3C4 File Offset: 0x0000B5C4
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.CanHaveQuestsOrIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnCanHaveQuestsOrIssuesInfoIsRequested));
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000D42D File Offset: 0x0000B62D
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_rescueFamilyQuestReadyToStart", ref this._rescueFamilyQuestReadyToStart);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000D444 File Offset: 0x0000B644
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (this._rescueFamilyQuestReadyToStart && party == MobileParty.MainParty && settlement.IsTown && !settlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && GameStateManager.Current.ActiveState is MapState && !Campaign.Current.ConversationManager.IsConversationFlowActive)
			{
				bool flag = false;
				foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests)
				{
					Hero questGiver = questBase.QuestGiver;
					if (((questGiver != null) ? questGiver.CurrentSettlement : null) == settlement)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					new RescueFamilyQuestBehavior.RescueFamilyQuest().StartQuest();
					this._rescueFamilyQuestReadyToStart = false;
					StoryModeHeroes.Radagos.UpdateLastKnownClosestSettlement(Settlement.CurrentSettlement);
					CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.Radagos.CharacterObject, null, true, true, false, false, false, false));
				}
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000D564 File Offset: 0x0000B764
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (quest is RebuildPlayerClanQuest)
			{
				this._rescueFamilyQuestReadyToStart = true;
				return;
			}
			if (quest is RescueFamilyQuestBehavior.RescueFamilyQuest)
			{
				this._rescueFamilyQuestReadyToStart = false;
				StoryModeHeroes.Radagos.CharacterObject.SetTransferableInPartyScreen(true);
				StoryModeHeroes.Radagos.CharacterObject.SetTransferableInHideouts(true);
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000D5B0 File Offset: 0x0000B7B0
		private void OnCanHaveQuestsOrIssuesInfoIsRequested(Hero hero, ref bool result)
		{
			if (!StoryModeManager.Current.MainStoryLine.FamilyRescued && (hero == StoryModeHeroes.Radagos || hero == StoryModeHeroes.RadagosHencman))
			{
				result = false;
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000D5D8 File Offset: 0x0000B7D8
		private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			if (hero == StoryModeHeroes.RadagosHencman && (!StoryModeManager.Current.MainStoryLine.FamilyRescued || this._rescueFamilyQuestReadyToStart || (Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest)) && causeOfDeath != 6)))
			{
				result = false;
			}
		}

		// Token: 0x040000C5 RID: 197
		private bool _rescueFamilyQuestReadyToStart;

		// Token: 0x02000072 RID: 114
		public class RescueFamilyQuest : StoryModeQuestBase
		{
			// Token: 0x170000D8 RID: 216
			// (get) Token: 0x06000615 RID: 1557 RVA: 0x000221C0 File Offset: 0x000203C0
			private TextObject _startQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=FyzsAZx8}{RADAGOS.LINK} said that he knows where your siblings are. He offered to attack together. He will wait for you at the hideout that he mentioned about near {SETTLEMENT_LINK}. You can see the hideout marked on the map.", null);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT_LINK", Settlement.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170000D9 RID: 217
			// (get) Token: 0x06000616 RID: 1558 RVA: 0x00022208 File Offset: 0x00020408
			private TextObject _defeatedQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=Ga8mDgab}You've been defeated at {HIDEOUT_BOSS.LINK}'s hideout. You can attack again when you are ready.", null);
					StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170000DA RID: 218
			// (get) Token: 0x06000617 RID: 1559 RVA: 0x0002223C File Offset: 0x0002043C
			private TextObject _letGoRadagosEndQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=DjQuB0TU}You saved your brothers and sister, and you decided to let {RADAGOS.LINK} go...", null);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170000DB RID: 219
			// (get) Token: 0x06000618 RID: 1560 RVA: 0x00022270 File Offset: 0x00020470
			private TextObject _executeRadagosEndQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ZpcV9MZE}You saved your brothers and sister, and you decided to execute {RADAGOS.LINK} because he caused the death of your parents.", null);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170000DC RID: 220
			// (get) Token: 0x06000619 RID: 1561 RVA: 0x000222A2 File Offset: 0x000204A2
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=HPNuqbSf}Rescue Your Family", null);
				}
			}

			// Token: 0x0600061A RID: 1562 RVA: 0x000222B0 File Offset: 0x000204B0
			public RescueFamilyQuest()
				: base("rescue_your_family_storymode_quest", null, CampaignTime.Never)
			{
				StoryModeManager.Current.MainStoryLine.FamilyRescued = true;
				this._radagos = StoryModeHeroes.Radagos;
				this._radagos.CharacterObject.SetTransferableInPartyScreen(false);
				this._radagos.CharacterObject.SetTransferableInHideouts(false);
				this._hideoutBoss = StoryModeHeroes.RadagosHencman;
				this._targetSettlementForSiblings = null;
				this._hideout = SettlementHelper.FindNearestHideout((Settlement s) => !StoryModeManager.Current.MainStoryLine.BusyHideouts.Contains(s.Hideout), null);
				StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
				this._reunionTalkDone = false;
				this._hideoutTalkDone = false;
				this._brotherConversationDone = false;
				this._radagosGoodByeConversationDone = false;
				this._raiderParties = new List<MobileParty>();
				this.InitializeHideout();
				base.AddTrackedObject(this._hideout);
				this.SetDialogs();
				this.AddGameMenus();
			}

			// Token: 0x0600061B RID: 1563 RVA: 0x000223AC File Offset: 0x000205AC
			protected override void InitializeQuestOnGameLoad()
			{
				this._radagos = StoryModeHeroes.Radagos;
				this._radagos.CharacterObject.SetTransferableInPartyScreen(false);
				this._radagos.CharacterObject.SetTransferableInHideouts(false);
				this._hideoutBoss = StoryModeHeroes.RadagosHencman;
				this.SetDialogs();
				this.AddGameMenus();
				StoryModeManager.Current.MainStoryLine.BusyHideouts.Add(this._hideout.Hideout);
				this.SelectTargetSettlementForSiblings();
			}

			// Token: 0x0600061C RID: 1564 RVA: 0x00022422 File Offset: 0x00020622
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == StoryModeHeroes.Radagos && StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && !StoryModeManager.Current.MainStoryLine.FamilyRescued)
				{
					result = false;
				}
			}

			// Token: 0x0600061D RID: 1565 RVA: 0x00022458 File Offset: 0x00020658
			protected override void OnCompleteWithSuccess()
			{
				StoryModeHeroes.ElderBrother.Clan = Clan.PlayerClan;
				StoryModeHeroes.LittleBrother.Clan = Clan.PlayerClan;
				StoryModeHeroes.LittleSister.Clan = Clan.PlayerClan;
				StoryModeHeroes.ElderBrother.ChangeState(1);
				EnterSettlementAction.ApplyForCharacterOnly(StoryModeHeroes.ElderBrother, this._targetSettlementForSiblings);
				if (StoryModeHeroes.LittleBrother.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
				{
					StoryModeHeroes.LittleBrother.ChangeState(1);
					EnterSettlementAction.ApplyForCharacterOnly(StoryModeHeroes.LittleBrother, this._targetSettlementForSiblings);
				}
				else
				{
					StoryModeHeroes.LittleBrother.ChangeState(0);
				}
				if (StoryModeHeroes.LittleSister.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
				{
					StoryModeHeroes.LittleSister.ChangeState(1);
					EnterSettlementAction.ApplyForCharacterOnly(StoryModeHeroes.LittleSister, this._targetSettlementForSiblings);
				}
				else
				{
					StoryModeHeroes.LittleSister.ChangeState(0);
				}
				StoryModeHeroes.ElderBrother.UpdateLastKnownClosestSettlement(this._targetSettlementForSiblings);
				StoryModeHeroes.LittleBrother.UpdateLastKnownClosestSettlement(this._targetSettlementForSiblings);
				StoryModeHeroes.LittleSister.UpdateLastKnownClosestSettlement(this._targetSettlementForSiblings);
				TextObject textObject = new TextObject("{=PDlaPVIP}{PLAYER_LITTLE_BROTHER.NAME} is the little brother of {PLAYER.LINK}.", null);
				StringHelpers.SetCharacterProperties("PLAYER_LITTLE_BROTHER", StoryModeHeroes.LittleBrother.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				StoryModeHeroes.LittleBrother.EncyclopediaText = textObject;
				TextObject textObject2 = new TextObject("{=7XTkTi9B}{PLAYER_LITTLE_SISTER.NAME} is the little sister of {PLAYER.LINK}.", null);
				StringHelpers.SetCharacterProperties("PLAYER_LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, textObject2, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
				StoryModeHeroes.LittleSister.EncyclopediaText = textObject2;
				TextObject textObject3 = new TextObject("{=LcxfWLgd}{PLAYER_BROTHER.NAME} is the elder brother of {PLAYER.LINK}.", null);
				StringHelpers.SetCharacterProperties("PLAYER_BROTHER", StoryModeHeroes.ElderBrother.CharacterObject, textObject3, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject3, false);
				StoryModeHeroes.ElderBrother.EncyclopediaText = textObject3;
			}

			// Token: 0x0600061E RID: 1566 RVA: 0x0002262F File Offset: 0x0002082F
			protected override void OnFinalize()
			{
				base.OnFinalize();
				StoryModeManager.Current.MainStoryLine.BusyHideouts.Remove(this._hideout.Hideout);
			}

			// Token: 0x0600061F RID: 1567 RVA: 0x00022657 File Offset: 0x00020857
			private void InitializeHideout()
			{
				this.CheckIfHideoutIsReady();
				this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.None;
			}

			// Token: 0x06000620 RID: 1568 RVA: 0x00022668 File Offset: 0x00020868
			private void CheckIfHideoutIsReady()
			{
				if (!this._hideout.Hideout.IsInfested)
				{
					for (int i = 0; i < 2; i++)
					{
						if (!this._hideout.Hideout.IsInfested)
						{
							this._raiderParties.Add(this.CreateRaiderParty(i, false));
						}
					}
				}
				this._hideout.Hideout.IsSpotted = true;
				this._hideout.IsVisible = true;
			}

			// Token: 0x06000621 RID: 1569 RVA: 0x000226D8 File Offset: 0x000208D8
			private void AddRadagosHencmanToHideout()
			{
				if (!this._hideout.Parties.Any((MobileParty p) => p.IsBanditBossParty))
				{
					this._raiderParties.Add(this.CreateRaiderParty(3, true));
				}
				foreach (MobileParty mobileParty in this._hideout.Parties)
				{
					if (mobileParty.IsBanditBossParty)
					{
						if (mobileParty.MemberRoster.GetTroopRoster().Any((TroopRosterElement t) => t.Character == this._hideout.Culture.BanditBoss))
						{
							TroopRosterElement troopRosterElement = mobileParty.MemberRoster.GetTroopRoster().First((TroopRosterElement t) => t.Character == this._hideout.Culture.BanditBoss);
							mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, 1, default(UniqueTroopDescriptor), 0);
						}
						this._hideoutBoss.ChangeState(1);
						mobileParty.MemberRoster.AddToCounts(this._hideoutBoss.CharacterObject, 1, true, 0, 0, true, -1);
						break;
					}
				}
			}

			// Token: 0x06000622 RID: 1570 RVA: 0x00022800 File Offset: 0x00020A00
			private void RemoveRadagosHencmanFromHideout()
			{
				foreach (MobileParty mobileParty in this._hideout.Parties)
				{
					if (mobileParty.MemberRoster.Contains(this._hideoutBoss.CharacterObject))
					{
						mobileParty.MemberRoster.RemoveTroop(this._hideoutBoss.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
						this._hideoutBoss.ChangeState(6);
						mobileParty.MemberRoster.AddToCounts(this._hideout.Culture.BanditBoss, 1, false, 0, 0, true, -1);
						break;
					}
				}
			}

			// Token: 0x06000623 RID: 1571 RVA: 0x000228BC File Offset: 0x00020ABC
			private MobileParty CreateRaiderParty(int number, bool isBanditBossParty)
			{
				MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("rescue_family_quest_raider_party_" + number, StoryModeHeroes.RadiersClan, this._hideout.Hideout, isBanditBossParty);
				TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
				CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(this._hideout.Culture.StringId + "_bandit");
				troopRoster.AddToCounts(@object, 5, false, 0, 0, true, -1);
				TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
				mobileParty.InitializeMobilePartyAtPosition(troopRoster, troopRoster2, this._hideout.Position2D);
				mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
				mobileParty.ActualClan = StoryModeHeroes.RadiersClan;
				mobileParty.Position2D = this._hideout.Position2D;
				mobileParty.Party.Visuals.SetMapIconAsDirty();
				float totalStrength = mobileParty.Party.TotalStrength;
				int num = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
				mobileParty.InitializePartyTrade(num);
				mobileParty.Ai.SetMoveGoToSettlement(this._hideout);
				mobileParty.Ai.SetDoNotMakeNewDecisions(true);
				mobileParty.SetPartyUsedByQuest(true);
				EnterSettlementAction.ApplyForParty(mobileParty, this._hideout);
				return mobileParty;
			}

			// Token: 0x06000624 RID: 1572 RVA: 0x000229F0 File Offset: 0x00020BF0
			private void SelectTargetSettlementForSiblings()
			{
				this._targetSettlementForSiblings = SettlementHelper.FindNearestTown((Settlement s) => s.OwnerClan.MapFaction == Clan.PlayerClan.MapFaction, null);
				if (this._targetSettlementForSiblings == null)
				{
					this._targetSettlementForSiblings = SettlementHelper.FindNearestTown((Settlement s) => !Clan.PlayerClan.MapFaction.IsAtWarWith(s.OwnerClan.MapFaction), null);
				}
				if (this._targetSettlementForSiblings == null)
				{
					this._targetSettlementForSiblings = SettlementHelper.FindRandomSettlement((Settlement s) => s.IsTown);
				}
			}

			// Token: 0x06000625 RID: 1573 RVA: 0x00022A90 File Offset: 0x00020C90
			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			}

			// Token: 0x06000626 RID: 1574 RVA: 0x00022B27 File Offset: 0x00020D27
			private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
			{
				if (killer == this._radagos && victim == this._hideoutBoss)
				{
					if (Campaign.Current.CurrentMenuContext != null)
					{
						Campaign.Current.CurrentMenuContext.SwitchToMenu("radagos_goodbye_menu");
						return;
					}
					GameMenu.ActivateGameMenu("radagos_goodbye_menu");
				}
			}

			// Token: 0x06000627 RID: 1575 RVA: 0x00022B68 File Offset: 0x00020D68
			private void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party.IsMainParty)
				{
					if (base.IsTrackEnabled && this._reunionTalkDone && !base.IsTracked(this._hideout))
					{
						base.AddTrackedObject(this._hideout);
					}
					if (settlement == this._hideout)
					{
						if (PartyBase.MainParty.MemberRoster.Contains(this._radagos.CharacterObject))
						{
							PartyBase.MainParty.MemberRoster.RemoveTroop(this._radagos.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
						}
						this.RemoveRadagosHencmanFromHideout();
					}
				}
			}

			// Token: 0x06000628 RID: 1576 RVA: 0x00022BF8 File Offset: 0x00020DF8
			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if (PlayerEncounter.Current != null && mapEvent.IsPlayerMapEvent && Settlement.CurrentSettlement == this._hideout)
				{
					if (mapEvent.WinningSide == mapEvent.PlayerSide)
					{
						this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Victory;
						return;
					}
					if (mapEvent.WinningSide == -1)
					{
						this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Retreated;
						if (Hero.MainHero.IsPrisoner && this._raiderParties.Contains(Hero.MainHero.PartyBelongedToAsPrisoner.MobileParty))
						{
							EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
							InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=WN6aHR6m}You were defeated by the bandits in the hideout but you managed to escape. You need to wait a while before attacking again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
						}
						if (this._hideout.Parties.Count == 0)
						{
							this.InitializeHideout();
						}
						this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.None;
						return;
					}
					this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Defeated;
				}
			}

			// Token: 0x06000629 RID: 1577 RVA: 0x00022CFC File Offset: 0x00020EFC
			private void OnGameMenuOpened(MenuCallbackArgs args)
			{
				if (this._hideoutBattleEndState != RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Victory && !this._hideoutBoss.IsHealthFull())
				{
					this._hideoutBoss.Heal(this._hideoutBoss.CharacterObject.MaxHitPoints(), false);
				}
				if (this._hideoutBattleEndState == RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Victory)
				{
					if (StoryModeHeroes.RadagosHencman.IsAlive)
					{
						CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.RadagosHencman.CharacterObject, null, true, true, false, false, false, false));
						return;
					}
					CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.ElderBrother.CharacterObject, null, true, true, false, false, false, false));
					this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.None;
					return;
				}
				else
				{
					if (this._hideoutBattleEndState == RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Retreated || this._hideoutBattleEndState == RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.Defeated)
					{
						base.AddLog(this._defeatedQuestLogText, false);
						DisableHeroAction.Apply(this._radagos);
						if (Hero.MainHero.IsPrisoner)
						{
							EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
							InformationManager.ShowInquiry(new InquiryData(new TextObject("{=FPhWhjq7}Defeated", null).ToString(), new TextObject("{=XSzmugWh}You were defeated by the raiders in the hideout but you managed to escape. You need to wait a while before attacking again.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
						}
						if (this._hideout.Parties.Count == 0)
						{
							this.InitializeHideout();
						}
						this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.None;
						return;
					}
					if (this._radagosGoodByeConversationDone && args.MenuContext.GameMenu.StringId == "radagos_goodbye_menu")
					{
						GameMenu.ExitToLast();
						base.CompleteQuestWithSuccess();
						return;
					}
					if (!this._hideoutTalkDone && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._hideout)
					{
						CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.Radagos.CharacterObject, null, true, true, false, false, false, false));
					}
					return;
				}
			}

			// Token: 0x0600062A RID: 1578 RVA: 0x00022EDC File Offset: 0x000210DC
			private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
			{
				if (this._hideoutTalkDone && settlement == this._hideout && mobileParty != null && mobileParty.IsMainParty)
				{
					if (!PartyBase.MainParty.MemberRoster.Contains(this._radagos.CharacterObject))
					{
						PartyBase.MainParty.MemberRoster.AddToCounts(this._radagos.CharacterObject, 1, false, 0, 0, true, -1);
					}
					this.AddRadagosHencmanToHideout();
				}
			}

			// Token: 0x0600062B RID: 1579 RVA: 0x00022F48 File Offset: 0x00021148
			private void HourlyTick()
			{
				this.CheckIfHideoutIsReady();
			}

			// Token: 0x0600062C RID: 1580 RVA: 0x00022F50 File Offset: 0x00021150
			protected override void SetDialogs()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=1yi00v5w}{PLAYER.NAME}! Good to see you. Believe it or not, I mean that. I've been looking for you...[if:happy][ib:demure]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.radagos_reunion_conversation_condition))
					.PlayerLine(new TextObject("{=pCNSEPEP}You escaped? Where's my brother? What happened?", null), null)
					.NpcLine(new TextObject("{=xknCpvcb}Calm down, now. I'll tell you everything.", null), null, null)
					.NpcLine(new TextObject("{=X1TJNkBV}We found your little brother and sister. But my former partner {HIDEOUT_BOSS.LINK} betrayed me. We came into his camp to negotiate the kids' release, and he seized us right then and there.", null), null, null)
					.NpcLine(new TextObject("{=UpUqL368}What scum, eh? Even in this profession, double-crossing your comrades is frowned upon.", null), null, null)
					.NpcLine(new TextObject("{=bJjAqCxk}I escaped - one of his men, a little guiltier than the rest, cut my bonds when the others were sleeping - but I can't let a traitor live. So I decided to find you and offer you a deal.[if:idle_angry][ib:warrior]", null), null, null)
					.NpcLine(new TextObject("{=PlpNTQqf}I know where {HIDEOUT_BOSS.LINK} is now. If you agree, we can attack together and save your kin.", null), null, null)
					.NpcLine(new TextObject("{=mmQRCHUM}But in return, I will have the pleasure of killing that bastard. So what do you say?[if:convo_mocking_revenge][ib:confident]", null), null, null)
					.PlayerLine(new TextObject("{=ypDmy5Rn}Uh, how can we possibly trust each other?", null), null)
					.NpcLine(new TextObject("{=VbJvL8yB}Oh you can't trust me. But you need me, and I figure you have enough men that you could easily slit my throat pretty quickly if I lead you into a trap. And I don't need to trust you - you're my vehicle of revenge, not my partner.", null), null, null)
					.PlayerLine(new TextObject("{=ft6zzDrJ}I can live with that. Let's go.", null), null)
					.NpcLine(new TextObject("{=HT9hW29s}Splendid! But I have a few things to do. There is a hideout near this city. {HIDEOUT_BOSS.LINK} keeps your siblings there. I will join you right where the path leads up, just out of sight of their scouts.[if:idle_normal][ib:normal]", null), null, null)
					.PlayerLine(new TextObject("{=GicEcLx2}See you there then. But, remember, if this is a trap or something, that will cost you your life.", null), null)
					.NpcLine(new TextObject("{=8b4Ndfep}Oh of course. I have no doubts on that score.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.radagos_reunion_conversation_consequence))
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=rDuegB1L}You've finally arrived! I have a few things to say before we attack.[ib:confident][if:convo_nonchalant]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.radagos_hideout_conversation_condition))
					.NpcLine(new TextObject("{=1T7p0O7B}We have to be clever. {HIDEOUT_BOSS.LINK} is a cunning fellow, in a low and base kind of way.", null), null, null)
					.PlayerLine(new TextObject("{=a29lmPLd}I defeated you before. I know how your gang operates. Less talking, more raiding. C'mon...", null), null)
					.NpcLine(new TextObject("{=QbsDYITB}That you did, that you did. Lead on, then.[ib:closed][if:convo_nonchalant]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.radagos_hideout_conversation_consequence))
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=PiKISvfu}{PLAYER.NAME}! I knew you'd come. Great Heaven. Damn, {?PLAYER.GENDER}sister{?}brother{\\?}, nothing can stop you! I love you, {?PLAYER.GENDER}sister{?}brother{\\?}.[if:happy][ib:demure]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.brother_hideout_conversation_condition))
					.PlayerLine(new TextObject("{=DIKPGwj1}So glad to see you safe. Is everyone okay?", null), null)
					.NpcLine(new TextObject("{=xachJ1hb}Yes, we are all fine. The little ones are scared but fine... We need to be quick and get the hell out of this place.[if:idle_insulted][ib:closed]", null), null, null)
					.NpcLine(new TextObject("{=p3Kia1OO}I'll take them to the nearest fortress immediately. They will be safe there.", null), null, null)
					.NpcLine(new TextObject("{=IC9Vg5MA}Meet me there later, when you're ready to tell me everything.[if:idle_normal][ib:normal]", null), null, null)
					.PlayerLine(new TextObject("{=LrItHItu}Okay brother, be careful. Take care.", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.brother_hideout_conversation_consequence;
					})
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=0I9siaQY}Bastards... You're the kin of my captives, right? I saw {RADAGOS.LINK} with you. You know he can't be trusted?[if:idle_angry][ib:warrior]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.bandit_hideout_boss_fight_start_on_condition))
					.PlayerLine(new TextObject("{=mWMkslbn}He led us here. Where are my brothers and my sister?", null), null)
					.NpcLine(new TextObject("{=heoCaRIr}Nah... There's no more talking. Kill me or I kill you, that's how this ends.[ib:confident]", null), null, null)
					.NpcLine(new TextObject("{=2GeiKTlS}I'll do you the honor of duelling you, and my men will stand down if you win.", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=ImLQNYWC}Very well - I'll duel you.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_start_duel_fight_on_consequence))
					.CloseDialog()
					.PlayerOption(new TextObject("{=MMv3hsmI}I don't duel slavers. Men, attack!", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_continue_battle_on_consequence))
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=G9iXmhGK}Look, we can still talk. I'll give you a pouch of silver.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.hideout_boss_prisoner_talk_condition))
					.PlayerLine(new TextObject("{=fM4eSVps}You said talking was a waste of time. You are {RADAGOS.NAME}'s property, now.", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.hideout_boss_prisoner_talk_consequence;
					})
					.CloseDialog(), this);
				string text;
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=3dhvmJDp}Well... Looks like we've gotten your kin back to you, so my end of our deal is complete. I'll be making myself scarce now.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.goodbye_conversation_with_radagos_condition))
					.GetOutputToken(ref text)
					.NpcLine(new TextObject("{=C79Xxm1b}Don't let your conscience bother you about letting me go, by the way. I won't get back into slaving. Burned too many bridges with my old colleagues, you might say. I'll find some other way to earn my keep - mercenary work, perhaps. Anyway, maybe our paths will cross again.", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=c1Q2irLi}Your men killed my parents. Did you really think you would not be punished?", null), null)
					.NpcLine(new TextObject("{=W7hi7jS4}Eh, well, I dared to hope, I suppose. All right then, I'm not going to grovel to you, so get it over with.", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=kz5PJbV1}I shall. For your many crimes, {RADAGOS.NAME}, your life is forfeit.", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.execute_radagos_consequence;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=HrdVRMgR}Perhaps by saving my brother and sister you earned your life back. Very well, go now.", null), null)
					.GotoDialogState(text)
					.EndPlayerOptions()
					.PlayerOption(new TextObject("{=RefpTQpr}Maybe. Goodbye, {RADAGOS.NAME}...", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.let_go_radagos_consequence;
					})
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog(), this);
			}

			// Token: 0x0600062D RID: 1581 RVA: 0x00023414 File Offset: 0x00021614
			private bool radagos_reunion_conversation_condition()
			{
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
				StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, null, false);
				return !this._reunionTalkDone && Hero.OneToOneConversationHero == this._radagos;
			}

			// Token: 0x0600062E RID: 1582 RVA: 0x00023462 File Offset: 0x00021662
			private void radagos_reunion_conversation_consequence()
			{
				this._reunionTalkDone = true;
				base.AddLog(this._startQuestLogText, false);
			}

			// Token: 0x0600062F RID: 1583 RVA: 0x00023479 File Offset: 0x00021679
			private bool radagos_hideout_conversation_condition()
			{
				StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, null, false);
				return !this._hideoutTalkDone && Settlement.CurrentSettlement == this._hideout && Hero.OneToOneConversationHero == this._radagos;
			}

			// Token: 0x06000630 RID: 1584 RVA: 0x000234B8 File Offset: 0x000216B8
			private void radagos_hideout_conversation_consequence()
			{
				this._hideoutTalkDone = true;
				if (!PartyBase.MainParty.MemberRoster.Contains(this._radagos.CharacterObject))
				{
					PartyBase.MainParty.MemberRoster.AddToCounts(this._radagos.CharacterObject, 1, false, 0, 0, true, -1);
				}
				this.AddRadagosHencmanToHideout();
			}

			// Token: 0x06000631 RID: 1585 RVA: 0x00023510 File Offset: 0x00021710
			private bool brother_hideout_conversation_condition()
			{
				if (!this._brotherConversationDone && Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother)
				{
					this.SelectTargetSettlementForSiblings();
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
					StringHelpers.SetCharacterProperties("LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, null, false);
					StringHelpers.SetCharacterProperties("LITTLE_BROTHER", StoryModeHeroes.LittleBrother.CharacterObject, null, false);
					MBTextManager.SetTextVariable("SETTLEMENT_LINK", this._targetSettlementForSiblings.EncyclopediaLinkWithName, false);
					Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
					{
						if (Campaign.Current.CurrentMenuContext != null)
						{
							Campaign.Current.CurrentMenuContext.SwitchToMenu("radagos_goodbye_menu");
							return;
						}
						GameMenu.ActivateGameMenu("radagos_goodbye_menu");
					};
					return true;
				}
				return false;
			}

			// Token: 0x06000632 RID: 1586 RVA: 0x000235C4 File Offset: 0x000217C4
			private void brother_hideout_conversation_consequence()
			{
				this._brotherConversationDone = true;
			}

			// Token: 0x06000633 RID: 1587 RVA: 0x000235D0 File Offset: 0x000217D0
			private bool bandit_hideout_boss_fight_start_on_condition()
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				if (encounteredParty == null || encounteredParty.IsMobile || encounteredParty.MapFaction == null || !encounteredParty.MapFaction.IsBanditFaction)
				{
					return false;
				}
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, null, false);
				return encounteredParty.MapFaction.IsBanditFaction && encounteredParty.IsSettlement && encounteredParty.Settlement == this._hideout && Mission.Current != null && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == this._hideoutBoss && encounteredParty.Settlement.IsHideout;
			}

			// Token: 0x06000634 RID: 1588 RVA: 0x00023674 File Offset: 0x00021874
			private void bandit_hideout_start_duel_fight_on_consequence()
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
			}

			// Token: 0x06000635 RID: 1589 RVA: 0x00023691 File Offset: 0x00021891
			private void bandit_hideout_continue_battle_on_consequence()
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
			}

			// Token: 0x06000636 RID: 1590 RVA: 0x000236AE File Offset: 0x000218AE
			private bool hideout_boss_prisoner_talk_condition()
			{
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, null, false);
				return Hero.OneToOneConversationHero == this._hideoutBoss;
			}

			// Token: 0x06000637 RID: 1591 RVA: 0x000236D5 File Offset: 0x000218D5
			private void hideout_boss_prisoner_talk_consequence()
			{
				if (this._hideoutBoss.IsAlive)
				{
					MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForInformingPlayer(this._radagos, this._hideoutBoss, 4));
				}
			}

			// Token: 0x06000638 RID: 1592 RVA: 0x000236FC File Offset: 0x000218FC
			private bool goodbye_conversation_with_radagos_condition()
			{
				if (this._brotherConversationDone && Hero.OneToOneConversationHero == this._radagos)
				{
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, null, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000639 RID: 1593 RVA: 0x0002374B File Offset: 0x0002194B
			private void execute_radagos_consequence()
			{
				base.AddLog(this._executeRadagosEndQuestLogText, false);
				this._brotherConversationDone = false;
				MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForInformingPlayer(Hero.MainHero, this._radagos, 4));
				this._radagosGoodByeConversationDone = true;
			}

			// Token: 0x0600063A RID: 1594 RVA: 0x0002377F File Offset: 0x0002197F
			private void let_go_radagos_consequence()
			{
				base.AddLog(this._letGoRadagosEndQuestLogText, false);
				this._brotherConversationDone = false;
				DisableHeroAction.Apply(this._radagos);
				this._radagosGoodByeConversationDone = true;
			}

			// Token: 0x0600063B RID: 1595 RVA: 0x000237A8 File Offset: 0x000219A8
			private void AddGameMenus()
			{
				TextObject textObject = new TextObject("{=kzgbBrYo}As you leave the hideout, {RADAGOS.LINK} comes to you and asks to talk.", null);
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
				base.AddGameMenu("radagos_goodbye_menu", textObject, new OnInitDelegate(this.radagos_goodbye_menu_on_init), 0, 0);
				base.AddGameMenuOption("radagos_goodbye_menu", "radagos_goodbye_menu_continue", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.radagos_goodbye_menu_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.radagos_goodbye_menu_continue_on_consequence), false, -1, null);
			}

			// Token: 0x0600063C RID: 1596 RVA: 0x00023829 File Offset: 0x00021A29
			private void radagos_goodbye_menu_on_init(MenuCallbackArgs args)
			{
			}

			// Token: 0x0600063D RID: 1597 RVA: 0x0002382B File Offset: 0x00021A2B
			private bool radagos_goodbye_menu_continue_on_condition(MenuCallbackArgs args)
			{
				args.optionLeaveType = 17;
				return true;
			}

			// Token: 0x0600063E RID: 1598 RVA: 0x00023838 File Offset: 0x00021A38
			private void radagos_goodbye_menu_continue_on_consequence(MenuCallbackArgs args)
			{
				CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(this._radagos.CharacterObject, null, true, true, false, false, false, false));
			}

			// Token: 0x0600063F RID: 1599 RVA: 0x00023872 File Offset: 0x00021A72
			[GameMenuInitializationHandler("radagos_goodbye_menu")]
			private static void quest_game_menus_on_init_background(MenuCallbackArgs args)
			{
				args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideout(null, null).Hideout.WaitMeshName);
			}

			// Token: 0x06000640 RID: 1600 RVA: 0x00023890 File Offset: 0x00021A90
			internal static void AutoGeneratedStaticCollectObjectsRescueFamilyQuest(object o, List<object> collectedObjects)
			{
				((RescueFamilyQuestBehavior.RescueFamilyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000641 RID: 1601 RVA: 0x0002389E File Offset: 0x00021A9E
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._hideout);
				collectedObjects.Add(this._raiderParties);
			}

			// Token: 0x06000642 RID: 1602 RVA: 0x000238BF File Offset: 0x00021ABF
			internal static object AutoGeneratedGetMemberValue_hideout(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideout;
			}

			// Token: 0x06000643 RID: 1603 RVA: 0x000238CC File Offset: 0x00021ACC
			internal static object AutoGeneratedGetMemberValue_reunionTalkDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._reunionTalkDone;
			}

			// Token: 0x06000644 RID: 1604 RVA: 0x000238DE File Offset: 0x00021ADE
			internal static object AutoGeneratedGetMemberValue_hideoutTalkDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideoutTalkDone;
			}

			// Token: 0x06000645 RID: 1605 RVA: 0x000238F0 File Offset: 0x00021AF0
			internal static object AutoGeneratedGetMemberValue_brotherConversationDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._brotherConversationDone;
			}

			// Token: 0x06000646 RID: 1606 RVA: 0x00023902 File Offset: 0x00021B02
			internal static object AutoGeneratedGetMemberValue_radagosGoodByeConversationDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._radagosGoodByeConversationDone;
			}

			// Token: 0x06000647 RID: 1607 RVA: 0x00023914 File Offset: 0x00021B14
			internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideoutBattleEndState;
			}

			// Token: 0x06000648 RID: 1608 RVA: 0x00023926 File Offset: 0x00021B26
			internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._raiderParties;
			}

			// Token: 0x04000224 RID: 548
			private const int RaiderPartySize = 10;

			// Token: 0x04000225 RID: 549
			private const int RaiderPartyCount = 2;

			// Token: 0x04000226 RID: 550
			private const string RescueFamilyRaiderPartyStringId = "rescue_family_quest_raider_party_";

			// Token: 0x04000227 RID: 551
			private Hero _radagos;

			// Token: 0x04000228 RID: 552
			private Hero _hideoutBoss;

			// Token: 0x04000229 RID: 553
			private Settlement _targetSettlementForSiblings;

			// Token: 0x0400022A RID: 554
			[SaveableField(1)]
			private readonly Settlement _hideout;

			// Token: 0x0400022B RID: 555
			[SaveableField(2)]
			private bool _reunionTalkDone;

			// Token: 0x0400022C RID: 556
			[SaveableField(3)]
			private bool _hideoutTalkDone;

			// Token: 0x0400022D RID: 557
			[SaveableField(4)]
			private bool _brotherConversationDone;

			// Token: 0x0400022E RID: 558
			[SaveableField(5)]
			private bool _radagosGoodByeConversationDone;

			// Token: 0x0400022F RID: 559
			[SaveableField(6)]
			private RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState _hideoutBattleEndState;

			// Token: 0x04000230 RID: 560
			[SaveableField(7)]
			private readonly List<MobileParty> _raiderParties;

			// Token: 0x02000096 RID: 150
			public class RebuildPlayerClanQuestBehaviorTypeDefiner : SaveableTypeDefiner
			{
				// Token: 0x060006DB RID: 1755 RVA: 0x00024AAC File Offset: 0x00022CAC
				public RebuildPlayerClanQuestBehaviorTypeDefiner()
					: base(4140000)
				{
				}

				// Token: 0x060006DC RID: 1756 RVA: 0x00024AB9 File Offset: 0x00022CB9
				protected override void DefineClassTypes()
				{
					base.AddClassDefinition(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest), 1, null);
				}

				// Token: 0x060006DD RID: 1757 RVA: 0x00024ACD File Offset: 0x00022CCD
				protected override void DefineEnumTypes()
				{
					base.AddEnumDefinition(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState), 10, null);
				}
			}

			// Token: 0x02000097 RID: 151
			private enum HideoutBattleEndState
			{
				// Token: 0x040002BD RID: 701
				None,
				// Token: 0x040002BE RID: 702
				Retreated,
				// Token: 0x040002BF RID: 703
				Defeated,
				// Token: 0x040002C0 RID: 704
				Victory
			}
		}
	}
}
