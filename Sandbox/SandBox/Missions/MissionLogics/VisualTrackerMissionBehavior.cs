using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000058 RID: 88
	public class VisualTrackerMissionBehavior : MissionLogic
	{
		// Token: 0x060003CD RID: 973 RVA: 0x0001BAAF File Offset: 0x00019CAF
		public override void OnAgentCreated(Agent agent)
		{
			this.CheckAgent(agent);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001BAB8 File Offset: 0x00019CB8
		private void CheckAgent(Agent agent)
		{
			if (agent.Character != null && this._visualTrackerManager.CheckTracked(agent.Character))
			{
				this.RegisterLocalOnlyObject(agent);
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001BADC File Offset: 0x00019CDC
		public override void AfterStart()
		{
			this.Refresh();
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0001BAE4 File Offset: 0x00019CE4
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._visualTrackerManager.TrackedObjectsVersion != this._trackedObjectsVersion)
			{
				this.Refresh();
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0001BB08 File Offset: 0x00019D08
		private void Refresh()
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				this.CheckAgent(agent);
			}
			this.RefreshCommonAreas();
			this._trackedObjectsVersion = this._visualTrackerManager.TrackedObjectsVersion;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0001BB78 File Offset: 0x00019D78
		public void RegisterLocalOnlyObject(ITrackableBase obj)
		{
			using (List<TrackedObject>.Enumerator enumerator = this._currentTrackedObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Object == obj)
					{
						return;
					}
				}
			}
			this._currentTrackedObjects.Add(new TrackedObject(obj));
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001BBE0 File Offset: 0x00019DE0
		private void RefreshCommonAreas()
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			foreach (CommonAreaMarker commonAreaMarker in MBExtensions.FindAllWithType<CommonAreaMarker>(base.Mission.ActiveMissionObjects).ToList<CommonAreaMarker>())
			{
				if (settlement.Alleys.Count >= commonAreaMarker.AreaIndex)
				{
					this.RegisterLocalOnlyObject(commonAreaMarker);
				}
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0001BC60 File Offset: 0x00019E60
		public override List<CompassItemUpdateParams> GetCompassTargets()
		{
			List<CompassItemUpdateParams> list = new List<CompassItemUpdateParams>();
			foreach (TrackedObject trackedObject in this._currentTrackedObjects)
			{
				list.Add(new CompassItemUpdateParams(trackedObject.Object, 17, trackedObject.Position, 4288256409U, uint.MaxValue));
			}
			return list;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0001BCD4 File Offset: 0x00019ED4
		private void RemoveLocalObject(ITrackableBase obj)
		{
			this._currentTrackedObjects.RemoveAll((TrackedObject x) => x.Object == obj);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0001BD06 File Offset: 0x00019F06
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			this.RemoveLocalObject(affectedAgent);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0001BD0F File Offset: 0x00019F0F
		public override void OnAgentDeleted(Agent affectedAgent)
		{
			this.RemoveLocalObject(affectedAgent);
		}

		// Token: 0x040001C8 RID: 456
		private List<TrackedObject> _currentTrackedObjects = new List<TrackedObject>();

		// Token: 0x040001C9 RID: 457
		private int _trackedObjectsVersion = -1;

		// Token: 0x040001CA RID: 458
		private readonly VisualTrackerManager _visualTrackerManager = Campaign.Current.VisualTrackerManager;
	}
}
