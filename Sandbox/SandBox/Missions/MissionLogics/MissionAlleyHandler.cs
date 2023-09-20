using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000044 RID: 68
	public class MissionAlleyHandler : MissionLogic
	{
		// Token: 0x0600035E RID: 862 RVA: 0x00018894 File Offset: 0x00016A94
		public override void OnRenderingStarted()
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsHuman)
				{
					CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
					bool flag;
					if (component == null)
					{
						flag = null != null;
					}
					else
					{
						AgentNavigator agentNavigator = component.AgentNavigator;
						flag = ((agentNavigator != null) ? agentNavigator.MemberOfAlley : null) != null;
					}
					if (flag && component.AgentNavigator.MemberOfAlley.Owner != Hero.MainHero && !this._rivalThugAgentsAndAgentNavigators.ContainsKey(agent))
					{
						this._rivalThugAgentsAndAgentNavigators.Add(agent, component.AgentNavigator);
					}
				}
			}
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00018948 File Offset: 0x00016B48
		public override void OnMissionTick(float dt)
		{
			if (Mission.Current.Mode == 2)
			{
				this.EndFightIfPlayerIsFarAwayOrNearGuard();
				return;
			}
			if (MBRandom.RandomFloat < dt * 10f)
			{
				this.CheckAndTriggerConversationWithRivalThug();
			}
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0001897C File Offset: 0x00016B7C
		private void CheckAndTriggerConversationWithRivalThug()
		{
			if (!this._conversationTriggeredWithRivalThug && !Campaign.Current.ConversationManager.IsConversationFlowActive && Agent.Main != null)
			{
				foreach (KeyValuePair<Agent, AgentNavigator> keyValuePair in this._rivalThugAgentsAndAgentNavigators)
				{
					if (keyValuePair.Key.IsActive())
					{
						Agent key = keyValuePair.Key;
						if (key.GetTrackDistanceToMainAgent() < 5f && keyValuePair.Value.CanSeeAgent(Agent.Main))
						{
							Mission.Current.GetMissionBehavior<MissionConversationLogic>().StartConversation(key, false, false);
							this._conversationTriggeredWithRivalThug = true;
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00018A44 File Offset: 0x00016C44
		public override void AfterStart()
		{
			MissionAlleyHandler._guardAgents = new List<Agent>();
			this._rivalThugAgentsAndAgentNavigators = new Dictionary<Agent, AgentNavigator>();
			MissionAlleyHandler._fightPosition = Vec3.Invalid;
			this._missionFightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00018A78 File Offset: 0x00016C78
		private void EndFightIfPlayerIsFarAwayOrNearGuard()
		{
			if (Agent.Main != null)
			{
				bool flag = false;
				foreach (Agent agent in MissionAlleyHandler._guardAgents)
				{
					if ((Agent.Main.Position - agent.Position).Length <= 10f)
					{
						flag = true;
						break;
					}
				}
				if (MissionAlleyHandler._fightPosition != Vec3.Invalid && (Agent.Main.Position - MissionAlleyHandler._fightPosition).Length >= 20f)
				{
					flag = true;
				}
				if (flag)
				{
					this.EndFight();
				}
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00018B38 File Offset: 0x00016D38
		private ValueTuple<bool, string> CanPlayerOccupyTheCurrentAlley()
		{
			TextObject textObject = TextObject.Empty;
			if (!Settlement.CurrentSettlement.Alleys.All((Alley x) => x.Owner != Hero.MainHero))
			{
				textObject = new TextObject("{=ribkM9dl}You already own another alley in the settlement.", null);
				return new ValueTuple<bool, string>(false, textObject.ToString());
			}
			if (!Campaign.Current.Models.AlleyModel.GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(CampaignMission.Current.LastVisitedAlley).Any((ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail> x) => x.Item2 == null || x.Item2 == 1))
			{
				textObject = new TextObject("{=hnhKJYbx}You don't have any suitable clan members to assign this alley. ({ROGUERY_SKILL} skill {NEEDED_SKILL_LEVEL} or higher, {TRAIT_NAME} trait {MAX_TRAIT_AMOUNT} or lower)", null);
				textObject.SetTextVariable("ROGUERY_SKILL", DefaultSkills.Roguery.Name);
				textObject.SetTextVariable("NEEDED_SKILL_LEVEL", 30);
				textObject.SetTextVariable("TRAIT_NAME", DefaultTraits.Mercy.Name);
				textObject.SetTextVariable("MAX_TRAIT_AMOUNT", 0);
				return new ValueTuple<bool, string>(false, textObject.ToString());
			}
			if (MobileParty.MainParty.MemberRoster.TotalRegulars < Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley)
			{
				textObject = new TextObject("{=zLnqZdIK}You don't have enough troops to assign this alley. (Needed at least {NEEDED_TROOP_NUMBER})", null);
				textObject.SetTextVariable("NEEDED_TROOP_NUMBER", Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley);
				return new ValueTuple<bool, string>(false, textObject.ToString());
			}
			return new ValueTuple<bool, string>(true, null);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00018CA4 File Offset: 0x00016EA4
		private void EndFight()
		{
			this._missionFightHandler.EndFight();
			foreach (Agent agent in MissionAlleyHandler._guardAgents)
			{
				agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().GetBehavior<FightBehavior>().IsActive = false;
			}
			MissionAlleyHandler._guardAgents.Clear();
			Mission.Current.SetMissionMode(0, false);
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00018D2C File Offset: 0x00016F2C
		private void OnTakeOverTheAlley()
		{
			AlleyHelper.CreateMultiSelectionInquiryForSelectingClanMemberToAlley(CampaignMission.Current.LastVisitedAlley, new Action<List<InquiryElement>>(this.OnCompanionSelectedForNewAlley), new Action<List<InquiryElement>>(this.OnCompanionSelectionCancel));
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00018D55 File Offset: 0x00016F55
		private void OnCompanionSelectionCancel(List<InquiryElement> obj)
		{
			this.OnLeaveItEmpty();
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00018D60 File Offset: 0x00016F60
		private void OnCompanionSelectedForNewAlley(List<InquiryElement> companion)
		{
			CharacterObject characterObject = companion.First<InquiryElement>().Identifier as CharacterObject;
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			troopRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
			AlleyHelper.OpenScreenForManagingAlley(troopRoster, new PartyPresentationDoneButtonDelegate(this.OnPartyScreenDoneClicked), new TextObject("{=s8dsW6m0}New Alley", null), new PartyPresentationCancelButtonDelegate(this.OnPartyScreenCancel));
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00018DB9 File Offset: 0x00016FB9
		private void OnPartyScreenCancel()
		{
			this.OnLeaveItEmpty();
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00018DC4 File Offset: 0x00016FC4
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (!affectedAgent.IsHuman)
			{
				return;
			}
			if (affectorAgent == Agent.Main && affectorAgent.IsHuman && affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				TalkBehavior behavior = affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>().GetBehavior<TalkBehavior>();
				if (behavior != null)
				{
					behavior.Disable();
				}
				if (!affectedAgent.IsEnemyOf(affectorAgent) && affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley != null)
				{
					this.StartCommonAreaBattle(affectedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley);
				}
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00018E48 File Offset: 0x00017048
		private bool OnPartyScreenDoneClicked(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty, PartyBase rightParty)
		{
			CampaignEventDispatcher.Instance.OnAlleyOccupiedByPlayer(CampaignMission.Current.LastVisitedAlley, leftMemberRoster);
			return true;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00018E60 File Offset: 0x00017060
		public void StartCommonAreaBattle(Alley alley)
		{
			MissionAlleyHandler._guardAgents.Clear();
			List<Agent> accompanyingAgents = new List<Agent>();
			foreach (Agent agent2 in Mission.Current.Agents)
			{
				LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent2);
				AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
				CharacterObject characterObject = (CharacterObject)agent2.Character;
				if (accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
				{
					accompanyingAgents.Add(agent2);
				}
				else if (characterObject != null && (characterObject.Occupation == 24 || characterObject.Occupation == 7))
				{
					MissionAlleyHandler._guardAgents.Add(agent2);
				}
			}
			List<Agent> list = Mission.Current.Agents.Where((Agent agent) => agent.IsHuman && agent.Character.IsHero && (agent.IsPlayerControlled || accompanyingAgents.Contains(agent))).ToList<Agent>();
			List<Agent> list2 = Mission.Current.Agents.Where((Agent agent) => agent.IsHuman && agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && agent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley == alley).ToList<Agent>();
			MissionAlleyHandler._fightPosition = Agent.Main.Position;
			Mission.Current.GetMissionBehavior<MissionFightHandler>().StartCustomFight(list, list2, false, false, new MissionFightHandler.OnFightEndDelegate(this.OnAlleyFightEnd));
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00018FB0 File Offset: 0x000171B0
		private void OnLeaveItEmpty()
		{
			CampaignEventDispatcher.Instance.OnAlleyClearedByPlayer(CampaignMission.Current.LastVisitedAlley);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00018FC8 File Offset: 0x000171C8
		private void OnAlleyFightEnd(bool isPlayerSideWon)
		{
			if (isPlayerSideWon)
			{
				object obj = new TextObject("{=4QfQBi2k}Alley fight won", null);
				TextObject textObject = new TextObject("{=8SK2BZum}You have cleared an alley which belonged to a gang leader. Now, you can either take it over for your own benefit or leave it empty to help the town. To own an alley, you will need to assign a suitable clan member and some troops to watch over it. This will provide denars to your clan, but also increase your crime rating.", null);
				TextObject textObject2 = new TextObject("{=qxY2ASqp}Take over the alley", null);
				TextObject textObject3 = new TextObject("{=jjEzdO0Y}Leave it empty", null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, textObject2.ToString(), textObject3.ToString(), new Action(this.OnTakeOverTheAlley), new Action(this.OnLeaveItEmpty), "", 0f, null, new Func<ValueTuple<bool, string>>(this.CanPlayerOccupyTheCurrentAlley), null), true, false);
			}
			else if (Agent.Main == null || !Agent.Main.IsActive())
			{
				Mission.Current.NextCheckTimeEndMission = 0f;
				Campaign.Current.GameMenuManager.SetNextMenu("settlement_player_unconscious");
			}
			MissionAlleyHandler._fightPosition = Vec3.Invalid;
		}

		// Token: 0x040001A9 RID: 425
		private const float ConstantForInitiatingConversation = 5f;

		// Token: 0x040001AA RID: 426
		private static Vec3 _fightPosition = Vec3.Invalid;

		// Token: 0x040001AB RID: 427
		private Dictionary<Agent, AgentNavigator> _rivalThugAgentsAndAgentNavigators;

		// Token: 0x040001AC RID: 428
		private const int DistanceForEndingAlleyFight = 20;

		// Token: 0x040001AD RID: 429
		private const int GuardAgentSafeZone = 10;

		// Token: 0x040001AE RID: 430
		private static List<Agent> _guardAgents;

		// Token: 0x040001AF RID: 431
		private bool _conversationTriggeredWithRivalThug;

		// Token: 0x040001B0 RID: 432
		private MissionFightHandler _missionFightHandler;
	}
}
