using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000270 RID: 624
	public class MissionBoundaryCrossingHandler : MissionLogic
	{
		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06002162 RID: 8546 RVA: 0x0007996C File Offset: 0x00077B6C
		// (remove) Token: 0x06002163 RID: 8547 RVA: 0x000799A4 File Offset: 0x00077BA4
		public event Action<float, float> StartTime;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06002164 RID: 8548 RVA: 0x000799DC File Offset: 0x00077BDC
		// (remove) Token: 0x06002165 RID: 8549 RVA: 0x00079A14 File Offset: 0x00077C14
		public event Action StopTime;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06002166 RID: 8550 RVA: 0x00079A4C File Offset: 0x00077C4C
		// (remove) Token: 0x06002167 RID: 8551 RVA: 0x00079A84 File Offset: 0x00077C84
		public event Action<float> TimeCount;

		// Token: 0x06002168 RID: 8552 RVA: 0x00079AB9 File Offset: 0x00077CB9
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			if (GameNetwork.IsSessionActive)
			{
				this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			}
			if (GameNetwork.IsServer)
			{
				this._agentTimers = new Dictionary<Agent, MissionTimer>();
			}
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x00079AE1 File Offset: 0x00077CE1
		public override void OnRemoveBehavior()
		{
			if (GameNetwork.IsSessionActive)
			{
				this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
			base.OnRemoveBehavior();
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x00079AF8 File Offset: 0x00077CF8
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<SetBoundariesState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetBoundariesState>(this.HandleServerEventSetPeerBoundariesState));
			}
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x00079B28 File Offset: 0x00077D28
		private void OnAgentWentOut(Agent agent, float startTimeInSeconds)
		{
			MissionTimer missionTimer = (GameNetwork.IsClient ? MissionTimer.CreateSynchedTimerClient(startTimeInSeconds, 10f) : new MissionTimer(10f));
			if (GameNetwork.IsServer)
			{
				this._agentTimers.Add(agent, missionTimer);
				MissionPeer missionPeer = agent.MissionPeer;
				NetworkCommunicator networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
				if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
				{
					GameNetwork.BeginModuleEventAsServer(networkCommunicator);
					GameNetwork.WriteMessage(new SetBoundariesState(true, missionTimer.GetStartTime().NumberOfTicks));
					GameNetwork.EndModuleEventAsServer();
				}
			}
			if (base.Mission.MainAgent == agent)
			{
				this._mainAgentLeaveTimer = missionTimer;
				Action<float, float> startTime = this.StartTime;
				if (startTime != null)
				{
					startTime(10f, 0f);
				}
				MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
				Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
				if (Mission.Current.Mode == MissionMode.Battle)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/out_of_map"), vec);
				}
			}
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x00079C1C File Offset: 0x00077E1C
		private void OnAgentWentInOrRemoved(Agent agent, bool isAgentRemoved)
		{
			if (GameNetwork.IsServer)
			{
				this._agentTimers.Remove(agent);
				if (!isAgentRemoved)
				{
					MissionPeer missionPeer = agent.MissionPeer;
					NetworkCommunicator networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
					if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
					{
						GameNetwork.BeginModuleEventAsServer(networkCommunicator);
						GameNetwork.WriteMessage(new SetBoundariesState(false));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
			if (base.Mission.MainAgent == agent)
			{
				this._mainAgentLeaveTimer = null;
				Action stopTime = this.StopTime;
				if (stopTime == null)
				{
					return;
				}
				stopTime();
			}
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x00079C9C File Offset: 0x00077E9C
		private void HandleAgentPunishment(Agent agent)
		{
			if (GameNetwork.IsSessionActive)
			{
				if (GameNetwork.IsServer)
				{
					Blow blow = new Blow(agent.Index);
					blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, 0);
					blow.DamageType = DamageTypes.Blunt;
					blow.BaseMagnitude = 10000f;
					blow.WeaponRecord.WeaponClass = WeaponClass.Undefined;
					blow.Position = agent.Position;
					blow.DamagedPercentage = 1f;
					agent.Die(blow, Agent.KillInfo.Invalid);
					return;
				}
			}
			else
			{
				base.Mission.RetreatMission();
			}
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x00079D24 File Offset: 0x00077F24
		public override void OnClearScene()
		{
			if (GameNetwork.IsServer)
			{
				using (List<Agent>.Enumerator enumerator = this._agentTimers.Keys.ToList<Agent>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent = enumerator.Current;
						this.OnAgentWentInOrRemoved(agent, true);
					}
					return;
				}
			}
			if (this._mainAgentLeaveTimer != null)
			{
				if (base.Mission.MainAgent != null)
				{
					this.OnAgentWentInOrRemoved(base.Mission.MainAgent, true);
					return;
				}
				this._mainAgentLeaveTimer = null;
			}
		}

		// Token: 0x0600216F RID: 8559 RVA: 0x00079DB8 File Offset: 0x00077FB8
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			this.OnAgentWentInOrRemoved(affectedAgent, true);
		}

		// Token: 0x06002170 RID: 8560 RVA: 0x00079DC4 File Offset: 0x00077FC4
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (GameNetwork.IsServer)
			{
				for (int i = base.Mission.Agents.Count - 1; i >= 0; i--)
				{
					Agent agent = base.Mission.Agents[i];
					if (agent.MissionPeer != null)
					{
						this.TickForAgentAsServer(agent);
					}
				}
			}
			else if (!GameNetwork.IsSessionActive && Agent.Main != null)
			{
				this.TickForMainAgent();
			}
			if (this._mainAgentLeaveTimer != null)
			{
				this._mainAgentLeaveTimer.Check(false);
				float num = 1f - this._mainAgentLeaveTimer.GetRemainingTimeInSeconds(true) / 10f;
				Action<float> timeCount = this.TimeCount;
				if (timeCount == null)
				{
					return;
				}
				timeCount(num);
			}
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x00079E74 File Offset: 0x00078074
		private void TickForMainAgent()
		{
			bool flag = !base.Mission.IsPositionInsideBoundaries(Agent.Main.Position.AsVec2);
			bool flag2 = this._mainAgentLeaveTimer != null;
			this.HandleAgentStateChange(Agent.Main, flag, flag2, this._mainAgentLeaveTimer);
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x00079EC0 File Offset: 0x000780C0
		private void TickForAgentAsServer(Agent agent)
		{
			bool flag = !base.Mission.IsPositionInsideBoundaries(agent.Position.AsVec2);
			bool flag2 = this._agentTimers.ContainsKey(agent);
			this.HandleAgentStateChange(agent, flag, flag2, flag2 ? this._agentTimers[agent] : null);
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x00079F12 File Offset: 0x00078112
		private void HandleAgentStateChange(Agent agent, bool isAgentOutside, bool isTimerActiveForAgent, MissionTimer timerInstance)
		{
			if (isAgentOutside && !isTimerActiveForAgent)
			{
				this.OnAgentWentOut(agent, 0f);
				return;
			}
			if (!isAgentOutside && isTimerActiveForAgent)
			{
				this.OnAgentWentInOrRemoved(agent, false);
				return;
			}
			if (isAgentOutside && timerInstance.Check(false))
			{
				this.HandleAgentPunishment(agent);
			}
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x00079F4C File Offset: 0x0007814C
		private void HandleServerEventSetPeerBoundariesState(SetBoundariesState message)
		{
			if (message.IsOutside)
			{
				this.OnAgentWentOut(base.Mission.MainAgent, message.StateStartTimeInSeconds);
				return;
			}
			this.OnAgentWentInOrRemoved(base.Mission.MainAgent, false);
		}

		// Token: 0x04000C5C RID: 3164
		private const float LeewayTime = 10f;

		// Token: 0x04000C60 RID: 3168
		private Dictionary<Agent, MissionTimer> _agentTimers;

		// Token: 0x04000C61 RID: 3169
		private MissionTimer _mainAgentLeaveTimer;
	}
}
