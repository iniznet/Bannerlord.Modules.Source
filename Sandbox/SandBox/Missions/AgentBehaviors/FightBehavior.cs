using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000071 RID: 113
	public class FightBehavior : AgentBehavior
	{
		// Token: 0x060004E9 RID: 1257 RVA: 0x0002318F File Offset: 0x0002138F
		public FightBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			if (base.OwnerAgent.HumanAIComponent == null)
			{
				base.OwnerAgent.AddComponent(new HumanAIComponent(base.OwnerAgent));
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000231CC File Offset: 0x000213CC
		public override float GetAvailability(bool isSimulation)
		{
			if (!MissionFightHandler.IsAgentAggressive(base.OwnerAgent))
			{
				return 0.1f;
			}
			return 1f;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x000231E8 File Offset: 0x000213E8
		protected override void OnActivate()
		{
			TextObject textObject = new TextObject("{=!}{p0} {p1} activate alarmed behavior group.", null);
			textObject.SetTextVariable("p0", base.OwnerAgent.Name.ToString());
			textObject.SetTextVariable("p1", base.OwnerAgent.Index.ToString());
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0002323C File Offset: 0x0002143C
		protected override void OnDeactivate()
		{
			TextObject textObject = new TextObject("{=!}{p0} {p1} deactivate fight behavior.", null);
			textObject.SetTextVariable("p0", base.OwnerAgent.Name.ToString());
			textObject.SetTextVariable("p1", base.OwnerAgent.Index.ToString());
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0002328E File Offset: 0x0002148E
		public override string GetDebugInfo()
		{
			return "Fight";
		}

		// Token: 0x04000259 RID: 601
		private readonly MissionAgentHandler _missionAgentHandler;
	}
}
