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
	public class MissionBoundaryCrossingHandler : MissionLogic
	{
		public event Action<float, float> StartTime;

		public event Action StopTime;

		public event Action<float> TimeCount;

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

		public override void OnRemoveBehavior()
		{
			if (GameNetwork.IsSessionActive)
			{
				this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
			base.OnRemoveBehavior();
		}

		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<SetBoundariesState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetBoundariesState>(this.HandleServerEventSetPeerBoundariesState));
			}
		}

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
					blow.GlobalPosition = agent.Position;
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

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			this.OnAgentWentInOrRemoved(affectedAgent, true);
		}

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

		private void TickForMainAgent()
		{
			bool flag = !base.Mission.IsPositionInsideBoundaries(Agent.Main.Position.AsVec2);
			bool flag2 = this._mainAgentLeaveTimer != null;
			this.HandleAgentStateChange(Agent.Main, flag, flag2, this._mainAgentLeaveTimer);
		}

		private void TickForAgentAsServer(Agent agent)
		{
			bool flag = !base.Mission.IsPositionInsideBoundaries(agent.Position.AsVec2);
			bool flag2 = this._agentTimers.ContainsKey(agent);
			this.HandleAgentStateChange(agent, flag, flag2, flag2 ? this._agentTimers[agent] : null);
		}

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

		private void HandleServerEventSetPeerBoundariesState(SetBoundariesState message)
		{
			if (message.IsOutside)
			{
				this.OnAgentWentOut(base.Mission.MainAgent, message.StateStartTimeInSeconds);
				return;
			}
			this.OnAgentWentInOrRemoved(base.Mission.MainAgent, false);
		}

		private const float LeewayTime = 10f;

		private Dictionary<Agent, MissionTimer> _agentTimers;

		private MissionTimer _mainAgentLeaveTimer;
	}
}
