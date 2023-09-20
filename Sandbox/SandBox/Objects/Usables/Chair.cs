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
	// Token: 0x02000026 RID: 38
	public class Chair : UsableMachine
	{
		// Token: 0x060001BE RID: 446 RVA: 0x0000C464 File Offset: 0x0000A664
		protected override void OnInit()
		{
			base.OnInit();
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.AutoSheathWeapons = true;
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000C4BC File Offset: 0x0000A6BC
		public bool IsAgentFullySitting(Agent usingAgent)
		{
			return base.StandingPoints.Count > 0 && base.StandingPoints.Contains(usingAgent.CurrentlyUsedGameObject) && usingAgent.IsSitting();
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000C4E7 File Offset: 0x0000A6E7
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000C4EF File Offset: 0x0000A6EF
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject(this.IsAgentFullySitting(Agent.Main) ? "{=QGdaakYW}{KEY} Get Up" : "{=bl2aRW8f}{KEY} Sit", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000C530 File Offset: 0x0000A730
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

		// Token: 0x060001C3 RID: 451 RVA: 0x0000C5A0 File Offset: 0x0000A7A0
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

		// Token: 0x060001C4 RID: 452 RVA: 0x0000C684 File Offset: 0x0000A884
		public override OrderType GetOrder(BattleSideEnum side)
		{
			return 0;
		}

		// Token: 0x040000B8 RID: 184
		public Chair.SittableType ChairType;

		// Token: 0x02000102 RID: 258
		public enum SittableType
		{
			// Token: 0x04000507 RID: 1287
			Chair,
			// Token: 0x04000508 RID: 1288
			Log,
			// Token: 0x04000509 RID: 1289
			Sofa,
			// Token: 0x0400050A RID: 1290
			Ground
		}
	}
}
