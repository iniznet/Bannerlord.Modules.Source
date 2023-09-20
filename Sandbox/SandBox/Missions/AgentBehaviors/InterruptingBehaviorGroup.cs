using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class InterruptingBehaviorGroup : AgentBehaviorGroup
	{
		public InterruptingBehaviorGroup(AgentNavigator navigator, Mission mission)
			: base(navigator, mission)
		{
		}

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

		public override float GetScore(bool isSimulation)
		{
			if (this.GetBestBehaviorIndex(isSimulation) != -1)
			{
				return 0.75f;
			}
			return 0f;
		}

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

		public override void ForceThink(float inSeconds)
		{
			this.Navigator.RefreshBehaviorGroups(false);
		}
	}
}
