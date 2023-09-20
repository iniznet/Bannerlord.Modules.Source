using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000361 RID: 865
	public class StandingPointForRangedArea : StandingPoint
	{
		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x06002F48 RID: 12104 RVA: 0x000C0EC1 File Offset: 0x000BF0C1
		public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.NoAttack | Agent.AIScriptedFrameFlags.ConsiderRotation;
			}
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x000C0EC4 File Offset: 0x000BF0C4
		protected internal override void OnInit()
		{
			base.OnInit();
			this.AutoSheathWeapons = false;
			this.LockUserFrames = false;
			this.LockUserPositions = true;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x000C0EF0 File Offset: 0x000BF0F0
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

		// Token: 0x06002F4B RID: 12107 RVA: 0x000C0F60 File Offset: 0x000BF160
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

		// Token: 0x06002F4C RID: 12108 RVA: 0x000C0FB6 File Offset: 0x000BF1B6
		public override bool HasAlternative()
		{
			return true;
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x000C0FB9 File Offset: 0x000BF1B9
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.HasUser)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x000C0FD2 File Offset: 0x000BF1D2
		protected internal override void OnTickParallel2(float dt)
		{
			base.OnTickParallel2(dt);
			if (base.HasUser && this.IsDisabledForAgent(base.UserAgent))
			{
				base.UserAgent.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
		}

		// Token: 0x04001364 RID: 4964
		public float ThrowingValueMultiplier = 5f;

		// Token: 0x04001365 RID: 4965
		public float RangedWeaponValueMultiplier = 2f;
	}
}
