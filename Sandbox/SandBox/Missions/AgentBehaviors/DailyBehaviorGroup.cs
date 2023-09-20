using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x0200006F RID: 111
	public class DailyBehaviorGroup : AgentBehaviorGroup
	{
		// Token: 0x060004CE RID: 1230 RVA: 0x00022528 File Offset: 0x00020728
		public DailyBehaviorGroup(AgentNavigator navigator, Mission mission)
			: base(navigator, mission)
		{
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00022534 File Offset: 0x00020734
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

		// Token: 0x060004D0 RID: 1232 RVA: 0x0002259C File Offset: 0x0002079C
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

		// Token: 0x060004D1 RID: 1233 RVA: 0x000225F8 File Offset: 0x000207F8
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

		// Token: 0x060004D2 RID: 1234 RVA: 0x000226D4 File Offset: 0x000208D4
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

		// Token: 0x060004D3 RID: 1235 RVA: 0x00022730 File Offset: 0x00020930
		private void SetCheckBehaviorTimer(float time)
		{
			if (this.CheckBehaviorTimer == null)
			{
				this.CheckBehaviorTimer = new Timer(base.Mission.CurrentTime, time, true);
				return;
			}
			this.CheckBehaviorTimer.Reset(base.Mission.CurrentTime, time);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0002276A File Offset: 0x0002096A
		public override float GetScore(bool isSimulation)
		{
			return 0.5f;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x00022774 File Offset: 0x00020974
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

		// Token: 0x060004D6 RID: 1238 RVA: 0x000227D0 File Offset: 0x000209D0
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

		// Token: 0x060004D7 RID: 1239 RVA: 0x0002285A File Offset: 0x00020A5A
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.CheckBehaviorTimer = null;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00022869 File Offset: 0x00020A69
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
