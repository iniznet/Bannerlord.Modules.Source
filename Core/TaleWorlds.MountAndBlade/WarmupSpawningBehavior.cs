using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002CE RID: 718
	public class WarmupSpawningBehavior : SpawningBehaviorBase
	{
		// Token: 0x06002753 RID: 10067 RVA: 0x00096EE6 File Offset: 0x000950E6
		public WarmupSpawningBehavior()
		{
			this.IsSpawningEnabled = true;
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x00096EF5 File Offset: 0x000950F5
		public override void OnTick(float dt)
		{
			if (this.IsSpawningEnabled && this._spawnCheckTimer.Check(base.Mission.CurrentTime))
			{
				this.SpawnAgents();
			}
			base.OnTick(dt);
		}

		// Token: 0x06002755 RID: 10069 RVA: 0x00096F24 File Offset: 0x00095124
		protected override void SpawnAgents()
		{
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.IsSynchronized)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.ControlledAgent == null && !component.HasSpawnedAgentVisuals && component.Team != null && component.Team != base.Mission.SpectatorTeam && component.TeamInitialPerkInfoReady && component.SpawnTimer.Check(base.Mission.CurrentTime))
					{
						IAgentVisual agentVisualForPeer = component.GetAgentVisualForPeer(0);
						BasicCultureObject basicCultureObject = ((component.Team.Side == BattleSideEnum.Attacker) ? @object : object2);
						int num = component.SelectedTroopIndex;
						IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses(basicCultureObject);
						MultiplayerClassDivisions.MPHeroClass mpheroClass = ((num < 0) ? null : mpheroClasses.ElementAt(num));
						if (mpheroClass == null && num < 0)
						{
							mpheroClass = mpheroClasses.First<MultiplayerClassDivisions.MPHeroClass>();
							num = 0;
						}
						BasicCharacterObject heroCharacter = mpheroClass.HeroCharacter;
						Equipment equipment = heroCharacter.Equipment.Clone(false);
						MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(component);
						IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null);
						if (enumerable != null)
						{
							foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable)
							{
								equipment[valueTuple.Item1] = valueTuple.Item2;
							}
						}
						MatrixFrame matrixFrame;
						if (agentVisualForPeer == null)
						{
							matrixFrame = this.SpawnComponent.GetSpawnFrame(component.Team, heroCharacter.Equipment.Horse.Item != null, false);
						}
						else
						{
							matrixFrame = agentVisualForPeer.GetFrame();
							matrixFrame.rotation.MakeUnit();
						}
						AgentBuildData agentBuildData = new AgentBuildData(heroCharacter).MissionPeer(component).Equipment(equipment).Team(component.Team)
							.TroopOrigin(new BasicBattleAgentOrigin(heroCharacter))
							.InitialPosition(matrixFrame.origin);
						Vec2 vec = matrixFrame.rotation.f.AsVec2;
						vec = vec.Normalized();
						AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(vec).IsFemale(component.Peer.IsFemale).BodyProperties(base.GetBodyProperties(component, basicCultureObject))
							.VisualsIndex(0)
							.ClothingColor1((component.Team == base.Mission.AttackerTeam) ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor)
							.ClothingColor2((component.Team == base.Mission.AttackerTeam) ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
						if (this.GameMode.ShouldSpawnVisualsForServer(networkCommunicator))
						{
							base.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(component, agentBuildData2, num, false, 0);
						}
						this.GameMode.HandleAgentVisualSpawning(networkCommunicator, agentBuildData2, 0, true);
					}
				}
			}
			if (base.Mission.AttackerTeam != null)
			{
				int num2 = 0;
				foreach (Agent agent in base.Mission.AttackerTeam.ActiveAgents)
				{
					if (agent.Character != null && agent.MissionPeer == null)
					{
						num2++;
					}
				}
				if (num2 < MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
				{
					base.SpawnBot(base.Mission.AttackerTeam, @object);
				}
			}
			if (base.Mission.DefenderTeam != null)
			{
				int num3 = 0;
				foreach (Agent agent2 in base.Mission.DefenderTeam.ActiveAgents)
				{
					if (agent2.Character != null && agent2.MissionPeer == null)
					{
						num3++;
					}
				}
				if (num3 < MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions))
				{
					base.SpawnBot(base.Mission.DefenderTeam, object2);
				}
			}
		}

		// Token: 0x06002756 RID: 10070 RVA: 0x00097384 File Offset: 0x00095584
		public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
		{
			return true;
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x00097387 File Offset: 0x00095587
		public override int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
		{
			return 3;
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x0009738A File Offset: 0x0009558A
		protected override bool IsRoundInProgress()
		{
			return Mission.Current.CurrentState == Mission.State.Continuing;
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x00097399 File Offset: 0x00095599
		public override void Clear()
		{
			base.Clear();
			base.RequestStopSpawnSession();
		}
	}
}
