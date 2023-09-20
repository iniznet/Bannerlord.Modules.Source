using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionAgentContourControllerView : MissionView
	{
		private bool _isAllowedByOption
		{
			get
			{
				return !BannerlordConfig.HideBattleUI || GameNetwork.IsMultiplayer;
			}
		}

		public MissionAgentContourControllerView()
		{
			this._contourAgents = new List<Agent>();
			this._isMultiplayer = GameNetwork.IsSessionActive;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isAllowedByOption)
			{
				bool getUIDebugMode = NativeConfig.GetUIDebugMode;
			}
		}

		private void PopulateContourListWithAgents()
		{
			this._contourAgents.Clear();
			Mission mission = base.Mission;
			bool flag;
			if (mission == null)
			{
				flag = null != null;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				flag = ((playerTeam != null) ? playerTeam.PlayerOrderController : null) != null;
			}
			if (flag)
			{
				foreach (Formation formation in Mission.Current.PlayerTeam.PlayerOrderController.SelectedFormations)
				{
					formation.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						if (!agent.IsMainAgent)
						{
							this._contourAgents.Add(agent);
						}
					}, null);
				}
			}
		}

		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			bool isAllowedByOption = this._isAllowedByOption;
		}

		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			if (this._isAllowedByOption)
			{
				this.RemoveContourFromFocusedAgent();
				this._currentFocusedAgent = null;
			}
		}

		private void AddContourToFocusedAgent()
		{
			if (this._currentFocusedAgent != null && !this._isContourAppliedToFocusedAgent)
			{
				MBAgentVisuals agentVisuals = this._currentFocusedAgent.AgentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.SetContourColor(new uint?(this._focusedContourColor), true);
				}
				this._isContourAppliedToFocusedAgent = true;
			}
		}

		private void RemoveContourFromFocusedAgent()
		{
			if (this._currentFocusedAgent != null && this._isContourAppliedToFocusedAgent)
			{
				if (this._contourAgents.Contains(this._currentFocusedAgent))
				{
					MBAgentVisuals agentVisuals = this._currentFocusedAgent.AgentVisuals;
					if (agentVisuals != null)
					{
						agentVisuals.SetContourColor(new uint?(this._nonFocusedContourColor), true);
					}
				}
				else
				{
					MBAgentVisuals agentVisuals2 = this._currentFocusedAgent.AgentVisuals;
					if (agentVisuals2 != null)
					{
						agentVisuals2.SetContourColor(null, true);
					}
				}
				this._isContourAppliedToFocusedAgent = false;
			}
		}

		private void ApplyContourToAllAgents()
		{
			if (!this._isContourAppliedToAllAgents)
			{
				foreach (Agent agent in this._contourAgents)
				{
					uint num = ((agent == this._currentFocusedAgent) ? this._focusedContourColor : (this._isMultiplayer ? this._friendlyContourColor : this._nonFocusedContourColor));
					MBAgentVisuals agentVisuals = agent.AgentVisuals;
					if (agentVisuals != null)
					{
						agentVisuals.SetContourColor(new uint?(num), true);
					}
				}
				this._isContourAppliedToAllAgents = true;
			}
		}

		private void RemoveContourFromAllAgents()
		{
			if (this._isContourAppliedToAllAgents)
			{
				foreach (Agent agent in this._contourAgents)
				{
					if (this._currentFocusedAgent == null || agent != this._currentFocusedAgent)
					{
						MBAgentVisuals agentVisuals = agent.AgentVisuals;
						if (agentVisuals != null)
						{
							agentVisuals.SetContourColor(null, true);
						}
					}
				}
				this._isContourAppliedToAllAgents = false;
			}
		}

		private const bool IsEnabled = false;

		private uint _nonFocusedContourColor = new Color(0.85f, 0.85f, 0.85f, 1f).ToUnsignedInteger();

		private uint _focusedContourColor = new Color(1f, 0.84f, 0.35f, 1f).ToUnsignedInteger();

		private uint _friendlyContourColor = new Color(0.44f, 0.83f, 0.26f, 1f).ToUnsignedInteger();

		private List<Agent> _contourAgents;

		private Agent _currentFocusedAgent;

		private bool _isContourAppliedToAllAgents;

		private bool _isContourAppliedToFocusedAgent;

		private bool _isMultiplayer;
	}
}
