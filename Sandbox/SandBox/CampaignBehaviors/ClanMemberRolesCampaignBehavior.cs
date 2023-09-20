using System;
using System.Collections.Generic;
using SandBox.Conversation;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x02000095 RID: 149
	public class ClanMemberRolesCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060006CE RID: 1742 RVA: 0x0003437C File Offset: 0x0003257C
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddDialogs));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.OnNewCompanionAdded));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.BeforeMissionOpened));
			CampaignEvents.OnHeroJoinedPartyEvent.AddNonSerializedListener(this, new Action<Hero, MobileParty>(this.OnHeroJoinedParty));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.OnHeroGetsBusyEvent.AddNonSerializedListener(this, new Action<Hero, HeroGetsBusyReasons>(this.OnHeroGetsBusy));
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00034458 File Offset: 0x00032658
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_isFollowingPlayer", ref this._isFollowingPlayer);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0003446C File Offset: 0x0003266C
		private static void FollowMainAgent()
		{
			DailyBehaviorGroup behaviorGroup = ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
			behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00034497 File Offset: 0x00032697
		public bool IsFollowingPlayer(Hero hero)
		{
			return this._isFollowingPlayer.Contains(hero);
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x000344A8 File Offset: 0x000326A8
		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddPlayerLine("clan_member_follow", "hero_main_options", "clan_member_follow_me", "{=blqTMwQT}Follow me.", new ConversationSentence.OnConditionDelegate(this.clan_member_follow_me_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("clan_member_dont_follow", "hero_main_options", "clan_member_dont_follow_me", "{=LPtWLajd}You can stop following me now. Thanks.", new ConversationSentence.OnConditionDelegate(this.clan_member_dont_follow_me_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("clan_members_follow", "hero_main_options", "clan_member_gather", "{=PUtbpIFI}Gather all my companions in the settlement and find me.", new ConversationSentence.OnConditionDelegate(this.clan_members_gather_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("clan_members_dont_follow", "hero_main_options", "clan_members_dont_follow_me", "{=FdwZlCCM}All of you can stop following me and return to what you were doing.", new ConversationSentence.OnConditionDelegate(this.clan_members_gather_end_on_condition), null, 100, null, null);
			campaignGameStarter.AddDialogLine("clan_member_gather_clan_members_accept", "clan_member_gather", "close_window", "{=KL8tVq8P}I shall do that.", null, new ConversationSentence.OnConsequenceDelegate(this.clan_member_gather_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("clan_member_follow_accept", "clan_member_follow_me", "close_window", "{=gm3wqjvi}Lead the way.", null, new ConversationSentence.OnConsequenceDelegate(this.clan_member_follow_me_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("clan_member_dont_follow_accept", "clan_member_dont_follow_me", "close_window", "{=akpaap9e}As you wish.", null, new ConversationSentence.OnConsequenceDelegate(this.clan_member_dont_follow_me_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("clan_members_dont_follow_accept", "clan_members_dont_follow_me", "close_window", "{=akpaap9e}As you wish.", null, new ConversationSentence.OnConsequenceDelegate(this.clan_members_dont_follow_me_on_consequence), 100, null);
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00034611 File Offset: 0x00032811
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty && PlayerEncounter.LocationEncounter != null)
			{
				PlayerEncounter.LocationEncounter.RemoveAllAccompanyingCharacters();
				this._isFollowingPlayer.Clear();
			}
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00034638 File Offset: 0x00032838
		private void BeforeMissionOpened()
		{
			if (PlayerEncounter.LocationEncounter != null)
			{
				foreach (Hero hero in this._isFollowingPlayer)
				{
					if (PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(hero.CharacterObject) == null)
					{
						this.AddClanMembersAsAccompanyingCharacter(hero, null);
					}
				}
			}
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x000346A8 File Offset: 0x000328A8
		private void OnHeroJoinedParty(Hero hero, MobileParty mobileParty)
		{
			if (hero.Clan == Clan.PlayerClan && mobileParty.IsMainParty && mobileParty.CurrentSettlement != null && PlayerEncounter.LocationEncounter != null && MobileParty.MainParty.IsActive && (mobileParty.CurrentSettlement.IsFortification || mobileParty.CurrentSettlement.IsVillage) && this._isFollowingPlayer.Count == 0)
			{
				this.UpdateAccompanyingCharacters();
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00034712 File Offset: 0x00032912
		private void OnMissionEnded(IMission mission)
		{
			this._gatherOrderedAgent = null;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0003471C File Offset: 0x0003291C
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver = null)
		{
			if (Mission.Current != null && issue.IsSolvingWithAlternative && details == 2)
			{
				if (this._isFollowingPlayer.Contains(issue.AlternativeSolutionHero))
				{
					this._isFollowingPlayer.Remove(issue.AlternativeSolutionHero);
				}
				PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(issue.AlternativeSolutionHero);
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00034774 File Offset: 0x00032974
		private void OnHeroGetsBusy(Hero hero, HeroGetsBusyReasons heroGetsBusyReason)
		{
			if (Mission.Current != null && (heroGetsBusyReason == 2 || heroGetsBusyReason == 3))
			{
				if (heroGetsBusyReason == 3)
				{
					for (int i = 0; i < Mission.Current.Agents.Count; i++)
					{
						Agent agent = Mission.Current.Agents[i];
						if (agent.IsHuman && agent.Character.IsHero && ((CharacterObject)agent.Character).HeroObject == hero)
						{
							this.ClearGatherOrderedAgentIfExists(agent);
							ClanMemberRolesCampaignBehavior.AdjustTheBehaviorsOfTheAgent(agent);
							break;
						}
					}
				}
				if (this._isFollowingPlayer.Contains(hero))
				{
					this._isFollowingPlayer.Remove(hero);
				}
				PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(hero);
			}
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x00034822 File Offset: 0x00032A22
		private void ClearGatherOrderedAgentIfExists(Agent agent)
		{
			if (this._gatherOrderedAgent == agent)
			{
				this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().RemoveBehavior<ScriptBehavior>();
				this._gatherOrderedAgent = null;
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x00034850 File Offset: 0x00032A50
		private void OnNewCompanionAdded(Hero newCompanion)
		{
			Location location = null;
			LocationComplex locationComplex = LocationComplex.Current;
			if (locationComplex != null)
			{
				foreach (Location location2 in locationComplex.GetListOfLocations())
				{
					foreach (LocationCharacter locationCharacter in location2.GetCharacterList())
					{
						if (locationCharacter.Character == newCompanion.CharacterObject)
						{
							location = LocationComplex.Current.GetLocationOfCharacter(locationCharacter);
							break;
						}
					}
				}
			}
			if (((locationComplex != null) ? locationComplex.GetLocationWithId("center") : null) != null && location == null)
			{
				AgentData agentData = new AgentData(new PartyAgentOrigin(PartyBase.MainParty, newCompanion.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(FaceGen.GetBaseMonsterFromRace(newCompanion.CharacterObject.Race)).NoHorses(true);
				locationComplex.GetLocationWithId("center").AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), null, true, 1, null, true, false, null, false, false, true));
			}
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x00034980 File Offset: 0x00032B80
		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (Campaign.Current.GameMode != 1 || MobileParty.MainParty.CurrentSettlement == null || LocationComplex.Current == null || (!settlement.IsTown && !settlement.IsCastle && !settlement.IsVillage))
			{
				return;
			}
			if (mobileParty == null && settlement == MobileParty.MainParty.CurrentSettlement && hero.Clan == Clan.PlayerClan)
			{
				if (this._isFollowingPlayer.Contains(hero) && hero.PartyBelongedTo == null)
				{
					this.RemoveAccompanyingHero(hero);
					if (this._isFollowingPlayer.Count == 0)
					{
						this.UpdateAccompanyingCharacters();
						return;
					}
				}
			}
			else if (mobileParty == MobileParty.MainParty && MobileParty.MainParty.IsActive)
			{
				this.UpdateAccompanyingCharacters();
			}
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x00034A30 File Offset: 0x00032C30
		private bool clan_member_follow_me_on_condition()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.LocationComplex != null && !Settlement.CurrentSettlement.IsHideout)
			{
				Location location = (Settlement.CurrentSettlement.IsVillage ? Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center") : Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center"));
				if (Hero.OneToOneConversationHero != null && ConversationMission.OneToOneConversationAgent != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty)
				{
					ICampaignMission campaignMission = CampaignMission.Current;
					if (((campaignMission != null) ? campaignMission.Location : null) == location && ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
					{
						return !(ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetActiveBehavior() is FollowAgentBehavior);
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x00034B14 File Offset: 0x00032D14
		private bool clan_member_dont_follow_me_on_condition()
		{
			return ConversationMission.OneToOneConversationAgent != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty && ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetActiveBehavior() is FollowAgentBehavior;
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x00034B7C File Offset: 0x00032D7C
		private bool clan_members_gather_on_condition()
		{
			if (GameStateManager.Current.ActiveState is MissionState)
			{
				if (this._gatherOrderedAgent != null || Settlement.CurrentSettlement == null)
				{
					return false;
				}
				AgentNavigator agentNavigator = ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
				InterruptingBehaviorGroup interruptingBehaviorGroup = ((agentNavigator != null) ? agentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>() : null);
				if (interruptingBehaviorGroup != null && interruptingBehaviorGroup.IsActive)
				{
					return false;
				}
				Agent oneToOneConversationAgent = ConversationMission.OneToOneConversationAgent;
				CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
				if (!oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Clan != Hero.MainHero.Clan)
				{
					return false;
				}
				foreach (Agent agent in Mission.Current.Agents)
				{
					CharacterObject characterObject = (CharacterObject)agent.Character;
					if (agent.IsHuman && agent != oneToOneConversationAgent && agent != Agent.Main && characterObject.IsHero && characterObject.HeroObject.Clan == Clan.PlayerClan && characterObject.HeroObject.PartyBelongedTo == MobileParty.MainParty)
					{
						AgentNavigator agentNavigator2 = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
						if (agentNavigator2 != null && !(agentNavigator2.GetActiveBehavior() is FollowAgentBehavior))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x00034CCC File Offset: 0x00032ECC
		private bool clan_members_gather_end_on_condition()
		{
			if (ConversationMission.OneToOneConversationAgent != null && this._gatherOrderedAgent == ConversationMission.OneToOneConversationAgent)
			{
				return !ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>().IsActive;
			}
			if (!this.IsAgentFollowingPlayerAsCompanion(ConversationMission.OneToOneConversationAgent))
			{
				return false;
			}
			foreach (Agent agent in Mission.Current.Agents)
			{
				if (agent != ConversationMission.OneToOneConversationAgent && this.IsAgentFollowingPlayerAsCompanion(agent))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x00034D78 File Offset: 0x00032F78
		private void clan_member_gather_on_consequence()
		{
			this._gatherOrderedAgent = ConversationMission.OneToOneConversationAgent;
			this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<ScriptBehavior>().IsActive = true;
			ScriptBehavior.AddTargetWithDelegate(this._gatherOrderedAgent, new ScriptBehavior.SelectTargetDelegate(this.SelectTarget), new ScriptBehavior.OnTargetReachedDelegate(this.OnTargetReached));
			this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<FollowAgentBehavior>().IsActive = false;
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00034DF3 File Offset: 0x00032FF3
		private void clan_member_dont_follow_me_on_consequence()
		{
			this.RemoveFollowBehavior(ConversationMission.OneToOneConversationAgent);
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x00034E00 File Offset: 0x00033000
		private void clan_members_dont_follow_me_on_consequence()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				this.RemoveFollowBehavior(agent);
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00034E58 File Offset: 0x00033058
		private void RemoveFollowBehavior(Agent agent)
		{
			this.ClearGatherOrderedAgentIfExists(agent);
			if (this.IsAgentFollowingPlayerAsCompanion(agent))
			{
				ClanMemberRolesCampaignBehavior.AdjustTheBehaviorsOfTheAgent(agent);
				LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent);
				this.RemoveAccompanyingHero(locationCharacter.Character.HeroObject);
			}
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00034E98 File Offset: 0x00033098
		private static void AdjustTheBehaviorsOfTheAgent(Agent agent)
		{
			DailyBehaviorGroup behaviorGroup = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.RemoveBehavior<FollowAgentBehavior>();
			ScriptBehavior behavior = behaviorGroup.GetBehavior<ScriptBehavior>();
			if (behavior != null)
			{
				behavior.IsActive = true;
			}
			WalkingBehavior walkingBehavior = behaviorGroup.GetBehavior<WalkingBehavior>();
			if (walkingBehavior == null)
			{
				walkingBehavior = behaviorGroup.AddBehavior<WalkingBehavior>();
			}
			walkingBehavior.IsActive = true;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00034EE8 File Offset: 0x000330E8
		private void clan_member_follow_me_on_consequence()
		{
			LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(ConversationMission.OneToOneConversationAgent);
			if (!this.IsFollowingPlayer(locationCharacter.Character.HeroObject))
			{
				this._isFollowingPlayer.Add(locationCharacter.Character.HeroObject);
			}
			this.AddClanMembersAsAccompanyingCharacter(locationCharacter.Character.HeroObject, locationCharacter);
			Campaign.Current.ConversationManager.ConversationEndOneShot += ClanMemberRolesCampaignBehavior.FollowMainAgent;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x00034F5C File Offset: 0x0003315C
		private bool SelectTarget(Agent agent, ref Agent targetAgent, ref UsableMachine targetEntity, ref WorldFrame targetFrame)
		{
			if (Agent.Main == null)
			{
				return false;
			}
			Agent agent2 = null;
			float num = float.MaxValue;
			foreach (Agent agent3 in Mission.Current.Agents)
			{
				CharacterObject characterObject = (CharacterObject)agent3.Character;
				CampaignAgentComponent component = agent3.GetComponent<CampaignAgentComponent>();
				if (agent3 != agent && agent3.IsHuman && characterObject.IsHero && characterObject.HeroObject.Clan == Clan.PlayerClan && characterObject.HeroObject.PartyBelongedTo == MobileParty.MainParty && component.AgentNavigator != null && agent3.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehavior<FollowAgentBehavior>() == null)
				{
					float num2 = agent.Position.DistanceSquared(agent3.Position);
					if (num2 < num)
					{
						num = num2;
						agent2 = agent3;
					}
				}
			}
			if (agent2 != null)
			{
				targetAgent = agent2;
				return true;
			}
			DailyBehaviorGroup behaviorGroup = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			FollowAgentBehavior behavior = behaviorGroup.GetBehavior<FollowAgentBehavior>();
			behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
			behavior.IsActive = true;
			behavior.SetTargetAgent(Agent.Main);
			ScriptBehavior behavior2 = behaviorGroup.GetBehavior<ScriptBehavior>();
			if (behavior2 != null)
			{
				behavior2.IsActive = false;
			}
			WalkingBehavior behavior3 = behaviorGroup.GetBehavior<WalkingBehavior>();
			if (behavior3 != null)
			{
				behavior3.IsActive = false;
			}
			LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent);
			if (!this.IsFollowingPlayer(locationCharacter.Character.HeroObject))
			{
				this._isFollowingPlayer.Add(locationCharacter.Character.HeroObject);
			}
			this.AddClanMembersAsAccompanyingCharacter(locationCharacter.Character.HeroObject, locationCharacter);
			this._gatherOrderedAgent = null;
			return false;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00035108 File Offset: 0x00033308
		private bool OnTargetReached(Agent agent, ref Agent targetAgent, ref UsableMachine targetEntity, ref WorldFrame targetFrame)
		{
			if (Agent.Main == null)
			{
				return false;
			}
			if (targetAgent == null)
			{
				return true;
			}
			DailyBehaviorGroup behaviorGroup = targetAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
			behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
			LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(targetAgent);
			if (!this.IsFollowingPlayer(locationCharacter.Character.HeroObject))
			{
				this._isFollowingPlayer.Add(locationCharacter.Character.HeroObject);
				this.AddClanMembersAsAccompanyingCharacter(locationCharacter.Character.HeroObject, locationCharacter);
			}
			targetAgent = null;
			return true;
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x00035198 File Offset: 0x00033398
		private void UpdateAccompanyingCharacters()
		{
			this._isFollowingPlayer.Clear();
			PlayerEncounter.LocationEncounter.RemoveAllAccompanyingCharacters();
			bool flag = false;
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					Hero heroObject = troopRosterElement.Character.HeroObject;
					if (heroObject != Hero.MainHero && !heroObject.IsPrisoner && !heroObject.IsWounded && heroObject.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && !flag)
					{
						this._isFollowingPlayer.Add(heroObject);
						flag = true;
					}
				}
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x00035264 File Offset: 0x00033464
		private void RemoveAccompanyingHero(Hero hero)
		{
			this._isFollowingPlayer.Remove(hero);
			PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(hero);
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00035280 File Offset: 0x00033480
		private bool IsAgentFollowingPlayerAsCompanion(Agent agent)
		{
			CharacterObject characterObject = ((agent != null) ? agent.Character : null) as CharacterObject;
			CampaignAgentComponent campaignAgentComponent = ((agent != null) ? agent.GetComponent<CampaignAgentComponent>() : null);
			if (agent != null && agent.IsHuman && characterObject != null && characterObject.IsHero && characterObject.HeroObject.Clan == Clan.PlayerClan && characterObject.HeroObject.PartyBelongedTo == MobileParty.MainParty)
			{
				AgentNavigator agentNavigator = campaignAgentComponent.AgentNavigator;
				return ((agentNavigator != null) ? agentNavigator.GetActiveBehavior() : null) is FollowAgentBehavior;
			}
			return false;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x00035304 File Offset: 0x00033504
		private void AddClanMembersAsAccompanyingCharacter(Hero member, LocationCharacter locationCharacter = null)
		{
			CharacterObject characterObject = member.CharacterObject;
			if (characterObject.IsHero && !characterObject.HeroObject.IsWounded && this.IsFollowingPlayer(member))
			{
				LocationCharacter locationCharacter2 = locationCharacter ?? LocationCharacter.CreateBodyguardHero(characterObject.HeroObject, MobileParty.MainParty, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFirstCompanionBehavior));
				PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(locationCharacter2, true);
				AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(characterObject);
				accompanyingCharacter.DisallowEntranceToAllLocations();
				accompanyingCharacter.AllowEntranceToLocations((Location x) => x == LocationComplex.Current.GetLocationWithId("center") || x == LocationComplex.Current.GetLocationWithId("village_center") || x == LocationComplex.Current.GetLocationWithId("tavern"));
			}
		}

		// Token: 0x040002F5 RID: 757
		private List<Hero> _isFollowingPlayer = new List<Hero>();

		// Token: 0x040002F6 RID: 758
		private Agent _gatherOrderedAgent;
	}
}
