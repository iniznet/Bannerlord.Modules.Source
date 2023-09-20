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
	public class VisualTrackerMissionBehavior : MissionLogic
	{
		public override void OnAgentCreated(Agent agent)
		{
			this.CheckAgent(agent);
		}

		private void CheckAgent(Agent agent)
		{
			if (agent.Character != null && this._visualTrackerManager.CheckTracked(agent.Character))
			{
				this.RegisterLocalOnlyObject(agent);
			}
		}

		public override void AfterStart()
		{
			this.Refresh();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._visualTrackerManager.TrackedObjectsVersion != this._trackedObjectsVersion)
			{
				this.Refresh();
			}
		}

		private void Refresh()
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				this.CheckAgent(agent);
			}
			this.RefreshCommonAreas();
			this._trackedObjectsVersion = this._visualTrackerManager.TrackedObjectsVersion;
		}

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

		public override List<CompassItemUpdateParams> GetCompassTargets()
		{
			List<CompassItemUpdateParams> list = new List<CompassItemUpdateParams>();
			foreach (TrackedObject trackedObject in this._currentTrackedObjects)
			{
				list.Add(new CompassItemUpdateParams(trackedObject.Object, 17, trackedObject.Position, 4288256409U, uint.MaxValue));
			}
			return list;
		}

		private void RemoveLocalObject(ITrackableBase obj)
		{
			this._currentTrackedObjects.RemoveAll((TrackedObject x) => x.Object == obj);
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			this.RemoveLocalObject(affectedAgent);
		}

		public override void OnAgentDeleted(Agent affectedAgent)
		{
			this.RemoveLocalObject(affectedAgent);
		}

		private List<TrackedObject> _currentTrackedObjects = new List<TrackedObject>();

		private int _trackedObjectsVersion = -1;

		private readonly VisualTrackerManager _visualTrackerManager = Campaign.Current.VisualTrackerManager;
	}
}
