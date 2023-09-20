using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentControllers
{
	// Token: 0x02000067 RID: 103
	public class AmbushBattleAgentController : AgentController
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000207D9 File Offset: 0x0001E9D9
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x000207BC File Offset: 0x0001E9BC
		public bool Aggressive
		{
			get
			{
				return this._aggressive;
			}
			set
			{
				this._aggressive = value;
				if (this._aggressive)
				{
					base.Owner.SetWatchState(2);
				}
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x000207E1 File Offset: 0x0001E9E1
		public override void OnInitialize()
		{
			this.Aggressive = false;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x000207EA File Offset: 0x0001E9EA
		public bool CheckArrivedAtWayPoint(GameEntity waypoint)
		{
			return waypoint.CheckPointWithOrientedBoundingBox(base.Owner.Position);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x000207FD File Offset: 0x0001E9FD
		public void UpdateState()
		{
			if (!this.IsAttacker)
			{
				this.UpdateDefendingAIAgentState();
				return;
			}
			this.UpdateAttackingAIAgentState();
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00020814 File Offset: 0x0001EA14
		private void UpdateDefendingAIAgentState()
		{
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00020818 File Offset: 0x0001EA18
		private void UpdateAttackingAIAgentState()
		{
			if (this._agentState == AmbushBattleAgentController.AgentState.MovingToBoulder || this._agentState == AmbushBattleAgentController.AgentState.SearchingForBoulder)
			{
				if (base.Owner.Character != Game.Current.PlayerTroop && !base.Owner.Character.IsPlayerCharacter && this._agentState != AmbushBattleAgentController.AgentState.SearchingForBoulder)
				{
					Vec3 origin = base.Owner.AgentVisuals.GetGlobalFrame().origin;
					Vec3 globalPosition = this.BoulderTarget.GlobalPosition;
					if (origin.DistanceSquared(globalPosition) < 0.16000001f)
					{
						MBDebug.Print("Picking up a boulder", 0, 12, 17592186044416UL);
						this._agentState = AmbushBattleAgentController.AgentState.PickingUpBoulder;
						base.Owner.DisableScriptedMovement();
						MatrixFrame globalFrame = this.BoulderTarget.GetGlobalFrame();
						Vec2 asVec = globalFrame.origin.AsVec2;
						base.Owner.SetTargetPositionAndDirectionSynched(ref asVec, ref globalFrame.rotation.f);
					}
				}
			}
			else if (this._agentState == AmbushBattleAgentController.AgentState.PickingUpBoulder)
			{
				this.PickUpBoulderWithAnimation();
			}
			if (this._agentState == AmbushBattleAgentController.AgentState.MovingBackToSpawn)
			{
				base.Owner.DisableScriptedMovement();
				this._agentState = AmbushBattleAgentController.AgentState.None;
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0002092C File Offset: 0x0001EB2C
		private void PickUpBoulderWithAnimation()
		{
			ActionIndexValueCache currentActionValue = base.Owner.GetCurrentActionValue(0);
			if (!this._boulderAddedToEquip && currentActionValue != this.act_pickup_boulder_begin)
			{
				base.Owner.SetActionChannel(0, this.act_pickup_boulder_begin, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				return;
			}
			if (!this._boulderAddedToEquip && currentActionValue == this.act_pickup_boulder_begin)
			{
				if (base.Owner.GetCurrentActionProgress(0) >= 0.7f)
				{
					this._boulderAddedToEquip = true;
					return;
				}
			}
			else if (!base.Owner.IsMainAgent && this._agentState == AmbushBattleAgentController.AgentState.PickingUpBoulder && currentActionValue != this.act_pickup_boulder_end && currentActionValue != this.act_pickup_boulder_begin)
			{
				base.Owner.ClearTargetFrame();
				if (!this.Aggressive)
				{
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, this.OriginalSpawnFrame.origin, false);
					base.Owner.SetScriptedPosition(ref worldPosition, false, 0);
					this._agentState = AmbushBattleAgentController.AgentState.MovingBackToSpawn;
					return;
				}
				this._agentState = AmbushBattleAgentController.AgentState.None;
			}
		}

		// Token: 0x0400021E RID: 542
		private readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		// Token: 0x0400021F RID: 543
		private readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		// Token: 0x04000220 RID: 544
		public bool IsAttacker;

		// Token: 0x04000221 RID: 545
		private bool _aggressive;

		// Token: 0x04000222 RID: 546
		public bool IsLeader;

		// Token: 0x04000223 RID: 547
		public GameEntity BoulderTarget;

		// Token: 0x04000224 RID: 548
		public bool HasBeenPlaced;

		// Token: 0x04000225 RID: 549
		public MatrixFrame OriginalSpawnFrame;

		// Token: 0x04000226 RID: 550
		private bool _boulderAddedToEquip;

		// Token: 0x04000227 RID: 551
		private AmbushBattleAgentController.AgentState _agentState = AmbushBattleAgentController.AgentState.SearchingForBoulder;

		// Token: 0x02000138 RID: 312
		private enum AgentState
		{
			// Token: 0x040005D1 RID: 1489
			None,
			// Token: 0x040005D2 RID: 1490
			SearchingForBoulder,
			// Token: 0x040005D3 RID: 1491
			MovingToBoulder,
			// Token: 0x040005D4 RID: 1492
			PickingUpBoulder,
			// Token: 0x040005D5 RID: 1493
			MovingBackToSpawn
		}
	}
}
