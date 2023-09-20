using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x02000319 RID: 793
	public class MerchantArmyOfPoachersIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x06002D30 RID: 11568 RVA: 0x000BC98A File Offset: 0x000BAB8A
		private void engage_poachers_consequence(MenuCallbackArgs args)
		{
			MerchantArmyOfPoachersIssueBehavior.Instance.StartQuestBattle();
		}

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06002D31 RID: 11569 RVA: 0x000BC998 File Offset: 0x000BAB98
		private static MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest Instance
		{
			get
			{
				MerchantArmyOfPoachersIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<MerchantArmyOfPoachersIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest merchantArmyOfPoachersIssueQuest;
						if ((merchantArmyOfPoachersIssueQuest = enumerator.Current as MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = merchantArmyOfPoachersIssueQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x06002D32 RID: 11570 RVA: 0x000BCA30 File Offset: 0x000BAC30
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06002D33 RID: 11571 RVA: 0x000BCA60 File Offset: 0x000BAC60
		private bool poachers_menu_back_condition(MenuCallbackArgs args)
		{
			return Hero.MainHero.IsWounded;
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x000BCA6C File Offset: 0x000BAC6C
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("army_of_poachers_village", "{=eaQxeRh6}A boy runs out of the village and asks you to talk to the leader of the poachers. The villagers want to avoid a fight outside their homes.", new OnInitDelegate(this.army_of_poachers_village_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "engage_the_poachers", "{=xF7he8fZ}Fight the poachers", new GameMenuOption.OnConditionDelegate(this.engage_poachers_condition), new GameMenuOption.OnConsequenceDelegate(this.engage_poachers_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "talk_to_the_poachers", "{=wwJGE28v}Negotiate with the poachers", new GameMenuOption.OnConditionDelegate(this.talk_to_leader_of_poachers_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_leader_of_poachers_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "back_poachers", "{=E1OwmQFb}Back", new GameMenuOption.OnConditionDelegate(this.poachers_menu_back_condition), new GameMenuOption.OnConsequenceDelegate(this.poachers_menu_back_consequence), false, -1, false, null);
		}

		// Token: 0x06002D35 RID: 11573 RVA: 0x000BCB2C File Offset: 0x000BAD2C
		private void army_of_poachers_village_on_init(MenuCallbackArgs args)
		{
			if (MerchantArmyOfPoachersIssueBehavior.Instance != null && MerchantArmyOfPoachersIssueBehavior.Instance.IsOngoing)
			{
				args.MenuContext.SetBackgroundMeshName(MerchantArmyOfPoachersIssueBehavior.Instance._questVillage.Settlement.SettlementComponent.WaitMeshName);
				if (MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty == null && !Hero.MainHero.IsWounded)
				{
					MerchantArmyOfPoachersIssueBehavior.Instance.CreatePoachersParty();
				}
				if (MerchantArmyOfPoachersIssueBehavior.Instance._isReadyToBeFinalized && PlayerEncounter.Current != null)
				{
					bool flag = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide;
					PlayerEncounter.Update();
					if (PlayerEncounter.Current == null)
					{
						MerchantArmyOfPoachersIssueBehavior.Instance._isReadyToBeFinalized = false;
						if (flag)
						{
							MerchantArmyOfPoachersIssueBehavior.Instance.QuestSuccessWithPlayerDefeatedPoachers();
						}
						else
						{
							MerchantArmyOfPoachersIssueBehavior.Instance.QuestFailWithPlayerDefeatedAgainstPoachers();
						}
					}
					else if (PlayerEncounter.Battle.WinningSide == BattleSideEnum.None)
					{
						PlayerEncounter.LeaveEncounter = true;
						PlayerEncounter.Update();
						MerchantArmyOfPoachersIssueBehavior.Instance.QuestFailWithPlayerDefeatedAgainstPoachers();
					}
					else if (flag && PlayerEncounter.Current != null && Game.Current.GameStateManager.ActiveState is MapState)
					{
						PlayerEncounter.Finish(true);
						MerchantArmyOfPoachersIssueBehavior.Instance.QuestSuccessWithPlayerDefeatedPoachers();
					}
				}
				if (MerchantArmyOfPoachersIssueBehavior.Instance != null && MerchantArmyOfPoachersIssueBehavior.Instance._talkedToPoachersBattleWillStart)
				{
					MerchantArmyOfPoachersIssueBehavior.Instance.StartQuestBattle();
				}
			}
		}

		// Token: 0x06002D36 RID: 11574 RVA: 0x000BCC6B File Offset: 0x000BAE6B
		private bool engage_poachers_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			if (Hero.MainHero.IsWounded)
			{
				args.Tooltip = new TextObject("{=gEHEQazX}You're heavily wounded and not fit for the fight. Come back when you're ready.", null);
				args.IsEnabled = false;
			}
			return true;
		}

		// Token: 0x06002D37 RID: 11575 RVA: 0x000BCC99 File Offset: 0x000BAE99
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06002D38 RID: 11576 RVA: 0x000BCC9B File Offset: 0x000BAE9B
		private bool talk_to_leader_of_poachers_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			if (Hero.MainHero.IsWounded)
			{
				args.Tooltip = new TextObject("{=gEHEQazX}You're heavily wounded and not fit for the fight. Come back when you're ready.", null);
				args.IsEnabled = false;
			}
			return true;
		}

		// Token: 0x06002D39 RID: 11577 RVA: 0x000BCCCA File Offset: 0x000BAECA
		private void poachers_menu_back_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06002D3A RID: 11578 RVA: 0x000BCCD8 File Offset: 0x000BAED8
		private bool ConditionsHold(Hero issueGiver, out Village questVillage)
		{
			questVillage = null;
			if (issueGiver.CurrentSettlement == null)
			{
				return false;
			}
			questVillage = issueGiver.CurrentSettlement.BoundVillages.GetRandomElementWithPredicate((Village x) => !x.Settlement.IsUnderRaid && !x.Settlement.IsRaided);
			if (questVillage != null && issueGiver.IsMerchant && issueGiver.GetTraitLevel(DefaultTraits.Mercy) + issueGiver.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				Town town = issueGiver.CurrentSettlement.Town;
				return town != null && town.Security <= (float)60;
			}
			return false;
		}

		// Token: 0x06002D3B RID: 11579 RVA: 0x000BCD6C File Offset: 0x000BAF6C
		private void talk_to_leader_of_poachers_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty.Party), MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty.Party, false, false, false, false, false, false));
		}

		// Token: 0x06002D3C RID: 11580 RVA: 0x000BCDC4 File Offset: 0x000BAFC4
		public void OnCheckForIssue(Hero hero)
		{
			Village village;
			if (this.ConditionsHold(hero, out village))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), IssueBase.IssueFrequency.Common, village));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), IssueBase.IssueFrequency.Common));
		}

		// Token: 0x06002D3D RID: 11581 RVA: 0x000BCE2C File Offset: 0x000BB02C
		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue(issueOwner, potentialIssueData.RelatedObject as Village);
		}

		// Token: 0x04000D9E RID: 3486
		private const IssueBase.IssueFrequency ArmyOfPoachersIssueFrequency = IssueBase.IssueFrequency.Common;

		// Token: 0x04000D9F RID: 3487
		private MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest _cachedQuest;

		// Token: 0x0200064F RID: 1615
		public class MerchantArmyOfPoachersIssue : IssueBase
		{
			// Token: 0x06004F02 RID: 20226 RVA: 0x0015E88A File Offset: 0x0015CA8A
			internal static void AutoGeneratedStaticCollectObjectsMerchantArmyOfPoachersIssue(object o, List<object> collectedObjects)
			{
				((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004F03 RID: 20227 RVA: 0x0015E898 File Offset: 0x0015CA98
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._questVillage);
			}

			// Token: 0x06004F04 RID: 20228 RVA: 0x0015E8AD File Offset: 0x0015CAAD
			internal static object AutoGeneratedGetMemberValue_questVillage(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue)o)._questVillage;
			}

			// Token: 0x170010E4 RID: 4324
			// (get) Token: 0x06004F05 RID: 20229 RVA: 0x0015E8BA File Offset: 0x0015CABA
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 12 + MathF.Ceiling(28f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170010E5 RID: 4325
			// (get) Token: 0x06004F06 RID: 20230 RVA: 0x0015E8D0 File Offset: 0x0015CAD0
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(5f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170010E6 RID: 4326
			// (get) Token: 0x06004F07 RID: 20231 RVA: 0x0015E8E5 File Offset: 0x0015CAE5
			protected override int RewardGold
			{
				get
				{
					return (int)(500f + 3000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170010E7 RID: 4327
			// (get) Token: 0x06004F08 RID: 20232 RVA: 0x0015E8FA File Offset: 0x0015CAFA
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Casualties | IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			// Token: 0x170010E8 RID: 4328
			// (get) Token: 0x06004F09 RID: 20233 RVA: 0x0015E8FE File Offset: 0x0015CAFE
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=Jk3mDlU6}Yeah... I've got some problems. A few years ago, I needed hides for my tannery and I hired some hunters. I didn't ask too many questions about where they came by the skins they sold me. Well, that was a bit of mistake. Now they've banded together as a gang and are trying to muscle me out of the leather business.", null);
				}
			}

			// Token: 0x170010E9 RID: 4329
			// (get) Token: 0x06004F0A RID: 20234 RVA: 0x0015E90B File Offset: 0x0015CB0B
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=apuNQC2W}What can I do for you?", null);
				}
			}

			// Token: 0x170010EA RID: 4330
			// (get) Token: 0x06004F0B RID: 20235 RVA: 0x0015E918 File Offset: 0x0015CB18
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=LbTETjZu}I want you to crush them. Go to {VILLAGE} and give them a lesson they won't forget.", null);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170010EB RID: 4331
			// (get) Token: 0x06004F0C RID: 20236 RVA: 0x0015E941 File Offset: 0x0015CB41
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=2ELhox6C}If you don't want to get involved in this yourself, leave one of your capable companions and {NUMBER_OF_TROOPS} men for some days.", null);
					textObject.SetTextVariable("NUMBER_OF_TROOPS", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			// Token: 0x170010EC RID: 4332
			// (get) Token: 0x06004F0D RID: 20237 RVA: 0x0015E960 File Offset: 0x0015CB60
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=b6naGx6H}I'll rid you of those poachers myself.", null);
				}
			}

			// Token: 0x170010ED RID: 4333
			// (get) Token: 0x06004F0E RID: 20238 RVA: 0x0015E96D File Offset: 0x0015CB6D
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=lA14Ubal}I can send a companion to hunt these poachers.", null);
				}
			}

			// Token: 0x170010EE RID: 4334
			// (get) Token: 0x06004F0F RID: 20239 RVA: 0x0015E97A File Offset: 0x0015CB7A
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=Xmtlrrmf}Thank you. Don't forget to warn your men. These poachers are not ordinary bandits. Good luck.", null);
				}
			}

			// Token: 0x170010EF RID: 4335
			// (get) Token: 0x06004F10 RID: 20240 RVA: 0x0015E987 File Offset: 0x0015CB87
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=51ahPi69}I understand that your men are still chasing those poachers. I realize that this mess might take a little time to clean up.", null);
				}
			}

			// Token: 0x170010F0 RID: 4336
			// (get) Token: 0x06004F11 RID: 20241 RVA: 0x0015E994 File Offset: 0x0015CB94
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170010F1 RID: 4337
			// (get) Token: 0x06004F12 RID: 20242 RVA: 0x0015E997 File Offset: 0x0015CB97
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170010F2 RID: 4338
			// (get) Token: 0x06004F13 RID: 20243 RVA: 0x0015E99C File Offset: 0x0015CB9C
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=428B377z}{ISSUE_GIVER.LINK}, a merchant of {QUEST_GIVER_SETTLEMENT}, told you that the poachers {?ISSUE_GIVER.GENDER}she{?}he{\\?} hired are now out of control. You asked {COMPANION.LINK} to take {NEEDED_MEN_COUNT} of your men to go to {QUEST_VILLAGE} and kill the poachers. They should rejoin your party in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("QUEST_VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x170010F3 RID: 4339
			// (get) Token: 0x06004F14 RID: 20244 RVA: 0x0015EA49 File Offset: 0x0015CC49
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=iHFo2kjz}Army of Poachers", null);
				}
			}

			// Token: 0x170010F4 RID: 4340
			// (get) Token: 0x06004F15 RID: 20245 RVA: 0x0015EA56 File Offset: 0x0015CC56
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=NCC4VUOc}{ISSUE_GIVER.LINK} wants you to get rid of the poachers who once worked for {?ISSUE_GIVER.GENDER}her{?}him{\\?} but are now out of control.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, null, false);
					return textObject;
				}
			}

			// Token: 0x06004F16 RID: 20246 RVA: 0x0015EA7B File Offset: 0x0015CC7B
			public MerchantArmyOfPoachersIssue(Hero issueOwner, Village questVillage)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this._questVillage = questVillage;
			}

			// Token: 0x06004F17 RID: 20247 RVA: 0x0015EA95 File Offset: 0x0015CC95
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementProsperity)
				{
					return 0.2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -1f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementLoyalty)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x06004F18 RID: 20248 RVA: 0x0015EAD4 File Offset: 0x0015CCD4
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06004F19 RID: 20249 RVA: 0x0015EAEC File Offset: 0x0015CCEC
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06004F1A RID: 20250 RVA: 0x0015EAFC File Offset: 0x0015CCFC
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				int skillValue = hero.GetSkillValue(DefaultSkills.Bow);
				int skillValue2 = hero.GetSkillValue(DefaultSkills.Crossbow);
				int skillValue3 = hero.GetSkillValue(DefaultSkills.Throwing);
				if (skillValue >= skillValue2 && skillValue >= skillValue3)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.Bow, 150);
				}
				return new ValueTuple<SkillObject, int>((skillValue2 >= skillValue3) ? DefaultSkills.Crossbow : DefaultSkills.Throwing, 150);
			}

			// Token: 0x06004F1B RID: 20251 RVA: 0x0015EB5F File Offset: 0x0015CD5F
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06004F1C RID: 20252 RVA: 0x0015EB80 File Offset: 0x0015CD80
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Common;
			}

			// Token: 0x06004F1D RID: 20253 RVA: 0x0015EB84 File Offset: 0x0015CD84
			public override bool IssueStayAliveConditions()
			{
				return !this._questVillage.Settlement.IsUnderRaid && !this._questVillage.Settlement.IsRaided && base.IssueOwner.CurrentSettlement.Town.Security <= 90f;
			}

			// Token: 0x06004F1E RID: 20254 RVA: 0x0015EBD8 File Offset: 0x0015CDD8
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				skill = null;
				relationHero = null;
				flag = IssueBase.PreconditionFlags.None;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
				}
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 15)
				{
					flag |= IssueBase.PreconditionFlags.NotEnoughTroops;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flag |= IssueBase.PreconditionFlags.AtWar;
				}
				return flag == IssueBase.PreconditionFlags.None;
			}

			// Token: 0x06004F1F RID: 20255 RVA: 0x0015EC45 File Offset: 0x0015CE45
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06004F20 RID: 20256 RVA: 0x0015EC47 File Offset: 0x0015CE47
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(20f), this._questVillage, base.IssueDifficultyMultiplier, this.RewardGold);
			}

			// Token: 0x06004F21 RID: 20257 RVA: 0x0015EC71 File Offset: 0x0015CE71
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x170010F5 RID: 4341
			// (get) Token: 0x06004F22 RID: 20258 RVA: 0x0015EC73 File Offset: 0x0015CE73
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06004F23 RID: 20259 RVA: 0x0015EC88 File Offset: 0x0015CE88
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueOwner.AddPower(30f);
				base.IssueOwner.CurrentSettlement.Prosperity += 50f;
			}

			// Token: 0x06004F24 RID: 20260 RVA: 0x0015ECC0 File Offset: 0x0015CEC0
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueOwner.AddPower(-50f);
				base.IssueOwner.CurrentSettlement.Prosperity -= 30f;
				base.IssueOwner.CurrentSettlement.Town.Security -= 5f;
				TraitLevelingHelper.OnIssueFailed(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
			}

			// Token: 0x04001A73 RID: 6771
			private const int AlternativeSolutionTroopTierRequirement = 2;

			// Token: 0x04001A74 RID: 6772
			private const int CompanionRequiredSkillLevel = 150;

			// Token: 0x04001A75 RID: 6773
			private const int MinimumRequiredMenCount = 15;

			// Token: 0x04001A76 RID: 6774
			private const int IssueDuration = 15;

			// Token: 0x04001A77 RID: 6775
			private const int QuestTimeLimit = 20;

			// Token: 0x04001A78 RID: 6776
			[SaveableField(10)]
			private Village _questVillage;
		}

		// Token: 0x02000650 RID: 1616
		public class MerchantArmyOfPoachersIssueQuest : QuestBase
		{
			// Token: 0x06004F25 RID: 20261 RVA: 0x0015ED42 File Offset: 0x0015CF42
			internal static void AutoGeneratedStaticCollectObjectsMerchantArmyOfPoachersIssueQuest(object o, List<object> collectedObjects)
			{
				((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004F26 RID: 20262 RVA: 0x0015ED50 File Offset: 0x0015CF50
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._poachersParty);
				collectedObjects.Add(this._questVillage);
			}

			// Token: 0x06004F27 RID: 20263 RVA: 0x0015ED71 File Offset: 0x0015CF71
			internal static object AutoGeneratedGetMemberValue_poachersParty(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._poachersParty;
			}

			// Token: 0x06004F28 RID: 20264 RVA: 0x0015ED7E File Offset: 0x0015CF7E
			internal static object AutoGeneratedGetMemberValue_questVillage(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._questVillage;
			}

			// Token: 0x06004F29 RID: 20265 RVA: 0x0015ED8B File Offset: 0x0015CF8B
			internal static object AutoGeneratedGetMemberValue_talkedToPoachersBattleWillStart(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._talkedToPoachersBattleWillStart;
			}

			// Token: 0x06004F2A RID: 20266 RVA: 0x0015ED9D File Offset: 0x0015CF9D
			internal static object AutoGeneratedGetMemberValue_isReadyToBeFinalized(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._isReadyToBeFinalized;
			}

			// Token: 0x06004F2B RID: 20267 RVA: 0x0015EDAF File Offset: 0x0015CFAF
			internal static object AutoGeneratedGetMemberValue_persuasionTriedOnce(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._persuasionTriedOnce;
			}

			// Token: 0x06004F2C RID: 20268 RVA: 0x0015EDC1 File Offset: 0x0015CFC1
			internal static object AutoGeneratedGetMemberValue_difficultyMultiplier(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._difficultyMultiplier;
			}

			// Token: 0x06004F2D RID: 20269 RVA: 0x0015EDD3 File Offset: 0x0015CFD3
			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._rewardGold;
			}

			// Token: 0x170010F6 RID: 4342
			// (get) Token: 0x06004F2E RID: 20270 RVA: 0x0015EDE5 File Offset: 0x0015CFE5
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=iHFo2kjz}Army of Poachers", null);
				}
			}

			// Token: 0x170010F7 RID: 4343
			// (get) Token: 0x06004F2F RID: 20271 RVA: 0x0015EDF2 File Offset: 0x0015CFF2
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170010F8 RID: 4344
			// (get) Token: 0x06004F30 RID: 20272 RVA: 0x0015EDF8 File Offset: 0x0015CFF8
			private TextObject _questStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=fk4ewfQh}{QUEST_GIVER.LINK}, a merchant of {SETTLEMENT}, told you that the poachers {?QUEST_GIVER.GENDER}she{?}he{\\?} hired before are now out of control. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to go to {VILLAGE} around midnight and kill the poachers.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170010F9 RID: 4345
			// (get) Token: 0x06004F31 RID: 20273 RVA: 0x0015EE62 File Offset: 0x0015D062
			private TextObject _questCanceledTargetVillageRaided
			{
				get
				{
					TextObject textObject = new TextObject("{=etYq1Tky}{VILLAGE} was raided and the poachers scattered.", null);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170010FA RID: 4346
			// (get) Token: 0x06004F32 RID: 20274 RVA: 0x0015EE8C File Offset: 0x0015D08C
			private TextObject _questCanceledWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=vW6kBki9}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170010FB RID: 4347
			// (get) Token: 0x06004F33 RID: 20275 RVA: 0x0015EEC0 File Offset: 0x0015D0C0
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170010FC RID: 4348
			// (get) Token: 0x06004F34 RID: 20276 RVA: 0x0015EEF4 File Offset: 0x0015D0F4
			private TextObject _questFailedAfterTalkingWithProachers
			{
				get
				{
					TextObject textObject = new TextObject("{=PIukmFYA}You decided not to get involved and left the village. You have failed to help {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170010FD RID: 4349
			// (get) Token: 0x06004F35 RID: 20277 RVA: 0x0015EF26 File Offset: 0x0015D126
			private TextObject _questSuccessPlayerComesToAnAgreementWithPoachers
			{
				get
				{
					return new TextObject("{=qPfJpwGa}You have persuaded the poachers to leave the district.", null);
				}
			}

			// Token: 0x170010FE RID: 4350
			// (get) Token: 0x06004F36 RID: 20278 RVA: 0x0015EF34 File Offset: 0x0015D134
			private TextObject _questFailWithPlayerDefeatedAgainstPoachers
			{
				get
				{
					TextObject textObject = new TextObject("{=p8Kfl5u6}You lost the battle against the poachers and failed to help {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170010FF RID: 4351
			// (get) Token: 0x06004F37 RID: 20279 RVA: 0x0015EF68 File Offset: 0x0015D168
			private TextObject _questSuccessWithPlayerDefeatedPoachers
			{
				get
				{
					TextObject textObject = new TextObject("{=8gNqLqFl}You have defeated the poachers and helped {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001100 RID: 4352
			// (get) Token: 0x06004F38 RID: 20280 RVA: 0x0015EF9A File Offset: 0x0015D19A
			private TextObject _questFailedWithTimeOutLogText
			{
				get
				{
					return new TextObject("{=HX7E09XJ}You failed to complete the quest in time.", null);
				}
			}

			// Token: 0x06004F39 RID: 20281 RVA: 0x0015EFA7 File Offset: 0x0015D1A7
			public MerchantArmyOfPoachersIssueQuest(string questId, Hero giverHero, CampaignTime duration, Village questVillage, float difficultyMultiplier, int rewardGold)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._questVillage = questVillage;
				this._talkedToPoachersBattleWillStart = false;
				this._isReadyToBeFinalized = false;
				this._difficultyMultiplier = difficultyMultiplier;
				this._rewardGold = rewardGold;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06004F3A RID: 20282 RVA: 0x0015EFE8 File Offset: 0x0015D1E8
			private bool SetStartDialogOnCondition()
			{
				if (this._poachersParty != null && CharacterObject.OneToOneConversationCharacter == ConversationHelper.GetConversationCharacterPartyLeader(this._poachersParty.Party))
				{
					MBTextManager.SetTextVariable("POACHER_PARTY_START_LINE", "{=j9MBwnWI}Well...Are you working for that merchant in the town ? So it's all fine when the rich folk trade in poached skins, but if we do it, armed men come to hunt us down.", false);
					if (this._persuasionTriedOnce)
					{
						MBTextManager.SetTextVariable("POACHER_PARTY_START_LINE", "{=Nn06TSq9}Anything else to say?", false);
					}
					return true;
				}
				return false;
			}

			// Token: 0x06004F3B RID: 20283 RVA: 0x0015F040 File Offset: 0x0015D240
			private DialogFlow GetPoacherPartyDialogFlow()
			{
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=!}{POACHER_PARTY_START_LINE}", null, null).Condition(() => this.SetStartDialogOnCondition())
					.Consequence(delegate
					{
						this._task = this.GetPersuasionTask();
					})
					.BeginPlayerOptions()
					.PlayerOption("{=afbLOXbb}Maybe we can come to an agreement.", null)
					.Condition(() => !this._persuasionTriedOnce)
					.Consequence(delegate
					{
						this._persuasionTriedOnce = true;
					})
					.GotoDialogState("start_poachers_persuasion")
					.PlayerOption("{=mvw1ayGt}I'm here to do the job I agreed to do, outlaw. Give up or die.", null)
					.NpcLine("{=hOVr77fd}You will never see the sunrise again!", null, null)
					.Consequence(delegate
					{
						this._talkedToPoachersBattleWillStart = true;
					})
					.CloseDialog()
					.PlayerOption("{=VJYEoOAc}Well... You have a point. Go on. We won't bother you any more.", null)
					.NpcLine("{=wglTyBbx}Thank you, friend. Go in peace.", null, null)
					.Consequence(delegate
					{
						Campaign.Current.GameMenuManager.SetNextMenu("village");
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.QuestFailedAfterTalkingWithPoachers;
					})
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			// Token: 0x06004F3C RID: 20284 RVA: 0x0015F130 File Offset: 0x0015D330
			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				dialog.AddDialogLine("poachers_persuasion_check_accepted", "start_poachers_persuasion", "poachers_persuasion_start_reservation", "{=6P1ruzsC}Maybe...", new ConversationSentence.OnConditionDelegate(this.persuasion_start_with_poachers_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_start_with_poachers_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_rejected", "poachers_persuasion_start_reservation", "start", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.persuasion_failed_with_poachers_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_rejected_with_poachers_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_attempt", "poachers_persuasion_start_reservation", "poachers_persuasion_select_option", "{=wM77S68a}What's there to discuss?", () => !this.persuasion_failed_with_poachers_on_condition(), null, this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_success", "poachers_persuasion_start_reservation", "close_window", "{=JQKCPllJ}You've made your point.", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.persuasion_complete_with_poachers_on_consequence), this, 200, null, null, null);
				string text = "poachers_persuasion_select_option_1";
				string text2 = "poachers_persuasion_select_option";
				string text3 = "poachers_persuasion_selected_option_response";
				string text4 = "{=!}{POACHERS_PERSUADE_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text5 = "poachers_persuasion_select_option_2";
				string text6 = "poachers_persuasion_select_option";
				string text7 = "poachers_persuasion_selected_option_response";
				string text8 = "{=!}{POACHERS_PERSUADE_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text9 = "poachers_persuasion_select_option_3";
				string text10 = "poachers_persuasion_select_option";
				string text11 = "poachers_persuasion_selected_option_response";
				string text12 = "{=!}{POACHERS_PERSUADE_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text13 = "poachers_persuasion_select_option_4";
				string text14 = "poachers_persuasion_select_option";
				string text15 = "poachers_persuasion_selected_option_response";
				string text16 = "{=!}{POACHERS_PERSUADE_ATTEMPT_4}";
				ConversationSentence.OnConditionDelegate onConditionDelegate4 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_4_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate4 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_4_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_4);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_4_on_condition);
				dialog.AddPlayerLine(text13, text14, text15, text16, onConditionDelegate4, onConsequenceDelegate4, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text17 = "poachers_persuasion_select_option_5";
				string text18 = "poachers_persuasion_select_option";
				string text19 = "poachers_persuasion_selected_option_response";
				string text20 = "{=!}{POACHERS_PERSUADE_ATTEMPT_5}";
				ConversationSentence.OnConditionDelegate onConditionDelegate5 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_5_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate5 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_5_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_5);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_5_on_condition);
				dialog.AddPlayerLine(text17, text18, text19, text20, onConditionDelegate5, onConsequenceDelegate5, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				dialog.AddDialogLine("poachers_persuasion_select_option_reaction", "poachers_persuasion_selected_option_response", "poachers_persuasion_start_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			// Token: 0x06004F3D RID: 20285 RVA: 0x0015F3F6 File Offset: 0x0015D5F6
			private void persuasion_start_with_poachers_on_consequence()
			{
				ConversationManager.StartPersuasion(2f, 1f, 0f, 2f, 2f, 0f, PersuasionDifficulty.MediumHard);
			}

			// Token: 0x06004F3E RID: 20286 RVA: 0x0015F41C File Offset: 0x0015D61C
			private bool persuasion_start_with_poachers_on_condition()
			{
				return this._poachersParty != null && CharacterObject.OneToOneConversationCharacter == ConversationHelper.GetConversationCharacterPartyLeader(this._poachersParty.Party);
			}

			// Token: 0x06004F3F RID: 20287 RVA: 0x0015F440 File Offset: 0x0015D640
			private PersuasionTask GetPersuasionTask()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=l7Jt5tvt}This is how I earn my living, and all your clever talk doesn't make it any different. Leave now!", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}TODO", null);
				persuasionTask.SpokenLine = new TextObject("{=wM77S68a}What's there to discuss?", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Easy, false, new TextObject("{=cQCs72U7}You're not bad people. You can easily ply your trade somewhere else, somewhere safe.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.ExtremelyHard, true, new TextObject("{=bioyMrUD}You are just a bunch of hunters. You don't stand a chance against us!", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=FO1oruNy}You talk about poor folk, but you think the people here like their village turned into a nest of outlaws?", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				TextObject textObject = new TextObject("{=S0NeQdLp}You had an agreement with {QUEST_GIVER.NAME}. Your word is your bond, no matter which side of the law you're on.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, textObject, null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs4);
				TextObject textObject2 = new TextObject("{=brW4pjPQ}Flee while you can. An army is already on its way here to hang you all.", null);
				PersuasionOptionArgs persuasionOptionArgs5 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Hard, true, textObject2, null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs5);
				return persuasionTask;
			}

			// Token: 0x06004F40 RID: 20288 RVA: 0x0015F578 File Offset: 0x0015D778
			private bool poachers_persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				if (item == PersuasionOptionResult.CriticalFailure)
				{
					this._task.BlockAllOptions();
				}
				return true;
			}

			// Token: 0x06004F41 RID: 20289 RVA: 0x0015F5B8 File Offset: 0x0015D7B8
			private void poachers_persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(PersuasionDifficulty.MediumHard);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, out num, out num2, difficulty);
				this._task.ApplyEffects(num, num2);
			}

			// Token: 0x06004F42 RID: 20290 RVA: 0x0015F614 File Offset: 0x0015D814
			private bool poachers_persuasion_select_option_1_on_condition()
			{
				if (this._task.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F43 RID: 20291 RVA: 0x0015F694 File Offset: 0x0015D894
			private bool poachers_persuasion_select_option_2_on_condition()
			{
				if (this._task.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F44 RID: 20292 RVA: 0x0015F714 File Offset: 0x0015D914
			private bool poachers_persuasion_select_option_3_on_condition()
			{
				if (this._task.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F45 RID: 20293 RVA: 0x0015F794 File Offset: 0x0015D994
			private bool poachers_persuasion_select_option_4_on_condition()
			{
				if (this._task.Options.Count > 3)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(3), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(3).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_4", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F46 RID: 20294 RVA: 0x0015F814 File Offset: 0x0015DA14
			private bool poachers_persuasion_select_option_5_on_condition()
			{
				if (this._task.Options.Count > 4)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(4), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(4).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_5", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F47 RID: 20295 RVA: 0x0015F894 File Offset: 0x0015DA94
			private void poachers_persuasion_select_option_1_on_consequence()
			{
				if (this._task.Options.Count > 0)
				{
					this._task.Options[0].BlockTheOption(true);
				}
			}

			// Token: 0x06004F48 RID: 20296 RVA: 0x0015F8C0 File Offset: 0x0015DAC0
			private void poachers_persuasion_select_option_2_on_consequence()
			{
				if (this._task.Options.Count > 1)
				{
					this._task.Options[1].BlockTheOption(true);
				}
			}

			// Token: 0x06004F49 RID: 20297 RVA: 0x0015F8EC File Offset: 0x0015DAEC
			private void poachers_persuasion_select_option_3_on_consequence()
			{
				if (this._task.Options.Count > 2)
				{
					this._task.Options[2].BlockTheOption(true);
				}
			}

			// Token: 0x06004F4A RID: 20298 RVA: 0x0015F918 File Offset: 0x0015DB18
			private void poachers_persuasion_select_option_4_on_consequence()
			{
				if (this._task.Options.Count > 3)
				{
					this._task.Options[3].BlockTheOption(true);
				}
			}

			// Token: 0x06004F4B RID: 20299 RVA: 0x0015F944 File Offset: 0x0015DB44
			private void poachers_persuasion_select_option_5_on_consequence()
			{
				if (this._task.Options.Count > 4)
				{
					this._task.Options[4].BlockTheOption(true);
				}
			}

			// Token: 0x06004F4C RID: 20300 RVA: 0x0015F970 File Offset: 0x0015DB70
			private bool persuasion_failed_with_poachers_on_condition()
			{
				if (this._task.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", this._task.FinalFailLine, false);
					return true;
				}
				return false;
			}

			// Token: 0x06004F4D RID: 20301 RVA: 0x0015F9CE File Offset: 0x0015DBCE
			private PersuasionOptionArgs poachers_persuasion_setup_option_1()
			{
				return this._task.Options.ElementAt(0);
			}

			// Token: 0x06004F4E RID: 20302 RVA: 0x0015F9E1 File Offset: 0x0015DBE1
			private PersuasionOptionArgs poachers_persuasion_setup_option_2()
			{
				return this._task.Options.ElementAt(1);
			}

			// Token: 0x06004F4F RID: 20303 RVA: 0x0015F9F4 File Offset: 0x0015DBF4
			private PersuasionOptionArgs poachers_persuasion_setup_option_3()
			{
				return this._task.Options.ElementAt(2);
			}

			// Token: 0x06004F50 RID: 20304 RVA: 0x0015FA07 File Offset: 0x0015DC07
			private PersuasionOptionArgs poachers_persuasion_setup_option_4()
			{
				return this._task.Options.ElementAt(3);
			}

			// Token: 0x06004F51 RID: 20305 RVA: 0x0015FA1A File Offset: 0x0015DC1A
			private PersuasionOptionArgs poachers_persuasion_setup_option_5()
			{
				return this._task.Options.ElementAt(4);
			}

			// Token: 0x06004F52 RID: 20306 RVA: 0x0015FA30 File Offset: 0x0015DC30
			private bool poachers_persuasion_clickable_option_1_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 0)
				{
					hintText = (this._task.Options.ElementAt(0).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(0).IsBlocked;
				}
				return false;
			}

			// Token: 0x06004F53 RID: 20307 RVA: 0x0015FA9C File Offset: 0x0015DC9C
			private bool poachers_persuasion_clickable_option_2_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 1)
				{
					hintText = (this._task.Options.ElementAt(1).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(1).IsBlocked;
				}
				return false;
			}

			// Token: 0x06004F54 RID: 20308 RVA: 0x0015FB08 File Offset: 0x0015DD08
			private bool poachers_persuasion_clickable_option_3_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 2)
				{
					hintText = (this._task.Options.ElementAt(2).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(2).IsBlocked;
				}
				return false;
			}

			// Token: 0x06004F55 RID: 20309 RVA: 0x0015FB74 File Offset: 0x0015DD74
			private bool poachers_persuasion_clickable_option_4_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 3)
				{
					hintText = (this._task.Options.ElementAt(3).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(3).IsBlocked;
				}
				return false;
			}

			// Token: 0x06004F56 RID: 20310 RVA: 0x0015FBE0 File Offset: 0x0015DDE0
			private bool poachers_persuasion_clickable_option_5_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 4)
				{
					hintText = (this._task.Options.ElementAt(4).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(4).IsBlocked;
				}
				return false;
			}

			// Token: 0x06004F57 RID: 20311 RVA: 0x0015FC4B File Offset: 0x0015DE4B
			private void persuasion_rejected_with_poachers_on_consequence()
			{
				PlayerEncounter.LeaveEncounter = false;
				ConversationManager.EndPersuasion();
			}

			// Token: 0x06004F58 RID: 20312 RVA: 0x0015FC58 File Offset: 0x0015DE58
			private void persuasion_complete_with_poachers_on_consequence()
			{
				PlayerEncounter.LeaveEncounter = true;
				ConversationManager.EndPersuasion();
				Campaign.Current.GameMenuManager.SetNextMenu("village");
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.QuestSuccessPlayerComesToAnAgreementWithPoachers;
			}

			// Token: 0x06004F59 RID: 20313 RVA: 0x0015FC94 File Offset: 0x0015DE94
			internal void StartQuestBattle()
			{
				PlayerEncounter.RestartPlayerEncounter(PartyBase.MainParty, this._poachersParty.Party, false);
				PlayerEncounter.StartBattle();
				PlayerEncounter.Update();
				this._talkedToPoachersBattleWillStart = false;
				GameMenu.ActivateGameMenu("army_of_poachers_village");
				CampaignMission.OpenBattleMission(this._questVillage.Settlement.LocationComplex.GetScene("village_center", 1));
				this._isReadyToBeFinalized = true;
			}

			// Token: 0x06004F5A RID: 20314 RVA: 0x0015FCFB File Offset: 0x0015DEFB
			private bool DialogCondition()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			// Token: 0x06004F5B RID: 20315 RVA: 0x0015FD0C File Offset: 0x0015DF0C
			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=IefM6uAy}Thank you. You'll be paid well. Also you can keep their illegally obtained leather.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.NpcLine(new TextObject("{=NC2VGafO}They skin their beasts in the woods, then go into the village after midnight to stash the hides. The villagers are terrified of them, I believe. If you go into the village late at night, you should be able to track them down.", null), null, null)
					.NpcLine(new TextObject("{=3pkVKMnA}Most poachers would probably run if they were surprised by armed men. But these ones are bold and desperate. Be ready for a fight.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=QNV1b5s5}Are those poachers still in business?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=JhJBBWab}They will be gone soon.", null), null)
					.NpcLine(new TextObject("{=gjGb044I}I hope they will be...", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=Gu3jF88V}Any night battle can easily go wrong. I need more time to prepare.", null), null)
					.NpcLine(new TextObject("{=2EiC1YyZ}Well, if they get wind of what you're up to, things could go very wrong for me. Do be quick.", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
				this.QuestCharacterDialogFlow = this.GetPoacherPartyDialogFlow();
			}

			// Token: 0x06004F5C RID: 20316 RVA: 0x0015FE24 File Offset: 0x0015E024
			internal void CreatePoachersParty()
			{
				this._poachersParty = MobileParty.CreateParty("poachers_party", null, null);
				TextObject textObject = new TextObject("{=WQa1R55u}Poachers Party", null);
				this._poachersParty.InitializeMobilePartyAtPosition(new TroopRoster(this._poachersParty.Party), new TroopRoster(this._poachersParty.Party), this._questVillage.Settlement.GetPosition2D);
				this._poachersParty.SetCustomName(textObject);
				EnterSettlementAction.ApplyForParty(this._poachersParty, Settlement.CurrentSettlement);
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("leather");
				int num = MathF.Ceiling(this._difficultyMultiplier * 5f) + MBRandom.RandomInt(0, 2);
				this._poachersParty.ItemRoster.AddToCounts(@object, num * 2);
				CharacterObject characterObject = CharacterObject.All.FirstOrDefault((CharacterObject t) => t.StringId == "poacher");
				int num2 = 10 + MathF.Ceiling(40f * this._difficultyMultiplier);
				this._poachersParty.MemberRoster.AddToCounts(characterObject, num2, false, 0, 0, true, -1);
				this._poachersParty.SetPartyUsedByQuest(true);
				this._poachersParty.Ai.DisableAi();
			}

			// Token: 0x06004F5D RID: 20317 RVA: 0x0015FF5A File Offset: 0x0015E15A
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._questStartedLogText, false);
				base.AddTrackedObject(this._questVillage.Settlement);
			}

			// Token: 0x06004F5E RID: 20318 RVA: 0x0015FF84 File Offset: 0x0015E184
			internal void QuestFailedAfterTalkingWithPoachers()
			{
				base.AddLog(this._questFailedAfterTalkingWithProachers, false);
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50),
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, 20)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Prosperity -= 30f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06004F5F RID: 20319 RVA: 0x0016002C File Offset: 0x0015E22C
			internal void QuestSuccessPlayerComesToAnAgreementWithPoachers()
			{
				base.AddLog(this._questSuccessPlayerComesToAnAgreementWithPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 10),
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, 50)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				base.QuestGiver.AddPower(30f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Prosperity += 50f;
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06004F60 RID: 20320 RVA: 0x001600F4 File Offset: 0x0015E2F4
			internal void QuestFailWithPlayerDefeatedAgainstPoachers()
			{
				base.AddLog(this._questFailWithPlayerDefeatedAgainstPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Prosperity -= 30f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06004F61 RID: 20321 RVA: 0x0016018C File Offset: 0x0015E38C
			internal void QuestSuccessWithPlayerDefeatedPoachers()
			{
				base.AddLog(this._questSuccessWithPlayerDefeatedPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				base.QuestGiver.AddPower(30f);
				base.QuestGiver.CurrentSettlement.Prosperity += 50f;
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06004F62 RID: 20322 RVA: 0x00160214 File Offset: 0x0015E414
			protected override void OnTimedOut()
			{
				base.AddLog(this._questFailedWithTimeOutLogText, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Prosperity -= 30f;
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
			}

			// Token: 0x06004F63 RID: 20323 RVA: 0x001602A4 File Offset: 0x0015E4A4
			private void QuestCanceledTargetVillageRaided()
			{
				base.AddLog(this._questCanceledTargetVillageRaided, false);
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06004F64 RID: 20324 RVA: 0x001602BC File Offset: 0x0015E4BC
			protected override void RegisterEvents()
			{
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventCheck));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.MapEventStarted));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.GameMenuOpened));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
				CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnCanHeroBecomePrisonerInfoIsRequested));
			}

			// Token: 0x06004F65 RID: 20325 RVA: 0x0016036A File Offset: 0x0015E56A
			private void OnCanHeroBecomePrisonerInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == Hero.MainHero && this._isReadyToBeFinalized)
				{
					result = false;
				}
			}

			// Token: 0x06004F66 RID: 20326 RVA: 0x00160380 File Offset: 0x0015E580
			private void OnHourlyTick()
			{
				if (PlayerEncounter.Current != null && PlayerEncounter.Current.IsPlayerWaiting && PlayerEncounter.EncounterSettlement == this._questVillage.Settlement && CampaignTime.Now.IsNightTime && !this._isReadyToBeFinalized && base.IsOngoing)
				{
					EnterSettlementAction.ApplyForParty(MobileParty.MainParty, this._questVillage.Settlement);
					GameMenu.SwitchToMenu("army_of_poachers_village");
				}
			}

			// Token: 0x06004F67 RID: 20327 RVA: 0x001603F0 File Offset: 0x0015E5F0
			private void GameMenuOpened(MenuCallbackArgs obj)
			{
				if (obj.MenuContext.GameMenu.StringId == "village" && CampaignTime.Now.IsNightTime && Settlement.CurrentSettlement == this._questVillage.Settlement && !this._isReadyToBeFinalized)
				{
					GameMenu.SwitchToMenu("army_of_poachers_village");
				}
				if (obj.MenuContext.GameMenu.StringId == "army_of_poachers_village" && this._isReadyToBeFinalized && MapEvent.PlayerMapEvent != null && MapEvent.PlayerMapEvent.HasWinner && this._poachersParty != null)
				{
					this._poachersParty.IsVisible = false;
				}
			}

			// Token: 0x06004F68 RID: 20328 RVA: 0x00160497 File Offset: 0x0015E697
			private void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				this.MapEventCheck(mapEvent);
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06004F69 RID: 20329 RVA: 0x001604B1 File Offset: 0x0015E6B1
			private void MapEventCheck(MapEvent mapEvent)
			{
				if (mapEvent.IsRaid && mapEvent.MapEventSettlement == this._questVillage.Settlement)
				{
					this.QuestCanceledTargetVillageRaided();
				}
			}

			// Token: 0x06004F6A RID: 20330 RVA: 0x001604D4 File Offset: 0x0015E6D4
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questCanceledWarDeclared);
				}
			}

			// Token: 0x06004F6B RID: 20331 RVA: 0x00160503 File Offset: 0x0015E703
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclared);
			}

			// Token: 0x06004F6C RID: 20332 RVA: 0x0016051A File Offset: 0x0015E71A
			protected override void OnFinalize()
			{
				if (this._poachersParty != null && this._poachersParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._poachersParty);
				}
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
				}
			}

			// Token: 0x06004F6D RID: 20333 RVA: 0x00160554 File Offset: 0x0015E754
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			// Token: 0x04001A79 RID: 6777
			[SaveableField(10)]
			internal MobileParty _poachersParty;

			// Token: 0x04001A7A RID: 6778
			[SaveableField(20)]
			internal Village _questVillage;

			// Token: 0x04001A7B RID: 6779
			[SaveableField(30)]
			internal bool _talkedToPoachersBattleWillStart;

			// Token: 0x04001A7C RID: 6780
			[SaveableField(40)]
			internal bool _isReadyToBeFinalized;

			// Token: 0x04001A7D RID: 6781
			[SaveableField(50)]
			internal bool _persuasionTriedOnce;

			// Token: 0x04001A7E RID: 6782
			[SaveableField(60)]
			internal float _difficultyMultiplier;

			// Token: 0x04001A7F RID: 6783
			[SaveableField(70)]
			internal int _rewardGold;

			// Token: 0x04001A80 RID: 6784
			private PersuasionTask _task;

			// Token: 0x04001A81 RID: 6785
			private const PersuasionDifficulty Difficulty = PersuasionDifficulty.MediumHard;
		}

		// Token: 0x02000651 RID: 1617
		public class MerchantArmyOfPoachersIssueBehaviorTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06004F75 RID: 20341 RVA: 0x001605CB File Offset: 0x0015E7CB
			public MerchantArmyOfPoachersIssueBehaviorTypeDefiner()
				: base(800000)
			{
			}

			// Token: 0x06004F76 RID: 20342 RVA: 0x001605D8 File Offset: 0x0015E7D8
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), 1, null);
				base.AddClassDefinition(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest), 2, null);
			}
		}
	}
}
