using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x020000A1 RID: 161
	public class RetirementCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600095A RID: 2394 RVA: 0x0004E6FB File Offset: 0x0004C8FB
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_hasTalkedWithHermitBefore", ref this._hasTalkedWithHermitBefore);
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0004E710 File Offset: 0x0004C910
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.GameMenuOpened));
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0004E764 File Offset: 0x0004C964
		private void GameMenuOpened(MenuCallbackArgs args)
		{
			if (args.MenuContext.GameMenu.StringId == "retirement_place")
			{
				if (this._selectedHeir != null)
				{
					PlayerEncounter.Finish(true);
					ApplyHeirSelectionAction.ApplyByRetirement(this._selectedHeir);
					this._selectedHeir = null;
					return;
				}
				if (this._playerEndedGame)
				{
					GameMenu.ExitToLast();
					this.ShowGameStatistics();
				}
			}
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0004E7C1 File Offset: 0x0004C9C1
		private void HourlyTick()
		{
			this.CheckRetirementSettlementVisibility();
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0004E7C9 File Offset: 0x0004C9C9
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this._retirementSettlement = Settlement.Find("retirement_retreat");
			this.SetupGameMenus(starter);
			this.SetupConversationDialogues(starter);
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0004E7EC File Offset: 0x0004C9EC
		private void SetupGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenu("retirement_place", "{=ruHt0Ub5}You are at the base of Mount Erithrys, an ancient volcano that has long been a refuge for oracles, seers and mystics. High up a steep valley, you can make out a number of caves carved into the soft volcanic rock. Coming closer, you see that some show signs of habitation. An old man in worn and tattered robes sits at the mouth of one of these caves, meditating in the cool mountain air.", new OnInitDelegate(this.retirement_menu_on_init), 0, 0, null);
			starter.AddGameMenuOption("retirement_place", "retirement_place_enter", "{=H3fobmyO}Approach", new GameMenuOption.OnConditionDelegate(this.enter_on_condition), new GameMenuOption.OnConsequenceDelegate(this.retirement_menu_on_enter), false, -1, false, null);
			starter.AddGameMenuOption("retirement_place", "retirement_place_leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.retirement_menu_on_leave), true, -1, false, null);
			starter.AddGameMenu("retirement_after_player_knockedout", "{=DK3QwC68}Your men on the slopes below saw you collapse, and rushed up to your aid. They tended to your wounds as well as they could, and you will survive. They are now awaiting your next decision.", null, 0, 0, null);
			starter.AddGameMenuOption("retirement_after_player_knockedout", "enter", "{=H3fobmyO}Approach", new GameMenuOption.OnConditionDelegate(this.enter_on_condition), new GameMenuOption.OnConsequenceDelegate(this.retirement_menu_on_enter), false, -1, false, null);
			starter.AddGameMenuOption("retirement_after_player_knockedout", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.retirement_menu_on_leave), true, -1, false, null);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0004E8F0 File Offset: 0x0004CAF0
		private void SetupConversationDialogues(CampaignGameStarter starter)
		{
			starter.AddDialogLine("hermit_start_1", "start", "player_answer", "{=09PJ02x6}Hello there. You must be one of those lordly types who ride their horses about this land, scrabbling for wealth and power. I wonder what you hope to gain from such things...", new ConversationSentence.OnConditionDelegate(this.hermit_start_talk_first_time_on_condition), null, 100, null);
			starter.AddDialogLine("hermit_start_2", "start", "player_accept_or_decline", "{=YmNT7HJ3}Have you made up your mind? Are you ready to let go of your earthly burdens and relish a simpler life?", new ConversationSentence.OnConditionDelegate(this.hermit_start_talk_on_condition), null, 100, null);
			starter.AddPlayerLine("player_answer_hermit_start_1", "player_answer", "hermit_answer_1", "{=1FbydhBb}I rather enjoy wealth and power.", null, null, 100, null, null);
			starter.AddPlayerLine("player_answer_hermit_start_2", "player_answer", "hermit_answer_2", "{=TOpysYqG}Power can mean the power to do good, you know.", null, null, 100, null, null);
			starter.AddDialogLine("hermit_answer_1", "hermit_answer_1", "hermit_answer_continue", "{=v7pJerga}Ah, but the thing about wealth and power is that someone is always trying to take it from you. Perhaps even one's own children. There is no rest for the powerful.", null, null, 100, null);
			starter.AddDialogLine("hermit_answer_2", "hermit_answer_2", "hermit_answer_continue", "{=1MaECke2}Many people tell themselves that they only want power to do good, but still seek it for its own sake. The only way to know for sure that you do not lust after power is to give it up.", null, null, 100, null);
			starter.AddDialogLine("hermit_answer_continue_1", "hermit_answer_continue", "player_accept_or_decline", "{=Q2RKkISe}There are a number of us up here in our caves. We dine well on locusts and wild honey. Some of us once knew wealth and power, but what we relish now is freedom. Freedom from the worry that what we hoard will be lost. Freedom from the fear that our actions offend Heaven. The only true freedom. We welcome newcomers, like your good self.", null, delegate
			{
				this._hasTalkedWithHermitBefore = true;
			}, 100, null);
			starter.AddPlayerLine("player_accept_or_decline_2", "player_accept_or_decline", "player_accept", "{=z2duf82b}Well, perhaps I would like to know peace for a while. But I would need to think about the future of my clan...", null, new ConversationSentence.OnConsequenceDelegate(this.hermit_player_select_heir_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_accept_or_decline_1", "player_accept_or_decline", "player_decline", "{=x4wnaovC}I am not sure I share your idea of freedom.", null, null, 100, null, null);
			starter.AddDialogLine("hermit_answer_player_decline", "player_decline", "close_window", "{=uXXH5GIU}Please, then, return to your wars and worry. I cannot help someone attain what they do not want. But I am not going anywhere, so if you desire my help at any time, do come again.", null, null, 100, null);
			starter.AddDialogLine("hermit_answer_player_accept", "player_accept", "hermit_player_select_heir", "{=6nIC6i2V}I have acolytes who seek me out from time to time. I can have one of them take a message to your kinfolk. Perhaps you would like to name an heir?", null, null, 100, null);
			starter.AddRepeatablePlayerLine("hermit_player_select_heir_1", "hermit_player_select_heir", "close_window", "{=!}{HEIR.NAME}", "{=epepvysB}I have someone else in mind.", "player_accept", new ConversationSentence.OnConditionDelegate(this.hermit_select_heir_multiple_on_condition), new ConversationSentence.OnConsequenceDelegate(this.hermit_select_heir_multiple_on_consequence), 100, null);
			starter.AddPlayerLine("hermit_player_select_heir_2", "hermit_player_select_heir", "close_window", "{=Qbp2AiRZ}I choose not to name anyone. I shall think no more of worldly things. (END GAME)", null, new ConversationSentence.OnConsequenceDelegate(this.hermit_player_retire_on_consequence), 100, null, null);
			starter.AddPlayerLine("hermit_player_select_heir_3", "hermit_player_select_heir", "close_window", "{=CH7b5LaX}I have changed my mind.", null, delegate
			{
				this._selectedHeir = null;
			}, 100, null, null);
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0004EB25 File Offset: 0x0004CD25
		private bool hermit_start_talk_first_time_on_condition()
		{
			return !this._hasTalkedWithHermitBefore && CharacterObject.OneToOneConversationCharacter.StringId == "sp_hermit";
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0004EB48 File Offset: 0x0004CD48
		private void hermit_player_retire_on_consequence()
		{
			TextObject textObject = new TextObject("{=LmoyzsTE}{PLAYER.NAME} will retire. {HEIR_PART} Would you like to continue?", null);
			if (this._selectedHeir == null)
			{
				TextObject textObject2 = new TextObject("{=RPgzaZeR}Your game will end.", null);
				textObject.SetTextVariable("HEIR_PART", textObject2);
			}
			else
			{
				TextObject textObject3 = new TextObject("{=GEvP9i5f}You will play on as {HEIR.NAME}.", null);
				TextObjectExtensions.SetCharacterProperties(textObject3, "HEIR", this._selectedHeir.CharacterObject, false);
				textObject.SetTextVariable("HEIR_PART", textObject3);
			}
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_decision", null).ToString(), textObject.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				if (this._selectedHeir != null)
				{
					this._hasTalkedWithHermitBefore = false;
				}
				else
				{
					this._playerEndedGame = true;
				}
				Mission.Current.EndMission();
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0004EC14 File Offset: 0x0004CE14
		private void hermit_player_select_heir_on_consequence()
		{
			List<Hero> list = new List<Hero>();
			foreach (KeyValuePair<Hero, int> keyValuePair in from x in Clan.PlayerClan.GetHeirApparents()
				orderby x.Value
				select x)
			{
				list.Add(keyValuePair.Key);
			}
			ConversationSentence.SetObjectsToRepeatOver(list, 5);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0004EC9C File Offset: 0x0004CE9C
		private void hermit_select_heir_multiple_on_consequence()
		{
			this._selectedHeir = ConversationSentence.SelectedRepeatObject as Hero;
			this.hermit_player_retire_on_consequence();
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0004ECB4 File Offset: 0x0004CEB4
		private bool hermit_select_heir_multiple_on_condition()
		{
			if (Clan.PlayerClan.GetHeirApparents().Any<KeyValuePair<Hero, int>>())
			{
				Hero hero = ConversationSentence.CurrentProcessedRepeatObject as Hero;
				if (hero != null)
				{
					TextObjectExtensions.SetCharacterProperties(ConversationSentence.SelectedRepeatLine, "HEIR", hero.CharacterObject, false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0004ECF9 File Offset: 0x0004CEF9
		private bool hermit_start_talk_on_condition()
		{
			return this._hasTalkedWithHermitBefore && CharacterObject.OneToOneConversationCharacter.StringId == "sp_hermit";
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0004ED1C File Offset: 0x0004CF1C
		private void ShowGameStatistics()
		{
			GameOverState gameOverState = Game.Current.GameStateManager.CreateState<GameOverState>(new object[] { 0 });
			Game.Current.GameStateManager.PushState(gameOverState, 0);
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0004ED5C File Offset: 0x0004CF5C
		private void retirement_menu_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
			if (currentSettlement != null)
			{
				Campaign.Current.GameMenuManager.MenuLocations.Clear();
				Campaign.Current.GameMenuManager.MenuLocations.Add(currentSettlement.LocationComplex.GetLocationWithId("retirement_retreat"));
				args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
				PlayerEncounter.EnterSettlement();
				PlayerEncounter.LocationEncounter = new RetirementEncounter(currentSettlement);
			}
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0004EDD9 File Offset: 0x0004CFD9
		private bool enter_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0004EDE3 File Offset: 0x0004CFE3
		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0004EDEE File Offset: 0x0004CFEE
		private void retirement_menu_on_enter(MenuCallbackArgs args)
		{
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("retirement_retreat"), null, null, null);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0004EE0D File Offset: 0x0004D00D
		private void retirement_menu_on_leave(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0004EE1C File Offset: 0x0004D01C
		private void CheckRetirementSettlementVisibility()
		{
			float hideoutSpottingDistance = Campaign.Current.Models.MapVisibilityModel.GetHideoutSpottingDistance();
			float num = MobileParty.MainParty.Position2D.DistanceSquared(this._retirementSettlement.Position2D);
			if (1f - num / (hideoutSpottingDistance * hideoutSpottingDistance) > 0f)
			{
				this._retirementSettlement.IsVisible = true;
				RetirementSettlementComponent retirementSettlementComponent = this._retirementSettlement.SettlementComponent as RetirementSettlementComponent;
				if (!retirementSettlementComponent.IsSpotted)
				{
					retirementSettlementComponent.IsSpotted = true;
				}
			}
		}

		// Token: 0x04000333 RID: 819
		private Hero _selectedHeir;

		// Token: 0x04000334 RID: 820
		private bool _hasTalkedWithHermitBefore;

		// Token: 0x04000335 RID: 821
		private bool _playerEndedGame;

		// Token: 0x04000336 RID: 822
		private Settlement _retirementSettlement;
	}
}
