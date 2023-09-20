using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class DailyBehaviorGroup : AgentBehaviorGroup
	{
		public DailyBehaviorGroup(AgentNavigator navigator, Mission mission)
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
			else if (this.CheckBehaviorTimer == null || this.CheckBehaviorTimer.Check(base.Mission.CurrentTime))
			{
				this.Think(isSimulation);
			}
			this.TickActiveBehaviors(dt, isSimulation);
		}

		public override void ConversationTick()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					agentBehavior.ConversationTick();
				}
			}
		}

		private void Think(bool isSimulation)
		{
			float num = 0f;
			float[] array = new float[this.Behaviors.Count];
			for (int i = 0; i < this.Behaviors.Count; i++)
			{
				array[i] = this.Behaviors[i].GetAvailability(isSimulation);
				num += array[i];
			}
			if (num > 0f)
			{
				float num2 = MBRandom.RandomFloat * num;
				int j = 0;
				while (j < array.Length)
				{
					float num3 = array[j];
					num2 -= num3;
					if (num2 < 0f)
					{
						if (!this.Behaviors[j].IsActive)
						{
							base.DisableAllBehaviors();
							this.Behaviors[j].IsActive = true;
							this.CheckBehaviorTime = this.Behaviors[j].CheckTime;
							this.SetCheckBehaviorTimer(this.CheckBehaviorTime);
							return;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
		}

		private void TickActiveBehaviors(float dt, bool isSimulation)
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					agentBehavior.Tick(dt, isSimulation);
				}
			}
		}

		private void SetCheckBehaviorTimer(float time)
		{
			if (this.CheckBehaviorTimer == null)
			{
				this.CheckBehaviorTimer = new Timer(base.Mission.CurrentTime, time, true);
				return;
			}
			this.CheckBehaviorTimer.Reset(base.Mission.CurrentTime, time);
		}

		public override float GetScore(bool isSimulation)
		{
			return 0.5f;
		}

		public override void OnAgentRemoved(Agent agent)
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					agentBehavior.OnAgentRemoved(agent);
				}
			}
		}

		protected override void OnActivate()
		{
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.OwnerAgent.Origin);
			if (locationCharacter != null && locationCharacter.ActionSetCode != locationCharacter.AlarmedActionSetCode)
			{
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(locationCharacter.GetAgentBuildData().AgentMonster, MBGlobals.GetActionSet(locationCharacter.ActionSetCode), locationCharacter.Character.GetStepSize(), false);
				base.OwnerAgent.SetActionSet(ref animationSystemData);
			}
			this.Navigator.SetItemsVisibility(true);
			this.Navigator.SetSpecialItem();
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.CheckBehaviorTimer = null;
		}

		public override void ForceThink(float inSeconds)
		{
			if (MathF.Abs(inSeconds) < 1E-45f)
			{
				this.Think(false);
				return;
			}
			this.SetCheckBehaviorTimer(inSeconds);
		}
	}
}
