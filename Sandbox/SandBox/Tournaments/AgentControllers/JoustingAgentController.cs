using System;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.AgentControllers
{
	// Token: 0x02000020 RID: 32
	public class JoustingAgentController : AgentController
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000B119 File Offset: 0x00009319
		// (set) Token: 0x0600017E RID: 382 RVA: 0x0000B121 File Offset: 0x00009321
		public JoustingAgentController.JoustingAgentState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (value != this._state)
				{
					this._state = value;
					this.JoustingMissionController.OnJoustingAgentStateChanged(base.Owner, value);
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000B145 File Offset: 0x00009345
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000B14D File Offset: 0x0000934D
		public TournamentJoustingMissionController JoustingMissionController { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000B158 File Offset: 0x00009358
		public Agent Opponent
		{
			get
			{
				if (this._opponentAgent == null)
				{
					foreach (Agent agent in base.Mission.Agents)
					{
						if (agent.IsHuman && agent != base.Owner)
						{
							this._opponentAgent = agent;
						}
					}
				}
				return this._opponentAgent;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000B1D0 File Offset: 0x000093D0
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000B1D8 File Offset: 0x000093D8
		public bool PrepareEquipmentsAfterDismount { get; private set; }

		// Token: 0x06000184 RID: 388 RVA: 0x0000B1E1 File Offset: 0x000093E1
		public override void OnInitialize()
		{
			this.JoustingMissionController = base.Mission.GetMissionBehavior<TournamentJoustingMissionController>();
			this._state = JoustingAgentController.JoustingAgentState.WaitingOpponent;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000B1FB File Offset: 0x000093FB
		public void UpdateState()
		{
			if (base.Owner.Character.IsPlayerCharacter)
			{
				this.UpdateMainAgentState();
				return;
			}
			this.UpdateAIAgentState();
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000B21C File Offset: 0x0000941C
		private void UpdateMainAgentState()
		{
			JoustingAgentController controller = this.Opponent.GetController<JoustingAgentController>();
			bool flag = this.JoustingMissionController.CornerStartList[this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position) && !this.JoustingMissionController.RegionBoxList[this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position);
			switch (this.State)
			{
			case JoustingAgentController.JoustingAgentState.GoToStartPosition:
				if (flag)
				{
					this.State = JoustingAgentController.JoustingAgentState.WaitInStartPosition;
					return;
				}
				break;
			case JoustingAgentController.JoustingAgentState.WaitInStartPosition:
				if (!flag)
				{
					this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
					return;
				}
				if (base.Owner.GetCurrentVelocity().LengthSquared < 0.0025000002f)
				{
					this.State = JoustingAgentController.JoustingAgentState.WaitingOpponent;
					return;
				}
				break;
			case JoustingAgentController.JoustingAgentState.WaitingOpponent:
				if (!flag)
				{
					this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
					return;
				}
				if (controller.State == JoustingAgentController.JoustingAgentState.WaitingOpponent || controller.State == JoustingAgentController.JoustingAgentState.Ready)
				{
					this.State = JoustingAgentController.JoustingAgentState.Ready;
					return;
				}
				break;
			case JoustingAgentController.JoustingAgentState.Ready:
				if (this.JoustingMissionController.IsAgentInTheTrack(base.Owner, true) && base.Owner.GetCurrentVelocity().LengthSquared > 0.0025000002f)
				{
					this.State = JoustingAgentController.JoustingAgentState.Riding;
					return;
				}
				if (controller.State == JoustingAgentController.JoustingAgentState.GoToStartPosition)
				{
					this.State = JoustingAgentController.JoustingAgentState.WaitingOpponent;
					return;
				}
				if (!this.JoustingMissionController.CornerStartList[this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position))
				{
					this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
					return;
				}
				break;
			case JoustingAgentController.JoustingAgentState.StartRiding:
				break;
			case JoustingAgentController.JoustingAgentState.Riding:
				if (this.JoustingMissionController.IsAgentInTheTrack(base.Owner, false))
				{
					this.State = JoustingAgentController.JoustingAgentState.RidingAtWrongSide;
				}
				if (this.JoustingMissionController.RegionExitBoxList[this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position))
				{
					this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
					this.CurrentCornerIndex = 1 - this.CurrentCornerIndex;
					return;
				}
				break;
			case JoustingAgentController.JoustingAgentState.RidingAtWrongSide:
				if (this.JoustingMissionController.IsAgentInTheTrack(base.Owner, true))
				{
					this.State = JoustingAgentController.JoustingAgentState.Riding;
					return;
				}
				if (this.JoustingMissionController.CornerStartList[1 - this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position))
				{
					this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
					this.CurrentCornerIndex = 1 - this.CurrentCornerIndex;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000B44C File Offset: 0x0000964C
		private void UpdateAIAgentState()
		{
			if (this.Opponent != null && this.Opponent.IsActive())
			{
				JoustingAgentController controller = this.Opponent.GetController<JoustingAgentController>();
				switch (this.State)
				{
				case JoustingAgentController.JoustingAgentState.GoingToBackStart:
					if (base.Owner.Position.Distance(this.JoustingMissionController.CornerBackStartList[this.CurrentCornerIndex].origin) < 3f && base.Owner.GetCurrentVelocity().LengthSquared < 0.0025000002f)
					{
						this.CurrentCornerIndex = 1 - this.CurrentCornerIndex;
						MatrixFrame globalFrame = this.JoustingMissionController.CornerStartList[this.CurrentCornerIndex].GetGlobalFrame();
						WorldPosition worldPosition;
						worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, globalFrame.origin, false);
						base.Owner.SetScriptedPositionAndDirection(ref worldPosition, globalFrame.rotation.f.AsVec2.RotationInRadians, false, 0);
						this.State = JoustingAgentController.JoustingAgentState.GoToStartPosition;
						return;
					}
					break;
				case JoustingAgentController.JoustingAgentState.GoToStartPosition:
					if (this.JoustingMissionController.CornerStartList[this.CurrentCornerIndex].CheckPointWithOrientedBoundingBox(base.Owner.Position) && base.Owner.GetCurrentVelocity().LengthSquared < 0.0025000002f)
					{
						this.State = JoustingAgentController.JoustingAgentState.WaitingOpponent;
						return;
					}
					break;
				case JoustingAgentController.JoustingAgentState.WaitInStartPosition:
					break;
				case JoustingAgentController.JoustingAgentState.WaitingOpponent:
					if (controller.State == JoustingAgentController.JoustingAgentState.WaitingOpponent || controller.State == JoustingAgentController.JoustingAgentState.Ready)
					{
						this.State = JoustingAgentController.JoustingAgentState.Ready;
						return;
					}
					break;
				case JoustingAgentController.JoustingAgentState.Ready:
					if (controller.State == JoustingAgentController.JoustingAgentState.Riding)
					{
						this.State = JoustingAgentController.JoustingAgentState.StartRiding;
						WorldPosition worldPosition2;
						worldPosition2..ctor(Mission.Current.Scene, UIntPtr.Zero, this.JoustingMissionController.CornerMiddleList[this.CurrentCornerIndex].origin, false);
						base.Owner.SetScriptedPosition(ref worldPosition2, false, 8);
						return;
					}
					if (controller.State == JoustingAgentController.JoustingAgentState.Ready)
					{
						WorldPosition worldPosition3;
						worldPosition3..ctor(Mission.Current.Scene, UIntPtr.Zero, this.JoustingMissionController.CornerStartList[this.CurrentCornerIndex].GetGlobalFrame().origin, false);
						base.Owner.SetScriptedPosition(ref worldPosition3, false, 8);
						return;
					}
					this.State = JoustingAgentController.JoustingAgentState.WaitingOpponent;
					return;
				case JoustingAgentController.JoustingAgentState.StartRiding:
					if (base.Owner.Position.Distance(this.JoustingMissionController.CornerMiddleList[this.CurrentCornerIndex].origin) < 3f)
					{
						WorldPosition worldPosition4;
						worldPosition4..ctor(Mission.Current.Scene, UIntPtr.Zero, this.JoustingMissionController.CornerFinishList[this.CurrentCornerIndex].origin, false);
						base.Owner.SetScriptedPosition(ref worldPosition4, false, 8);
						this.State = JoustingAgentController.JoustingAgentState.Riding;
						return;
					}
					break;
				case JoustingAgentController.JoustingAgentState.Riding:
					if (base.Owner.Position.Distance(this.JoustingMissionController.CornerFinishList[this.CurrentCornerIndex].origin) < 3f)
					{
						WorldPosition worldPosition5;
						worldPosition5..ctor(Mission.Current.Scene, UIntPtr.Zero, this.JoustingMissionController.CornerBackStartList[this.CurrentCornerIndex].origin, false);
						base.Owner.SetScriptedPosition(ref worldPosition5, false, 0);
						this.State = JoustingAgentController.JoustingAgentState.GoingToBackStart;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000B790 File Offset: 0x00009990
		public void PrepareAgentToSwordDuel()
		{
			if (base.Owner.MountAgent != null)
			{
				base.Owner.Controller = 1;
				WorldPosition worldPosition = this.Opponent.GetWorldPosition();
				base.Owner.SetScriptedPosition(ref worldPosition, false, 32);
				this.PrepareEquipmentsAfterDismount = true;
				return;
			}
			this.PrepareEquipmentsForSwordDuel();
			base.Owner.DisableScriptedMovement();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000B7EB File Offset: 0x000099EB
		public void PrepareEquipmentsForSwordDuel()
		{
			this.AddEquipmentsForSwordDuel();
			base.Owner.WieldInitialWeapons(2);
			this.PrepareEquipmentsAfterDismount = false;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000B808 File Offset: 0x00009A08
		private void AddEquipmentsForSwordDuel()
		{
			base.Owner.DropItem(0, 0);
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>("wooden_sword_t1");
			ItemModifier itemModifier = null;
			IAgentOriginBase origin = base.Owner.Origin;
			MissionWeapon missionWeapon;
			missionWeapon..ctor(@object, itemModifier, (origin != null) ? origin.Banner : null);
			base.Owner.EquipWeaponWithNewEntity(2, ref missionWeapon);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000B863 File Offset: 0x00009A63
		public bool IsRiding()
		{
			return this.State == JoustingAgentController.JoustingAgentState.StartRiding || this.State == JoustingAgentController.JoustingAgentState.Riding;
		}

		// Token: 0x0400009F RID: 159
		private JoustingAgentController.JoustingAgentState _state;

		// Token: 0x040000A1 RID: 161
		public int CurrentCornerIndex;

		// Token: 0x040000A2 RID: 162
		private const float MaxDistance = 3f;

		// Token: 0x040000A3 RID: 163
		public int Score;

		// Token: 0x040000A4 RID: 164
		private Agent _opponentAgent;

		// Token: 0x02000101 RID: 257
		public enum JoustingAgentState
		{
			// Token: 0x040004FD RID: 1277
			GoingToBackStart,
			// Token: 0x040004FE RID: 1278
			GoToStartPosition,
			// Token: 0x040004FF RID: 1279
			WaitInStartPosition,
			// Token: 0x04000500 RID: 1280
			WaitingOpponent,
			// Token: 0x04000501 RID: 1281
			Ready,
			// Token: 0x04000502 RID: 1282
			StartRiding,
			// Token: 0x04000503 RID: 1283
			Riding,
			// Token: 0x04000504 RID: 1284
			RidingAtWrongSide,
			// Token: 0x04000505 RID: 1285
			SwordDuel
		}
	}
}
