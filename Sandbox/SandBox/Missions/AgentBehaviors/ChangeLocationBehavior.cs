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
	// Token: 0x0200006E RID: 110
	public class ChangeLocationBehavior : AgentBehavior
	{
		// Token: 0x060004C7 RID: 1223 RVA: 0x0002223C File Offset: 0x0002043C
		public ChangeLocationBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._initializeTime = base.Mission.CurrentTime;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00022268 File Offset: 0x00020468
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

		// Token: 0x060004C9 RID: 1225 RVA: 0x000222E8 File Offset: 0x000204E8
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

		// Token: 0x060004CA RID: 1226 RVA: 0x00022390 File Offset: 0x00020590
		protected override void OnActivate()
		{
			base.OnActivate();
			this._selectedDoor = null;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0002239F File Offset: 0x0002059F
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._selectedDoor = null;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x000223AE File Offset: 0x000205AE
		public override string GetDebugInfo()
		{
			if (this._selectedDoor != null)
			{
				return "Go to " + this._selectedDoor.ToLocation.StringId;
			}
			return "Change Location no target";
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x000223D8 File Offset: 0x000205D8
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

		// Token: 0x04000247 RID: 583
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x04000248 RID: 584
		private readonly float _initializeTime;

		// Token: 0x04000249 RID: 585
		private Passage _selectedDoor;
	}
}
