using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentControllers
{
	public class AmbushBattleAgentController : AgentController
	{
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

		public override void OnInitialize()
		{
			this.Aggressive = false;
		}

		public bool CheckArrivedAtWayPoint(GameEntity waypoint)
		{
			return waypoint.CheckPointWithOrientedBoundingBox(base.Owner.Position);
		}

		public void UpdateState()
		{
			if (!this.IsAttacker)
			{
				this.UpdateDefendingAIAgentState();
				return;
			}
			this.UpdateAttackingAIAgentState();
		}

		private void UpdateDefendingAIAgentState()
		{
		}

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

		private readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		private readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		public bool IsAttacker;

		private bool _aggressive;

		public bool IsLeader;

		public GameEntity BoulderTarget;

		public bool HasBeenPlaced;

		public MatrixFrame OriginalSpawnFrame;

		private bool _boulderAddedToEquip;

		private AmbushBattleAgentController.AgentState _agentState = AmbushBattleAgentController.AgentState.SearchingForBoulder;

		private enum AgentState
		{
			None,
			SearchingForBoulder,
			MovingToBoulder,
			PickingUpBoulder,
			MovingBackToSpawn
		}
	}
}
