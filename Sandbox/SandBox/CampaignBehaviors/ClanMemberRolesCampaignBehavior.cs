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
	public class ClanMemberRolesCampaignBehavior : CampaignBehaviorBase
	{
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

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_isFollowingPlayer", ref this._isFollowingPlayer);
		}

		private static void FollowMainAgent()
		{
			DailyBehaviorGroup behaviorGroup = ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
			behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
		}

		public bool IsFollowingPlayer(Hero hero)
		{
			return this._isFollowingPlayer.Contains(hero);
		}

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

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty && PlayerEncounter.LocationEncounter != null)
			{
				PlayerEncounter.LocationEncounter.RemoveAllAccompanyingCharacters();
				this._isFollowingPlayer.Clear();
			}
		}

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

		private void OnHeroJoinedParty(Hero hero, MobileParty mobileParty)
		{
			if (hero.Clan == Clan.PlayerClan && mobileParty.IsMainParty && mobileParty.CurrentSettlement != null && PlayerEncounter.LocationEncounter != null && MobileParty.MainParty.IsActive && (mobileParty.CurrentSettlement.IsFortification || mobileParty.CurrentSettlement.IsVillage) && this._isFollowingPlayer.Count == 0)
			{
				this.UpdateAccompanyingCharacters();
			}
		}

		private void OnMissionEnded(IMission mission)
		{
			this._gatherOrderedAgent = null;
		}

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

		private void ClearGatherOrderedAgentIfExists(Agent agent)
		{
			if (this._gatherOrderedAgent == agent)
			{
				this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().RemoveBehavior<ScriptBehavior>();
				this._gatherOrderedAgent = null;
			}
		}

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

		private bool clan_member_dont_follow_me_on_condition()
		{
			return ConversationMission.OneToOneConversationAgent != null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty && ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator != null && ConversationMission.OneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetActiveBehavior() is FollowAgentBehavior;
		}

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

		private void clan_member_gather_on_consequence()
		{
			this._gatherOrderedAgent = ConversationMission.OneToOneConversationAgent;
			this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<ScriptBehavior>().IsActive = true;
			ScriptBehavior.AddTargetWithDelegate(this._gatherOrderedAgent, new ScriptBehavior.SelectTargetDelegate(this.SelectTarget), new ScriptBehavior.OnTargetReachedDelegate(this.OnTargetReached));
			this._gatherOrderedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<FollowAgentBehavior>().IsActive = false;
		}

		private void clan_member_dont_follow_me_on_consequence()
		{
			this.RemoveFollowBehavior(ConversationMission.OneToOneConversationAgent);
		}

		private void clan_members_dont_follow_me_on_consequence()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				this.RemoveFollowBehavior(agent);
			}
		}

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

		private void RemoveAccompanyingHero(Hero hero)
		{
			this._isFollowingPlayer.Remove(hero);
			PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(hero);
		}

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

		private List<Hero> _isFollowingPlayer = new List<Hero>();

		private Agent _gatherOrderedAgent;
	}
}
