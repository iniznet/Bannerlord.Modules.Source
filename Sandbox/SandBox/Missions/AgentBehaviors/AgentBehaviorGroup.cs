using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x0200006B RID: 107
	public abstract class AgentBehaviorGroup
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x000215DF File Offset: 0x0001F7DF
		public Agent OwnerAgent
		{
			get
			{
				return this.Navigator.OwnerAgent;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x000215EC File Offset: 0x0001F7EC
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x000215F4 File Offset: 0x0001F7F4
		public AgentBehavior ScriptedBehavior { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x000215FD File Offset: 0x0001F7FD
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x00021605 File Offset: 0x0001F805
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

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x0002162C File Offset: 0x0001F82C
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x00021634 File Offset: 0x0001F834
		public Mission Mission { get; private set; }

		// Token: 0x0600049F RID: 1183 RVA: 0x0002163D File Offset: 0x0001F83D
		protected AgentBehaviorGroup(AgentNavigator navigator, Mission mission)
		{
			this.Mission = mission;
			this.Behaviors = new List<AgentBehavior>();
			this.Navigator = navigator;
			this._isActive = false;
			this.ScriptedBehavior = null;
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x00021678 File Offset: 0x0001F878
		public T AddBehavior<T>() where T : AgentBehavior
		{
			T t = Activator.CreateInstance(typeof(T), new object[] { this }) as T;
			if (t != null)
			{
				foreach (AgentBehavior agentBehavior in this.Behaviors)
				{
					if (agentBehavior.GetType() == t.GetType())
					{
						return agentBehavior as T;
					}
				}
				this.Behaviors.Add(t);
				return t;
			}
			return t;
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0002172C File Offset: 0x0001F92C
		public T GetBehavior<T>() where T : AgentBehavior
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior is T)
				{
					return (T)((object)agentBehavior);
				}
			}
			return default(T);
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00021794 File Offset: 0x0001F994
		public bool HasBehavior<T>() where T : AgentBehavior
		{
			using (List<AgentBehavior>.Enumerator enumerator = this.Behaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is T)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x000217F0 File Offset: 0x0001F9F0
		public void RemoveBehavior<T>() where T : AgentBehavior
		{
			for (int i = 0; i < this.Behaviors.Count; i++)
			{
				if (this.Behaviors[i] is T)
				{
					bool isActive = this.Behaviors[i].IsActive;
					this.Behaviors[i].IsActive = false;
					if (this.ScriptedBehavior == this.Behaviors[i])
					{
						this.ScriptedBehavior = null;
					}
					this.Behaviors.RemoveAt(i);
					if (isActive)
					{
						this.ForceThink(0f);
					}
				}
			}
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x00021880 File Offset: 0x0001FA80
		public void SetScriptedBehavior<T>() where T : AgentBehavior
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior is T)
				{
					this.ScriptedBehavior = agentBehavior;
					this.ForceThink(0f);
					break;
				}
			}
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x000218E8 File Offset: 0x0001FAE8
		public void DisableScriptedBehavior()
		{
			if (this.ScriptedBehavior != null)
			{
				this.ScriptedBehavior.IsActive = false;
				this.ScriptedBehavior = null;
				this.ForceThink(0f);
			}
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x00021910 File Offset: 0x0001FB10
		public void DisableAllBehaviors()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				agentBehavior.IsActive = false;
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00021964 File Offset: 0x0001FB64
		public AgentBehavior GetActiveBehavior()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					return agentBehavior;
				}
			}
			return null;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x000219C0 File Offset: 0x0001FBC0
		public virtual void Tick(float dt, bool isSimulation)
		{
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x000219C2 File Offset: 0x0001FBC2
		public virtual void ConversationTick()
		{
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x000219C4 File Offset: 0x0001FBC4
		public virtual void OnAgentRemoved(Agent agent)
		{
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x000219C6 File Offset: 0x0001FBC6
		protected virtual void OnActivate()
		{
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x000219C8 File Offset: 0x0001FBC8
		protected virtual void OnDeactivate()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				agentBehavior.IsActive = false;
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00021A1C File Offset: 0x0001FC1C
		public virtual float GetScore(bool isSimulation)
		{
			return 0f;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00021A23 File Offset: 0x0001FC23
		public virtual void ForceThink(float inSeconds)
		{
		}

		// Token: 0x04000237 RID: 567
		public AgentNavigator Navigator;

		// Token: 0x04000238 RID: 568
		public List<AgentBehavior> Behaviors;

		// Token: 0x04000239 RID: 569
		protected float CheckBehaviorTime = 5f;

		// Token: 0x0400023A RID: 570
		protected Timer CheckBehaviorTimer;

		// Token: 0x0400023C RID: 572
		private bool _isActive;
	}
}
