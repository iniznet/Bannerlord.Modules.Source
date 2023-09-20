using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPointForRangedArea : StandingPoint
	{
		public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.NoAttack | Agent.AIScriptedFrameFlags.ConsiderRotation;
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.AutoSheathWeapons = false;
			this.LockUserFrames = false;
			this.LockUserPositions = true;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			if (wieldedItemIndex == EquipmentIndex.None)
			{
				return true;
			}
			WeaponComponentData currentUsageItem = agent.Equipment[wieldedItemIndex].CurrentUsageItem;
			if (currentUsageItem == null || !currentUsageItem.IsRangedWeapon)
			{
				return true;
			}
			if (wieldedItemIndex == EquipmentIndex.ExtraWeaponSlot)
			{
				return this.ThrowingValueMultiplier <= 0f || base.IsDisabledForAgent(agent);
			}
			return this.RangedWeaponValueMultiplier <= 0f || base.IsDisabledForAgent(agent);
		}

		public override float GetUsageScoreForAgent(Agent agent)
		{
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			float num = 0f;
			if (wieldedItemIndex != EquipmentIndex.None && agent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon)
			{
				num = ((wieldedItemIndex == EquipmentIndex.ExtraWeaponSlot) ? this.ThrowingValueMultiplier : this.RangedWeaponValueMultiplier);
			}
			return base.GetUsageScoreForAgent(agent) + num;
		}

		public override bool HasAlternative()
		{
			return true;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.HasUser)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTickParallel2(float dt)
		{
			base.OnTickParallel2(dt);
			if (base.HasUser && this.IsDisabledForAgent(base.UserAgent))
			{
				base.UserAgent.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
		}

		public float ThrowingValueMultiplier = 5f;

		public float RangedWeaponValueMultiplier = 2f;
	}
}
