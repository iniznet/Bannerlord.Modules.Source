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
	public class RescueFamilyQuestBehavior : CampaignBehaviorBase
	{
		internal RescueFamilyQuestBehavior()
		{
			this._rescueFamilyQuestReadyToStart = false;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.CanHaveQuestsOrIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnCanHaveQuestsOrIssuesInfoIsRequested));
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_rescueFamilyQuestReadyToStart", ref this._rescueFamilyQuestReadyToStart);
		}

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

		private void OnCanHaveQuestsOrIssuesInfoIsRequested(Hero hero, ref bool result)
		{
			if (!StoryModeManager.Current.MainStoryLine.FamilyRescued && (hero == StoryModeHeroes.Radagos || hero == StoryModeHeroes.RadagosHencman))
			{
				result = false;
			}
		}

		private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			if (hero == StoryModeHeroes.RadagosHencman && (!StoryModeManager.Current.MainStoryLine.FamilyRescued || this._rescueFamilyQuestReadyToStart || (Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest)) && causeOfDeath != 6)))
			{
				result = false;
			}
		}

		private bool _rescueFamilyQuestReadyToStart;

		public class RescueFamilyQuest : StoryModeQuestBase
		{
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

			private TextObject _defeatedQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=Ga8mDgab}You've been defeated at {HIDEOUT_BOSS.LINK}'s hideout. You can attack again when you are ready.", null);
					StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _letGoRadagosEndQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=DjQuB0TU}You saved your brothers and sister, and you decided to let {RADAGOS.LINK} go...", null);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _executeRadagosEndQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ZpcV9MZE}You saved your brothers and sister, and you decided to execute {RADAGOS.LINK} because he caused the death of your parents.", null);
					StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=HPNuqbSf}Rescue Your Family", null);
				}
			}

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

			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == StoryModeHeroes.Radagos && StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && !StoryModeManager.Current.MainStoryLine.FamilyRescued)
				{
					result = false;
				}
			}

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

			protected override void OnFinalize()
			{
				base.OnFinalize();
				StoryModeManager.Current.MainStoryLine.BusyHideouts.Remove(this._hideout.Hideout);
			}

			private void InitializeHideout()
			{
				this.CheckIfHideoutIsReady();
				this._hideoutBattleEndState = RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState.None;
			}

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

			private MobileParty CreateRaiderParty(int number, bool isBanditBossParty)
			{
				MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("rescue_family_quest_raider_party_" + number, this._hideout.OwnerClan, this._hideout.Hideout, isBanditBossParty);
				TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
				CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(this._hideout.Culture.StringId + "_bandit");
				troopRoster.AddToCounts(@object, 5, false, 0, 0, true, -1);
				TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
				mobileParty.InitializeMobilePartyAtPosition(troopRoster, troopRoster2, this._hideout.Position2D);
				mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
				mobileParty.ActualClan = this._hideout.OwnerClan;
				mobileParty.Position2D = this._hideout.Position2D;
				mobileParty.Party.SetVisualAsDirty();
				float totalStrength = mobileParty.Party.TotalStrength;
				int num = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
				mobileParty.InitializePartyTrade(num);
				mobileParty.Ai.SetMoveGoToSettlement(this._hideout);
				mobileParty.Ai.SetDoNotMakeNewDecisions(true);
				mobileParty.SetPartyUsedByQuest(true);
				EnterSettlementAction.ApplyForParty(mobileParty, this._hideout);
				return mobileParty;
			}

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

			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			}

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

			protected override void HourlyTick()
			{
				this.CheckIfHideoutIsReady();
			}

			protected override void SetDialogs()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=1yi00v5w}{PLAYER.NAME}! Good to see you. Believe it or not, I mean that. I've been looking for you...[if:convo_calm_friendly][ib:normal2]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.radagos_reunion_conversation_condition))
					.PlayerLine(new TextObject("{=pCNSEPEP}You escaped? Where's my brother? What happened?", null), null)
					.NpcLine(new TextObject("{=xknCpvcb}Calm down, now. I'll tell you everything.[ib:closed2][if:convo_grave]", null), null, null)
					.NpcLine(new TextObject("{=X1TJNkBV}We found your little brother and sister. But my former partner {HIDEOUT_BOSS.LINK} betrayed me. We came into his camp to negotiate the kids' release, and he seized us right then and there.[if:convo_angry_voice]", null), null, null)
					.NpcLine(new TextObject("{=UpUqL368}What scum, eh? Even in this profession, double-crossing your comrades is frowned upon.", null), null, null)
					.NpcLine(new TextObject("{=bJjAqCxk}I escaped - one of his men, a little guiltier than the rest, cut my bonds when the others were sleeping - but I can't let a traitor live. So I decided to find you and offer you a deal.[if:convo_focused_voice][ib:hip]", null), null, null)
					.NpcLine(new TextObject("{=PlpNTQqf}I know where {HIDEOUT_BOSS.LINK} is now. If you agree, we can attack together and save your kin.", null), null, null)
					.NpcLine(new TextObject("{=mmQRCHUM}But in return, I will have the pleasure of killing that bastard. So what do you say?[if:convo_snide_voice][ib:confident2]", null), null, null)
					.PlayerLine(new TextObject("{=ypDmy5Rn}Uh, how can we possibly trust each other?", null), null)
					.NpcLine(new TextObject("{=VbJvL8yB}Oh you can't trust me. But you need me, and I figure you have enough men that you could easily slit my throat pretty quickly if I lead you into a trap. And I don't need to trust you - you're my vehicle of revenge, not my partner.[if:convo_grave]", null), null, null)
					.PlayerLine(new TextObject("{=ft6zzDrJ}I can live with that. Let's go.", null), null)
					.NpcLine(new TextObject("{=HT9hW29s}Splendid! But I have a few things to do. There is a hideout near this city. {HIDEOUT_BOSS.LINK} keeps your siblings there. I will join you right where the path leads up, just out of sight of their scouts.[if:convo_snide_voice][ib:hip]", null), null, null)
					.PlayerLine(new TextObject("{=GicEcLx2}See you there then. But, remember, if this is a trap or something, that will cost you your life.", null), null)
					.NpcLine(new TextObject("{=8b4Ndfep}Oh of course. I have no doubts on that score.[if:convo_nonchalant]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.radagos_reunion_conversation_consequence))
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=rDuegB1L}You've finally arrived! I have a few things to say before we attack.[ib:confident2][if:convo_nonchalant]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.radagos_hideout_conversation_condition))
					.NpcLine(new TextObject("{=1T7p0O7B}We have to be clever. {HIDEOUT_BOSS.LINK} is a cunning fellow, in a low and base kind of way.[if:convo_normal]", null), null, null)
					.PlayerLine(new TextObject("{=a29lmPLd}I defeated you before. I know how your gang operates. Less talking, more raiding. C'mon...", null), null)
					.NpcLine(new TextObject("{=QbsDYITB}That you did, that you did. Lead on, then.[ib:closed2][if:convo_calm_friendly]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.radagos_hideout_conversation_consequence))
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 160).NpcLine(new TextObject("{=PiKISvfu}{PLAYER.NAME}! I knew you'd come. Great Heaven. Damn, {?PLAYER.GENDER}sister{?}brother{\\?}, nothing can stop you! I love you, {?PLAYER.GENDER}sister{?}brother{\\?}.[if:convo_calm_friendly][ib:aggressive2]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.brother_hideout_conversation_condition))
					.PlayerLine(new TextObject("{=DIKPGwj1}So glad to see you safe. Is everyone okay?", null), null)
					.NpcLine(new TextObject("{=xachJ1hb}Yes, we are all fine. The little ones are scared but fine... We need to be quick and get the hell out of this place.[if:convo_calm_friendly][ib:confident]", null), null, null)
					.NpcLine(new TextObject("{=p3Kia1OO}I'll take them to the nearest fortress immediately. They will be safe there.", null), null, null)
					.NpcLine(new TextObject("{=IC9Vg5MA}Meet me there later, when you're ready to tell me everything.[if:convo_normal][ib:normal2]", null), null, null)
					.PlayerLine(new TextObject("{=LrItHItu}Okay brother, be careful. Take care.", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.brother_hideout_conversation_consequence;
					})
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=0I9siaQY}Bastards... You're the kin of my captives, right? I saw {RADAGOS.LINK} with you. You know he can't be trusted?[if:convo_confused_annoyed][ib:aggressive]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.bandit_hideout_boss_fight_start_on_condition))
					.PlayerLine(new TextObject("{=mWMkslbn}He led us here. Where are my brothers and my sister?", null), null)
					.NpcLine(new TextObject("{=heoCaRIr}Nah... There's no more talking. Kill me or I kill you, that's how this ends.[ib:warrior][if:convo_bared_teeth]", null), null, null)
					.NpcLine(new TextObject("{=2GeiKTlS}I'll do you the honor of duelling you, and my men will stand down if you win.[if:convo_predatory]", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=ImLQNYWC}Very well - I'll duel you.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_start_duel_fight_on_consequence))
					.CloseDialog()
					.PlayerOption(new TextObject("{=MMv3hsmI}I don't duel slavers. Men, attack!", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_continue_battle_on_consequence))
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=G9iXmhGK}Look, we can still talk. I'll give you a pouch of silver.[ib:weary][if:convo_confused_voice]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.hideout_boss_prisoner_talk_condition))
					.PlayerLine(new TextObject("{=fM4eSVps}You said talking was a waste of time. You are {RADAGOS.NAME}'s property, now.", null), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.hideout_boss_prisoner_talk_consequence;
					})
					.CloseDialog(), this);
				string text;
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=3dhvmJDp}Well... Looks like we've gotten your kin back to you, so my end of our deal is complete. I'll be making myself scarce now.[ib:hip][if:convo_uncomfortable_voice]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.goodbye_conversation_with_radagos_condition))
					.GetOutputToken(ref text)
					.NpcLine(new TextObject("{=C79Xxm1b}Don't let your conscience bother you about letting me go, by the way. I won't get back into slaving. Burned too many bridges with my old colleagues, you might say. I'll find some other way to earn my keep - mercenary work, perhaps. Anyway, maybe our paths will cross again.[if:convo_empathic_voice]", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=c1Q2irLi}Your men killed my parents. Did you really think you would not be punished?", null), null)
					.NpcLine(new TextObject("{=W7hi7jS4}Eh, well, I dared to hope, I suppose. All right then, I'm not going to grovel to you, so get it over with.[ib:hip][if:convo_uncomfortable_voice]", null), null, null)
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

			private bool radagos_reunion_conversation_condition()
			{
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
				StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, null, false);
				return !this._reunionTalkDone && Hero.OneToOneConversationHero == this._radagos;
			}

			private void radagos_reunion_conversation_consequence()
			{
				this._reunionTalkDone = true;
				base.AddLog(this._startQuestLogText, false);
			}

			private bool radagos_hideout_conversation_condition()
			{
				StringHelpers.SetCharacterProperties("HIDEOUT_BOSS", this._hideoutBoss.CharacterObject, null, false);
				return !this._hideoutTalkDone && Settlement.CurrentSettlement == this._hideout && Hero.OneToOneConversationHero == this._radagos;
			}

			private void radagos_hideout_conversation_consequence()
			{
				this._hideoutTalkDone = true;
				if (!PartyBase.MainParty.MemberRoster.Contains(this._radagos.CharacterObject))
				{
					PartyBase.MainParty.MemberRoster.AddToCounts(this._radagos.CharacterObject, 1, false, 0, 0, true, -1);
				}
				this.AddRadagosHencmanToHideout();
			}

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

			private void brother_hideout_conversation_consequence()
			{
				this._brotherConversationDone = true;
			}

			private bool bandit_hideout_boss_fight_start_on_condition()
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				if (encounteredParty == null || encounteredParty.IsMobile || encounteredParty.MapFaction == null || !encounteredParty.MapFaction.IsBanditFaction)
				{
					return false;
				}
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, null, false);
				return encounteredParty.IsSettlement && encounteredParty.Settlement.IsHideout && encounteredParty.Settlement == this._hideout && Mission.Current != null && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == this._hideoutBoss;
			}

			private void bandit_hideout_start_duel_fight_on_consequence()
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
			}

			private void bandit_hideout_continue_battle_on_consequence()
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
			}

			private bool hideout_boss_prisoner_talk_condition()
			{
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, null, false);
				return Hero.OneToOneConversationHero == this._hideoutBoss;
			}

			private void hideout_boss_prisoner_talk_consequence()
			{
				if (this._hideoutBoss.IsAlive)
				{
					MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForInformingPlayer(this._radagos, this._hideoutBoss, 4));
				}
			}

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

			private void execute_radagos_consequence()
			{
				base.AddLog(this._executeRadagosEndQuestLogText, false);
				this._brotherConversationDone = false;
				MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForInformingPlayer(Hero.MainHero, this._radagos, 4));
				this._radagosGoodByeConversationDone = true;
			}

			private void let_go_radagos_consequence()
			{
				base.AddLog(this._letGoRadagosEndQuestLogText, false);
				this._brotherConversationDone = false;
				DisableHeroAction.Apply(this._radagos);
				this._radagosGoodByeConversationDone = true;
			}

			private void AddGameMenus()
			{
				TextObject textObject = new TextObject("{=kzgbBrYo}As you leave the hideout, {RADAGOS.LINK} comes to you and asks to talk.", null);
				StringHelpers.SetCharacterProperties("RADAGOS", this._radagos.CharacterObject, textObject, false);
				base.AddGameMenu("radagos_goodbye_menu", textObject, new OnInitDelegate(this.radagos_goodbye_menu_on_init), 0, 0);
				base.AddGameMenuOption("radagos_goodbye_menu", "radagos_goodbye_menu_continue", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.radagos_goodbye_menu_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.radagos_goodbye_menu_continue_on_consequence), false, -1);
			}

			private void radagos_goodbye_menu_on_init(MenuCallbackArgs args)
			{
			}

			private bool radagos_goodbye_menu_continue_on_condition(MenuCallbackArgs args)
			{
				args.optionLeaveType = 17;
				return true;
			}

			private void radagos_goodbye_menu_continue_on_consequence(MenuCallbackArgs args)
			{
				CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(this._radagos.CharacterObject, null, true, true, false, false, false, false));
			}

			[GameMenuInitializationHandler("radagos_goodbye_menu")]
			private static void quest_game_menus_on_init_background(MenuCallbackArgs args)
			{
				args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideout(null, null).Hideout.WaitMeshName);
			}

			internal static void AutoGeneratedStaticCollectObjectsRescueFamilyQuest(object o, List<object> collectedObjects)
			{
				((RescueFamilyQuestBehavior.RescueFamilyQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._hideout);
				collectedObjects.Add(this._raiderParties);
			}

			internal static object AutoGeneratedGetMemberValue_hideout(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideout;
			}

			internal static object AutoGeneratedGetMemberValue_reunionTalkDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._reunionTalkDone;
			}

			internal static object AutoGeneratedGetMemberValue_hideoutTalkDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideoutTalkDone;
			}

			internal static object AutoGeneratedGetMemberValue_brotherConversationDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._brotherConversationDone;
			}

			internal static object AutoGeneratedGetMemberValue_radagosGoodByeConversationDone(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._radagosGoodByeConversationDone;
			}

			internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._hideoutBattleEndState;
			}

			internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
			{
				return ((RescueFamilyQuestBehavior.RescueFamilyQuest)o)._raiderParties;
			}

			private const int RaiderPartySize = 10;

			private const int RaiderPartyCount = 2;

			private const string RescueFamilyRaiderPartyStringId = "rescue_family_quest_raider_party_";

			private Hero _radagos;

			private Hero _hideoutBoss;

			private Settlement _targetSettlementForSiblings;

			[SaveableField(1)]
			private readonly Settlement _hideout;

			[SaveableField(2)]
			private bool _reunionTalkDone;

			[SaveableField(3)]
			private bool _hideoutTalkDone;

			[SaveableField(4)]
			private bool _brotherConversationDone;

			[SaveableField(5)]
			private bool _radagosGoodByeConversationDone;

			[SaveableField(6)]
			private RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState _hideoutBattleEndState;

			[SaveableField(7)]
			private readonly List<MobileParty> _raiderParties;

			public class RebuildPlayerClanQuestBehaviorTypeDefiner : SaveableTypeDefiner
			{
				public RebuildPlayerClanQuestBehaviorTypeDefiner()
					: base(4140000)
				{
				}

				protected override void DefineClassTypes()
				{
					base.AddClassDefinition(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest), 1, null);
				}

				protected override void DefineEnumTypes()
				{
					base.AddEnumDefinition(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest.HideoutBattleEndState), 10, null);
				}
			}

			private enum HideoutBattleEndState
			{
				None,
				Retreated,
				Defeated,
				Victory
			}
		}
	}
}
