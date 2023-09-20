using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.AI;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables
{
	public class SmithingMachine : UsableMachine
	{
		protected override void OnInit()
		{
			base.OnInit();
			this._machineUsePoint = (AnimationPoint)base.PilotStandingPoint;
			if (this._machineUsePoint == null)
			{
				"Entity(" + base.GameEntity.Name + ") with script(SmithingMachine) does not have a valid 'PilotStandingPoint'.";
			}
			this._machineUsePoint.IsDeactivated = false;
			this._machineUsePoint.IsDisabledForPlayers = true;
			this._machineUsePoint.KeepOldVisibility = true;
			this._anvilUsePoint = (AnimationPoint)base.StandingPoints.First((StandingPoint x) => x != this._machineUsePoint);
			this._anvilUsePoint.IsDeactivated = true;
			this._anvilUsePoint.KeepOldVisibility = true;
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.IsDisabledForPlayers = true;
			}
			this._actionsWithoutLeftHandItem = new List<ActionIndexCache>(4)
			{
				ActionIndexCache.Create("act_smithing_machine_anvil_start"),
				ActionIndexCache.Create("act_smithing_machine_anvil_part_2"),
				ActionIndexCache.Create("act_smithing_machine_anvil_part_4"),
				ActionIndexCache.Create("act_smithing_machine_anvil_part_5")
			};
			base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_idle", 0, 1f);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=OCRafO5h}Bellows", null).ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			switch (this._state)
			{
			case SmithingMachine.State.Stable:
				if (this._machineUsePoint.HasUser && this._machineUsePoint.UserAgent.GetCurrentVelocity().LengthSquared < 0.0001f)
				{
					this._machineUsePoint.UserAgent.SetActionChannel(0, this.CharacterReadyActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					this._state = SmithingMachine.State.Preparation;
				}
				if (this._anvilUsePoint.HasUser)
				{
					this._state = SmithingMachine.State.UseAnvilPoint;
					return;
				}
				break;
			case SmithingMachine.State.Preparation:
				if (!this._machineUsePoint.HasUser)
				{
					base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_idle_with_blend_in", 0, 1f);
					this._state = SmithingMachine.State.Stable;
					return;
				}
				if (this._machineUsePoint.UserAgent.GetCurrentActionValue(0) == this.CharacterReadyActionIndex && this._machineUsePoint.UserAgent.GetCurrentActionProgress(0) > 0.99f)
				{
					base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_loop", 0, 1f);
					this._machineUsePoint.UserAgent.SetActionChannel(0, this.CharacterUseActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					this._state = SmithingMachine.State.Working;
					return;
				}
				break;
			case SmithingMachine.State.Working:
				if (!this._machineUsePoint.HasUser)
				{
					base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_idle_with_blend_in", 0, 1f);
					this._state = SmithingMachine.State.Stable;
					this._disableTimer = null;
					this._anvilUsePoint.IsDeactivated = false;
					return;
				}
				if (this._machineUsePoint.UserAgent.GetCurrentActionValue(0) != this.CharacterUseActionIndex)
				{
					base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_idle_with_blend_in", 0, 1f);
					this._state = SmithingMachine.State.Paused;
					this._remainingTimeToReset = this._disableTimer.Duration - this._disableTimer.ElapsedTime();
					return;
				}
				if (this._disableTimer == null)
				{
					this._disableTimer = new Timer(Mission.Current.CurrentTime, 9.8f, true);
					return;
				}
				if (this._disableTimer.Check(Mission.Current.CurrentTime))
				{
					base.SetAnimationAtChannelSynched("anim_merchant_smithing_machine_idle_with_blend_in", 0, 1f);
					this._disableTimer = null;
					this._machineUsePoint.IsDeactivated = true;
					this._anvilUsePoint.IsDeactivated = false;
					this._state = SmithingMachine.State.Stable;
					return;
				}
				break;
			case SmithingMachine.State.Paused:
				if (this._machineUsePoint.IsRotationCorrectDuringUsage())
				{
					this._machineUsePoint.UserAgent.SetActionChannel(0, this.CharacterReadyActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				}
				if (this._machineUsePoint.UserAgent.GetCurrentActionValue(0) == this.CharacterReadyActionIndex)
				{
					this._state = SmithingMachine.State.Preparation;
					this._disableTimer.Reset(Mission.Current.CurrentTime, this._remainingTimeToReset);
					this._remainingTimeToReset = 0f;
					return;
				}
				break;
			case SmithingMachine.State.UseAnvilPoint:
			{
				if (!this._anvilUsePoint.HasUser)
				{
					this._state = SmithingMachine.State.Stable;
					this._disableTimer = null;
					this._machineUsePoint.IsDeactivated = false;
					return;
				}
				if (this._disableTimer == null)
				{
					this._disableTimer = new Timer(Mission.Current.CurrentTime, 96f, true);
					this._leftItemIsVisible = true;
					return;
				}
				if (this._disableTimer.Check(Mission.Current.CurrentTime))
				{
					this._disableTimer = null;
					this._anvilUsePoint.IsDeactivated = true;
					this._machineUsePoint.IsDeactivated = false;
					this._state = SmithingMachine.State.Stable;
					return;
				}
				ActionIndexCache currentAction = this._anvilUsePoint.UserAgent.GetCurrentAction(0);
				if (this._leftItemIsVisible && this._actionsWithoutLeftHandItem.Contains(currentAction))
				{
					this._anvilUsePoint.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(this._anvilUsePoint.UserAgent.Monster.OffHandItemBoneIndex, this._anvilUsePoint.LeftHandItem, false);
					this._leftItemIsVisible = false;
					return;
				}
				if (!this._leftItemIsVisible && !this._actionsWithoutLeftHandItem.Contains(currentAction))
				{
					this._anvilUsePoint.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(this._anvilUsePoint.UserAgent.Monster.OffHandItemBoneIndex, this._anvilUsePoint.LeftHandItem, true);
					this._leftItemIsVisible = true;
				}
				break;
			}
			default:
				return;
			}
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		private const string MachineIdleAnimationName = "anim_merchant_smithing_machine_idle";

		private const string MachineIdleWithBlendInAnimationName = "anim_merchant_smithing_machine_idle_with_blend_in";

		private const string MachineUseAnimationName = "anim_merchant_smithing_machine_loop";

		private readonly ActionIndexCache CharacterReadyActionIndex = ActionIndexCache.Create("act_use_smithing_machine_ready");

		private readonly ActionIndexCache CharacterUseActionIndex = ActionIndexCache.Create("act_use_smithing_machine_loop");

		private AnimationPoint _anvilUsePoint;

		private AnimationPoint _machineUsePoint;

		private SmithingMachine.State _state;

		private Timer _disableTimer;

		private float _remainingTimeToReset;

		private List<ActionIndexCache> _actionsWithoutLeftHandItem;

		private bool _leftItemIsVisible;

		private enum State
		{
			Stable,
			Preparation,
			Working,
			Paused,
			UseAnvilPoint
		}
	}
}
