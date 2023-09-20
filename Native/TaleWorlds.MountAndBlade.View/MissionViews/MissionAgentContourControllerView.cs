using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000041 RID: 65
	public class MissionAgentContourControllerView : MissionView
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0001A41D File Offset: 0x0001861D
		private bool _isAllowedByOption
		{
			get
			{
				return !BannerlordConfig.HideBattleUI || GameNetwork.IsMultiplayer;
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0001A430 File Offset: 0x00018630
		public MissionAgentContourControllerView()
		{
			this._contourAgents = new List<Agent>();
			this._isMultiplayer = GameNetwork.IsSessionActive;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0001A4CE File Offset: 0x000186CE
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isAllowedByOption)
			{
				bool getUIDebugMode = NativeConfig.GetUIDebugMode;
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0001A4E8 File Offset: 0x000186E8
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

		// Token: 0x060002EF RID: 751 RVA: 0x0001A580 File Offset: 0x00018780
		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			bool isAllowedByOption = this._isAllowedByOption;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0001A592 File Offset: 0x00018792
		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			if (this._isAllowedByOption)
			{
				this.RemoveContourFromFocusedAgent();
				this._currentFocusedAgent = null;
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001A5B1 File Offset: 0x000187B1
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

		// Token: 0x060002F2 RID: 754 RVA: 0x0001A5EC File Offset: 0x000187EC
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

		// Token: 0x060002F3 RID: 755 RVA: 0x0001A668 File Offset: 0x00018868
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

		// Token: 0x060002F4 RID: 756 RVA: 0x0001A704 File Offset: 0x00018904
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

		// Token: 0x0400020E RID: 526
		private const bool IsEnabled = false;

		// Token: 0x0400020F RID: 527
		private uint _nonFocusedContourColor = new Color(0.85f, 0.85f, 0.85f, 1f).ToUnsignedInteger();

		// Token: 0x04000210 RID: 528
		private uint _focusedContourColor = new Color(1f, 0.84f, 0.35f, 1f).ToUnsignedInteger();

		// Token: 0x04000211 RID: 529
		private uint _friendlyContourColor = new Color(0.44f, 0.83f, 0.26f, 1f).ToUnsignedInteger();

		// Token: 0x04000212 RID: 530
		private List<Agent> _contourAgents;

		// Token: 0x04000213 RID: 531
		private Agent _currentFocusedAgent;

		// Token: 0x04000214 RID: 532
		private bool _isContourAppliedToAllAgents;

		// Token: 0x04000215 RID: 533
		private bool _isContourAppliedToFocusedAgent;

		// Token: 0x04000216 RID: 534
		private bool _isMultiplayer;
	}
}
