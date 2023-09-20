using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000268 RID: 616
	public class DeploymentHandler : MissionLogic
	{
		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x060020E5 RID: 8421 RVA: 0x00076009 File Offset: 0x00074209
		public Team team
		{
			get
			{
				return base.Mission.PlayerTeam;
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060020E6 RID: 8422 RVA: 0x00076016 File Offset: 0x00074216
		public bool IsPlayerAttacker
		{
			get
			{
				return this.isPlayerAttacker;
			}
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x0007601E File Offset: 0x0007421E
		public DeploymentHandler(bool isPlayerAttacker)
		{
			this.isPlayerAttacker = isPlayerAttacker;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00076030 File Offset: 0x00074230
		public override void EarlyStart()
		{
			BattleSideEnum battleSideEnum = (this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			this.SetDeploymentBoundary(battleSideEnum);
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x00076051 File Offset: 0x00074251
		public override void AfterStart()
		{
			base.AfterStart();
			this.previousMissionMode = base.Mission.Mode;
			base.Mission.SetMissionMode(MissionMode.Deployment, true);
			this.team.OnOrderIssued += this.OrderController_OnOrderIssued;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x0007608E File Offset: 0x0007428E
		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00076098 File Offset: 0x00074298
		internal static void OrderController_OnOrderIssued_Aux(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			DeploymentHandler.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.appliedFormations = appliedFormations;
			bool flag = false;
			using (List<Formation>.Enumerator enumerator = CS$<>8__locals1.appliedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CountOfUnits > 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			switch (orderType)
			{
			case OrderType.None:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "OrderController_OnOrderIssued_Aux", 109);
				return;
			case OrderType.Move:
			case OrderType.MoveToLineSegment:
			case OrderType.MoveToLineSegmentWithHorizontalLayout:
			case OrderType.FollowMe:
			case OrderType.FollowEntity:
			case OrderType.Advance:
			case OrderType.FallBack:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.Charge:
			case OrderType.ChargeWithTarget:
			case OrderType.GuardMe:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.StandYourGround:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.Retreat:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.LookAtEnemy:
			case OrderType.LookAtDirection:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.ArrangementLine:
			case OrderType.ArrangementCloseOrder:
			case OrderType.ArrangementLoose:
			case OrderType.ArrangementCircular:
			case OrderType.ArrangementSchiltron:
			case OrderType.ArrangementVee:
			case OrderType.ArrangementColumn:
			case OrderType.ArrangementScatter:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.FormCustom:
			case OrderType.FormDeep:
			case OrderType.FormWide:
			case OrderType.FormWider:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.CohesionHigh:
			case OrderType.CohesionMedium:
			case OrderType.CohesionLow:
			case OrderType.HoldFire:
			case OrderType.FireAtWill:
			case OrderType.UseAnyWeapon:
			case OrderType.UseBluntWeaponsOnly:
				return;
			case OrderType.Mount:
			case OrderType.Dismount:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.AIControlOn:
			case OrderType.AIControlOff:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref CS$<>8__locals1);
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.Transfer:
			case OrderType.Use:
			case OrderType.AttackEntity:
				DeploymentHandler.<OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref CS$<>8__locals1);
				return;
			case OrderType.PointDefence:
				Debug.FailedAssert("will be removed", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "OrderController_OnOrderIssued_Aux", 184);
				return;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "OrderController_OnOrderIssued_Aux", 187);
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00076280 File Offset: 0x00074480
		public void ForceUpdateAllUnits()
		{
			this.OrderController_OnOrderIssued(OrderType.Move, this.team.FormationsIncludingSpecialAndEmpty, Array.Empty<object>());
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00076299 File Offset: 0x00074499
		public virtual void FinishDeployment()
		{
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x0007629B File Offset: 0x0007449B
		public override void OnRemoveBehavior()
		{
			if (this.team != null)
			{
				this.team.OnOrderIssued -= this.OrderController_OnOrderIssued;
			}
			base.Mission.SetMissionMode(this.previousMissionMode, false);
			base.OnRemoveBehavior();
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x000762D4 File Offset: 0x000744D4
		public void SetDeploymentBoundary(BattleSideEnum side)
		{
			IEnumerable<GameEntity> enumerable = base.Mission.Scene.FindEntitiesWithTagExpression("deployment_castle_boundary(_\\d+)*");
			Regex regex = new Regex("deployment_castle_boundary(_\\d+)*");
			Func<GameEntity, string> getExpressedTag = delegate(GameEntity e)
			{
				foreach (string text2 in e.Tags)
				{
					Match match = regex.Match(text2);
					if (match.Success)
					{
						return match.Value;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "SetDeploymentBoundary", 239);
				return null;
			};
			Func<GameEntity, bool> <>9__2;
			foreach (IGrouping<string, GameEntity> grouping in from e in enumerable
				group e by getExpressedTag(e))
			{
				IEnumerable<GameEntity> enumerable2 = grouping;
				Func<GameEntity, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (GameEntity e) => e.HasTag(side.ToString()));
				}
				if (enumerable2.Any(func))
				{
					string text = getExpressedTag(grouping.First<GameEntity>());
					bool flag = !grouping.Any((GameEntity e) => e.HasTag("out"));
					IEnumerable<Vec2> enumerable3 = grouping.Select((GameEntity bp) => bp.GlobalPosition.AsVec2);
					base.Mission.Boundaries.Add(text, enumerable3.ToList<Vec2>(), flag);
				}
			}
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00076418 File Offset: 0x00074618
		public void RemoveAllBoundaries()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in base.Mission.Boundaries)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (string text in list)
			{
				base.Mission.Boundaries.Remove(text);
			}
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x000764C0 File Offset: 0x000746C0
		public void InitializeDeploymentPoints()
		{
			if (!this.areDeploymentPointsInitialized)
			{
				foreach (DeploymentPoint deploymentPoint in base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>())
				{
					deploymentPoint.Hide();
				}
				this.areDeploymentPointsInitialized = true;
			}
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x00076524 File Offset: 0x00074724
		[CompilerGenerated]
		internal unsafe static void <OrderController_OnOrderIssued_Aux>g__ForceUpdateFormationParams|10_0(ref DeploymentHandler.<>c__DisplayClass10_0 A_0)
		{
			foreach (Formation formation in A_0.appliedFormations)
			{
				if (formation.CountOfUnits > 0)
				{
					bool flag = false;
					if (formation.IsPlayerTroopInFormation)
					{
						flag = formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Follow;
					}
					formation.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.UpdateCachedAndFormationValues(true, false);
					}, flag ? Mission.Current.MainAgent : null);
				}
			}
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x000765CC File Offset: 0x000747CC
		[CompilerGenerated]
		internal unsafe static void <OrderController_OnOrderIssued_Aux>g__ForcePositioning|10_1(ref DeploymentHandler.<>c__DisplayClass10_0 A_0)
		{
			foreach (Formation formation in A_0.appliedFormations)
			{
				if (formation.CountOfUnits > 0)
				{
					Vec2 direction = formation.FacingOrder.GetDirection(formation, null);
					Formation formation2 = formation;
					MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
					formation2.SetPositioning(new WorldPosition?(movementOrder.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None)), new Vec2?(direction), null);
				}
			}
		}

		// Token: 0x04000C21 RID: 3105
		protected MissionMode previousMissionMode;

		// Token: 0x04000C22 RID: 3106
		protected readonly bool isPlayerAttacker;

		// Token: 0x04000C23 RID: 3107
		private const string BoundaryTagExpression = "deployment_castle_boundary(_\\d+)*";

		// Token: 0x04000C24 RID: 3108
		private bool areDeploymentPointsInitialized;
	}
}
