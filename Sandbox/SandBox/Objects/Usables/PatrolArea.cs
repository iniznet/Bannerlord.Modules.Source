using System;
using SandBox.AI;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables
{
	// Token: 0x02000029 RID: 41
	public class PatrolArea : UsableMachine
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x0000CB43 File Offset: 0x0000AD43
		// (set) Token: 0x060001DA RID: 474 RVA: 0x0000CB4B File Offset: 0x0000AD4B
		private int ActiveIndex
		{
			get
			{
				return this._activeIndex;
			}
			set
			{
				if (this._activeIndex != value)
				{
					base.StandingPoints[value].IsDeactivated = false;
					base.StandingPoints[this._activeIndex].IsDeactivated = true;
					this._activeIndex = value;
				}
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000CB86 File Offset: 0x0000AD86
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.ActionMessage;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000CB99 File Offset: 0x0000AD99
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			StandingPoint pilotStandingPoint = base.PilotStandingPoint;
			if (pilotStandingPoint == null)
			{
				return null;
			}
			return pilotStandingPoint.DescriptionMessage.ToString();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000CBB1 File Offset: 0x0000ADB1
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000CBBC File Offset: 0x0000ADBC
		protected override void OnInit()
		{
			base.OnInit();
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.IsDeactivated = true;
			}
			this.ActiveIndex = base.StandingPoints.Count - 1;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000CC34 File Offset: 0x0000AE34
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000CC40 File Offset: 0x0000AE40
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (base.StandingPoints[this.ActiveIndex].HasAIUser)
			{
				this.ActiveIndex = ((this.ActiveIndex == 0) ? (base.StandingPoints.Count - 1) : (this.ActiveIndex - 1));
			}
		}

		// Token: 0x040000C3 RID: 195
		public int AreaIndex;

		// Token: 0x040000C4 RID: 196
		private int _activeIndex;
	}
}
