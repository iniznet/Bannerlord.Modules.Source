using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000394 RID: 916
	public class GovernorCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600368E RID: 13966 RVA: 0x000F39AC File Offset: 0x000F1BAC
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.OnHeroChangedClanEvent.AddNonSerializedListener(this, new Action<Hero, Clan>(this.OnHeroChangedClan));
		}

		// Token: 0x0600368F RID: 13967 RVA: 0x000F3A15 File Offset: 0x000F1C15
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06003690 RID: 13968 RVA: 0x000F3A20 File Offset: 0x000F1C20
		private void DailyTickSettlement(Settlement settlement)
		{
			if ((settlement.IsTown || settlement.IsCastle) && settlement.Town.Governor != null)
			{
				Hero governor = settlement.Town.Governor;
				if (governor.GetPerkValue(DefaultPerks.Charm.InBloom) && MBRandom.RandomFloat < DefaultPerks.Charm.MeaningfulFavors.SecondaryBonus)
				{
					Hero randomElementWithPredicate = settlement.Notables.GetRandomElementWithPredicate((Hero x) => x.IsFemale != governor.IsFemale);
					if (randomElementWithPredicate != null)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(governor.Clan.Leader, randomElementWithPredicate, 1, true);
					}
				}
				if (governor.GetPerkValue(DefaultPerks.Charm.YoungAndRespectful) && MBRandom.RandomFloat < DefaultPerks.Charm.MeaningfulFavors.SecondaryBonus)
				{
					Hero randomElementWithPredicate2 = settlement.Notables.GetRandomElementWithPredicate((Hero x) => x.IsFemale == governor.IsFemale);
					if (randomElementWithPredicate2 != null)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(governor.Clan.Leader, randomElementWithPredicate2, 1, true);
					}
				}
				if (governor.GetPerkValue(DefaultPerks.Charm.MeaningfulFavors) && MBRandom.RandomFloat < DefaultPerks.Charm.MeaningfulFavors.SecondaryBonus)
				{
					foreach (Hero hero in settlement.Notables)
					{
						if (hero.Power >= 200f)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(settlement.OwnerClan.Leader, hero, 1, true);
						}
					}
				}
				SkillLevelingManager.OnSettlementGoverned(governor, settlement);
			}
		}

		// Token: 0x06003691 RID: 13969 RVA: 0x000F3BA4 File Offset: 0x000F1DA4
		private void OnHeroChangedClan(Hero hero, Clan oldClan)
		{
			if (hero.GovernorOf != null && hero.GovernorOf.OwnerClan != hero.Clan)
			{
				ChangeGovernorAction.RemoveGovernorOf(hero);
			}
		}

		// Token: 0x06003692 RID: 13970 RVA: 0x000F3BC7 File Offset: 0x000F1DC7
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003693 RID: 13971 RVA: 0x000F3BCC File Offset: 0x000F1DCC
		private void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("governor_talk_start", "hero_main_options", "governor_talk_start_reply", "{=zBo78JQb}How are things doing here in {GOVERNOR_SETTLEMENT}?", new ConversationSentence.OnConditionDelegate(this.governor_talk_start_on_condition), null, 100, null, null);
			starter.AddDialogLine("governor_talk_start_reply", "governor_talk_start_reply", "lord_pretalk", "{=!}{SETTLEMENT_DESCRIPTION}", new ConversationSentence.OnConditionDelegate(this.governor_talk_start_reply_on_condition), null, 200, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_start", "hero_main_options", "governor_kingdom_creation_reply", "{=EKuB6Ohf}It is time to take a momentous step... It is time to proclaim a new kingdom.", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_start_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_start_on_consequence), 200, new ConversationSentence.OnClickableConditionDelegate(this.governor_talk_kingdom_creation_start_clickable_condition), null);
			starter.AddDialogLine("governor_talk_kingdom_creation_reply", "governor_kingdom_creation_reply", "governor_kingdom_creation_culture_selection", "{=ZyNjXUHc}I am at your command.", null, null, 100, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_culture_selection", "governor_kingdom_creation_culture_selection", "governor_kingdom_creation_culture_selection_options", "{=jxEVSu98}The language of our documents, and our customary laws... Whose should we use?", null, null, 100, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selection_option", "governor_kingdom_creation_culture_selection_options", "governor_kingdom_creation_culture_selected", "{CULTURE_OPTION_0}", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_culture_option_0_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_culture_option_0_on_consequence), 100, null, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selection_option", "governor_kingdom_creation_culture_selection_options", "governor_kingdom_creation_culture_selected", "{CULTURE_OPTION_1}", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_culture_option_1_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_culture_option_1_on_consequence), 100, null, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selection_option", "governor_kingdom_creation_culture_selection_options", "governor_kingdom_creation_culture_selected", "{CULTURE_OPTION_2}", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_culture_option_2_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_culture_option_2_on_consequence), 100, null, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selection_other", "governor_kingdom_creation_culture_selection_options", "governor_kingdom_creation_culture_selection", "{=kcuNzSvf}I have another people in mind.", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_culture_other_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_culture_other_on_consequence), 100, null, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selection_cancel", "governor_kingdom_creation_culture_selection_options", "governor_kingdom_creation_exit", "{=hbzs5tLd}On second thought, perhaps now is not the right time.", null, null, 100, null, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_exit_reply", "governor_kingdom_creation_exit", "close_window", "{=ppi6eVos}As you wish.", null, null, 100, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_culture_selected", "governor_kingdom_creation_culture_selected", "governor_kingdom_creation_culture_selected_confirmation", "{=VOtKthQU}Yes. A kingdom using {CULTURE_ADJECTIVE} law would institute the following: {INITIAL_POLICY_NAMES}.", new ConversationSentence.OnConditionDelegate(this.governor_kingdom_creation_culture_selected_on_condition), null, 100, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selected_player_reply", "governor_kingdom_creation_culture_selected_confirmation", "governor_kingdom_creation_name_selection", "{=dzXaXKaC}Very well.", null, null, 100, null, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_culture_selected_player_reply_2", "governor_kingdom_creation_culture_selected_confirmation", "governor_kingdom_creation_culture_selection", "{=kTjsx8gN}Perhaps we should choose another set of laws and customs.", null, null, 100, null, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_name_selection", "governor_kingdom_creation_name_selection", "governor_kingdom_creation_name_selection_response", "{=wT1ducZX}Now. What will the kingdom be called?", null, null, 100, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_name_selection_player", "governor_kingdom_creation_name_selection_response", "governor_kingdom_creation_name_selection_prompted", "{=XRoG766S}I'll name it...", null, new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_name_selection_on_consequence), 100, null, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_name_selection_response", "governor_kingdom_creation_name_selection_prompted", "governor_kingdom_creation_name_selected", "{=shf5aY3l}I'm listening...", null, null, 100, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_name_selection_cancel", "governor_kingdom_creation_name_selection_response", "governor_kingdom_creation_exit", "{=7HpfrmIU}On a second thought... Now is not the right time to do this.", null, null, 100, null, null);
			starter.AddDialogLine("governor_talk_kingdom_creation_name_selection_final_response", "governor_kingdom_creation_name_selected", "governor_kingdom_creation_finalization", "{=CzJZ5zhT}So it shall be proclaimed throughout your domain. May {KINGDOM_NAME} forever be victorious!", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_finalization_on_condition), null, 100, null);
			starter.AddPlayerLine("governor_talk_kingdom_creation_finalization", "governor_kingdom_creation_finalization", "close_window", "{=VRbbIWNf}So it shall be.", new ConversationSentence.OnConditionDelegate(this.governor_talk_kingdom_creation_finalization_on_condition), new ConversationSentence.OnConsequenceDelegate(this.governor_talk_kingdom_creation_finalization_on_consequence), 100, null, null);
		}

		// Token: 0x06003694 RID: 13972 RVA: 0x000F3F31 File Offset: 0x000F2131
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (victim.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(victim);
			}
		}

		// Token: 0x06003695 RID: 13973 RVA: 0x000F3F44 File Offset: 0x000F2144
		private bool governor_talk_start_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.GovernorOf != null && Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.IsTown && Hero.OneToOneConversationHero.CurrentSettlement.Town == Hero.OneToOneConversationHero.GovernorOf && Hero.OneToOneConversationHero.GovernorOf.Owner.Owner == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("GOVERNOR_SETTLEMENT", Hero.OneToOneConversationHero.CurrentSettlement.Name, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003696 RID: 13974 RVA: 0x000F3FD4 File Offset: 0x000F21D4
		private bool governor_talk_start_reply_on_condition()
		{
			Settlement currentSettlement = Hero.OneToOneConversationHero.CurrentSettlement;
			TextObject textObject = TextObject.Empty;
			switch (currentSettlement.Town.GetProsperityLevel())
			{
			case SettlementComponent.ProsperityLevel.Low:
				textObject = new TextObject("{=rbJEuVKg}Things could certainly be better, my {?HERO.GENDER}lady{?}lord{\\?}. The merchants say business is slow, and the people complain that goods are expensive and in short supply.", null);
				break;
			case SettlementComponent.ProsperityLevel.Mid:
				textObject = new TextObject("{=HgdbSrq9}Things are all right, my {?HERO.GENDER}lady{?}lord{\\?}. The merchants say that they are breaking even, for the most part. Some prices are high, but most of what the people need is available.", null);
				break;
			case SettlementComponent.ProsperityLevel.High:
				textObject = new TextObject("{=8G94SlPD}We are doing well, my {?HERO.GENDER}lady{?}lord{\\?}. The merchants say business is brisk, and everything the people need appears to be in good supply.", null);
				break;
			}
			StringHelpers.SetCharacterProperties("HERO", CharacterObject.PlayerCharacter, textObject, false);
			MBTextManager.SetTextVariable("SETTLEMENT_DESCRIPTION", textObject.ToString(), false);
			return true;
		}

		// Token: 0x06003697 RID: 13975 RVA: 0x000F405C File Offset: 0x000F225C
		private bool governor_talk_kingdom_creation_start_on_condition()
		{
			return Clan.PlayerClan.Kingdom == null && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.GovernorOf != null && Hero.OneToOneConversationHero.GovernorOf.Settlement.MapFaction == Hero.MainHero.MapFaction;
		}

		// Token: 0x06003698 RID: 13976 RVA: 0x000F40AA File Offset: 0x000F22AA
		private void governor_talk_kingdom_creation_start_on_consequence()
		{
			this._availablePlayerKingdomCultures.Clear();
			this._availablePlayerKingdomCultures = Campaign.Current.Models.KingdomCreationModel.GetAvailablePlayerKingdomCultures().ToList<CultureObject>();
			this._kingdomCreationCurrentCulturePageIndex = 0;
		}

		// Token: 0x06003699 RID: 13977 RVA: 0x000F40E0 File Offset: 0x000F22E0
		private bool governor_talk_kingdom_creation_start_clickable_condition(out TextObject explanation)
		{
			List<TextObject> list;
			bool flag = Campaign.Current.Models.KingdomCreationModel.IsPlayerKingdomCreationPossible(out list);
			string text = "";
			foreach (TextObject textObject in list)
			{
				text += textObject;
				if (textObject != list[list.Count - 1])
				{
					text += "\n";
				}
			}
			explanation = new TextObject(text, null);
			return flag;
		}

		// Token: 0x0600369A RID: 13978 RVA: 0x000F4178 File Offset: 0x000F2378
		private bool governor_talk_kingdom_creation_culture_option_0_on_condition()
		{
			return this.HandleAvailableCultureConditionAndText(0);
		}

		// Token: 0x0600369B RID: 13979 RVA: 0x000F4181 File Offset: 0x000F2381
		private bool governor_talk_kingdom_creation_culture_option_1_on_condition()
		{
			return this.HandleAvailableCultureConditionAndText(1);
		}

		// Token: 0x0600369C RID: 13980 RVA: 0x000F418A File Offset: 0x000F238A
		private bool governor_talk_kingdom_creation_culture_option_2_on_condition()
		{
			return this.HandleAvailableCultureConditionAndText(2);
		}

		// Token: 0x0600369D RID: 13981 RVA: 0x000F4194 File Offset: 0x000F2394
		private bool HandleAvailableCultureConditionAndText(int index)
		{
			int cultureIndex = this.GetCultureIndex(index);
			if (this._availablePlayerKingdomCultures.Count > cultureIndex)
			{
				TextObject textObject = new TextObject("{=mY6DbVfc}The language and laws of {CULTURE_NAME}.", null);
				textObject.SetTextVariable("CULTURE_NAME", FactionHelper.GetInformalNameForFactionCulture(this._availablePlayerKingdomCultures[cultureIndex]));
				MBTextManager.SetTextVariable("CULTURE_OPTION_" + index.ToString(), textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x0600369E RID: 13982 RVA: 0x000F41FB File Offset: 0x000F23FB
		private int GetCultureIndex(int optionIndex)
		{
			return this._kingdomCreationCurrentCulturePageIndex * 3 + optionIndex;
		}

		// Token: 0x0600369F RID: 13983 RVA: 0x000F4207 File Offset: 0x000F2407
		private void governor_talk_kingdom_creation_culture_option_0_on_consequence()
		{
			this._kingdomCreationChosenCulture = this._availablePlayerKingdomCultures[this.GetCultureIndex(0)];
		}

		// Token: 0x060036A0 RID: 13984 RVA: 0x000F4221 File Offset: 0x000F2421
		private void governor_talk_kingdom_creation_culture_option_1_on_consequence()
		{
			this._kingdomCreationChosenCulture = this._availablePlayerKingdomCultures[this.GetCultureIndex(1)];
		}

		// Token: 0x060036A1 RID: 13985 RVA: 0x000F423B File Offset: 0x000F243B
		private void governor_talk_kingdom_creation_culture_option_2_on_consequence()
		{
			this._kingdomCreationChosenCulture = this._availablePlayerKingdomCultures[this.GetCultureIndex(2)];
		}

		// Token: 0x060036A2 RID: 13986 RVA: 0x000F4255 File Offset: 0x000F2455
		private bool governor_talk_kingdom_creation_culture_other_on_condition()
		{
			return this._availablePlayerKingdomCultures.Count > 3;
		}

		// Token: 0x060036A3 RID: 13987 RVA: 0x000F4265 File Offset: 0x000F2465
		private void governor_talk_kingdom_creation_culture_other_on_consequence()
		{
			this._kingdomCreationCurrentCulturePageIndex++;
			if (this._kingdomCreationCurrentCulturePageIndex > MathF.Ceiling((float)this._availablePlayerKingdomCultures.Count / 3f) - 1)
			{
				this._kingdomCreationCurrentCulturePageIndex = 0;
			}
		}

		// Token: 0x060036A4 RID: 13988 RVA: 0x000F42A0 File Offset: 0x000F24A0
		private bool governor_kingdom_creation_culture_selected_on_condition()
		{
			TextObject textObject = GameTexts.GameTextHelper.MergeTextObjectsWithComma(this._kingdomCreationChosenCulture.DefaultPolicyList.Select((PolicyObject t) => t.Name).ToList<TextObject>(), true);
			MBTextManager.SetTextVariable("INITIAL_POLICY_NAMES", textObject, false);
			MBTextManager.SetTextVariable("CULTURE_ADJECTIVE", FactionHelper.GetAdjectiveForFactionCulture(this._kingdomCreationChosenCulture), false);
			return true;
		}

		// Token: 0x060036A5 RID: 13989 RVA: 0x000F430C File Offset: 0x000F250C
		private void governor_talk_kingdom_creation_name_selection_on_consequence()
		{
			this._kingdomCreationChosenName = TextObject.Empty;
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=RuaA8t97}Kingdom Name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnKingdomNameSelectionDone), new Action(this.OnKingdomNameSelectionCancel), false, new Func<string, Tuple<bool, string>>(FactionHelper.IsKingdomNameApplicable), "", ""), false, false);
		}

		// Token: 0x060036A6 RID: 13990 RVA: 0x000F4396 File Offset: 0x000F2596
		private void OnKingdomNameSelectionDone(string chosenName)
		{
			this._kingdomCreationChosenName = new TextObject(chosenName, null);
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		// Token: 0x060036A7 RID: 13991 RVA: 0x000F43B4 File Offset: 0x000F25B4
		private void OnKingdomNameSelectionCancel()
		{
			Campaign.Current.ConversationManager.EndConversation();
		}

		// Token: 0x060036A8 RID: 13992 RVA: 0x000F43C5 File Offset: 0x000F25C5
		private bool governor_talk_kingdom_creation_finalization_on_condition()
		{
			MBTextManager.SetTextVariable("KINGDOM_NAME", this._kingdomCreationChosenName, false);
			return true;
		}

		// Token: 0x060036A9 RID: 13993 RVA: 0x000F43DC File Offset: 0x000F25DC
		private void governor_talk_kingdom_creation_finalization_on_consequence()
		{
			Campaign.Current.KingdomManager.CreateKingdom(this._kingdomCreationChosenName, this._kingdomCreationChosenName, this._kingdomCreationChosenCulture, Clan.PlayerClan, this._kingdomCreationChosenCulture.DefaultPolicyList, null, null, null);
		}

		// Token: 0x04001170 RID: 4464
		private const int CultureDialogueOptionCount = 3;

		// Token: 0x04001171 RID: 4465
		private List<CultureObject> _availablePlayerKingdomCultures = new List<CultureObject>();

		// Token: 0x04001172 RID: 4466
		private int _kingdomCreationCurrentCulturePageIndex;

		// Token: 0x04001173 RID: 4467
		private CultureObject _kingdomCreationChosenCulture;

		// Token: 0x04001174 RID: 4468
		private TextObject _kingdomCreationChosenName;
	}
}
