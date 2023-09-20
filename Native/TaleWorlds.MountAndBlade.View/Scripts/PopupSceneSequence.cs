using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x02000038 RID: 56
	public class PopupSceneSequence : ScriptComponentBehavior
	{
		// Token: 0x06000297 RID: 663 RVA: 0x00017B61 File Offset: 0x00015D61
		public void InitializeWithAgentVisuals(AgentVisuals visuals)
		{
			this._agentVisuals = visuals;
			this._time = 0f;
			this._triggered = false;
			this._state = 0;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00017B8B File Offset: 0x00015D8B
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00017B9F File Offset: 0x00015D9F
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00017BAC File Offset: 0x00015DAC
		protected override void OnTick(float dt)
		{
			this._time += dt;
			if (!this._triggered)
			{
				if (this._state == 0 && this._time >= this.InitialActivationTime)
				{
					this._triggered = true;
					this.OnInitialState();
				}
				if (this._state == 1 && this._time >= this.PositiveActivationTime)
				{
					this._triggered = true;
					this.OnPositiveState();
				}
				if (this._state == 2 && this._time >= this.NegativeActivationTime)
				{
					this._triggered = true;
					this.OnNegativeState();
				}
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00017C3A File Offset: 0x00015E3A
		public virtual void OnInitialState()
		{
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00017C3C File Offset: 0x00015E3C
		public virtual void OnPositiveState()
		{
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00017C3E File Offset: 0x00015E3E
		public virtual void OnNegativeState()
		{
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00017C40 File Offset: 0x00015E40
		public void SetInitialState()
		{
			this._triggered = false;
			this._state = 0;
			this._time = 0f;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00017C5B File Offset: 0x00015E5B
		public void SetPositiveState()
		{
			this._triggered = false;
			this._state = 1;
			this._time = 0f;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00017C76 File Offset: 0x00015E76
		public void SetNegativeState()
		{
			this._triggered = false;
			this._state = 2;
			this._time = 0f;
		}

		// Token: 0x040001BB RID: 443
		public float InitialActivationTime;

		// Token: 0x040001BC RID: 444
		public float PositiveActivationTime;

		// Token: 0x040001BD RID: 445
		public float NegativeActivationTime;

		// Token: 0x040001BE RID: 446
		protected AgentVisuals _agentVisuals;

		// Token: 0x040001BF RID: 447
		protected float _time;

		// Token: 0x040001C0 RID: 448
		protected bool _triggered;

		// Token: 0x040001C1 RID: 449
		protected int _state;
	}
}
