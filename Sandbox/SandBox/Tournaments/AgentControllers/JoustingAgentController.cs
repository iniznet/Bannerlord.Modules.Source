using System;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.AgentControllers
{
	public class JoustingAgentController : AgentController
	{
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

		public TournamentJoustingMissionController JoustingMissionController { get; private set; }

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

		public bool PrepareEquipmentsAfterDismount { get; private set; }

		public override void OnInitialize()
		{
			this.JoustingMissionController = base.Mission.GetMissionBehavior<TournamentJoustingMissionController>();
			this._state = JoustingAgentController.JoustingAgentState.WaitingOpponent;
		}

		public void UpdateState()
		{
			if (base.Owner.Character.IsPlayerCharacter)
			{
				this.UpdateMainAgentState();
				return;
			}
			this.UpdateAIAgentState();
		}

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

		public void PrepareEquipmentsForSwordDuel()
		{
			this.AddEquipmentsForSwordDuel();
			base.Owner.WieldInitialWeapons(2, 0);
			this.PrepareEquipmentsAfterDismount = false;
		}

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

		public bool IsRiding()
		{
			return this.State == JoustingAgentController.JoustingAgentState.StartRiding || this.State == JoustingAgentController.JoustingAgentState.Riding;
		}

		private JoustingAgentController.JoustingAgentState _state;

		public int CurrentCornerIndex;

		private const float MaxDistance = 3f;

		public int Score;

		private Agent _opponentAgent;

		public enum JoustingAgentState
		{
			GoingToBackStart,
			GoToStartPosition,
			WaitInStartPosition,
			WaitingOpponent,
			Ready,
			StartRiding,
			Riding,
			RidingAtWrongSide,
			SwordDuel
		}
	}
}
