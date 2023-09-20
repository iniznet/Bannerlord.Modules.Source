using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MissionGamepadHapticEffectsHandler : MissionLogic
	{
		public override void OnMissionStateActivated()
		{
			base.OnMissionStateActivated();
			this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.Off);
		}

		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this.SetTriggerFeedback(0, 0, 0, 0);
		}

		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			Agent mainAgent = base.Mission.MainAgent;
			if (mainAgent == null || mainAgent.State != AgentState.Active || !mainAgent.CombatActionsEnabled || mainAgent.IsCheering || base.Mission.IsOrderMenuOpen)
			{
				this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.Off);
				return;
			}
			MissionWeapon wieldedWeapon = mainAgent.WieldedWeapon;
			bool flag = wieldedWeapon.CurrentUsageItem != null && wieldedWeapon.CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.StringHeldByHand);
			bool flag2 = wieldedWeapon.CurrentUsageItem != null && wieldedWeapon.CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.HasString) && !wieldedWeapon.CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.StringHeldByHand);
			if (flag)
			{
				Agent.ActionStage actionStage = ((mainAgent != null) ? mainAgent.GetCurrentActionStage(1) : Agent.ActionStage.None);
				if (actionStage == Agent.ActionStage.None || actionStage == Agent.ActionStage.ReloadMidPhase || actionStage == Agent.ActionStage.ReloadLastPhase)
				{
					this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.SoftFeedback);
					return;
				}
				if (actionStage == Agent.ActionStage.AttackReady)
				{
					float aimingTimer = mainAgent.GetAimingTimer();
					float num = MBMath.Lerp(0f, 1f, MBMath.ClampFloat((aimingTimer - mainAgent.AgentDrivenProperties.WeaponUnsteadyBeginTime) / (mainAgent.AgentDrivenProperties.WeaponUnsteadyEndTime - mainAgent.AgentDrivenProperties.WeaponUnsteadyBeginTime), 0f, 1f), 1E-05f);
					if (num > 0f)
					{
						MathF.Pow(num, 2.7182817f);
						float[] array = new float[] { num };
						float num2 = MBMath.ClampFloat(1f - num, 0.1f, 1f);
						float[] array2 = new float[] { num2 };
						float[] array3 = new float[] { 0.05f };
						this.SetTriggerVibration(null, null, null, 0, array, array2, array3, array3.Length);
						this._triggerState = MissionGamepadHapticEffectsHandler.TriggerState.Vibration;
						return;
					}
					this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.HardFeedback);
					return;
				}
				else
				{
					if (actionStage == Agent.ActionStage.AttackRelease)
					{
						this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.OffWithRumble);
						return;
					}
					this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.Off);
					return;
				}
			}
			else
			{
				if (!flag2)
				{
					this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.Off);
					return;
				}
				if (((mainAgent != null) ? mainAgent.GetCurrentActionStage(1) : Agent.ActionStage.None) == Agent.ActionStage.AttackRelease)
				{
					this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.OffWithRumble);
					return;
				}
				this.SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState.SoftFeedback);
				return;
			}
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			base.OnAgentBuild(agent, banner);
			if (agent.IsMainAgent)
			{
				agent.OnAgentHealthChanged += this.MainAgentHealthChange;
				agent.OnMountHealthChanged += this.MainAgentsMountHealthChange;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent.IsMainAgent)
			{
				this.SetTriggerFeedback(0, 0, 0, 0);
				affectedAgent.OnAgentHealthChanged -= this.MainAgentHealthChange;
				affectedAgent.OnMountHealthChanged -= this.MainAgentsMountHealthChange;
			}
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.SetTriggerFeedback(0, 0, 0, 0);
		}

		private void MainAgentHealthChange(Agent agent, float oldHealth, float newHealth)
		{
			float num = oldHealth - newHealth;
			if (num > 0.001f)
			{
				this.SetRumbleEffectBasedOnDamage(num);
			}
		}

		private void MainAgentsMountHealthChange(Agent agent, Agent mount, float oldHealth, float newHealth)
		{
			float num = oldHealth - newHealth;
			if (num > 0.001f)
			{
				this.SetRumbleEffectBasedOnDamage(num);
			}
		}

		private void SetTriggerState(MissionGamepadHapticEffectsHandler.TriggerState triggerState)
		{
			if (this._triggerState != triggerState)
			{
				switch (triggerState)
				{
				case MissionGamepadHapticEffectsHandler.TriggerState.Off:
					this.SetTriggerFeedback(0, 0, 0, 0);
					break;
				case MissionGamepadHapticEffectsHandler.TriggerState.OffWithRumble:
				{
					this.SetTriggerFeedback(0, 0, 0, 0);
					float[] array = new float[] { 0.5f, 0.8f, 0.5f };
					float[] array2 = new float[] { 0.1f, 0.2f, 0.1f };
					float[] array3 = new float[] { 0.1f, 0.1f, 0.1f };
					float[] array4 = new float[] { 0.1f, 0.1f, 0.1f };
					this.SetRumbleEffect(array, array3, array.Length, array2, array4, array2.Length);
					break;
				}
				case MissionGamepadHapticEffectsHandler.TriggerState.SoftFeedback:
					this.SetTriggerFeedback(0, 0, 0, 1);
					break;
				case MissionGamepadHapticEffectsHandler.TriggerState.HardFeedback:
					this.SetTriggerFeedback(0, 0, 0, 4);
					break;
				case MissionGamepadHapticEffectsHandler.TriggerState.Weapon:
					this.SetTriggerWeaponEffect(0, 0, 0, 4, 7, 7);
					break;
				default:
					Debug.FailedAssert("Unexpected trigger state:" + triggerState, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\MissionGamepadHapticEffectsHandler.cs", "SetTriggerState", 204);
					break;
				}
				this._triggerState = triggerState;
			}
		}

		private void SetRumbleEffectBasedOnDamage(float damage)
		{
			damage = MathF.Min(0.01f * damage, 0.5f);
			float[] array = new float[]
			{
				damage + 0.1f,
				damage + 0.2f,
				damage + 0.3f,
				damage + 0.2f,
				damage + 0.1f
			};
			float[] array2 = new float[5];
			array2[1] = damage + 0.3f;
			array2[2] = damage + 0.5f;
			array2[3] = damage + 0.3f;
			float[] array3 = array2;
			float[] array4 = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
			float[] array5 = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
			this.SetRumbleEffect(array, array4, array.Length, array3, array5, array3.Length);
		}

		private void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements)
		{
			EngineApplicationInterface.IInput.SetRumbleEffect(lowFrequencyLevels, lowFrequencyDurations, numLowFrequencyElements, highFrequencyLevels, highFrequencyDurations, numHighFrequencyElements);
		}

		private void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
			EngineApplicationInterface.IInput.SetTriggerFeedback(leftTriggerPosition, leftTriggerStrength, rightTriggerPosition, rightTriggerStrength);
		}

		private void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
			EngineApplicationInterface.IInput.SetTriggerWeaponEffect(leftStartPosition, leftEnd_position, leftStrength, rightStartPosition, rightEndPosition, rightStrength);
		}

		private void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
			EngineApplicationInterface.IInput.SetTriggerVibration(leftTriggerAmplitudes, leftTriggerFrequencies, leftTriggerDurations, numLeftTriggerElements, rightTriggerAmplitudes, rightTriggerFrequencies, rightTriggerDurations, numRightTriggerElements);
		}

		private MissionGamepadHapticEffectsHandler.TriggerState _triggerState;

		private enum TriggerState
		{
			Off,
			OffWithRumble,
			SoftFeedback,
			HardFeedback,
			Weapon,
			Vibration
		}
	}
}
