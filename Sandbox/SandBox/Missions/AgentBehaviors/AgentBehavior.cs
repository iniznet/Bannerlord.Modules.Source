using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x0200006A RID: 106
	public abstract class AgentBehavior
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x0002151D File Offset: 0x0001F71D
		public AgentNavigator Navigator
		{
			get
			{
				return this.BehaviorGroup.Navigator;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x0002152A File Offset: 0x0001F72A
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x00021532 File Offset: 0x0001F732
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
					if (this._isActive)
					{
						this.OnActivate();
						return;
					}
					this.OnDeactivate();
				}
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00021559 File Offset: 0x0001F759
		public Agent OwnerAgent
		{
			get
			{
				return this.Navigator.OwnerAgent;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00021566 File Offset: 0x0001F766
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x0002156E File Offset: 0x0001F76E
		public Mission Mission { get; private set; }

		// Token: 0x0600048D RID: 1165 RVA: 0x00021578 File Offset: 0x0001F778
		protected AgentBehavior(AgentBehaviorGroup behaviorGroup)
		{
			this.Mission = behaviorGroup.Mission;
			this.CheckTime = 40f + MBRandom.RandomFloat * 20f;
			this.BehaviorGroup = behaviorGroup;
			this._isActive = false;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x000215C7 File Offset: 0x0001F7C7
		public virtual float GetAvailability(bool isSimulation)
		{
			return 0f;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x000215CE File Offset: 0x0001F7CE
		public virtual void Tick(float dt, bool isSimulation)
		{
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x000215D0 File Offset: 0x0001F7D0
		public virtual void ConversationTick()
		{
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x000215D2 File Offset: 0x0001F7D2
		protected virtual void OnActivate()
		{
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x000215D4 File Offset: 0x0001F7D4
		protected virtual void OnDeactivate()
		{
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x000215D6 File Offset: 0x0001F7D6
		public virtual bool CheckStartWithBehavior()
		{
			return false;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x000215D9 File Offset: 0x0001F7D9
		public virtual void OnSpecialTargetChanged()
		{
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x000215DB File Offset: 0x0001F7DB
		public virtual void SetCustomWanderTarget(UsableMachine customUsableMachine)
		{
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x000215DD File Offset: 0x0001F7DD
		public virtual void OnAgentRemoved(Agent agent)
		{
		}

		// Token: 0x06000497 RID: 1175
		public abstract string GetDebugInfo();

		// Token: 0x04000233 RID: 563
		public float CheckTime = 15f;

		// Token: 0x04000234 RID: 564
		protected readonly AgentBehaviorGroup BehaviorGroup;

		// Token: 0x04000235 RID: 565
		private bool _isActive;
	}
}
