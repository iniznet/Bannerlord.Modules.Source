using System;
using System.Collections.Generic;
using SandBox.Missions.MissionLogics;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class ChangeLocationBehavior : AgentBehavior
	{
		public ChangeLocationBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._initializeTime = base.Mission.CurrentTime;
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._selectedDoor == null)
			{
				Passage passage = this.SelectADoor();
				if (passage != null)
				{
					this._selectedDoor = passage;
					base.Navigator.SetTarget(this._selectedDoor, false);
					return;
				}
			}
			else if (this._selectedDoor.ToLocation.CharacterCount >= this._selectedDoor.ToLocation.ProsperityMax)
			{
				base.Navigator.SetTarget(null, false);
				base.Navigator.ForceThink(0f);
				this._selectedDoor = null;
			}
		}

		private Passage SelectADoor()
		{
			Passage passage = null;
			List<Passage> list = new List<Passage>();
			foreach (UsableMachine usableMachine in this._missionAgentHandler.TownPassageProps)
			{
				Passage passage2 = (Passage)usableMachine;
				if (passage2.GetVacantStandingPointForAI(base.OwnerAgent) != null && passage2.ToLocation.CharacterCount < passage2.ToLocation.ProsperityMax)
				{
					list.Add(passage2);
				}
			}
			if (list.Count > 0)
			{
				passage = list[MBRandom.RandomInt(list.Count)];
			}
			return passage;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this._selectedDoor = null;
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._selectedDoor = null;
		}

		public override string GetDebugInfo()
		{
			if (this._selectedDoor != null)
			{
				return "Go to " + this._selectedDoor.ToLocation.StringId;
			}
			return "Change Location no target";
		}

		public override float GetAvailability(bool isSimulation)
		{
			float num = 0f;
			bool flag = false;
			bool flag2 = false;
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.OwnerAgent.Origin);
			if (base.Mission.CurrentTime < 5f || locationCharacter.FixedLocation || !this._missionAgentHandler.HasPassages())
			{
				return 0f;
			}
			foreach (UsableMachine usableMachine in this._missionAgentHandler.TownPassageProps)
			{
				Passage passage = usableMachine as Passage;
				if (passage.ToLocation.CanAIEnter(locationCharacter) && passage.ToLocation.CharacterCount < passage.ToLocation.ProsperityMax)
				{
					flag = true;
					if (passage.PilotStandingPoint.GameEntity.GetGlobalFrame().origin.Distance(base.OwnerAgent.Position) < 1f)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag)
			{
				if (!flag2)
				{
					num = (CampaignMission.Current.Location.IsIndoor ? 0.1f : 0.05f);
				}
				else if (base.Mission.CurrentTime - this._initializeTime > 10f)
				{
					num = 0.01f;
				}
			}
			return num;
		}

		private readonly MissionAgentHandler _missionAgentHandler;

		private readonly float _initializeTime;

		private Passage _selectedDoor;
	}
}
