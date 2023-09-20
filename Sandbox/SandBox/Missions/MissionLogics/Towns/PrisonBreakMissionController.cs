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
	// Token: 0x0200005B RID: 91
	public class PrisonBreakMissionController : MissionLogic
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x0001C512 File Offset: 0x0001A712
		public PrisonBreakMissionController(CharacterObject prisonerCharacter, CharacterObject companionCharacter)
		{
			this._prisonerCharacter = prisonerCharacter;
			this._companionCharacter = companionCharacter;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0001C528 File Offset: 0x0001A728
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001C53C File Offset: 0x0001A73C
		public override void OnBehaviorInitialize()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition += this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0001C558 File Offset: 0x0001A758
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

		// Token: 0x060003F3 RID: 1011 RVA: 0x0001C710 File Offset: 0x0001A910
		public override void OnMissionTick(float dt)
		{
			SandBoxHelpers.MissionHelper.FadeOutAgents(this._agentsToRemove, true, true);
			this._agentsToRemove.Clear();
			if (this._prisonerAgent != null)
			{
				this.CheckPrisonerSwitchToAlarmState();
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001C738 File Offset: 0x0001A938
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (this._guardAgents != null && usedObject is AnimationPoint && this._guardAgents.Contains(userAgent))
			{
				userAgent.StopUsingGameObject(true, 1);
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001C760 File Offset: 0x0001A960
		public override void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			if (userAgent == Agent.Main && agent == this._prisonerAgent)
			{
				this.SwitchPrisonerFollowingState(false);
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0001C77A File Offset: 0x0001A97A
		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return userAgent == Agent.Main && otherAgent == this._prisonerAgent;
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0001C790 File Offset: 0x0001A990
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

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001C858 File Offset: 0x0001AA58
		public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
		{
			if (agent == this._prisonerAgent && flag != 1)
			{
				this.SwitchPrisonerFollowingState(true);
			}
			this.UpdateDoorPermission();
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001C874 File Offset: 0x0001AA74
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

		// Token: 0x060003FA RID: 1018 RVA: 0x0001CB24 File Offset: 0x0001AD24
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

		// Token: 0x060003FB RID: 1019 RVA: 0x0001CB8C File Offset: 0x0001AD8C
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

		// Token: 0x060003FC RID: 1020 RVA: 0x0001CC18 File Offset: 0x0001AE18
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

		// Token: 0x060003FD RID: 1021 RVA: 0x0001CCCC File Offset: 0x0001AECC
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

		// Token: 0x060003FE RID: 1022 RVA: 0x0001CD44 File Offset: 0x0001AF44
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

		// Token: 0x060003FF RID: 1023 RVA: 0x0001CE24 File Offset: 0x0001B024
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

		// Token: 0x06000400 RID: 1024 RVA: 0x0001CEE4 File Offset: 0x0001B0E4
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

		// Token: 0x06000401 RID: 1025 RVA: 0x0001CF88 File Offset: 0x0001B188
		private bool IsAgentInteractionAllowed_AdditionalCondition()
		{
			return true;
		}

		// Token: 0x040001D8 RID: 472
		private const int PrisonerSwitchToAlarmedDistance = 3;

		// Token: 0x040001D9 RID: 473
		private readonly CharacterObject _prisonerCharacter;

		// Token: 0x040001DA RID: 474
		private readonly CharacterObject _companionCharacter;

		// Token: 0x040001DB RID: 475
		private List<Agent> _guardAgents;

		// Token: 0x040001DC RID: 476
		private List<Agent> _agentsToRemove;

		// Token: 0x040001DD RID: 477
		private Agent _prisonerAgent;

		// Token: 0x040001DE RID: 478
		private List<AreaMarker> _areaMarkers;

		// Token: 0x040001DF RID: 479
		private bool _isPrisonerFollowing;
	}
}
