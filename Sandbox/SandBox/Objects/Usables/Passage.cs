using System;
using SandBox.AI;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables
{
	// Token: 0x02000028 RID: 40
	public class Passage : UsableMachine
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000CAE4 File Offset: 0x0000ACE4
		public Location ToLocation
		{
			get
			{
				PassageUsePoint passageUsePoint;
				if ((passageUsePoint = base.PilotStandingPoint as PassageUsePoint) == null)
				{
					return null;
				}
				return passageUsePoint.ToLocation;
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000CB08 File Offset: 0x0000AD08
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.DescriptionMessage.ToString();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000CB20 File Offset: 0x0000AD20
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.ActionMessage;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000CB33 File Offset: 0x0000AD33
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new PassageAI(this);
		}
	}
}
