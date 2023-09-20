using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	// Token: 0x0200007B RID: 123
	public class FamilyFeudIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000549 RID: 1353 RVA: 0x00025FA0 File Offset: 0x000241A0
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00025FBC File Offset: 0x000241BC
		public void OnCheckForIssue(Hero hero)
		{
			Settlement settlement;
			Hero hero2;
			if (this.ConditionsHold(hero, out settlement, out hero2))
			{
				KeyValuePair<Hero, Settlement> keyValuePair = new KeyValuePair<Hero, Settlement>(hero2, settlement);
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(FamilyFeudIssueBehavior.FamilyFeudIssue), 2, keyValuePair));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(FamilyFeudIssueBehavior.FamilyFeudIssue), 2));
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00026034 File Offset: 0x00024234
		private bool ConditionsHold(Hero issueGiver, out Settlement otherVillage, out Hero otherNotable)
		{
			otherVillage = null;
			otherNotable = null;
			if (!issueGiver.IsNotable)
			{
				return false;
			}
			if (issueGiver.IsRuralNotable && issueGiver.CurrentSettlement.IsVillage)
			{
				Settlement bound = issueGiver.CurrentSettlement.Village.Bound;
				if (bound.IsTown)
				{
					foreach (Village village in LinQuick.WhereQ<Village>(bound.BoundVillages, (Village x) => x != issueGiver.CurrentSettlement.Village))
					{
						Hero hero = LinQuick.FirstOrDefaultQ<Hero>(village.Settlement.Notables, (Hero y) => y.IsRuralNotable && y.CanHaveQuestsOrIssues() && y.GetTraitLevel(DefaultTraits.Mercy) <= 0);
						if (hero != null)
						{
							otherVillage = village.Settlement;
							otherNotable = hero;
						}
					}
					return otherVillage != null;
				}
			}
			return false;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0002613C File Offset: 0x0002433C
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			KeyValuePair<Hero, Settlement> keyValuePair = (KeyValuePair<Hero, Settlement>)potentialIssueData.RelatedObject;
			return new FamilyFeudIssueBehavior.FamilyFeudIssue(issueOwner, keyValuePair.Key, keyValuePair.Value);
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00026171 File Offset: 0x00024371
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0400029D RID: 669
		private const IssueBase.IssueFrequency FamilyFeudIssueFrequency = 2;

		// Token: 0x02000149 RID: 329
		public class FamilyFeudIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000D7D RID: 3453 RVA: 0x000639AE File Offset: 0x00061BAE
			public FamilyFeudIssueTypeDefiner()
				: base(1087000)
			{
			}

			// Token: 0x06000D7E RID: 3454 RVA: 0x000639BB File Offset: 0x00061BBB
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(FamilyFeudIssueBehavior.FamilyFeudIssue), 1, null);
				base.AddClassDefinition(typeof(FamilyFeudIssueBehavior.FamilyFeudIssueQuest), 2, null);
			}
		}

		// Token: 0x0200014A RID: 330
		public class FamilyFeudIssueMissionBehavior : MissionLogic
		{
			// Token: 0x06000D7F RID: 3455 RVA: 0x000639E1 File Offset: 0x00061BE1
			public FamilyFeudIssueMissionBehavior(Action<Agent, Agent, int> agentHitAction)
			{
				this.OnAgentHitAction = agentHitAction;
			}

			// Token: 0x06000D80 RID: 3456 RVA: 0x000639F0 File Offset: 0x00061BF0
			public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
			{
				Action<Agent, Agent, int> onAgentHitAction = this.OnAgentHitAction;
				if (onAgentHitAction == null)
				{
					return;
				}
				onAgentHitAction(affectedAgent, affectorAgent, blow.InflictedDamage);
			}

			// Token: 0x04000600 RID: 1536
			private Action<Agent, Agent, int> OnAgentHitAction;
		}

		// Token: 0x0200014B RID: 331
		public class FamilyFeudIssue : IssueBase
		{
			// Token: 0x170000F2 RID: 242
			// (get) Token: 0x06000D81 RID: 3457 RVA: 0x00063A0B File Offset: 0x00061C0B
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x170000F3 RID: 243
			// (get) Token: 0x06000D82 RID: 3458 RVA: 0x00063A0E File Offset: 0x00061C0E
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 3 + MathF.Ceiling(5f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170000F4 RID: 244
			// (get) Token: 0x06000D83 RID: 3459 RVA: 0x00063A23 File Offset: 0x00061C23
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(7f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170000F5 RID: 245
			// (get) Token: 0x06000D84 RID: 3460 RVA: 0x00063A38 File Offset: 0x00061C38
			protected override int RewardGold
			{
				get
				{
					return (int)(350f + 1500f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170000F6 RID: 246
			// (get) Token: 0x06000D85 RID: 3461 RVA: 0x00063A4D File Offset: 0x00061C4D
			// (set) Token: 0x06000D86 RID: 3462 RVA: 0x00063A55 File Offset: 0x00061C55
			[SaveableProperty(30)]
			public override Hero CounterOfferHero { get; protected set; }

			// Token: 0x170000F7 RID: 247
			// (get) Token: 0x06000D87 RID: 3463 RVA: 0x00063A5E File Offset: 0x00061C5E
			public override int NeededInfluenceForLordSolution
			{
				get
				{
					return 20;
				}
			}

			// Token: 0x170000F8 RID: 248
			// (get) Token: 0x06000D88 RID: 3464 RVA: 0x00063A62 File Offset: 0x00061C62
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 700f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170000F9 RID: 249
			// (get) Token: 0x06000D89 RID: 3465 RVA: 0x00063A78 File Offset: 0x00061C78
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=zRJ1bQFO}{ISSUE_GIVER.LINK}, a landowner from {ISSUE_GIVER_SETTLEMENT}, told you about an incident that is about to turn into an ugly feud. One of the youngsters killed another in an accident and the victim's family refused blood money as compensation and wants blood. You decided to leave {COMPANION.LINK} with some men for {RETURN_DAYS} days to let things cool down. They should return with the reward of {REWARD_GOLD}{GOLD_ICON} denars as promised by {ISSUE_GIVER.LINK} after {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("ISSUE_GIVER_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x170000FA RID: 250
			// (get) Token: 0x06000D8A RID: 3466 RVA: 0x00063B13 File Offset: 0x00061D13
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170000FB RID: 251
			// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00063B16 File Offset: 0x00061D16
			public override bool IsThereLordSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170000FC RID: 252
			// (get) Token: 0x06000D8C RID: 3468 RVA: 0x00063B19 File Offset: 0x00061D19
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=7qPda0SA}One of my relatives fell victim to his temper during a quarrel and killed a man from {TARGET_VILLAGE}. We have offered to pay blood money but the family of the deceased have stubbornly refused it. As it turns out, the deceased is kin to {TARGET_NOTABLE}, an elder of this region and now the men of {TARGET_VILLAGE} have sworn to kill my relative.", null);
					textObject.SetTextVariable("TARGET_VILLAGE", this._targetVillage.Name);
					textObject.SetTextVariable("TARGET_NOTABLE", this._targetNotable.Name);
					return textObject;
				}
			}

			// Token: 0x170000FD RID: 253
			// (get) Token: 0x06000D8D RID: 3469 RVA: 0x00063B54 File Offset: 0x00061D54
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=XX3sWsVX}This sounds pretty serious. Go on.", null);
				}
			}

			// Token: 0x170000FE RID: 254
			// (get) Token: 0x06000D8E RID: 3470 RVA: 0x00063B64 File Offset: 0x00061D64
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=mgUoXwZt}My family is concerned for the boy's life. He has gone hiding around the village commons. We need someone who can protect him until {TARGET_NOTABLE.LINK} sees reason, accepts the blood money and ends the feud. We would be eternally grateful, if you can help my relative and take him with you for a while maybe.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_VILLAGE", this._targetVillage.Name);
					return textObject;
				}
			}

			// Token: 0x170000FF RID: 255
			// (get) Token: 0x06000D8F RID: 3471 RVA: 0x00063BAD File Offset: 0x00061DAD
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=cDYz49kZ}You can keep my relative under your protection for a time until the calls for vengeance die down. Maybe you can leave one of your warrior companions and {ALTERNATIVE_TROOP_COUNT} men with him to protect him.", null);
					textObject.SetTextVariable("ALTERNATIVE_TROOP_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			// Token: 0x17000100 RID: 256
			// (get) Token: 0x06000D90 RID: 3472 RVA: 0x00063BCC File Offset: 0x00061DCC
			protected override TextObject LordSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=oJt4bemH}{QUEST_GIVER.LINK}, a landowner from {QUEST_SETTLEMENT}, told you about an incident that is about to turn into an ugly feud. One young man killed another in an quarrel and the victim's family refused blood money compensation, demanding vengeance instead.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x17000101 RID: 257
			// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00063C34 File Offset: 0x00061E34
			protected override TextObject LordSolutionCounterOfferRefuseLog
			{
				get
				{
					TextObject textObject = new TextObject("{=JqN5BSjN}As the dispenser of justice in the district, you decided to allow {TARGET_NOTABLE.LINK} to take vengeance for {?TARGET_NOTABLE.GENDER}her{?}his{\\?} kinsman. You failed to protect the culprit as you promised. {QUEST_GIVER.LINK} is furious.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000102 RID: 258
			// (get) Token: 0x06000D92 RID: 3474 RVA: 0x00063C80 File Offset: 0x00061E80
			protected override TextObject LordSolutionCounterOfferAcceptLog
			{
				get
				{
					TextObject textObject = new TextObject("{=UxrXNSW7}As the ruler, you have let {TARGET_NOTABLE.LINK} to take {?TARGET_NOTABLE.GENDER}her{?}him{\\?} kinsman's vengeance and failed to protect the boy as you have promised to {QUEST_GIVER.LINK}. {QUEST_GIVER.LINK} is furious.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000103 RID: 259
			// (get) Token: 0x06000D93 RID: 3475 RVA: 0x00063CDC File Offset: 0x00061EDC
			public override TextObject IssueLordSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=tsjwrZCZ}I am sure that, as {?PLAYER.GENDER}lady{?}lord{\\?} of this district, you will not let these unlawful threats go unpunished. As the lord of the region, you can talk to {TARGET_NOTABLE.LINK} and force him to accept the blood money.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000104 RID: 260
			// (get) Token: 0x06000D94 RID: 3476 RVA: 0x00063D0E File Offset: 0x00061F0E
			public override TextObject IssuePlayerResponseAfterLordExplanation
			{
				get
				{
					return new TextObject("{=A3GfCPUb}I'm not sure about using my authority in this way. Is there any other way to solve this?", null);
				}
			}

			// Token: 0x17000105 RID: 261
			// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00063D1B File Offset: 0x00061F1B
			public override TextObject IssuePlayerResponseAfterAlternativeExplanation
			{
				get
				{
					return new TextObject("{=8EaCJ2uw}What else can I do?", null);
				}
			}

			// Token: 0x17000106 RID: 262
			// (get) Token: 0x06000D96 RID: 3478 RVA: 0x00063D28 File Offset: 0x00061F28
			public override TextObject IssueLordSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=Du31GKSb}As the magistrate of this district, I hereby order that blood money shall be accepted. This is a crime of passion, not malice. Tell {TARGET_NOTABLE.LINK} to take the silver or face my wrath!", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000107 RID: 263
			// (get) Token: 0x06000D97 RID: 3479 RVA: 0x00063D5A File Offset: 0x00061F5A
			public override TextObject IssueLordSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=xNyLPMnx}Thank you, my {?PLAYER.GENDER}lady{?}lord{\\?}, thank you.", null);
				}
			}

			// Token: 0x17000108 RID: 264
			// (get) Token: 0x06000D98 RID: 3480 RVA: 0x00063D68 File Offset: 0x00061F68
			public override TextObject IssueLordSolutionCounterOfferExplanationByOtherNpc
			{
				get
				{
					TextObject textObject = new TextObject("{=vjk2q3OT}{?PLAYER.GENDER}Madam{?}Sir{\\?}, {TARGET_NOTABLE.LINK}'s nephew murdered one of my kinsman, and it is our right to take vengeance on the murderer. Custom gives us the right of vengeance. Everyone must know that we are willing to avenge our sons, or others will think little of killing them. Does it do us good to be a clan of old men and women, drowning in silver, if all our sons are slain? Please sir, allow us to take vengeance. We promise we won't let this turn into a senseless blood feud.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000109 RID: 265
			// (get) Token: 0x06000D99 RID: 3481 RVA: 0x00063DAC File Offset: 0x00061FAC
			public override TextObject IssueLordSolutionCounterOfferBriefByOtherNpc
			{
				get
				{
					return new TextObject("{=JhbbB2dp}My {?PLAYER.GENDER}lady{?}lord{\\?}, may I have a word please?", null);
				}
			}

			// Token: 0x1700010A RID: 266
			// (get) Token: 0x06000D9A RID: 3482 RVA: 0x00063DB9 File Offset: 0x00061FB9
			public override TextObject IssueLordSolutionCounterOfferAcceptByPlayer
			{
				get
				{
					return new TextObject("{=TIVHLAjy}You may have a point. I hereby revoke my previous decision.", null);
				}
			}

			// Token: 0x1700010B RID: 267
			// (get) Token: 0x06000D9B RID: 3483 RVA: 0x00063DC6 File Offset: 0x00061FC6
			public override TextObject IssueLordSolutionCounterOfferAcceptResponseByOtherNpc
			{
				get
				{
					return new TextObject("{=A9uSikTY}Thank you my {?PLAYER.GENDER}lady{?}lord{\\?}.", null);
				}
			}

			// Token: 0x1700010C RID: 268
			// (get) Token: 0x06000D9C RID: 3484 RVA: 0x00063DD3 File Offset: 0x00061FD3
			public override TextObject IssueLordSolutionCounterOfferDeclineByPlayer
			{
				get
				{
					return new TextObject("{=Vs9DfZmJ}No. My word is final. You will have to take the blood money.", null);
				}
			}

			// Token: 0x1700010D RID: 269
			// (get) Token: 0x06000D9D RID: 3485 RVA: 0x00063DE0 File Offset: 0x00061FE0
			public override TextObject IssueLordSolutionCounterOfferDeclineResponseByOtherNpc
			{
				get
				{
					return new TextObject("{=3oaVUNdr}I hope you won't be regret with your decision, my {?PLAYER.GENDER}lady{?}lord{\\?}.", null);
				}
			}

			// Token: 0x1700010E RID: 270
			// (get) Token: 0x06000D9E RID: 3486 RVA: 0x00063DED File Offset: 0x00061FED
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=VcfZdKcp}Don't worry, I will protect your relative.", null);
				}
			}

			// Token: 0x1700010F RID: 271
			// (get) Token: 0x06000D9F RID: 3487 RVA: 0x00063DFA File Offset: 0x00061FFA
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=ZpDQxmzJ}Family Feud", null);
				}
			}

			// Token: 0x17000110 RID: 272
			// (get) Token: 0x06000DA0 RID: 3488 RVA: 0x00063E08 File Offset: 0x00062008
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=aSZvZRYC}A relative of {QUEST_GIVER.NAME} kills a relative of {TARGET_NOTABLE.NAME}. {QUEST_GIVER.NAME} offers to pay blood money for the crime but {TARGET_NOTABLE.NAME} wants revenge.", null);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000111 RID: 273
			// (get) Token: 0x06000DA1 RID: 3489 RVA: 0x00063E52 File Offset: 0x00062052
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=9ZngZ6W7}I will have one of my companions and {REQUIRED_TROOP_AMOUNT} of my men protect your kinsman for {RETURN_DAYS} days. ", null);
					textObject.SetTextVariable("REQUIRED_TROOP_AMOUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x17000112 RID: 274
			// (get) Token: 0x06000DA2 RID: 3490 RVA: 0x00063E83 File Offset: 0x00062083
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					TextObject textObject = new TextObject("{=n9QRnxbC}I have no doubt that {TARGET_NOTABLE.LINK} will have to accept the offer after seeing the boy with that many armed men behind him. Thank you, {?PLAYER.GENDER}madam{?}sir{\\?}, for helping to ending this without more blood.", null);
					TextObjectExtensions.SetCharacterProperties(textObject, "TARGET_NOTABLE", this._targetNotable.CharacterObject, false);
					return textObject;
				}
			}

			// Token: 0x17000113 RID: 275
			// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x00063EA8 File Offset: 0x000620A8
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=MaGPKGHA}Thank you my {?PLAYER.GENDER}lady{?}lord{\\?}. I am sure your men will protect the boy and {TARGET_NOTABLE.LINK} will have nothing to do but to accept the blood money. I have to add, I'm ready to pay you {REWARD_GOLD}{GOLD_ICON} denars for your trouble.", null);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x17000114 RID: 276
			// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00063F10 File Offset: 0x00062110
			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=lmVCRD4Q}I hope {QUEST_GIVER.LINK} can work out that trouble with {?QUEST_GIVER.GENDER}her{?}his{\\?} kinsman.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000115 RID: 277
			// (get) Token: 0x06000DA5 RID: 3493 RVA: 0x00063F44 File Offset: 0x00062144
			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=vS6oZJPA}Your companion {COMPANION.LINK} and your men returns with the news of their success. Apparently {TARGET_NOTABLE.LINK} and {?TARGET_NOTABLE.GENDER}her{?}his{\\?} thugs finds the culprit and tries to murder him but your men manages to drive them away. {COMPANION.LINK} tells you that they bloodied their noses so badly that they wouldn’t dare to try again. {QUEST_GIVER.LINK} is grateful and sends {?QUEST_GIVER.GENDER}her{?}his{\\?} regards with a purse full of {REWARD}{GOLD_ICON} denars.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x06000DA6 RID: 3494 RVA: 0x00063FC9 File Offset: 0x000621C9
			public FamilyFeudIssue(Hero issueOwner, Hero targetNotable, Settlement targetVillage)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._targetNotable = targetNotable;
				this._targetVillage = targetVillage;
			}

			// Token: 0x06000DA7 RID: 3495 RVA: 0x00063FEA File Offset: 0x000621EA
			public override void OnHeroCanBeSelectedInInventoryInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonResrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000DA8 RID: 3496 RVA: 0x00063FF4 File Offset: 0x000621F4
			public override void OnHeroCanHavePartyRoleOrBeGovernorInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonResrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000DA9 RID: 3497 RVA: 0x00063FFE File Offset: 0x000621FE
			public override void OnHeroCanLeadPartyInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonResrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000DAA RID: 3498 RVA: 0x00064008 File Offset: 0x00062208
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonResrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000DAB RID: 3499 RVA: 0x00064012 File Offset: 0x00062212
			private void CommonResrictionInfoIsRequested(Hero hero, ref bool result)
			{
				if (this._targetNotable == hero)
				{
					result = false;
				}
			}

			// Token: 0x06000DAC RID: 3500 RVA: 0x00064020 File Offset: 0x00062220
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -1f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.1f;
				}
				return 0f;
			}

			// Token: 0x06000DAD RID: 3501 RVA: 0x00064043 File Offset: 0x00062243
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Athletics) >= hero.GetSkillValue(DefaultSkills.Charm)) ? DefaultSkills.Athletics : DefaultSkills.Charm, 120);
			}

			// Token: 0x06000DAE RID: 3502 RVA: 0x00064070 File Offset: 0x00062270
			protected override void LordSolutionConsequenceWithAcceptCounterOffer()
			{
				TraitLevelingHelper.OnIssueSolvedThroughBetrayal(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				this.RelationshipChangeWithIssueOwner = -10;
				ChangeRelationAction.ApplyPlayerRelation(this._targetNotable, 5, true, true);
				base.IssueOwner.CurrentSettlement.Village.Bound.Prosperity -= 5f;
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security -= 5f;
			}

			// Token: 0x06000DAF RID: 3503 RVA: 0x00064104 File Offset: 0x00062304
			protected override void LordSolutionConsequenceWithRefuseCounterOffer()
			{
				this.ApplySuccessRewards();
			}

			// Token: 0x06000DB0 RID: 3504 RVA: 0x0006410C File Offset: 0x0006230C
			public override bool LordSolutionCondition(out TextObject explanation)
			{
				if (base.IssueOwner.CurrentSettlement.OwnerClan == Clan.PlayerClan)
				{
					explanation = TextObject.Empty;
					return true;
				}
				explanation = new TextObject("{=9y0zpKUF}You need to be the owner of this settlement!", null);
				return false;
			}

			// Token: 0x06000DB1 RID: 3505 RVA: 0x0006413C File Offset: 0x0006233C
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000DB2 RID: 3506 RVA: 0x0006415D File Offset: 0x0006235D
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000DB3 RID: 3507 RVA: 0x00064175 File Offset: 0x00062375
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06000DB4 RID: 3508 RVA: 0x00064184 File Offset: 0x00062384
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.ApplySuccessRewards();
				float randomFloat = MBRandom.RandomFloat;
				SkillObject skillObject;
				if (randomFloat <= 0.33f)
				{
					skillObject = DefaultSkills.OneHanded;
				}
				else if (randomFloat <= 0.66f)
				{
					skillObject = DefaultSkills.TwoHanded;
				}
				else
				{
					skillObject = DefaultSkills.Polearm;
				}
				base.AlternativeSolutionHero.AddSkillXp(skillObject, (float)((int)(500f + 700f * base.IssueDifficultyMultiplier)));
			}

			// Token: 0x06000DB5 RID: 3509 RVA: 0x000641E8 File Offset: 0x000623E8
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -10;
				ChangeRelationAction.ApplyPlayerRelation(this._targetNotable, 5, true, true);
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security -= 5f;
				base.IssueOwner.CurrentSettlement.Village.Bound.Prosperity -= 5f;
			}

			// Token: 0x06000DB6 RID: 3510 RVA: 0x0006425C File Offset: 0x0006245C
			private void ApplySuccessRewards()
			{
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				this.RelationshipChangeWithIssueOwner = 10;
				ChangeRelationAction.ApplyPlayerRelation(this._targetNotable, -5, true, true);
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security += 10f;
			}

			// Token: 0x06000DB7 RID: 3511 RVA: 0x000642BB File Offset: 0x000624BB
			protected override void AfterIssueCreation()
			{
				this.CounterOfferHero = base.IssueOwner.CurrentSettlement.Notables.FirstOrDefault((Hero x) => x.CharacterObject.IsHero && x.CharacterObject.HeroObject != base.IssueOwner);
			}

			// Token: 0x06000DB8 RID: 3512 RVA: 0x000642E4 File Offset: 0x000624E4
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06000DB9 RID: 3513 RVA: 0x000642E6 File Offset: 0x000624E6
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new FamilyFeudIssueBehavior.FamilyFeudIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(20f), this._targetVillage, this._targetNotable, this.RewardGold);
			}

			// Token: 0x06000DBA RID: 3514 RVA: 0x00064310 File Offset: 0x00062510
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 2;
			}

			// Token: 0x06000DBB RID: 3515 RVA: 0x00064314 File Offset: 0x00062514
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				skill = null;
				relationHero = null;
				flag = 0;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag = (int)(flag | 1U);
					relationHero = issueGiver;
				}
				if (FactionManager.IsAtWarAgainstFaction(issueGiver.CurrentSettlement.MapFaction, Hero.MainHero.MapFaction))
				{
					flag = (int)(flag | 64U);
				}
				return flag == 0U;
			}

			// Token: 0x06000DBC RID: 3516 RVA: 0x0006436C File Offset: 0x0006256C
			public override bool IssueStayAliveConditions()
			{
				return this._targetNotable != null && this._targetNotable.IsActive && (this.CounterOfferHero == null || (this.CounterOfferHero.IsActive && this.CounterOfferHero.CurrentSettlement == base.IssueSettlement));
			}

			// Token: 0x06000DBD RID: 3517 RVA: 0x000643BC File Offset: 0x000625BC
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x06000DBE RID: 3518 RVA: 0x000643BE File Offset: 0x000625BE
			internal static void AutoGeneratedStaticCollectObjectsFamilyFeudIssue(object o, List<object> collectedObjects)
			{
				((FamilyFeudIssueBehavior.FamilyFeudIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000DBF RID: 3519 RVA: 0x000643CC File Offset: 0x000625CC
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetVillage);
				collectedObjects.Add(this._targetNotable);
				collectedObjects.Add(this.CounterOfferHero);
			}

			// Token: 0x06000DC0 RID: 3520 RVA: 0x000643F9 File Offset: 0x000625F9
			internal static object AutoGeneratedGetMemberValueCounterOfferHero(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssue)o).CounterOfferHero;
			}

			// Token: 0x06000DC1 RID: 3521 RVA: 0x00064406 File Offset: 0x00062606
			internal static object AutoGeneratedGetMemberValue_targetVillage(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssue)o)._targetVillage;
			}

			// Token: 0x06000DC2 RID: 3522 RVA: 0x00064413 File Offset: 0x00062613
			internal static object AutoGeneratedGetMemberValue_targetNotable(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssue)o)._targetNotable;
			}

			// Token: 0x04000601 RID: 1537
			private const int CompanionRequiredSkillLevel = 120;

			// Token: 0x04000602 RID: 1538
			private const int QuestTimeLimit = 20;

			// Token: 0x04000603 RID: 1539
			private const int IssueDuration = 30;

			// Token: 0x04000604 RID: 1540
			private const int TroopTierForAlternativeSolution = 2;

			// Token: 0x04000605 RID: 1541
			[SaveableField(10)]
			private Settlement _targetVillage;

			// Token: 0x04000606 RID: 1542
			[SaveableField(20)]
			private Hero _targetNotable;
		}

		// Token: 0x0200014C RID: 332
		public class FamilyFeudIssueQuest : QuestBase
		{
			// Token: 0x17000116 RID: 278
			// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x00064447 File Offset: 0x00062647
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000117 RID: 279
			// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x0006444A File Offset: 0x0006264A
			private bool FightEnded
			{
				get
				{
					return this._isCulpritDiedInMissionFight || this._isNotableKnockedDownInMissionFight || this._persuationInDoneAndSuccessfull;
				}
			}

			// Token: 0x17000118 RID: 280
			// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x00064464 File Offset: 0x00062664
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=ZpDQxmzJ}Family Feud", null);
				}
			}

			// Token: 0x17000119 RID: 281
			// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x00064474 File Offset: 0x00062674
			private TextObject PlayerStartsQuestLogText1
			{
				get
				{
					TextObject textObject = new TextObject("{=rjHQpVDZ}{QUEST_GIVER.LINK} a landowner from {QUEST_GIVER_SETTLEMENT}, told you about an incident that is about to turn into an ugly feud. One of the youngsters killed another during a quarrel and the victim's family refuses the blood money as compensation and wants blood.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x1700011A RID: 282
			// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x000644C4 File Offset: 0x000626C4
			private TextObject PlayerStartsQuestLogText2
			{
				get
				{
					TextObject textObject = new TextObject("{=fgRq7kF2}You have accepted to talk to {CULPRIT.LINK} in {QUEST_GIVER_SETTLEMENT} first and convince him to go to {TARGET_NOTABLE.LINK} with you in {TARGET_SETTLEMENT} and mediate the issue between them peacefully and end unnecessary bloodshed. {QUEST_GIVER.LINK} said {?QUEST_GIVER.GENDER}she{?}he{\\?} will pay you {REWARD_GOLD} once the boy is safe again.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("REWARD_GOLD", this._rewardGold);
					return textObject;
				}
			}

			// Token: 0x1700011B RID: 283
			// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x0006456C File Offset: 0x0006276C
			private TextObject SuccessQuestSolutionLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=KJ61SXEU}You have successfully protected {CULPRIT.LINK} from harm as you have promised. {QUEST_GIVER.LINK} is grateful for your service and sends his regards with a purse full of {REWARD_GOLD}{GOLD_ICON} denars for your trouble.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD_GOLD", this._rewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x1700011C RID: 284
			// (get) Token: 0x06000DCA RID: 3530 RVA: 0x000645DC File Offset: 0x000627DC
			private TextObject CulpritJoinedPlayerPartyLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=s5fXZf2f}You have convinced {CULPRIT.LINK} to go to {TARGET_SETTLEMENT} to face {TARGET_NOTABLE.LINK} to try to solve this issue peacefully. He agreed on the condition that you protect him from his victim's angry relatives.", null);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x1700011D RID: 285
			// (get) Token: 0x06000DCB RID: 3531 RVA: 0x00064640 File Offset: 0x00062840
			private TextObject QuestGiverVillageRaidedBeforeTalkingToCulpritCancel
			{
				get
				{
					TextObject textObject = new TextObject("{=gJG0xmAq}{QUEST_GIVER.LINK}'s village {QUEST_SETTLEMENT} was raided. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x1700011E RID: 286
			// (get) Token: 0x06000DCC RID: 3532 RVA: 0x00064690 File Offset: 0x00062890
			private TextObject TargetVillageRaidedBeforeTalkingToCulpritCancel
			{
				get
				{
					TextObject textObject = new TextObject("{=WqY4nvHc}{TARGET_NOTABLE.LINK}'s village {TARGET_SETTLEMENT} was raided. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x1700011F RID: 287
			// (get) Token: 0x06000DCD RID: 3533 RVA: 0x000646F4 File Offset: 0x000628F4
			private TextObject CulpritDiedQuestFail
			{
				get
				{
					TextObject textObject = new TextObject("{=6zcG8eng}You tried to defend {CULPRIT.LINK} but you were overcome. {NOTABLE.LINK} took {?NOTABLE.GENDER}her{?}his{\\?} revenge. You failed to protect {CULPRIT.LINK} as promised to {QUEST_GIVER.LINK}. {?QUEST_GIVER.GENDER}she{?}he{\\?} is furious.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000120 RID: 288
			// (get) Token: 0x06000DCE RID: 3534 RVA: 0x00064758 File Offset: 0x00062958
			private TextObject PlayerDiedInNotableBattle
			{
				get
				{
					TextObject textObject = new TextObject("{=kG92fjCY}You fell unconscious while defending {CULPRIT.LINK}. {TARGET_NOTABLE.LINK} has taken revenge. You failed to protect {CULPRIT.LINK} as you promised {QUEST_GIVER.LINK}. {?QUEST_GIVER.GENDER}She{?}He{\\?} is furious.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000121 RID: 289
			// (get) Token: 0x06000DCF RID: 3535 RVA: 0x000647BC File Offset: 0x000629BC
			private TextObject FailQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=LWjIbTBi}You failed to protect {CULPRIT.LINK} as you promised {QUEST_GIVER.LINK}. {QUEST_GIVER.LINK} is furious.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000122 RID: 290
			// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x00064808 File Offset: 0x00062A08
			private TextObject CulpritNoLongerAClanMember
			{
				get
				{
					TextObject textObject = new TextObject("{=wWrEvkuj}{CULPRIT.LINK} is no longer a member of your clan. Your agreement with {QUEST_GIVER.LINK} was terminated.", null);
					StringHelpers.SetCharacterProperties("CULPRIT", this._culprit.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06000DD1 RID: 3537 RVA: 0x00064854 File Offset: 0x00062A54
			public FamilyFeudIssueQuest(string questId, Hero questGiver, CampaignTime duration, Settlement targetSettlement, Hero targetHero, int rewardGold)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._targetSettlement = targetSettlement;
				this._targetNotable = targetHero;
				this._culpritJoinedPlayerParty = false;
				this._checkForMissionEvents = false;
				this._culprit = HeroCreator.CreateSpecialHero(MBObjectManager.Instance.GetObject<CharacterObject>("townsman_" + targetSettlement.Culture.StringId), targetSettlement, null, null, -1);
				this._culprit.SetNewOccupation(16);
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("short_sword_t3");
				this._culprit.CivilianEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(@object, null, null, false));
				this._culprit.BattleEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(@object, null, null, false));
				this._notableGangsterCharacterObject = questGiver.CurrentSettlement.MapFaction.Culture.GangleaderBodyguard;
				this._rewardGold = rewardGold;
				this.InitializeQuestDialogs();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06000DD2 RID: 3538 RVA: 0x00064940 File Offset: 0x00062B40
			private void InitializeQuestDialogs()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCulpritDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableThugDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowBeforeTalkingToCulprit(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowAfterTalkingToCulprit(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowAfterKillingCulprit(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowAfterPlayerBetrayCulprit(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCulpritDialogFlowAfterCulpritJoin(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowAfterNotableKnowdown(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotableDialogFlowAfterQuestEnd(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCulpritDialogFlowAfterQuestEnd(), this);
			}

			// Token: 0x06000DD3 RID: 3539 RVA: 0x00064A29 File Offset: 0x00062C29
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
				this.InitializeQuestDialogs();
				this._notableGangsterCharacterObject = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_1");
			}

			// Token: 0x06000DD4 RID: 3540 RVA: 0x00064A4C File Offset: 0x00062C4C
			private DialogFlow GetNotableDialogFlowBeforeTalkingToCulprit()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=dpTHWqwv}Are you the {?PLAYER.GENDER}woman{?}man{\\?} who thinks our blood is cheap, that we will accept silver for the life of one of our own?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.notable_culprit_is_not_near_on_condition))
					.NpcLine(new TextObject("{=Vd22iVGE}Well {?PLAYER.GENDER}lady{?}sir{\\?}, sorry to disappoint you, but our people have some self-respect.", null), null, null)
					.PlayerLine(new TextObject("{=a3AFjfsU}We will see. ", null), null)
					.NpcLine(new TextObject("{=AeJqCMJc}Yes, you will see. Good day to you. ", null), null, null)
					.CloseDialog();
			}

			// Token: 0x06000DD5 RID: 3541 RVA: 0x00064AC4 File Offset: 0x00062CC4
			private DialogFlow GetNotableDialogFlowAfterKillingCulprit()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=108Dchvt}Stop! We don't need to fight any longer. We have no quarrel with you as justice has been served.", null), null, null).Condition(() => this._isCulpritDiedInMissionFight && Hero.OneToOneConversationHero == this._targetNotable && !this._playerBetrayedCulprit)
					.NpcLine(new TextObject("{=NMrzr7Me}Now, leave peacefully...", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.CulpritDiedInNotableFightFail;
					})
					.CloseDialog();
			}

			// Token: 0x06000DD6 RID: 3542 RVA: 0x00064B28 File Offset: 0x00062D28
			private DialogFlow GetNotableDialogFlowAfterPlayerBetrayCulprit()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=4aiabOd4}I knew you are a reasonable {?PLAYER.GENDER}woman{?}man{\\?}.", null), null, null).Condition(() => this._isCulpritDiedInMissionFight && this._playerBetrayedCulprit && Hero.OneToOneConversationHero == this._targetNotable)
					.NpcLine(new TextObject("{=NMrzr7Me}Now, leave peacefully...", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.CulpritDiedInNotableFightFail;
					})
					.CloseDialog();
			}

			// Token: 0x06000DD7 RID: 3543 RVA: 0x00064B8C File Offset: 0x00062D8C
			private DialogFlow GetCulpritDialogFlowAfterCulpritJoin()
			{
				TextObject textObject = new TextObject("{=56ynu2bW}Yes, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
				TextObject textObject2 = new TextObject("{=c452Kevh}Well I'm anxious, but I am in your hands now. I trust you will protect me {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(() => !this.FightEnded && this._culpritJoinedPlayerParty && Hero.OneToOneConversationHero == this._culprit)
					.PlayerLine(new TextObject("{=p1ETQbzg}Just checking on you.", null), null)
					.NpcLine(textObject2, null, null)
					.CloseDialog();
			}

			// Token: 0x06000DD8 RID: 3544 RVA: 0x00064C18 File Offset: 0x00062E18
			private DialogFlow GetNotableDialogFlowAfterQuestEnd()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=UBFS1JLj}I have no problem with the boy anymore, okay? Just leave me alone.", null), null, null).Condition(() => this.FightEnded && !this._persuationInDoneAndSuccessfull && Hero.OneToOneConversationHero == this._targetNotable && !this._playerBetrayedCulprit)
					.CloseDialog()
					.NpcLine(new TextObject("{=adbQR9j0}I got my gold, you got your boy. Now leave me alone...", null), null, null)
					.Condition(() => this.FightEnded && this._persuationInDoneAndSuccessfull && Hero.OneToOneConversationHero == this._targetNotable && !this._playerBetrayedCulprit)
					.CloseDialog();
			}

			// Token: 0x06000DD9 RID: 3545 RVA: 0x00064C81 File Offset: 0x00062E81
			private DialogFlow GetCulpritDialogFlowAfterQuestEnd()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=OybG76Kf}Thank you for saving me, sir. I won't forget what you did here today.", null), null, null).Condition(() => this.FightEnded && Hero.OneToOneConversationHero == this._culprit)
					.CloseDialog();
			}

			// Token: 0x06000DDA RID: 3546 RVA: 0x00064CB8 File Offset: 0x00062EB8
			private DialogFlow GetNotableDialogFlowAfterNotableKnowdown()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=c6GbRQlg}Stop. We don’t need to fight any longer. You have made your point. We will accept the blood money.", null), (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsMainAgent(agent)).Condition(new ConversationSentence.OnConditionDelegate(this.multi_character_conversation_condition_after_fight))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.multi_character_conversation_consequence_after_fight))
					.NpcLine(new TextObject("{=pS0bBRjt}You! Go to your family and tell them to send us the blood money.", null), (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsCulprit(agent))
					.NpcLine(new TextObject("{=nxs2U0Yk}Leave now and never come back! If we ever see you here we will kill you.", null), (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsCulprit(agent))
					.NpcLine("{=udD7Y7mO}Thank you, my {?PLAYER.GENDER}lady{?}sir{\\?}, for protecting me. I will go and tell {ISSUE_GIVER.LINK} of your success here.", (IAgent agent) => this.IsCulprit(agent), (IAgent agent) => this.IsMainAgent(agent))
					.Condition(new ConversationSentence.OnConditionDelegate(this.AfterNotableKnowdownEndingCondition))
					.PlayerLine(new TextObject("{=g8qb3Ame}Thank you.", null), (IAgent agent) => this.IsCulprit(agent))
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerAndCulpritKnockedDownNotableQuestSuccess;
					})
					.CloseDialog();
			}

			// Token: 0x06000DDB RID: 3547 RVA: 0x00064DD0 File Offset: 0x00062FD0
			private bool AfterNotableKnowdownEndingCondition()
			{
				StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, null, false);
				return true;
			}

			// Token: 0x06000DDC RID: 3548 RVA: 0x00064DEB File Offset: 0x00062FEB
			private void PlayerAndCulpritKnockedDownNotableQuestSuccess()
			{
				this._conversationAfterFightIsDone = true;
				this.HandleAgentBehaviorAfterQuestConversations();
			}

			// Token: 0x06000DDD RID: 3549 RVA: 0x00064DFC File Offset: 0x00062FFC
			private void HandleAgentBehaviorAfterQuestConversations()
			{
				foreach (AccompanyingCharacter accompanyingCharacter in PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer)
				{
					if (accompanyingCharacter.LocationCharacter.Character == this._culprit.CharacterObject)
					{
						accompanyingCharacter.LocationCharacter.SpecialTargetTag = "npc_common";
						accompanyingCharacter.LocationCharacter.CharacterRelation = 0;
						this._culpritAgent.SetMortalityState(1);
						this._culpritAgent.SetTeam(Team.Invalid, false);
						DailyBehaviorGroup behaviorGroup = this._culpritAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
						behaviorGroup.AddBehavior<WalkingBehavior>();
						behaviorGroup.RemoveBehavior<FollowAgentBehavior>();
						this._culpritAgent.ResetEnemyCaches();
						this._culpritAgent.InvalidateTargetAgent();
						this._culpritAgent.InvalidateAIWeaponSelections();
						this._culpritAgent.SetWatchState(0);
						if (this._notableAgent != null)
						{
							this._notableAgent.ResetEnemyCaches();
							this._notableAgent.InvalidateTargetAgent();
							this._notableAgent.InvalidateAIWeaponSelections();
							this._notableAgent.SetWatchState(0);
						}
						this._culpritAgent.TryToSheathWeaponInHand(1, 3);
						this._culpritAgent.TryToSheathWeaponInHand(0, 3);
					}
				}
				Mission.Current.SetMissionMode(0, false);
			}

			// Token: 0x06000DDE RID: 3550 RVA: 0x00064F5C File Offset: 0x0006315C
			private void ApplySuccessConsequences()
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				Hero.MainHero.Clan.Renown += 1f;
				this.RelationshipChangeWithQuestGiver = 10;
				ChangeRelationAction.ApplyPlayerRelation(this._targetNotable, -5, true, true);
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security += 10f;
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06000DDF RID: 3551 RVA: 0x00064FDE File Offset: 0x000631DE
			private bool multi_character_conversation_condition_after_fight()
			{
				return !this._conversationAfterFightIsDone && Hero.OneToOneConversationHero == this._targetNotable && this._isNotableKnockedDownInMissionFight;
			}

			// Token: 0x06000DE0 RID: 3552 RVA: 0x00064FFD File Offset: 0x000631FD
			private void multi_character_conversation_consequence_after_fight()
			{
				if (Mission.Current.GetMissionBehavior<MissionConversationLogic>() != null)
				{
					Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { this._culpritAgent }, true);
				}
				this._conversationAfterFightIsDone = true;
			}

			// Token: 0x06000DE1 RID: 3553 RVA: 0x00065034 File Offset: 0x00063234
			private DialogFlow GetNotableDialogFlowAfterTalkingToCulprit()
			{
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=nh7a3Nog}Well well. Who did you bring to see us? Did he bring his funeral shroud with him? I hope so. He's not leaving here alive.", null), (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsCulprit(agent)).Condition(new ConversationSentence.OnConditionDelegate(this.multi_character_conversation_on_condition))
					.NpcLine(new TextObject("{=RsOmvdmU}We have come to talk! Just listen to us please!", null), (IAgent agent) => this.IsCulprit(agent), (IAgent agent) => this.IsTargetNotable(agent))
					.NpcLine("{=JUjvu4XL}I knew we'd find you eventually. Now you will face justice!", (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsCulprit(agent))
					.PlayerLine("{=UQyCoQCY}Wait! This lad is now under my protection. We have come to talk in peace..", (IAgent agent) => this.IsTargetNotable(agent))
					.NpcLine("{=7AiP4BwY}What there is to talk about? This bastard murdered one of my kinsman, and it is our right to take vengeance on him!", (IAgent agent) => this.IsTargetNotable(agent), (IAgent agent) => this.IsMainAgent(agent))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=2iVytG2y}I am not convinced. I will protect the accused until you see reason.", null), null)
					.NpcLine(new TextObject("{=4HokUcma}You will regret pushing your nose into issues that do not concern you!", null), null, null)
					.NpcLine(new TextObject("{=vjOkDM6C}If you defend a murderer than you die like a murderer. Boys, kill them all!", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
						{
							this.StartFightWithNotableGang(false);
						};
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=boAcQxVV}You're breaking the law.", null), null)
					.Condition(delegate
					{
						if (this._task != null)
						{
							return !this._task.Options.All((PersuasionOptionArgs x) => x.IsBlocked);
						}
						return true;
					})
					.GotoDialogState("start_notable_family_feud_persuasion")
					.PlayerOption(new TextObject("{=J5cQPqGQ}You are right. You are free to deliver justice as you see fit.", null), null)
					.NpcLine(new TextObject("{=aRPLW15x}Thank you. I knew you are a reasonable {?PLAYER.GENDER}woman{?}man{\\?}.", null), null, null)
					.NpcLine(new TextObject("{=k5R4qGtL}What? Are you just going to leave me here to be killed? My kin will never forget this!", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCulprit), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent))
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
						{
							this._playerBetrayedCulprit = true;
							this.StartFightWithNotableGang(this._playerBetrayedCulprit);
						};
					})
					.CloseDialog();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			// Token: 0x06000DE2 RID: 3554 RVA: 0x000651EE File Offset: 0x000633EE
			private bool IsMainAgent(IAgent agent)
			{
				return agent == Mission.Current.MainAgent;
			}

			// Token: 0x06000DE3 RID: 3555 RVA: 0x000651FD File Offset: 0x000633FD
			private bool IsTargetNotable(IAgent agent)
			{
				return agent.Character == this._targetNotable.CharacterObject;
			}

			// Token: 0x06000DE4 RID: 3556 RVA: 0x00065212 File Offset: 0x00063412
			private bool IsCulprit(IAgent agent)
			{
				return agent.Character == this._culprit.CharacterObject;
			}

			// Token: 0x06000DE5 RID: 3557 RVA: 0x00065228 File Offset: 0x00063428
			private bool notable_culprit_is_not_near_on_condition()
			{
				return Hero.OneToOneConversationHero == this._targetNotable && Mission.Current != null && !this.FightEnded && Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 10f, new MBList<Agent>()).All((Agent a) => a.Character != this._culprit.CharacterObject);
			}

			// Token: 0x06000DE6 RID: 3558 RVA: 0x0006528C File Offset: 0x0006348C
			private bool multi_character_conversation_on_condition()
			{
				if (Hero.OneToOneConversationHero != this._targetNotable || Mission.Current == null || this.FightEnded)
				{
					return false;
				}
				MBList<Agent> nearbyAgents = Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 10f, new MBList<Agent>());
				if (Extensions.IsEmpty<Agent>(nearbyAgents) || nearbyAgents.All((Agent a) => a.Character != this._culprit.CharacterObject))
				{
					return false;
				}
				foreach (Agent agent in nearbyAgents)
				{
					if (agent.Character == this._culprit.CharacterObject)
					{
						this._culpritAgent = agent;
						if (Mission.Current.GetMissionBehavior<MissionConversationLogic>() != null)
						{
							Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { this._culpritAgent }, true);
							break;
						}
						break;
					}
				}
				return true;
			}

			// Token: 0x06000DE7 RID: 3559 RVA: 0x00065380 File Offset: 0x00063580
			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				dialog.AddDialogLine("family_feud_notable_persuasion_check_accepted", "start_notable_family_feud_persuasion", "family_feud_notable_persuasion_start_reservation", "{=6P1ruzsC}Maybe...", null, new ConversationSentence.OnConsequenceDelegate(this.persuasion_start_with_notable_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("family_feud_notable_persuasion_failed", "family_feud_notable_persuasion_start_reservation", "persuation_failed", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.persuasion_failed_with_family_feud_notable_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_failed_with_notable_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("family_feud_notable_persuasion_rejected", "persuation_failed", "close_window", "{=vjOkDM6C}If you defend a murderer than you die like a murderer. Boys, kill them all!", null, new ConversationSentence.OnConsequenceDelegate(this.persuasion_failed_with_notable_start_fight_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("family_feud_notable_persuasion_attempt", "family_feud_notable_persuasion_start_reservation", "family_feud_notable_persuasion_select_option", "{CONTINUE_PERSUASION_LINE}", () => !this.persuasion_failed_with_family_feud_notable_on_condition(), null, this, 100, null, null, null);
				dialog.AddDialogLine("family_feud_notable_persuasion_success", "family_feud_notable_persuasion_start_reservation", "close_window", "{=qIQbIjVS}All right! I spare the boy's life. Now get out of my sight", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.persuasion_complete_with_notable_on_consequence), this, int.MaxValue, null, null, null);
				string text = "family_feud_notable_persuasion_select_option_1";
				string text2 = "family_feud_notable_persuasion_select_option";
				string text3 = "family_feud_notable_persuasion_selected_option_response";
				string text4 = "{=!}{FAMILY_FEUD_PERSUADE_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text5 = "family_feud_notable_persuasion_select_option_2";
				string text6 = "family_feud_notable_persuasion_select_option";
				string text7 = "family_feud_notable_persuasion_selected_option_response";
				string text8 = "{=!}{FAMILY_FEUD_PERSUADE_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text9 = "family_feud_notable_persuasion_select_option_3";
				string text10 = "family_feud_notable_persuasion_select_option";
				string text11 = "family_feud_notable_persuasion_selected_option_response";
				string text12 = "{=!}{FAMILY_FEUD_PERSUADE_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				dialog.AddDialogLine("family_feud_notable_persuasion_select_option_reaction", "family_feud_notable_persuasion_selected_option_response", "family_feud_notable_persuasion_start_reservation", "{=D0xDRqvm}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			// Token: 0x06000DE8 RID: 3560 RVA: 0x000655C1 File Offset: 0x000637C1
			private void persuasion_complete_with_notable_on_consequence()
			{
				ConversationManager.EndPersuasion();
				this._persuationInDoneAndSuccessfull = true;
				this.HandleAgentBehaviorAfterQuestConversations();
			}

			// Token: 0x06000DE9 RID: 3561 RVA: 0x000655D5 File Offset: 0x000637D5
			private void persuasion_failed_with_notable_on_consequence()
			{
				ConversationManager.EndPersuasion();
			}

			// Token: 0x06000DEA RID: 3562 RVA: 0x000655DC File Offset: 0x000637DC
			private void persuasion_failed_with_notable_start_fight_on_consequence()
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
				{
					this.StartFightWithNotableGang(false);
				};
			}

			// Token: 0x06000DEB RID: 3563 RVA: 0x000655FC File Offset: 0x000637FC
			private bool persuasion_failed_with_family_feud_notable_on_condition()
			{
				MBTextManager.SetTextVariable("CONTINUE_PERSUASION_LINE", "{=7B7BhVhV}Let's see what you will come up with...", false);
				if (this._task.Options.Any((PersuasionOptionArgs x) => x.IsBlocked))
				{
					MBTextManager.SetTextVariable("CONTINUE_PERSUASION_LINE", "{=wvbiyZfp}What else do you have to say?", false);
				}
				if (this._task.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", this._task.FinalFailLine, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000DEC RID: 3564 RVA: 0x000656AC File Offset: 0x000638AC
			private void persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(4);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, ref num, ref num2, difficulty);
				this._task.ApplyEffects(num, num2);
			}

			// Token: 0x06000DED RID: 3565 RVA: 0x00065708 File Offset: 0x00063908
			private bool persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				return true;
			}

			// Token: 0x06000DEE RID: 3566 RVA: 0x00065737 File Offset: 0x00063937
			private void persuasion_start_with_notable_on_consequence()
			{
				this._task = this.GetPersuasionTask();
				ConversationManager.StartPersuasion(2f, 1f, 0f, 2f, 2f, 0f, 4);
			}

			// Token: 0x06000DEF RID: 3567 RVA: 0x0006576C File Offset: 0x0006396C
			private bool persuasion_select_option_1_on_condition()
			{
				if (this._task.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("FAMILY_FEUD_PERSUADE_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000DF0 RID: 3568 RVA: 0x000657EC File Offset: 0x000639EC
			private bool persuasion_select_option_2_on_condition()
			{
				if (this._task.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("FAMILY_FEUD_PERSUADE_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000DF1 RID: 3569 RVA: 0x0006586C File Offset: 0x00063A6C
			private bool persuasion_select_option_3_on_condition()
			{
				if (this._task.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("FAMILY_FEUD_PERSUADE_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000DF2 RID: 3570 RVA: 0x000658EC File Offset: 0x00063AEC
			private void persuasion_select_option_1_on_consequence()
			{
				if (this._task.Options.Count > 0)
				{
					this._task.Options[0].BlockTheOption(true);
				}
			}

			// Token: 0x06000DF3 RID: 3571 RVA: 0x00065918 File Offset: 0x00063B18
			private void persuasion_select_option_2_on_consequence()
			{
				if (this._task.Options.Count > 1)
				{
					this._task.Options[1].BlockTheOption(true);
				}
			}

			// Token: 0x06000DF4 RID: 3572 RVA: 0x00065944 File Offset: 0x00063B44
			private void persuasion_select_option_3_on_consequence()
			{
				if (this._task.Options.Count > 2)
				{
					this._task.Options[2].BlockTheOption(true);
				}
			}

			// Token: 0x06000DF5 RID: 3573 RVA: 0x00065970 File Offset: 0x00063B70
			private PersuasionOptionArgs persuasion_setup_option_1()
			{
				return this._task.Options.ElementAt(0);
			}

			// Token: 0x06000DF6 RID: 3574 RVA: 0x00065983 File Offset: 0x00063B83
			private PersuasionOptionArgs persuasion_setup_option_2()
			{
				return this._task.Options.ElementAt(1);
			}

			// Token: 0x06000DF7 RID: 3575 RVA: 0x00065996 File Offset: 0x00063B96
			private PersuasionOptionArgs persuasion_setup_option_3()
			{
				return this._task.Options.ElementAt(2);
			}

			// Token: 0x06000DF8 RID: 3576 RVA: 0x000659AC File Offset: 0x00063BAC
			private bool persuasion_clickable_option_1_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Any<PersuasionOptionArgs>())
				{
					hintText = (this._task.Options.ElementAt(0).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(0).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000DF9 RID: 3577 RVA: 0x00065A18 File Offset: 0x00063C18
			private bool persuasion_clickable_option_2_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 1)
				{
					hintText = (this._task.Options.ElementAt(1).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(1).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000DFA RID: 3578 RVA: 0x00065A84 File Offset: 0x00063C84
			private bool persuasion_clickable_option_3_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 2)
				{
					hintText = (this._task.Options.ElementAt(2).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(2).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000DFB RID: 3579 RVA: 0x00065AF0 File Offset: 0x00063CF0
			private PersuasionTask GetPersuasionTask()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=rzGqa5oD}Revenge will be taken. Save your breath for the fight...", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}IF YOU SEE THIS. CALL CAMPAIGN TEAM.", null);
				persuasionTask.SpokenLine = new TextObject("{=6P1ruzsC}Maybe...", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, 0, 1, false, new TextObject("{=K9i5SaDc}Blood money is appropriate for a crime of passion. But you kill this boy in cold blood, you will be a real murderer in the eyes of the law, and will no doubt die.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, 0, -3, true, new TextObject("{=FUL8TcYa}I promised to protect the boy at the cost of my life. If you try to harm him, you will bleed for it.", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, 0, 0, false, new TextObject("{=Ytws5O9S}Some day you may wish to save the life of one of your sons through blood money. If you refuse mercy, mercy may be refused you.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				return persuasionTask;
			}

			// Token: 0x06000DFC RID: 3580 RVA: 0x00065BB4 File Offset: 0x00063DB4
			private void StartFightWithNotableGang(bool playerBetrayedCulprit)
			{
				this._notableAgent = (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents[0];
				List<Agent> list = new List<Agent> { this._culpritAgent };
				List<Agent> list2 = new List<Agent> { this._notableAgent };
				MBList<Agent> mblist = new MBList<Agent>();
				foreach (Agent agent in Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 30f, mblist))
				{
					if ((CharacterObject)agent.Character == this._notableGangsterCharacterObject)
					{
						list2.Add(agent);
					}
				}
				if (playerBetrayedCulprit)
				{
					Agent.Main.SetTeam(Mission.Current.SpectatorTeam, false);
				}
				else
				{
					list.Add(Agent.Main);
					foreach (Agent agent2 in list2)
					{
						agent2.Defensiveness = 2f;
					}
					this._culpritAgent.Health = 350f;
					this._culpritAgent.BaseHealthLimit = 350f;
					this._culpritAgent.HealthLimit = 350f;
				}
				this._notableAgent.Health = 350f;
				this._notableAgent.BaseHealthLimit = 350f;
				this._notableAgent.HealthLimit = 350f;
				Mission.Current.GetMissionBehavior<MissionFightHandler>().StartCustomFight(list, list2, false, false, delegate(bool isPlayerSideWon)
				{
					if (this._isNotableKnockedDownInMissionFight)
					{
						if (Agent.Main != null && this._notableAgent.Position.DistanceSquared(Agent.Main.Position) < 49f)
						{
							MissionConversationLogic.Current.StartConversation(this._notableAgent, false, false);
							return;
						}
						this.PlayerAndCulpritKnockedDownNotableQuestSuccess();
						return;
					}
					else
					{
						if (Agent.Main != null && this._notableAgent.Position.DistanceSquared(Agent.Main.Position) < 49f)
						{
							MissionConversationLogic.Current.StartConversation(this._notableAgent, false, false);
							return;
						}
						this.CulpritDiedInNotableFightFail();
						return;
					}
				});
			}

			// Token: 0x06000DFD RID: 3581 RVA: 0x00065D68 File Offset: 0x00063F68
			private void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage)
			{
				if (base.IsOngoing && !this._persuationInDoneAndSuccessfull && affectedAgent.Health <= (float)damage && Agent.Main != null)
				{
					if (affectedAgent == this._notableAgent && !this._isNotableKnockedDownInMissionFight)
					{
						affectedAgent.Health = 50f;
						this._isNotableKnockedDownInMissionFight = true;
						Mission.Current.GetMissionBehavior<MissionFightHandler>().EndFight();
					}
					if (affectedAgent == this._culpritAgent && !this._isCulpritDiedInMissionFight)
					{
						Blow blow = default(Blow);
						blow.DamageCalculated = true;
						blow.BaseMagnitude = (float)damage;
						blow.InflictedDamage = damage;
						blow.DamagedPercentage = 1f;
						blow.OwnerId = affectorAgent.Index;
						Blow blow2 = blow;
						affectedAgent.Die(blow2, -1);
						this._isCulpritDiedInMissionFight = true;
					}
				}
			}

			// Token: 0x06000DFE RID: 3582 RVA: 0x00065E30 File Offset: 0x00064030
			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=JjXETjYb}We would be eternally grateful if you can help my relative. I have to add, I'm ready to pay you {REWARD_GOLD}{GOLD_ICON} denars for your trouble. He is hiding somewhere here, go talk to him and tell him he is safe now.", null), null, null).Condition(delegate
				{
					MBTextManager.SetTextVariable("REWARD_GOLD", this._rewardGold);
					MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
					return Hero.OneToOneConversationHero == base.QuestGiver;
				})
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=ndDpjT8s}Have you been able to talk with my boy yet?", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=ETiAbgHa}I will talk with them right away", null), null)
					.NpcLine(new TextObject("{=qmqTLZ9R}Thank you {?PLAYER.GENDER}madam{?}sir{\\?}. You are a savior.", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=18NtjryL}Not yet, but I will soon.", null), null)
					.NpcLine(new TextObject("{=HeIIW3EH}We are waiting for your good news {?PLAYER.GENDER}milady{?}sir{\\?}.", null), null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			// Token: 0x06000DFF RID: 3583 RVA: 0x00065F1C File Offset: 0x0006411C
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this.PlayerStartsQuestLogText1, false);
				base.AddLog(this.PlayerStartsQuestLogText2, false);
				base.AddTrackedObject(this._targetNotable);
				base.AddTrackedObject(this._culprit);
				Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center");
				Settlement.CurrentSettlement.LocationComplex.ChangeLocation(this.CreateCulpritLocationCharacter(Settlement.CurrentSettlement.Culture, 0), null, locationWithId);
			}

			// Token: 0x06000E00 RID: 3584 RVA: 0x00065F9C File Offset: 0x0006419C
			private DialogFlow GetCulpritDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=w0HPC53e}Who are you? What do you want from me?", null), null, null).Condition(() => !this._culpritJoinedPlayerParty && Hero.OneToOneConversationHero == this._culprit)
					.PlayerLine(new TextObject("{=UGTCe2qP}Relax. I've talked with your relative, {QUEST_GIVER.NAME}. I know all about your situation. I'm here to help.", null), null)
					.Condition(delegate
					{
						StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
						return Hero.OneToOneConversationHero == this._culprit;
					})
					.NpcLine(new TextObject("{=45llLiYG}How will you help? Will you protect me?", null), null, null)
					.PlayerLine(new TextObject("{=4mwSvCgG}Yes I will. Come now, I will take you with me to {TARGET_NOTABLE.NAME} to resolve this issue peacefully.", null), null)
					.Condition(delegate
					{
						StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, null, false);
						return Hero.OneToOneConversationHero == this._culprit;
					})
					.NpcLine(new TextObject("{=bHRZhYzd}No! I won't go anywhere near them! They'll kill me![ib:closed]", null), null, null)
					.PlayerLine(new TextObject("{=sakSp6H8}You can't hide in the shadows forever. I pledge on my honor to protect you if things turn ugly.", null), null)
					.NpcLine(new TextObject("{=4CFOH0kB}I'm still not sure about all this, but I trust you. Let's go get this over.", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.CulpritJoinedPlayersArmy;
					})
					.CloseDialog();
			}

			// Token: 0x06000E01 RID: 3585 RVA: 0x0006607C File Offset: 0x0006427C
			private DialogFlow GetNotableThugDialogFlow()
			{
				TextObject textObject = new TextObject("{=QMaYa25R}If you dare to even breathe against {TARGET_NOTABLE.LINK}, it will be your last. You got it scum?", null);
				StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject, false);
				TextObject textObject2 = new TextObject("{=vGnY4KBO}I care very little for your threats. My business is with {TARGET_NOTABLE.LINK}.", null);
				StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this._targetNotable.CharacterObject, textObject2, false);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(delegate
				{
					if (this._notableThugs != null)
					{
						return this._notableThugs.Exists((LocationCharacter x) => x.AgentOrigin == Campaign.Current.ConversationManager.ConversationAgents[0].Origin);
					}
					return false;
				})
					.PlayerLine(textObject2, null)
					.CloseDialog();
			}

			// Token: 0x06000E02 RID: 3586 RVA: 0x00066104 File Offset: 0x00064304
			private void CulpritJoinedPlayersArmy()
			{
				AddCompanionAction.Apply(Clan.PlayerClan, this._culprit);
				AddHeroToPartyAction.Apply(this._culprit, MobileParty.MainParty, true);
				base.AddLog(this.CulpritJoinedPlayerPartyLogText, false);
				if (Mission.Current != null)
				{
					DailyBehaviorGroup behaviorGroup = ((Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents[0]).GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
					behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
					behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
				}
				this._culpritJoinedPlayerParty = true;
			}

			// Token: 0x06000E03 RID: 3587 RVA: 0x0006618C File Offset: 0x0006438C
			protected override void RegisterEvents()
			{
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.OnVillageRaid));
				CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnd));
				CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, new Action<Hero, RemoveCompanionAction.RemoveCompanionDetail>(this.OnCompanionRemoved));
				CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnPrisonerTaken));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.CanMoveToSettlementEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanMoveToSettlement));
				CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
				CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			}

			// Token: 0x06000E04 RID: 3588 RVA: 0x000662DC File Offset: 0x000644DC
			private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
			{
				if (!this._culpritJoinedPlayerParty && Settlement.CurrentSettlement == base.QuestGiver.CurrentSettlement)
				{
					Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center").AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateCulpritLocationCharacter), Settlement.CurrentSettlement.Culture, 0, 1);
				}
			}

			// Token: 0x06000E05 RID: 3589 RVA: 0x00066334 File Offset: 0x00064534
			private void CanMoveToSettlement(Hero hero, ref bool result)
			{
				this.CommonRestrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000E06 RID: 3590 RVA: 0x0006633E File Offset: 0x0006453E
			public override void OnHeroCanBeSelectedInInventoryInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonRestrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000E07 RID: 3591 RVA: 0x00066348 File Offset: 0x00064548
			public override void OnHeroCanHavePartyRoleOrBeGovernorInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonRestrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000E08 RID: 3592 RVA: 0x00066352 File Offset: 0x00064552
			public override void OnHeroCanLeadPartyInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonRestrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000E09 RID: 3593 RVA: 0x0006635C File Offset: 0x0006455C
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				this.CommonRestrictionInfoIsRequested(hero, ref result);
			}

			// Token: 0x06000E0A RID: 3594 RVA: 0x00066366 File Offset: 0x00064566
			private void CommonRestrictionInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._culprit || this._targetNotable == hero)
				{
					result = false;
				}
			}

			// Token: 0x06000E0B RID: 3595 RVA: 0x0006637D File Offset: 0x0006457D
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06000E0C RID: 3596 RVA: 0x00066390 File Offset: 0x00064590
			private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
			{
				if (hero == this._targetNotable)
				{
					result = false;
					return;
				}
				if (hero == Hero.MainHero && Settlement.CurrentSettlement == this._targetSettlement && Mission.Current != null)
				{
					result = false;
				}
			}

			// Token: 0x06000E0D RID: 3597 RVA: 0x000663C0 File Offset: 0x000645C0
			private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
			{
				if (victim == this._targetNotable)
				{
					TextObject textObject = ((detail == 7) ? this.TargetHeroDisappearedLogText : this.TargetHeroDiedLogText);
					StringHelpers.SetCharacterProperties("QUEST_TARGET", this._targetNotable.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					base.AddLog(textObject, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06000E0E RID: 3598 RVA: 0x00066429 File Offset: 0x00064629
			private void OnPrisonerTaken(PartyBase capturer, Hero prisoner)
			{
				if (prisoner == this._culprit)
				{
					base.AddLog(this.FailQuestLogText, false);
					this.TiemoutFailConsequences();
					base.CompleteQuestWithFail(null);
				}
			}

			// Token: 0x06000E0F RID: 3599 RVA: 0x00066450 File Offset: 0x00064650
			private void OnVillageRaid(Village village)
			{
				if (village == this._targetSettlement.Village)
				{
					base.AddLog(this.TargetVillageRaidedBeforeTalkingToCulpritCancel, false);
					base.CompleteQuestWithCancel(null);
					return;
				}
				if (village == base.QuestGiver.CurrentSettlement.Village && !this._culpritJoinedPlayerParty)
				{
					base.AddLog(this.QuestGiverVillageRaidedBeforeTalkingToCulpritCancel, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06000E10 RID: 3600 RVA: 0x000664B1 File Offset: 0x000646B1
			private void OnCompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
			{
				if (base.IsOngoing && !this._isCulpritDiedInMissionFight && !this._isPlayerKnockedOutMissionFight && companion == this._culprit)
				{
					base.AddLog(this.CulpritNoLongerAClanMember, false);
					this.TiemoutFailConsequences();
					base.CompleteQuestWithFail(null);
				}
			}

			// Token: 0x06000E11 RID: 3601 RVA: 0x000664F0 File Offset: 0x000646F0
			public void OnMissionStarted(IMission iMission)
			{
				if (this._checkForMissionEvents)
				{
					if (PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.All((AccompanyingCharacter x) => x.LocationCharacter.Character != this._culprit.CharacterObject))
					{
						LocationCharacter locationCharacterOfHero = LocationComplex.Current.GetLocationCharacterOfHero(this._culprit);
						if (locationCharacterOfHero != null)
						{
							PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(locationCharacterOfHero, true);
						}
					}
					FamilyFeudIssueBehavior.FamilyFeudIssueMissionBehavior familyFeudIssueMissionBehavior = new FamilyFeudIssueBehavior.FamilyFeudIssueMissionBehavior(new Action<Agent, Agent, int>(this.OnAgentHit));
					Mission.Current.AddMissionBehavior(familyFeudIssueMissionBehavior);
					Mission.Current.GetMissionBehavior<MissionConversationLogic>().SetSpawnArea("common_area_2");
				}
			}

			// Token: 0x06000E12 RID: 3602 RVA: 0x00066574 File Offset: 0x00064774
			private void OnMissionEnd(IMission mission)
			{
				if (this._checkForMissionEvents)
				{
					this._notableAgent = null;
					this._culpritAgent = null;
					if (Agent.Main == null)
					{
						base.AddLog(this.PlayerDiedInNotableBattle, false);
						this.RelationshipChangeWithQuestGiver = -10;
						base.QuestGiver.CurrentSettlement.Village.Bound.Prosperity -= 5f;
						base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security -= 5f;
						this._isPlayerKnockedOutMissionFight = true;
						base.CompleteQuestWithFail(null);
						return;
					}
					if (this._isNotableKnockedDownInMissionFight)
					{
						base.AddLog(this.SuccessQuestSolutionLogText, false);
						this.ApplySuccessConsequences();
						return;
					}
					if (this._isCulpritDiedInMissionFight)
					{
						if (this._playerBetrayedCulprit)
						{
							base.AddLog(this.FailQuestLogText, false);
							TraitLevelingHelper.OnIssueSolvedThroughBetrayal(Hero.MainHero, new Tuple<TraitObject, int>[]
							{
								new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
							});
							ChangeRelationAction.ApplyPlayerRelation(this._targetNotable, 5, true, true);
						}
						else
						{
							base.AddLog(this.CulpritDiedQuestFail, false);
						}
						this.RelationshipChangeWithQuestGiver = -10;
						base.QuestGiver.CurrentSettlement.Village.Bound.Prosperity -= 5f;
						base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security -= 5f;
						base.CompleteQuestWithFail(null);
						return;
					}
					if (this._persuationInDoneAndSuccessfull)
					{
						base.AddLog(this.SuccessQuestSolutionLogText, false);
						this.ApplySuccessConsequences();
					}
				}
			}

			// Token: 0x06000E13 RID: 3603 RVA: 0x0006670F File Offset: 0x0006490F
			private void OnGameMenuOpened(MenuCallbackArgs args)
			{
				if (this._culpritJoinedPlayerParty && Hero.MainHero.CurrentSettlement == this._targetSettlement)
				{
					this._checkForMissionEvents = args.MenuContext.GameMenu.StringId == "village";
				}
			}

			// Token: 0x06000E14 RID: 3604 RVA: 0x0006674B File Offset: 0x0006494B
			public void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party == MobileParty.MainParty)
				{
					if (settlement == this._targetSettlement)
					{
						this._checkForMissionEvents = false;
					}
					if (settlement == base.QuestGiver.CurrentSettlement && this._culpritJoinedPlayerParty)
					{
						base.AddTrackedObject(this._targetSettlement);
					}
				}
			}

			// Token: 0x06000E15 RID: 3605 RVA: 0x00066788 File Offset: 0x00064988
			public void OnBeforeMissionOpened()
			{
				if (this._checkForMissionEvents)
				{
					Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center");
					if (locationWithId != null)
					{
						locationWithId.GetLocationCharacter(this._targetNotable).SpecialTargetTag = "common_area_2";
						if (this._notableThugs == null)
						{
							this._notableThugs = new List<LocationCharacter>();
						}
						else
						{
							this._notableThugs.Clear();
						}
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateNotablesThugs), Settlement.CurrentSettlement.Culture, 0, MathF.Ceiling(Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier() * 3f));
					}
				}
			}

			// Token: 0x06000E16 RID: 3606 RVA: 0x0006682C File Offset: 0x00064A2C
			private LocationCharacter CreateCulpritLocationCharacter(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(this._culprit.CharacterObject.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, this._culprit.CharacterObject.IsFemale, "_villager"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(this._culprit.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFirstCompanionBehavior), "common_area_2", true, relation, tuple.Item1, true, false, null, false, false, true);
			}

			// Token: 0x06000E17 RID: 3607 RVA: 0x000668D0 File Offset: 0x00064AD0
			private LocationCharacter CreateNotablesThugs(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(this._notableGangsterCharacterObject.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, this._notableGangsterCharacterObject.IsFemale, "_villain"), monsterWithSuffix);
				LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(this._notableGangsterCharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "common_area_2", true, relation, tuple.Item1, true, false, null, false, false, true);
				this._notableThugs.Add(locationCharacter);
				return locationCharacter;
			}

			// Token: 0x06000E18 RID: 3608 RVA: 0x00066974 File Offset: 0x00064B74
			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if (mapEvent.IsPlayerMapEvent && this._culpritJoinedPlayerParty && !MobileParty.MainParty.MemberRoster.GetTroopRoster().Exists((TroopRosterElement x) => x.Character == this._culprit.CharacterObject))
				{
					base.AddLog(this.FailQuestLogText, false);
					this.TiemoutFailConsequences();
					base.CompleteQuestWithFail(null);
				}
			}

			// Token: 0x06000E19 RID: 3609 RVA: 0x000669CE File Offset: 0x00064BCE
			private void CulpritDiedInNotableFightFail()
			{
				this._conversationAfterFightIsDone = true;
				this.HandleAgentBehaviorAfterQuestConversations();
			}

			// Token: 0x06000E1A RID: 3610 RVA: 0x000669E0 File Offset: 0x00064BE0
			protected override void OnFinalize()
			{
				if (this._culprit.IsPlayerCompanion)
				{
					RemoveCompanionAction.ApplyByDeath(Clan.PlayerClan, this._culprit);
				}
				if (this._culprit.IsAlive)
				{
					this._culprit.Clan = null;
					DisableHeroAction.Apply(this._culprit);
				}
			}

			// Token: 0x06000E1B RID: 3611 RVA: 0x00066A2E File Offset: 0x00064C2E
			protected override void OnTimedOut()
			{
				base.AddLog(this.FailQuestLogText, false);
				this.TiemoutFailConsequences();
			}

			// Token: 0x06000E1C RID: 3612 RVA: 0x00066A44 File Offset: 0x00064C44
			private void TiemoutFailConsequences()
			{
				TraitLevelingHelper.OnIssueSolvedThroughBetrayal(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				this.RelationshipChangeWithQuestGiver = -10;
				base.QuestGiver.CurrentSettlement.Village.Bound.Prosperity -= 5f;
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security -= 5f;
			}

			// Token: 0x06000E1D RID: 3613 RVA: 0x00066ACA File Offset: 0x00064CCA
			internal static void AutoGeneratedStaticCollectObjectsFamilyFeudIssueQuest(object o, List<object> collectedObjects)
			{
				((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000E1E RID: 3614 RVA: 0x00066AD8 File Offset: 0x00064CD8
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetSettlement);
				collectedObjects.Add(this._targetNotable);
				collectedObjects.Add(this._culprit);
			}

			// Token: 0x06000E1F RID: 3615 RVA: 0x00066B05 File Offset: 0x00064D05
			internal static object AutoGeneratedGetMemberValue_targetSettlement(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._targetSettlement;
			}

			// Token: 0x06000E20 RID: 3616 RVA: 0x00066B12 File Offset: 0x00064D12
			internal static object AutoGeneratedGetMemberValue_targetNotable(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._targetNotable;
			}

			// Token: 0x06000E21 RID: 3617 RVA: 0x00066B1F File Offset: 0x00064D1F
			internal static object AutoGeneratedGetMemberValue_culprit(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._culprit;
			}

			// Token: 0x06000E22 RID: 3618 RVA: 0x00066B2C File Offset: 0x00064D2C
			internal static object AutoGeneratedGetMemberValue_culpritJoinedPlayerParty(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._culpritJoinedPlayerParty;
			}

			// Token: 0x06000E23 RID: 3619 RVA: 0x00066B3E File Offset: 0x00064D3E
			internal static object AutoGeneratedGetMemberValue_checkForMissionEvents(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._checkForMissionEvents;
			}

			// Token: 0x06000E24 RID: 3620 RVA: 0x00066B50 File Offset: 0x00064D50
			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((FamilyFeudIssueBehavior.FamilyFeudIssueQuest)o)._rewardGold;
			}

			// Token: 0x04000608 RID: 1544
			private const int CustomCulpritAgentHealth = 350;

			// Token: 0x04000609 RID: 1545
			private const int CustomTargetNotableAgentHealth = 350;

			// Token: 0x0400060A RID: 1546
			public const string CommonAreaTag = "common_area_2";

			// Token: 0x0400060B RID: 1547
			[SaveableField(10)]
			private readonly Settlement _targetSettlement;

			// Token: 0x0400060C RID: 1548
			[SaveableField(20)]
			private Hero _targetNotable;

			// Token: 0x0400060D RID: 1549
			[SaveableField(30)]
			private Hero _culprit;

			// Token: 0x0400060E RID: 1550
			[SaveableField(40)]
			private bool _culpritJoinedPlayerParty;

			// Token: 0x0400060F RID: 1551
			[SaveableField(50)]
			private bool _checkForMissionEvents;

			// Token: 0x04000610 RID: 1552
			[SaveableField(70)]
			private int _rewardGold;

			// Token: 0x04000611 RID: 1553
			private bool _isCulpritDiedInMissionFight;

			// Token: 0x04000612 RID: 1554
			private bool _isPlayerKnockedOutMissionFight;

			// Token: 0x04000613 RID: 1555
			private bool _isNotableKnockedDownInMissionFight;

			// Token: 0x04000614 RID: 1556
			private bool _conversationAfterFightIsDone;

			// Token: 0x04000615 RID: 1557
			private bool _persuationInDoneAndSuccessfull;

			// Token: 0x04000616 RID: 1558
			private bool _playerBetrayedCulprit;

			// Token: 0x04000617 RID: 1559
			private Agent _notableAgent;

			// Token: 0x04000618 RID: 1560
			private Agent _culpritAgent;

			// Token: 0x04000619 RID: 1561
			private CharacterObject _notableGangsterCharacterObject;

			// Token: 0x0400061A RID: 1562
			private List<LocationCharacter> _notableThugs;

			// Token: 0x0400061B RID: 1563
			private PersuasionTask _task;

			// Token: 0x0400061C RID: 1564
			private const PersuasionDifficulty Difficulty = 4;
		}
	}
}
