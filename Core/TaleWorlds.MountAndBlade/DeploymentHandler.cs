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
	public class DeploymentHandler : MissionLogic
	{
		public Team team
		{
			get
			{
				return base.Mission.PlayerTeam;
			}
		}

		public bool IsPlayerAttacker
		{
			get
			{
				return this.isPlayerAttacker;
			}
		}

		public DeploymentHandler(bool isPlayerAttacker)
		{
			this.isPlayerAttacker = isPlayerAttacker;
		}

		public override void EarlyStart()
		{
			BattleSideEnum battleSideEnum = (this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			this.SetDeploymentBoundary(battleSideEnum);
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.previousMissionMode = base.Mission.Mode;
			base.Mission.SetMissionMode(MissionMode.Deployment, true);
			this.team.OnOrderIssued += this.OrderController_OnOrderIssued;
		}

		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
		}

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
				Debug.FailedAssert("will be removed", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "OrderController_OnOrderIssued_Aux", 182);
				return;
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "OrderController_OnOrderIssued_Aux", 185);
		}

		public void ForceUpdateAllUnits()
		{
			this.OrderController_OnOrderIssued(OrderType.Move, this.team.FormationsIncludingSpecialAndEmpty, Array.Empty<object>());
		}

		public virtual void FinishDeployment()
		{
		}

		public override void OnRemoveBehavior()
		{
			if (this.team != null)
			{
				this.team.OnOrderIssued -= this.OrderController_OnOrderIssued;
			}
			base.Mission.SetMissionMode(this.previousMissionMode, false);
			base.OnRemoveBehavior();
		}

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
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\DeploymentHandler.cs", "SetDeploymentBoundary", 237);
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

		protected MissionMode previousMissionMode;

		protected readonly bool isPlayerAttacker;

		private const string BoundaryTagExpression = "deployment_castle_boundary(_\\d+)*";

		private bool areDeploymentPointsInitialized;
	}
}
