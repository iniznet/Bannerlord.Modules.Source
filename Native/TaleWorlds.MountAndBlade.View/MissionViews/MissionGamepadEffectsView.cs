using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	[DefaultView]
	public class MissionGamepadEffectsView : MissionView
	{
		public override void OnMissionStateActivated()
		{
			base.OnMissionStateActivated();
			this.ResetTriggerFeedback();
			this.ResetTriggerVibration();
			this._isAdaptiveTriggerEnabled = NativeOptions.GetConfig(15) != 0f;
			this._usingAlternativeAiming = NativeOptions.GetConfig(19) != 0f;
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		private void OnGamepadActiveStateChanged()
		{
			if (!TaleWorlds.InputSystem.Input.IsGamepadActive)
			{
				this.ResetTriggerFeedback();
				this.ResetTriggerVibration();
			}
		}

		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this.ResetTriggerFeedback();
			this.ResetTriggerVibration();
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			Agent mainAgent = base.Mission.MainAgent;
			if (this._isAdaptiveTriggerEnabled)
			{
				if (mainAgent != null && mainAgent.State == 1 && mainAgent.CombatActionsEnabled && !mainAgent.IsCheering && !base.Mission.IsOrderMenuOpen && this.IsMissionModeApplicableForAdaptiveTrigger(base.Mission.Mode))
				{
					MissionWeapon wieldedWeapon = mainAgent.WieldedWeapon;
					WeaponComponentData currentUsageItem = wieldedWeapon.CurrentUsageItem;
					bool flag = currentUsageItem != null && Extensions.HasAllFlags<WeaponFlags>(currentUsageItem.WeaponFlags, 3072L);
					WeaponComponentData currentUsageItem2 = wieldedWeapon.CurrentUsageItem;
					bool flag2 = currentUsageItem2 != null && Extensions.HasAllFlags<WeaponFlags>(currentUsageItem2.WeaponFlags, 1024L) && !Extensions.HasAllFlags<WeaponFlags>(wieldedWeapon.CurrentUsageItem.WeaponFlags, 3072L);
					WeaponComponentData currentUsageItem3 = wieldedWeapon.CurrentUsageItem;
					bool flag3 = currentUsageItem3 != null && currentUsageItem3.IsRangedWeapon && wieldedWeapon.CurrentUsageItem.IsConsumable;
					WeaponComponentData currentUsageItem4 = wieldedWeapon.CurrentUsageItem;
					bool flag4 = (currentUsageItem4 != null && Extensions.HasAllFlags<WeaponFlags>(currentUsageItem4.WeaponFlags, 1L)) || mainAgent.WieldedOffhandWeapon.IsShield();
					if (flag)
					{
						this.HandleBowAdaptiveTriggers();
						return;
					}
					if (flag2)
					{
						this.HandleCrossbowAdaptiveTriggers();
						return;
					}
					if (flag3)
					{
						this.HandleThrowableAdaptiveTriggers();
						return;
					}
					if (flag4)
					{
						this.HandleMeleeAdaptiveTriggers();
						return;
					}
					if (mainAgent.CurrentlyUsedGameObject == null)
					{
						this._currentlyUsedSiegeWeapon = null;
						this._currentlyUsedMissionObject = null;
						this.ResetTriggerFeedback();
						this.ResetTriggerVibration();
						return;
					}
					if (mainAgent.CurrentlyUsedGameObject != this._currentlyUsedMissionObject)
					{
						RangedSiegeWeapon rangedSiegeWeapon;
						if ((rangedSiegeWeapon = this.GetUsableMachineFromUsableMissionObject(mainAgent.CurrentlyUsedGameObject) as RangedSiegeWeapon) != null)
						{
							this._currentlyUsedSiegeWeapon = rangedSiegeWeapon;
						}
						this._currentlyUsedMissionObject = mainAgent.CurrentlyUsedGameObject;
					}
					if (this._currentlyUsedSiegeWeapon != null)
					{
						this.HandleRangedSiegeEngineAdaptiveTriggers(this._currentlyUsedSiegeWeapon);
						return;
					}
				}
				else
				{
					this.ResetTriggerFeedback();
					this.ResetTriggerVibration();
				}
			}
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, ref affectorWeapon, ref blow, ref attackCollisionData);
			if (affectedAgent == Agent.Main)
			{
				AttackCollisionData attackCollisionData2 = attackCollisionData;
				if (attackCollisionData2.CollisionResult != 3)
				{
					attackCollisionData2 = attackCollisionData;
					if (attackCollisionData2.CollisionResult != 5)
					{
						attackCollisionData2 = attackCollisionData;
						if (attackCollisionData2.CollisionResult != 4)
						{
							goto IL_92;
						}
					}
				}
				float[] array = new float[] { 0.5f };
				float[] array2 = new float[] { 0.3f };
				float[] array3 = new float[] { 0.3f };
				this.SetTriggerVibration(array, array2, array3, array3.Length, null, null, null, 0);
				this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
				IL_92:
				if (affectedAgent.WieldedOffhandWeapon.IsEmpty)
				{
					attackCollisionData2 = attackCollisionData;
					if (attackCollisionData2.AttackBlockedWithShield)
					{
						this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
						return;
					}
				}
			}
			else if (affectorAgent == Agent.Main)
			{
				AttackCollisionData attackCollisionData2 = attackCollisionData;
				if (attackCollisionData2.CollisionResult != 1)
				{
					attackCollisionData2 = attackCollisionData;
					if (attackCollisionData2.CollisionResult != 3)
					{
						return;
					}
				}
				MissionWeapon missionWeapon = affectorWeapon;
				if (!missionWeapon.IsEmpty)
				{
					missionWeapon = affectorWeapon;
					if (missionWeapon.IsShield())
					{
						float[] array4 = new float[] { 1f };
						float[] array5 = new float[] { 0.1f };
						float[] array6 = new float[] { 0.35f };
						this.SetTriggerVibration(array4, array5, array6, array6.Length, null, null, null, 0);
					}
				}
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent.IsMainAgent)
			{
				this.ResetTriggerFeedback();
				this.ResetTriggerVibration();
			}
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
		}

		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType changedNativeOptionsType)
		{
			if (changedNativeOptionsType == 15)
			{
				bool isAdaptiveTriggerEnabled = this._isAdaptiveTriggerEnabled;
				this._isAdaptiveTriggerEnabled = NativeOptions.GetConfig(15) != 0f;
				this._usingAlternativeAiming = NativeOptions.GetConfig(19) != 0f;
				if (isAdaptiveTriggerEnabled && !this._isAdaptiveTriggerEnabled)
				{
					this.ResetTriggerFeedback();
					this.ResetTriggerVibration();
				}
			}
		}

		private bool IsMissionModeApplicableForAdaptiveTrigger(MissionMode mode)
		{
			switch (mode)
			{
			case 0:
			case 2:
			case 3:
			case 4:
			case 7:
				return true;
			}
			return false;
		}

		private void HandleBowAdaptiveTriggers()
		{
			Agent mainAgent = base.Mission.MainAgent;
			Agent.ActionStage actionStage = ((mainAgent != null) ? mainAgent.GetCurrentActionStage(1) : (-1));
			if (actionStage == -1 || actionStage == 3 || actionStage == 4)
			{
				this.SetTriggerState(this._usingAlternativeAiming ? MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackLeft : MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackRight);
				return;
			}
			if (actionStage == null)
			{
				float num = mainAgent.GetAimingTimer() - mainAgent.AgentDrivenProperties.WeaponUnsteadyBeginTime;
				if (num > 0f)
				{
					float num2 = mainAgent.AgentDrivenProperties.WeaponUnsteadyEndTime - mainAgent.AgentDrivenProperties.WeaponUnsteadyBeginTime;
					float num3 = MBMath.ClampFloat(num / num2, 0f, 1f);
					float num4 = MBMath.Lerp(0f, 1f, num3, 1E-05f);
					float[] array = new float[] { num4 };
					float num5 = MBMath.ClampFloat(1f - num4, 0.1f, 1f);
					float[] array2 = new float[] { num5 };
					float[] array3 = new float[] { 0.05f };
					if (this._usingAlternativeAiming)
					{
						this.SetTriggerVibration(array, array2, array3, array3.Length, null, null, null, 0);
					}
					else
					{
						this.SetTriggerVibration(null, null, null, 0, array, array2, array3, array3.Length);
					}
					this._triggerState = MissionGamepadEffectsView.TriggerState.Vibration;
				}
				else
				{
					this.SetTriggerState(this._usingAlternativeAiming ? MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackLeft : MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackRight);
					float[] array4 = new float[] { 0.07f };
					float[] array5 = new float[] { 0.5f };
					float[] array6 = new float[] { 0.5f };
					if (this._usingAlternativeAiming)
					{
						this.SetTriggerVibration(array4, array5, array6, array6.Length, null, null, null, 0);
					}
					else
					{
						this.SetTriggerVibration(null, null, null, 0, array4, array5, array6, array6.Length);
					}
				}
				if (this._usingAlternativeAiming)
				{
					this.SetTriggerWeaponEffect(0, 0, 0, 3, 7, 8);
					return;
				}
				this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
				return;
			}
			else
			{
				if (actionStage == 2)
				{
					this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
					return;
				}
				this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
				return;
			}
		}

		private void HandleCrossbowAdaptiveTriggers()
		{
			Agent mainAgent = base.Mission.MainAgent;
			Agent.ActionStage actionStage = ((mainAgent != null) ? mainAgent.GetCurrentActionStage(1) : (-1));
			if (actionStage == 3)
			{
				this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
				return;
			}
			if (actionStage == 2)
			{
				float[] array = new float[] { 0.01f };
				float[] array2 = new float[] { 0.08f };
				float[] array3 = new float[] { 0.05f };
				this.SetTriggerVibration(null, null, null, 0, array, array2, array3, array3.Length);
				this.SetTriggerState(MissionGamepadEffectsView.TriggerState.Off);
				return;
			}
			if (actionStage == null)
			{
				if (this._usingAlternativeAiming)
				{
					this.SetTriggerWeaponEffect(0, 0, 0, 3, 7, 8);
					return;
				}
			}
			else if (!this._usingAlternativeAiming)
			{
				this.SetTriggerWeaponEffect(0, 0, 0, 3, 7, 8);
			}
		}

		private void HandleThrowableAdaptiveTriggers()
		{
			WeaponComponentData currentUsageItem = base.Mission.MainAgent.WieldedOffhandWeapon.CurrentUsageItem;
			bool flag = currentUsageItem != null && Extensions.HasAnyFlag<WeaponFlags>(currentUsageItem.WeaponFlags, 268435456L);
			this._triggerFeedback[2] = 0;
			this._triggerFeedback[3] = 3;
			if (flag)
			{
				this._triggerFeedback[0] = 4;
				this._triggerFeedback[1] = 2;
			}
			else
			{
				this._triggerFeedback[0] = 0;
				this._triggerFeedback[1] = 0;
			}
			this.SetTriggerFeedback(this._triggerFeedback[0], this._triggerFeedback[1], this._triggerFeedback[2], this._triggerFeedback[3]);
		}

		private void HandleMeleeAdaptiveTriggers()
		{
			Agent mainAgent = base.Mission.MainAgent;
			MissionWeapon wieldedWeapon = mainAgent.WieldedWeapon;
			WeaponComponentData currentUsageItem = wieldedWeapon.CurrentUsageItem;
			bool flag = currentUsageItem != null && Extensions.HasAllFlags<WeaponFlags>(currentUsageItem.WeaponFlags, 16L);
			WeaponComponentData currentUsageItem2 = mainAgent.WieldedOffhandWeapon.CurrentUsageItem;
			bool flag2 = currentUsageItem2 != null && Extensions.HasAnyFlag<WeaponFlags>(currentUsageItem2.WeaponFlags, 268435456L);
			if (flag)
			{
				this._triggerFeedback[2] = 3;
				this._triggerFeedback[3] = 0;
			}
			else if (wieldedWeapon.CurrentUsageItem == null)
			{
				this._triggerFeedback[2] = 0;
				this._triggerFeedback[3] = 0;
			}
			else
			{
				this._triggerFeedback[2] = 4;
				this._triggerFeedback[3] = 1;
			}
			if (flag2 || flag || wieldedWeapon.CurrentUsageItem != null)
			{
				this._triggerFeedback[0] = 4;
				this._triggerFeedback[1] = 2;
			}
			else
			{
				this._triggerFeedback[0] = 0;
				this._triggerFeedback[1] = 0;
			}
			this.SetTriggerFeedback(this._triggerFeedback[0], this._triggerFeedback[1], this._triggerFeedback[2], this._triggerFeedback[3]);
		}

		private void HandleRangedSiegeEngineAdaptiveTriggers(RangedSiegeWeapon rangedSiegeWeapon)
		{
			if (!(rangedSiegeWeapon is Ballista) && !(rangedSiegeWeapon is FireBallista))
			{
				this.ResetTriggerFeedback();
				this.ResetTriggerVibration();
				return;
			}
			if (rangedSiegeWeapon.State == null)
			{
				this.SetTriggerWeaponEffect(0, 0, 0, 4, 6, 10);
				return;
			}
			if (rangedSiegeWeapon.State == 2 || rangedSiegeWeapon.State == 1)
			{
				this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
				float[] array = new float[] { 0.2f, 0.4f, 0.2f };
				float[] array2 = new float[] { 0.2f, 0.4f, 0.2f };
				float[] array3 = new float[] { 0.2f, 0.3f, 0.2f };
				this.SetTriggerVibration(null, null, null, 0, array, array2, array3, array3.Length);
				return;
			}
			this.ResetTriggerFeedback();
			this.ResetTriggerVibration();
		}

		private UsableMachine GetUsableMachineFromUsableMissionObject(UsableMissionObject usableMissionObject)
		{
			StandingPoint standingPoint;
			if ((standingPoint = usableMissionObject as StandingPoint) != null)
			{
				GameEntity gameEntity = standingPoint.GameEntity;
				while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
				{
					gameEntity = gameEntity.Parent;
				}
				if (gameEntity != null)
				{
					UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
					if (firstScriptOfType != null)
					{
						return firstScriptOfType;
					}
				}
			}
			return null;
		}

		private void SetTriggerState(MissionGamepadEffectsView.TriggerState triggerState)
		{
			if (this._triggerState != triggerState)
			{
				switch (triggerState)
				{
				case MissionGamepadEffectsView.TriggerState.Off:
					this.ResetTriggerFeedback();
					this.ResetTriggerVibration();
					break;
				case MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackLeft:
					this.SetTriggerFeedback(0, 2, 0, 0);
					this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
					break;
				case MissionGamepadEffectsView.TriggerState.SoftTriggerFeedbackRight:
					this.SetTriggerFeedback(0, 0, 0, 2);
					this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
					break;
				case MissionGamepadEffectsView.TriggerState.HardTriggerFeedbackLeft:
					this.SetTriggerFeedback(0, 4, 0, 0);
					this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
					break;
				case MissionGamepadEffectsView.TriggerState.HardTriggerFeedbackRight:
					this.SetTriggerFeedback(0, 0, 0, 4);
					this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
					break;
				case MissionGamepadEffectsView.TriggerState.WeaponEffect:
					this.SetTriggerWeaponEffect(0, 0, 0, 4, 7, 7);
					break;
				default:
					Debug.FailedAssert("Unexpected trigger state:" + triggerState, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\MissionViews\\MissionGamepadEffectsView.cs", "SetTriggerState", 500);
					break;
				}
				this._triggerState = triggerState;
			}
		}

		private void ResetTriggerFeedback()
		{
			this._triggerFeedback[0] = 0;
			this._triggerFeedback[1] = 0;
			this._triggerFeedback[2] = 0;
			this._triggerFeedback[3] = 0;
			this.SetTriggerFeedback(0, 0, 0, 0);
			this.SetTriggerWeaponEffect(0, 0, 0, 0, 0, 0);
			this._triggerState = MissionGamepadEffectsView.TriggerState.Off;
		}

		private void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
			TaleWorlds.InputSystem.Input.SetTriggerFeedback(leftTriggerPosition, leftTriggerStrength, rightTriggerPosition, rightTriggerStrength);
		}

		private void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
			TaleWorlds.InputSystem.Input.SetTriggerWeaponEffect(leftStartPosition, leftEnd_position, leftStrength, rightStartPosition, rightEndPosition, rightStrength);
		}

		private void ResetTriggerVibration()
		{
			float[] array = new float[1];
			this.SetTriggerVibration(array, array, array, 0, array, array, array, 0);
		}

		private void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
			TaleWorlds.InputSystem.Input.SetTriggerVibration(leftTriggerAmplitudes, leftTriggerFrequencies, leftTriggerDurations, numLeftTriggerElements, rightTriggerAmplitudes, rightTriggerFrequencies, rightTriggerDurations, numRightTriggerElements);
		}

		private static void SetLightbarColor(float red, float green, float blue)
		{
			TaleWorlds.InputSystem.Input.SetLightbarColor(red, green, blue);
		}

		private MissionGamepadEffectsView.TriggerState _triggerState;

		private readonly byte[] _triggerFeedback = new byte[4];

		private bool _isAdaptiveTriggerEnabled;

		private bool _usingAlternativeAiming;

		private RangedSiegeWeapon _currentlyUsedSiegeWeapon;

		private UsableMissionObject _currentlyUsedMissionObject;

		private enum TriggerState
		{
			Off,
			SoftTriggerFeedbackLeft,
			SoftTriggerFeedbackRight,
			HardTriggerFeedbackLeft,
			HardTriggerFeedbackRight,
			WeaponEffect,
			Vibration
		}
	}
}
