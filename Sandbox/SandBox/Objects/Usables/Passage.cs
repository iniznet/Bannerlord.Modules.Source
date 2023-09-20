using System;
using SandBox.AI;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables
{
	public class Passage : UsableMachine
	{
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

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.DescriptionMessage.ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.ActionMessage;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new PassageAI(this);
		}
	}
}
