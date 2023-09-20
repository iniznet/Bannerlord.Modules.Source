using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.CampaignBehaviors;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Missions.MissionLogics.Towns
{
	public class PrisonBreakMissionController : MissionLogic
	{
		public PrisonBreakMissionController(CharacterObject prisonerCharacter, CharacterObject companionCharacter)
		{
			this._prisonerCharacter = prisonerCharacter;
			this._companionCharacter = companionCharacter;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		public override void OnBehaviorInitialize()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition += this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		public override void AfterStart()
		{
			this._isPrisonerFollowing = true;
			MBTextManager.SetTextVariable("IS_PRISONER_FOLLOWING", this._isPrisonerFollowing ? 1 : 0);
			base.Mission.SetMissionMode(4, true);
			base.Mission.IsInventoryAccessible = false;
			base.Mission.IsQuestScreenAccessible = true;
			LocationCharacter firstLocationCharacterOfCharacter = LocationComplex.Current.GetFirstLocationCharacterOfCharacter(this._prisonerCharacter);
			PlayerEncounter.LocationEncounter.AddAccompanyingCharacter(firstLocationCharacterOfCharacter, true);
			this._areaMarkers = (from area in MBExtensions.FindAllWithType<AreaMarker>(base.Mission.ActiveMissionObjects)
				orderby area.AreaIndex
				select area).ToList<AreaMarker>();
			MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			foreach (UsableMachine usableMachine in missionBehavior.TownPassageProps)
			{
				usableMachine.Deactivate();
			}
			missionBehavior.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, true, false, false, false, "");
			missionBehavior.SpawnLocationCharacters(null);
			this.ArrangeGuardCount();
			this._prisonerAgent = base.Mission.Agents.First((Agent x) => x.Character == this._prisonerCharacter);
			this.PreparePrisonAgent();
			missionBehavior.SimulateAgent(this._prisonerAgent);
			for (int i = 0; i < this._guardAgents.Count; i++)
			{
				Agent agent = this._guardAgents[i];
				agent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag = this._areaMarkers[i % this._areaMarkers.Count].Tag;
				missionBehavior.SimulateAgent(agent);
			}
			this.SetTeams();
		}

		public override void OnMissionTick(float dt)
		{
			SandBoxHelpers.MissionHelper.FadeOutAgents(this._agentsToRemove, true, true);
			this._agentsToRemove.Clear();
			if (this._prisonerAgent != null)
			{
				this.CheckPrisonerSwitchToAlarmState();
			}
		}

		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (this._guardAgents != null && usedObject is AnimationPoint && this._guardAgents.Contains(userAgent))
			{
				userAgent.StopUsingGameObject(true, 1);
			}
		}

		public override void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			if (userAgent == Agent.Main && agent == this._prisonerAgent)
			{
				this.SwitchPrisonerFollowingState(false);
			}
		}

		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return userAgent == Agent.Main && otherAgent == this._prisonerAgent;
		}

		private void PreparePrisonAgent()
		{
			this._prisonerAgent.Health = this._prisonerAgent.HealthLimit;
			this._prisonerAgent.Defensiveness = 2f;
			AgentNavigator agentNavigator = this._prisonerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.RemoveBehaviorGroup<AlarmedBehaviorGroup>();
			agentNavigator.SpecialTargetTag = "sp_prison_break_prisoner";
			ItemObject itemObject = Extensions.MinBy<ItemObject, int>(Items.All.Where((ItemObject x) => x.IsCraftedWeapon && x.Type == 2 && x.WeaponComponent.GetItemType() == 2 && x.IsCivilian), (ItemObject x) => x.Value);
			MissionWeapon missionWeapon;
			missionWeapon..ctor(itemObject, null, this._prisonerCharacter.HeroObject.ClanBanner);
			this._prisonerAgent.EquipWeaponWithNewEntity(0, ref missionWeapon);
		}

		public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
		{
			if (agent == this._prisonerAgent && flag != 1)
			{
				this.SwitchPrisonerFollowingState(true);
			}
			this.UpdateDoorPermission();
		}

		private void ArrangeGuardCount()
		{
			int num = 2 + Settlement.CurrentSettlement.Town.GetWallLevel();
			float security = Settlement.CurrentSettlement.Town.Security;
			if (security < 40f)
			{
				num--;
			}
			else if (security > 70f)
			{
				num++;
			}
			this._guardAgents = base.Mission.Agents.Where(delegate(Agent x)
			{
				CharacterObject characterObject;
				return (characterObject = x.Character as CharacterObject) != null && characterObject.IsSoldier;
			}).ToList<Agent>();
			this._agentsToRemove = new List<Agent>();
			int count = this._guardAgents.Count;
			if (count > num)
			{
				int num2 = count - num;
				for (int i = 0; i < count; i++)
				{
					if (num2 <= 0)
					{
						break;
					}
					Agent agent = this._guardAgents[i];
					if (!agent.Character.IsHero)
					{
						this._agentsToRemove.Add(agent);
						num2--;
					}
				}
			}
			else if (count < num)
			{
				List<LocationCharacter> list = (from x in LocationComplex.Current.GetListOfCharactersInLocation("prison")
					where !x.Character.IsHero && x.Character.IsSoldier
					select x).ToList<LocationCharacter>();
				if (Extensions.IsEmpty<LocationCharacter>(list))
				{
					AgentData agentData = GuardsCampaignBehavior.PrepareGuardAgentDataFromGarrison(PlayerEncounter.LocationEncounter.Settlement.Culture.Guard, true, false);
					LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddStandGuardBehaviors), "sp_guard", true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_guard"), false, false, null, false, false, true);
					list.Add(locationCharacter);
				}
				int count2 = list.Count;
				Location locationWithId = LocationComplex.Current.GetLocationWithId("prison");
				int num3 = num - count;
				for (int j = 0; j < num3; j++)
				{
					LocationCharacter locationCharacter2 = list[j % count2];
					LocationCharacter locationCharacter3 = new LocationCharacter(new AgentData(new SimpleAgentOrigin(locationCharacter2.Character, -1, locationCharacter2.AgentData.AgentOrigin.Banner, default(UniqueTroopDescriptor))).Equipment(locationCharacter2.AgentData.AgentOverridenEquipment).Monster(locationCharacter2.AgentData.AgentMonster).NoHorses(true), locationCharacter2.AddBehaviors, this._areaMarkers[j % this._areaMarkers.Count].Tag, true, 2, locationCharacter2.ActionSetCode, locationCharacter2.UseCivilianEquipment, false, null, false, false, true);
					LocationComplex.Current.ChangeLocation(locationCharacter3, null, locationWithId);
				}
			}
			this._guardAgents = base.Mission.Agents.Where((Agent x) => x.Character is CharacterObject && ((CharacterObject)x.Character).IsSoldier && !this._agentsToRemove.Contains(x)).ToList<Agent>();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._guardAgents.Contains(affectedAgent))
			{
				this._guardAgents.Remove(affectedAgent);
				this.UpdateDoorPermission();
				if (this._prisonerAgent != null)
				{
					AgentFlag agentFlags = this._prisonerAgent.GetAgentFlags();
					this._prisonerAgent.SetAgentFlags(agentFlags & -65537);
					return;
				}
			}
			else if (this._prisonerAgent == affectedAgent)
			{
				this._prisonerAgent = null;
			}
		}

		private void CheckPrisonerSwitchToAlarmState()
		{
			foreach (Agent agent in this._guardAgents)
			{
				if (this._prisonerAgent.Position.DistanceSquared(agent.Position) < 3f)
				{
					AgentFlag agentFlags = this._prisonerAgent.GetAgentFlags();
					this._prisonerAgent.SetAgentFlags(agentFlags | 65536);
				}
			}
		}

		private void SwitchPrisonerFollowingState(bool forceFollow = false)
		{
			this._isPrisonerFollowing = forceFollow || !this._isPrisonerFollowing;
			MBTextManager.SetTextVariable("IS_PRISONER_FOLLOWING", this._isPrisonerFollowing ? 1 : 0);
			FollowAgentBehavior behavior = this._prisonerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().GetBehavior<FollowAgentBehavior>();
			if (this._isPrisonerFollowing)
			{
				this._prisonerAgent.SetCrouchMode(false);
				behavior.SetTargetAgent(Agent.Main);
				AgentFlag agentFlags = this._prisonerAgent.GetAgentFlags();
				this._prisonerAgent.SetAgentFlags(agentFlags & -65537);
			}
			else
			{
				behavior.SetTargetAgent(null);
				this._prisonerAgent.SetCrouchMode(true);
			}
			this._prisonerAgent.AIStateFlags = 0;
		}

		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			bool flag;
			if (Agent.Main != null && Agent.Main.IsActive() && !Extensions.IsEmpty<Agent>(this._guardAgents))
			{
				flag = this._guardAgents.All((Agent x) => !x.IsActive());
			}
			else
			{
				flag = true;
			}
			canLeave = flag;
			if (!canLeave)
			{
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat", null), 0, null, "");
			}
			return null;
		}

		private void SetTeams()
		{
			base.Mission.PlayerTeam.SetIsEnemyOf(base.Mission.PlayerEnemyTeam, true);
			this._prisonerAgent.SetTeam(base.Mission.PlayerTeam, true);
			if (this._companionCharacter != null)
			{
				base.Mission.Agents.First((Agent x) => x.Character == this._companionCharacter).SetTeam(base.Mission.PlayerTeam, true);
			}
			foreach (Agent agent in this._guardAgents)
			{
				agent.SetTeam(base.Mission.PlayerEnemyTeam, true);
				AgentFlag agentFlags = agent.GetAgentFlags();
				agent.SetAgentFlags((agentFlags | 65536) & -1048577);
			}
		}

		protected override void OnEndMission()
		{
			if (PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter.Character == this._prisonerCharacter))
			{
				PlayerEncounter.LocationEncounter.RemoveAccompanyingCharacter(this._prisonerCharacter.HeroObject);
			}
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				GameMenu.SwitchToMenu("settlement_prison_break_fail_player_unconscious");
			}
			else if (this._prisonerAgent == null || !this._prisonerAgent.IsActive())
			{
				GameMenu.SwitchToMenu("settlement_prison_break_fail_prisoner_unconscious");
			}
			else
			{
				GameMenu.SwitchToMenu("settlement_prison_break_success");
			}
			Campaign.Current.GameMenuManager.NextLocation = null;
			Campaign.Current.GameMenuManager.PreviousLocation = null;
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition -= this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		private void UpdateDoorPermission()
		{
			bool flag;
			if (!Extensions.IsEmpty<Agent>(this._guardAgents))
			{
				flag = this._guardAgents.All((Agent x) => x.CurrentWatchState != 2);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			foreach (UsableMachine usableMachine in base.Mission.GetMissionBehavior<MissionAgentHandler>().TownPassageProps)
			{
				if (flag2)
				{
					usableMachine.Activate();
				}
				else
				{
					usableMachine.Deactivate();
				}
			}
		}

		private bool IsAgentInteractionAllowed_AdditionalCondition()
		{
			return true;
		}

		private const int PrisonerSwitchToAlarmedDistance = 3;

		private readonly CharacterObject _prisonerCharacter;

		private readonly CharacterObject _companionCharacter;

		private List<Agent> _guardAgents;

		private List<Agent> _agentsToRemove;

		private Agent _prisonerAgent;

		private List<AreaMarker> _areaMarkers;

		private bool _isPrisonerFollowing;
	}
}
