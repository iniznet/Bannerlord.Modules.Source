using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Missions.MissionLogics;
using StoryMode.GameComponents.CampaignBehaviors;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
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
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.TutorialPhase
{
	// Token: 0x0200001D RID: 29
	public class FindHideoutTutorialQuest : StoryModeQuestBase
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00005CF8 File Offset: 0x00003EF8
		private TextObject _startQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=gSBGpUBm}Find {RADAGOS.LINK}' hideout.", null);
				StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00005D2C File Offset: 0x00003F2C
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=NvkWtb8f}Find the hideout of {RADAGOS.NAME}' gang and defeat them", null);
				StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, textObject, false);
				return textObject;
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00005D60 File Offset: 0x00003F60
		public FindHideoutTutorialQuest(Hero questGiver, Settlement hideout)
			: base("find_hideout_tutorial_quest", questGiver, CampaignTime.Never)
		{
			this._hideout = hideout;
			FindHideoutTutorialQuest._activeHideoutStringId = this._hideout.StringId;
			this._hideout.Name = new TextObject("{=9xaEPyNV}{RADAGOS.NAME}' Hideout", null);
			StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, this._hideout.Name, false);
			this._raiderParties = new List<MobileParty>();
			this._foughtWithRadagos = false;
			this._talkedWithRadagos = false;
			this._talkedWithBrother = false;
			this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.None;
			this.InitializeHideout();
			base.AddTrackedObject(this._hideout);
			this.SetDialogs();
			this.AddGameMenus();
			base.InitializeQuestOnCreation();
			base.AddLog(this._startQuestLog, false);
			TutorialPhase.Instance.SetTutorialFocusSettlement(this._hideout);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00005E38 File Offset: 0x00004038
		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005EA1 File Offset: 0x000040A1
		public override void OnHeroCanDieInfoIsRequested(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			if (hero == StoryModeHeroes.Radagos)
			{
				result = false;
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005EAE File Offset: 0x000040AE
		public override void OnHeroCanBeSelectedInInventoryInfoIsRequested(Hero hero, ref bool result)
		{
			if (hero == StoryModeHeroes.Radagos)
			{
				result = false;
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005EBB File Offset: 0x000040BB
		public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
		{
			if (hero == StoryModeHeroes.Radagos)
			{
				result = false;
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00005EC8 File Offset: 0x000040C8
		protected override void InitializeQuestOnGameLoad()
		{
			FindHideoutTutorialQuest._activeHideoutStringId = this._hideout.StringId;
			this._hideout.Name = new TextObject("{=9xaEPyNV}{RADAGOS.NAME}'s Hideout", null);
			StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, this._hideout.Name, false);
			this.SetDialogs();
			this.AddGameMenus();
			if (this._raiderParties.Count > 2)
			{
				for (int i = this._raiderParties.Count - 1; i >= 0; i--)
				{
					if (this._raiderParties[i].MapEvent == null && !this._raiderParties[i].IsActive)
					{
						this._raiderParties.Remove(this._raiderParties[i]);
					}
				}
				for (int j = this._raiderParties.Count - 1; j >= 0; j--)
				{
					if (!this._raiderParties[j].IsBanditBossParty && this._raiderParties[j].MapEvent == null)
					{
						if (!this._raiderParties[j].IsActive)
						{
							this._raiderParties.Remove(this._raiderParties[j]);
						}
						else
						{
							DestroyPartyAction.Apply(null, this._raiderParties[j]);
						}
					}
					if (this._raiderParties.Count <= 2)
					{
						break;
					}
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000601C File Offset: 0x0000421C
		protected override void OnStartQuest()
		{
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.StartCameraAnimation(this._hideout.GatePosition, 1f);
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006058 File Offset: 0x00004258
		private void InitializeHideout()
		{
			this._hideout.Hideout.IsSpotted = true;
			this._hideout.IsVisible = true;
			if (!this._hideout.Hideout.IsInfested)
			{
				int num = 0;
				while (num < 2 && !this._hideout.Hideout.IsInfested)
				{
					this._raiderParties.Add(this.CreateRaiderParty(this._raiderParties.Count + 1, false));
					num++;
				}
			}
			if (!this._hideout.Parties.Any((MobileParty p) => p.IsBanditBossParty))
			{
				this._raiderParties.Add(this.CreateRaiderParty(this._raiderParties.Count + 1, true));
			}
			foreach (MobileParty mobileParty in this._hideout.Parties)
			{
				if (mobileParty.IsBanditBossParty)
				{
					int totalRegulars = mobileParty.MemberRoster.TotalRegulars;
					mobileParty.MemberRoster.Clear();
					mobileParty.MemberRoster.AddToCounts(StoryModeHeroes.Radagos.CharacterObject, 1, false, 0, 0, true, -1);
					CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("storymode_quest_raider");
					mobileParty.MemberRoster.AddToCounts(@object, totalRegulars, false, 0, 0, true, -1);
					StoryModeHeroes.Radagos.Heal(100, false);
					break;
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000061DC File Offset: 0x000043DC
		private MobileParty CreateRaiderParty(int number, bool isBanditBossParty)
		{
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty("radagos_raider_party_" + number, StoryModeHeroes.RadiersClan, this._hideout.Hideout, isBanditBossParty);
			TroopRoster troopRoster = new TroopRoster(mobileParty.Party);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("storymode_quest_raider");
			int num = 4 - troopRoster.TotalManCount;
			if (num > 0)
			{
				troopRoster.AddToCounts(@object, num, false, 0, 0, true, -1);
			}
			TroopRoster troopRoster2 = new TroopRoster(mobileParty.Party);
			mobileParty.InitializeMobilePartyAtPosition(troopRoster, troopRoster2, this._hideout.Position2D);
			mobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
			mobileParty.ActualClan = StoryModeHeroes.RadiersClan;
			mobileParty.Position2D = this._hideout.Position2D;
			mobileParty.Party.SetCustomOwner(StoryModeHeroes.Radagos);
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			EnterSettlementAction.ApplyForParty(mobileParty, this._hideout);
			float totalStrength = mobileParty.Party.TotalStrength;
			int num2 = (int)(1f * MBRandom.RandomFloat * 20f * totalStrength + 50f);
			mobileParty.InitializePartyTrade(num2);
			mobileParty.Ai.SetMoveGoToSettlement(this._hideout);
			EnterSettlementAction.ApplyForParty(mobileParty, this._hideout);
			mobileParty.SetPartyUsedByQuest(true);
			return mobileParty;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000631C File Offset: 0x0000451C
		protected override void SetDialogs()
		{
			StringHelpers.SetCharacterProperties("TACTEOS", StoryModeHeroes.Tacitus.CharacterObject, null, false);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000015).NpcLine(new TextObject("{=R3CnF55p}So... Who's this that comes through my place of business, killing my employees?[if:idle_angry][ib:warrior]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.bandit_hideout_boss_fight_start_on_condition))
				.PlayerLine(new TextObject("{=itRoeaJf}We heard you took our little brother and sister. Where are they?", null), null)
				.NpcLine(new TextObject("{=eVgAY7ts}Good Heaven, I'll need a better description than that. My men have harvested dozens of little brats in this region. Quite good hunting grounds! Already sent most of them south to a slave market I know, though.[ib:confident]", null), null, null)
				.NpcLine(new TextObject("{=wWLnZ6G4}Since your hunt for your kin is fruitless, how about you clear off and save your own lives? Either that or I force you to lick up all the blood you've spilled here with your tongues. Or... You and I could settle this, one on one.[ib:confident]", null), null, null)
				.BeginPlayerOptions()
				.PlayerOption(new TextObject("{=ImLQNYWC}Very well - I'll duel you.", null), null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_start_duel_fight_on_consequence))
				.CloseDialog()
				.PlayerOption(new TextObject("{=MMv3hsmI}I don't duel slavers. Men, attack!", null), null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_continue_battle_on_consequence))
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000010).NpcLine(new TextObject("{=ZhZ7MCeh}Well. I recognize defeat when I see it. If I'm going to be your captive, let me introduce myself. I'm Radagos.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.radagos_meeting_conversation_condition))
				.NpcLine(new TextObject("{=w0CUaEU7}You haven't cut my throat yet, which was a wise move. I'm sure I can find a way to be worth more to you alive than dead.", null), null, null)
				.PlayerLine(new TextObject("{=vDRRsed8}You'd better help us get our brother and sister back, or you'll swing from a tree.", null), null)
				.NpcLine(new TextObject("{=7O8IwMgU}Oh, you'll need my help all right, if you want to get them back - alive, that is. See, my boys have some pretty specific instructions about what to do if there's a rescue attempt...", null), null, null)
				.NpcLine(new TextObject("{=FWSwngVX}Shall we get on the road? Remember - if I drop dead of exhaustion, or drown in some river, that's it for your little dears. I don't expect a cozy palanquin, now, but you'd best not make it too hard a trip for me.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.radagos_meeting_conversation_consequence))
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1000020).NpcLine(new TextObject("{=qp2zYfua}I was hoping to find more treasure here, but I think business wasn't going too well for {RADAGOS.NAME} and his gang.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.brother_farewell_conversation_condition))
				.NpcLine(new TextObject("{=J4qetbZb}I found this strange looking metal piece though. It doesn't look too valuable, but it could be the artifact {TACTEOS.NAME} was talking about. Maybe we can sell it to one of the noble clans for a hefty price.", null), null, null)
				.PlayerLine(new TextObject("{=OffNcRby}All right then. Let's get on the road.", null), null)
				.NpcLine(new TextObject("{=j9f9Ts7i}I have a better idea. We would have a better chance if we split up now. I'll take {RADAGOS.NAME} and go find the slaver market and look for a way to free the children. However we must be careful not to endanger their lives and it could be better to just buy them. We need to have our purses full for that though.", null), null, null)
				.NpcLine(new TextObject("{=fp6QBO7l}I'll need to take these men with us. {RADAGOS.NAME} is a slippery one. I don't want him getting away.", null), null, null)
				.PlayerLine(new TextObject("{=RJ9NbuYr}So you want me to raise the money to ransom the little ones?", null), null)
				.NpcLine(new TextObject("{=4OUnPjZc}Indeed. You'll have to find a way to do that. Maybe this bronze thing can help.", null), null, null)
				.NpcLine(new TextObject("{=5soUEFEJ}{TACTEOS.NAME} said it could be worth a fortune to the right person, if you manage not to get killed. If he's telling the truth, you must be careful. Never reveal that you have it. Try to understand its value, and how it can be sold.", null), null, null)
				.NpcLine(new TextObject("{=jPKIN2r4}One more thing. When you are talking to nobles and other people of importance, make sure you present yourself as someone from a distant but distinguished family.", null), null, null)
				.NpcLine(new TextObject("{=GVMGXfxS}You can use our family name if you like or make up a new one. You will have a better chance of obtaining an audience with nobles and it'll be easier for me to find you by asking around.", null), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.SelectClanName))
				.NpcLine(new TextObject("{=qIltCuBe}Get on the road now. Once I locate the little ones, I'll come find you.", null), null, null)
				.Condition(new ConversationSentence.OnConditionDelegate(this.OpenBannerSelectionScreen))
				.CloseDialog(), this);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000065D0 File Offset: 0x000047D0
		private bool bandit_hideout_boss_fight_start_on_condition()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			return encounteredParty != null && !encounteredParty.IsMobile && encounteredParty.MapFaction.IsBanditFaction && (!this._foughtWithRadagos && encounteredParty.MapFaction.IsBanditFaction && encounteredParty.IsSettlement && encounteredParty.Settlement == this._hideout && Mission.Current != null && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.Radagos) && encounteredParty.Settlement.IsHideout;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000665B File Offset: 0x0000485B
		private void bandit_hideout_start_duel_fight_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
			this._dueledRadagos = true;
			this._foughtWithRadagos = true;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006686 File Offset: 0x00004886
		private void bandit_hideout_continue_battle_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
			this._foughtWithRadagos = true;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000066AA File Offset: 0x000048AA
		private bool radagos_meeting_conversation_condition()
		{
			return this._foughtWithRadagos && Hero.OneToOneConversationHero == StoryModeHeroes.Radagos;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000066C2 File Offset: 0x000048C2
		private void radagos_meeting_conversation_consequence()
		{
			StoryModeHeroes.Radagos.SetHasMet();
			DisableHeroAction.Apply(StoryModeHeroes.Radagos);
			this._talkedWithRadagos = true;
			Campaign.Current.ConversationManager.ConversationEndOneShot += this.OpenBrotherConversationMenu;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000066FA File Offset: 0x000048FA
		private void OpenBrotherConversationMenu()
		{
			GameMenu.ActivateGameMenu("brother_chest_menu");
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00006708 File Offset: 0x00004908
		private bool brother_farewell_conversation_condition()
		{
			StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, null, false);
			StringHelpers.SetCharacterProperties("TACTEOS", StoryModeHeroes.Tacitus.CharacterObject, null, false);
			return Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother && this._talkedWithRadagos;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00006758 File Offset: 0x00004958
		private void SelectClanName()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=JJiKk4ow}Select your family name: ", null).ToString(), string.Empty, true, false, GameTexts.FindText("str_done", null).ToString(), null, new Action<string>(this.OnChangeClanNameDone), null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsClanNameApplicable), "", Clan.PlayerClan.Name.ToString()), false, false);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000067C8 File Offset: 0x000049C8
		private void OnChangeClanNameDone(string newClanName)
		{
			TextObject textObject = GameTexts.FindText("str_generic_clan_name", null);
			textObject.SetTextVariable("CLAN_NAME", new TextObject(newClanName, null));
			Clan.PlayerClan.InitializeClan(textObject, textObject, Clan.PlayerClan.Culture, Clan.PlayerClan.Banner, default(Vec2), false);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000681E File Offset: 0x00004A1E
		private bool OpenBannerSelectionScreen()
		{
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>(), 0);
			return true;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00006840 File Offset: 0x00004A40
		private void OnGameMenuOpened(MenuCallbackArgs menuCallbackArgs)
		{
			StoryModeHeroes.Radagos.Heal(StoryModeHeroes.Radagos.MaxHitPoints, false);
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._hideout && this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.None && menuCallbackArgs.MenuContext.GameMenu.StringId != "radagos_hideout" && menuCallbackArgs.MenuContext.GameMenu.StringId != "brother_chest_menu")
			{
				GameMenu.SwitchToMenu("radagos_hideout");
			}
			if (this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Victory && this._talkedWithRadagos && menuCallbackArgs.MenuContext.GameMenu.StringId != "brother_chest_menu")
			{
				Campaign.Current.GameMenuManager.SetNextMenu("brother_chest_menu");
				return;
			}
			if (this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Defeated || this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Retreated)
			{
				foreach (MobileParty mobileParty in this._hideout.Parties)
				{
					foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
					{
						if (troopRosterElement.Character.IsHero)
						{
							troopRosterElement.Character.HeroObject.Heal(50 - troopRosterElement.Character.HeroObject.HitPoints, false);
						}
						else
						{
							int elementWoundedNumber = mobileParty.MemberRoster.GetElementWoundedNumber(mobileParty.MemberRoster.FindIndexOfTroop(troopRosterElement.Character));
							if (elementWoundedNumber > 0)
							{
								mobileParty.MemberRoster.AddToCounts(troopRosterElement.Character, 0, false, -elementWoundedNumber, 0, true, -1);
							}
						}
					}
					if (!mobileParty.IsBanditBossParty && mobileParty.MemberRoster.TotalManCount < 4)
					{
						int totalManCount = mobileParty.MemberRoster.TotalManCount;
						CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("storymode_quest_raider");
						mobileParty.MemberRoster.AddToCounts(@object, 4 - totalManCount, false, 0, 0, true, -1);
					}
					if (MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.1.1", 17949) && mobileParty.IsBanditBossParty && mobileParty.MemberRoster.GetTroopCount(StoryModeHeroes.Radagos.CharacterObject) <= 0)
					{
						mobileParty.MemberRoster.AddToCounts(StoryModeHeroes.Radagos.CharacterObject, 1, false, 0, 0, true, -1);
					}
				}
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
					Hero elderBrother = StoryModeHeroes.ElderBrother;
					if (elderBrother.PartyBelongedToAsPrisoner != null)
					{
						EndCaptivityAction.ApplyByPeace(elderBrother, null);
					}
					if (!elderBrother.IsActive)
					{
						elderBrother.ChangeState(1);
					}
					if (elderBrother.PartyBelongedTo == null)
					{
						PartyBase.MainParty.MemberRoster.AddToCounts(elderBrother.CharacterObject, 1, false, 0, 0, true, -1);
					}
				}
				if (this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Defeated || this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Retreated)
				{
					TextObject textObject = new TextObject("{=Zq9qXcCk}You are defeated by the {RADAGOS.NAME}' Party, but your brother saved you. It doesn't look like they're going anywhere, though, so you should attack again once you're ready. You must have at least {NUMBER} members in your party. If you don't, go back to {QUEST_VILLAGE} and recruit some more troops.", null);
					StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, textObject, false);
					textObject.SetTextVariable("NUMBER", 4);
					textObject.SetTextVariable("QUEST_VILLAGE", Settlement.Find("village_ES3_2").Name);
					InformationManager.ShowInquiry(new InquiryData(((this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Defeated) ? new TextObject("{=FPhWhjq7}Defeated", null) : new TextObject("{=w6Wa3lSL}Retreated", null)).ToString(), textObject.ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, delegate
					{
						this._hideout.Hideout.IsSpotted = true;
						this._hideout.IsVisible = true;
					}, null, "", 0f, null, null, null), false, false);
				}
				if (menuCallbackArgs.MenuContext.GameMenu.StringId == "radagos_hideout" && this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Retreated)
				{
					PlayerEncounter.Finish(true);
				}
				if (this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Defeated || this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Retreated)
				{
					if (Hero.MainHero.HitPoints < 50)
					{
						Hero.MainHero.Heal(50 - Hero.MainHero.HitPoints, false);
					}
					Hero elderBrother2 = StoryModeHeroes.ElderBrother;
					if (elderBrother2.HitPoints < 50)
					{
						elderBrother2.Heal(50 - elderBrother2.HitPoints, false);
					}
					if (elderBrother2.PartyBelongedToAsPrisoner != null)
					{
						EndCaptivityAction.ApplyByPeace(elderBrother2, null);
					}
					if (elderBrother2.PartyBelongedTo == null)
					{
						PartyBase.MainParty.MemberRoster.AddToCounts(elderBrother2.CharacterObject, 1, false, 0, 0, true, -1);
					}
				}
				this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.None;
				this._foughtWithRadagos = false;
				foreach (MobileParty mobileParty2 in this._hideout.Parties)
				{
					foreach (TroopRosterElement troopRosterElement2 in mobileParty2.PrisonRoster.GetTroopRoster())
					{
						if (this._mainPartyTroopBackup.Contains(troopRosterElement2.Character))
						{
							int num = mobileParty2.PrisonRoster.FindIndexOfTroop(troopRosterElement2.Character);
							int elementWoundedNumber2 = mobileParty2.PrisonRoster.GetElementWoundedNumber(num);
							int num2 = mobileParty2.PrisonRoster.GetTroopCount(troopRosterElement2.Character) - elementWoundedNumber2;
							if (num2 > 0)
							{
								mobileParty2.PrisonRoster.AddToCounts(troopRosterElement2.Character, -num2, false, 0, 0, true, -1);
								PartyBase.MainParty.MemberRoster.AddToCounts(troopRosterElement2.Character, num2, false, 0, 0, true, -1);
							}
						}
					}
				}
				List<CharacterObject> mainPartyTroopBackup = this._mainPartyTroopBackup;
				if (mainPartyTroopBackup == null)
				{
					return;
				}
				mainPartyTroopBackup.Clear();
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006E1C File Offset: 0x0000501C
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.Victory && !this._talkedWithRadagos)
			{
				CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, false, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.Radagos.CharacterObject, null, true, true, false, false, false, false), "", "");
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006E71 File Offset: 0x00005071
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._raiderParties.Contains(mobileParty))
			{
				this._raiderParties.Remove(mobileParty);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006E90 File Offset: 0x00005090
		private void OnGameLoadFinished()
		{
			for (int i = this._hideout.Parties.Count - 1; i >= 0; i--)
			{
				MobileParty mobileParty = this._hideout.Parties[i];
				if (mobileParty.IsBandit && mobileParty.MapEvent == null)
				{
					while (mobileParty.MemberRoster.TotalManCount > 4)
					{
						foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
						{
							if (!troopRosterElement.Character.IsHero)
							{
								mobileParty.MemberRoster.RemoveTroop(troopRosterElement.Character, 1, default(UniqueTroopDescriptor), 0);
							}
							if (mobileParty.MemberRoster.TotalManCount <= 4)
							{
								break;
							}
						}
					}
				}
			}
			while (this._hideout.Party.MemberRoster.TotalManCount > 4 && this._hideout.Party.MapEvent == null)
			{
				foreach (TroopRosterElement troopRosterElement2 in this._hideout.Party.MemberRoster.GetTroopRoster())
				{
					if (!troopRosterElement2.Character.IsHero)
					{
						this._hideout.Party.MemberRoster.RemoveTroop(troopRosterElement2.Character, 1, default(UniqueTroopDescriptor), 0);
					}
					if (this._hideout.Party.MemberRoster.TotalManCount <= 4)
					{
						break;
					}
				}
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007040 File Offset: 0x00005240
		private void AddGameMenus()
		{
			StringHelpers.SetCharacterProperties("TACTEOS", StoryModeHeroes.Tacitus.CharacterObject, null, false);
			StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, null, false);
			base.AddGameMenu("radagos_hideout", new TextObject("{=z8LQn2Uh}You have arrived at the hideout.", null), new OnInitDelegate(this.radagos_hideout_menu_on_init), 0, 0);
			base.AddGameMenuOption("radagos_hideout", "enter_hideout", new TextObject("{=zxMOqlhs}Attack", null), new GameMenuOption.OnConditionDelegate(this.enter_radagos_hideout_condition), new GameMenuOption.OnConsequenceDelegate(this.enter_radagos_hideout_on_consequence), false, -1, null);
			base.AddGameMenuOption("radagos_hideout", "leave_hideout", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.leave_radagos_hideout_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_radagos_hideout_on_consequence), true, -1, null);
			base.AddGameMenu("brother_chest_menu", new TextObject("{=bhQ6Jbom}You come across a chest with an old piece of bronze in it. It's so battered and corroded that it could have been anything from a cup to a crown. This must be the chest {TACTEOS.NAME} mentioned to you, that had something to do with 'Neretzes' Folly'.", null), new OnInitDelegate(this.brother_chest_menu_on_init), 0, 0);
			base.AddGameMenuOption("brother_chest_menu", "brother_chest_menu_continue", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.brother_chest_menu_on_condition), new GameMenuOption.OnConsequenceDelegate(this.brother_chest_menu_on_consequence), false, -1, null);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00007165 File Offset: 0x00005365
		private void brother_chest_menu_on_init(MenuCallbackArgs menuCallbackArgs)
		{
			if (this._talkedWithBrother)
			{
				this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.None;
				PlayerEncounter.Finish(true);
				base.CompleteQuestWithSuccess();
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007182 File Offset: 0x00005382
		private bool brother_chest_menu_on_condition(MenuCallbackArgs menuCallbackArgs)
		{
			menuCallbackArgs.optionLeaveType = 17;
			return base.IsOngoing;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007194 File Offset: 0x00005394
		private void brother_chest_menu_on_consequence(MenuCallbackArgs menuCallbackArgs)
		{
			this._talkedWithBrother = true;
			CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, false, false, false, false, false), new ConversationCharacterData(StoryModeHeroes.ElderBrother.CharacterObject, null, true, true, false, false, false, false), "", "");
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000071E0 File Offset: 0x000053E0
		private void radagos_hideout_menu_on_init(MenuCallbackArgs menuCallbackArgs)
		{
			menuCallbackArgs.MenuTitle = new TextObject("{=8OIwHZF1}Hideout", null);
			StringHelpers.SetCharacterProperties("RADAGOS", StoryModeHeroes.Radagos.CharacterObject, null, false);
			if (PlayerEncounter.Current != null)
			{
				MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
				if (playerMapEvent != null)
				{
					if (playerMapEvent.WinningSide == playerMapEvent.PlayerSide)
					{
						if (this._dueledRadagos)
						{
							AchievementsCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<AchievementsCampaignBehavior>();
							if (behavior != null)
							{
								behavior.OnRadagosDuelWon();
							}
						}
						this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.Victory;
					}
					else if (playerMapEvent.WinningSide == -1)
					{
						this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.Retreated;
					}
					else
					{
						this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.Defeated;
					}
					this._dueledRadagos = false;
				}
				if (this._hideoutBattleEndState != FindHideoutTutorialQuest.HideoutBattleEndState.None)
				{
					PlayerEncounter.Update();
				}
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000728C File Offset: 0x0000548C
		private bool enter_radagos_hideout_condition(MenuCallbackArgs menuCallbackArgs)
		{
			menuCallbackArgs.optionLeaveType = 1;
			if (MobileParty.MainParty.MemberRoster.TotalManCount < 4)
			{
				menuCallbackArgs.IsEnabled = false;
				menuCallbackArgs.Tooltip = new TextObject("{=kaZ1XtDX}You are not strong enough to attack. Recruit more troops from the village.", null);
			}
			return base.IsOngoing && this._hideoutBattleEndState == FindHideoutTutorialQuest.HideoutBattleEndState.None;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000072E0 File Offset: 0x000054E0
		private void enter_radagos_hideout_on_consequence(MenuCallbackArgs menuCallbackArgs)
		{
			this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.None;
			this._mainPartyTroopBackup = new List<CharacterObject>();
			foreach (TroopRosterElement troopRosterElement in PartyBase.MainParty.MemberRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsHero)
				{
					this._mainPartyTroopBackup.Add(troopRosterElement.Character);
				}
			}
			if (!this._hideout.Hideout.IsInfested || this._hideout.Parties.Count < 3)
			{
				this.InitializeHideout();
			}
			foreach (MobileParty mobileParty in this._hideout.Parties)
			{
				if (mobileParty.IsBanditBossParty && mobileParty.MemberRoster.Contains(mobileParty.Party.Culture.BanditBoss))
				{
					mobileParty.MemberRoster.RemoveTroop(mobileParty.Party.Culture.BanditBoss, 1, default(UniqueTroopDescriptor), 0);
				}
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
				PlayerEncounter.Update();
			}
			CampaignMission.OpenHideoutBattleMission(Settlement.CurrentSettlement.Hideout.SceneName, null);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007444 File Offset: 0x00005644
		private bool leave_radagos_hideout_condition(MenuCallbackArgs menuCallbackArgs)
		{
			menuCallbackArgs.optionLeaveType = 16;
			return base.IsOngoing;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007454 File Offset: 0x00005654
		private void leave_radagos_hideout_on_consequence(MenuCallbackArgs menuCallbackArgs)
		{
			this._hideoutBattleEndState = FindHideoutTutorialQuest.HideoutBattleEndState.None;
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007464 File Offset: 0x00005664
		[GameMenuInitializationHandler("radagos_hideout")]
		[GameMenuInitializationHandler("brother_chest_menu")]
		private static void quest_game_menus_on_init_background(MenuCallbackArgs args)
		{
			Settlement settlement = Settlement.Find(FindHideoutTutorialQuest._activeHideoutStringId);
			args.MenuContext.SetBackgroundMeshName(settlement.Hideout.WaitMeshName);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00007494 File Offset: 0x00005694
		protected override void OnCompleteWithSuccess()
		{
			this._hideout.Name = new TextObject("{=8OIwHZF1}Hideout", null);
			this._hideout.Party.Visuals.SetMapIconAsDirty();
			StoryModeHeroes.Radagos.Heal(100, false);
			StoryModeManager.Current.MainStoryLine.CompleteTutorialPhase(false);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000074E9 File Offset: 0x000056E9
		internal static void AutoGeneratedStaticCollectObjectsFindHideoutTutorialQuest(object o, List<object> collectedObjects)
		{
			((FindHideoutTutorialQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000074F7 File Offset: 0x000056F7
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._hideout);
			collectedObjects.Add(this._raiderParties);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007518 File Offset: 0x00005718
		internal static object AutoGeneratedGetMemberValue_hideout(object o)
		{
			return ((FindHideoutTutorialQuest)o)._hideout;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00007525 File Offset: 0x00005725
		internal static object AutoGeneratedGetMemberValue_raiderParties(object o)
		{
			return ((FindHideoutTutorialQuest)o)._raiderParties;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00007532 File Offset: 0x00005732
		internal static object AutoGeneratedGetMemberValue_talkedWithRadagos(object o)
		{
			return ((FindHideoutTutorialQuest)o)._talkedWithRadagos;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00007544 File Offset: 0x00005744
		internal static object AutoGeneratedGetMemberValue_talkedWithBrother(object o)
		{
			return ((FindHideoutTutorialQuest)o)._talkedWithBrother;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007556 File Offset: 0x00005756
		internal static object AutoGeneratedGetMemberValue_hideoutBattleEndState(object o)
		{
			return ((FindHideoutTutorialQuest)o)._hideoutBattleEndState;
		}

		// Token: 0x0400006D RID: 109
		private const string RaiderPartyStringId = "radagos_raider_party_";

		// Token: 0x0400006E RID: 110
		private const int RaiderPartyCount = 2;

		// Token: 0x0400006F RID: 111
		private const int RaiderPartySize = 4;

		// Token: 0x04000070 RID: 112
		private const int MainPartyHealHitPointLimit = 50;

		// Token: 0x04000071 RID: 113
		private const int MaximumHealth = 100;

		// Token: 0x04000072 RID: 114
		private const int PlayerPartySizeMinLimitToAttack = 4;

		// Token: 0x04000073 RID: 115
		[SaveableField(1)]
		private readonly Settlement _hideout;

		// Token: 0x04000074 RID: 116
		[SaveableField(2)]
		private List<MobileParty> _raiderParties;

		// Token: 0x04000075 RID: 117
		private bool _foughtWithRadagos;

		// Token: 0x04000076 RID: 118
		private bool _dueledRadagos;

		// Token: 0x04000077 RID: 119
		[SaveableField(4)]
		private bool _talkedWithRadagos;

		// Token: 0x04000078 RID: 120
		[SaveableField(5)]
		private bool _talkedWithBrother;

		// Token: 0x04000079 RID: 121
		[SaveableField(6)]
		private FindHideoutTutorialQuest.HideoutBattleEndState _hideoutBattleEndState;

		// Token: 0x0400007A RID: 122
		private List<CharacterObject> _mainPartyTroopBackup;

		// Token: 0x0400007B RID: 123
		private static string _activeHideoutStringId;

		// Token: 0x02000059 RID: 89
		public enum HideoutBattleEndState
		{
			// Token: 0x040001DD RID: 477
			None,
			// Token: 0x040001DE RID: 478
			Retreated,
			// Token: 0x040001DF RID: 479
			Defeated,
			// Token: 0x040001E0 RID: 480
			Victory
		}
	}
}
