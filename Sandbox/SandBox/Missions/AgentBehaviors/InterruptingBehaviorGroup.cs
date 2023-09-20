using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000074 RID: 116
	public class InterruptingBehaviorGroup : AgentBehaviorGroup
	{
		// Token: 0x0600050E RID: 1294 RVA: 0x000247F1 File Offset: 0x000229F1
		public InterruptingBehaviorGroup(AgentNavigator navigator, Mission mission)
			: base(navigator, mission)
		{
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x000247FC File Offset: 0x000229FC
		public override void Tick(float dt, bool isSimulation)
		{
			if (base.ScriptedBehavior != null)
			{
				if (!base.ScriptedBehavior.IsActive)
				{
					base.DisableAllBehaviors();
					base.ScriptedBehavior.IsActive = true;
				}
			}
			else
			{
				int bestBehaviorIndex = this.GetBestBehaviorIndex(isSimulation);
				if (bestBehaviorIndex != -1 && !this.Behaviors[bestBehaviorIndex].IsActive)
				{
					base.DisableAllBehaviors();
					this.Behaviors[bestBehaviorIndex].IsActive = true;
				}
			}
			this.TickActiveBehaviors(dt, isSimulation);
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x00024874 File Offset: 0x00022A74
		private void TickActiveBehaviors(float dt, bool isSimulation)
		{
			for (int i = this.Behaviors.Count - 1; i >= 0; i--)
			{
				AgentBehavior agentBehavior = this.Behaviors[i];
				if (agentBehavior.IsActive)
				{
					agentBehavior.Tick(dt, isSimulation);
				}
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x000248B6 File Offset: 0x00022AB6
		public override float GetScore(bool isSimulation)
		{
			if (this.GetBestBehaviorIndex(isSimulation) != -1)
			{
				return 0.75f;
			}
			return 0f;
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x000248D0 File Offset: 0x00022AD0
		private int GetBestBehaviorIndex(bool isSimulation)
		{
			float num = 0f;
			int num2 = -1;
			for (int i = 0; i < this.Behaviors.Count; i++)
			{
				float availability = this.Behaviors[i].GetAvailability(isSimulation);
				if (availability > num)
				{
					num = availability;
					num2 = i;
				}
			}
			return num2;
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x00024917 File Offset: 0x00022B17
		public override void ForceThink(float inSeconds)
		{
			this.Navigator.RefreshBehaviorGroups(false);
		}
	}
}
