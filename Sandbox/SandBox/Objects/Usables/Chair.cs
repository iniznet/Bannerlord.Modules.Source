using System;
using System.Linq;
using SandBox.AI;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables
{
	public class Chair : UsableMachine
	{
		protected override void OnInit()
		{
			base.OnInit();
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.AutoSheathWeapons = true;
			}
		}

		public bool IsAgentFullySitting(Agent usingAgent)
		{
			return base.StandingPoints.Count > 0 && base.StandingPoints.Contains(usingAgent.CurrentlyUsedGameObject) && usingAgent.IsSitting();
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject(this.IsAgentFullySitting(Agent.Main) ? "{=QGdaakYW}{KEY} Get Up" : "{=bl2aRW8f}{KEY} Sit", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			switch (this.ChairType)
			{
			case Chair.SittableType.Log:
				return new TextObject("{=9pgOGq7X}Log", null).ToString();
			case Chair.SittableType.Sofa:
				return new TextObject("{=GvLZKQ1U}Sofa", null).ToString();
			case Chair.SittableType.Ground:
				return new TextObject("{=L7ZQtIuM}Ground", null).ToString();
			default:
				return new TextObject("{=OgTUrRlR}Chair", null).ToString();
			}
		}

		public override StandingPoint GetBestPointAlternativeTo(StandingPoint standingPoint, Agent agent)
		{
			AnimationPoint animationPoint = standingPoint as AnimationPoint;
			if (animationPoint == null || animationPoint.GroupId < 0)
			{
				return animationPoint;
			}
			WorldFrame worldFrame = standingPoint.GetUserFrameForAgent(agent);
			float num = worldFrame.Origin.GetGroundVec3().DistanceSquared(agent.Position);
			foreach (StandingPoint standingPoint2 in base.StandingPoints)
			{
				AnimationPoint animationPoint2;
				if ((animationPoint2 = standingPoint2 as AnimationPoint) != null && standingPoint != standingPoint2 && animationPoint.GroupId == animationPoint2.GroupId && !animationPoint2.IsDisabledForAgent(agent))
				{
					worldFrame = animationPoint2.GetUserFrameForAgent(agent);
					float num2 = worldFrame.Origin.GetGroundVec3().DistanceSquared(agent.Position);
					if (num2 < num)
					{
						num = num2;
						animationPoint = animationPoint2;
					}
				}
			}
			return animationPoint;
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			return 0;
		}

		public Chair.SittableType ChairType;

		public enum SittableType
		{
			Chair,
			Log,
			Sofa,
			Ground
		}
	}
}
