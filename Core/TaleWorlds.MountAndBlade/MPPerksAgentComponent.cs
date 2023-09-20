using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D8 RID: 728
	public class MPPerksAgentComponent : AgentComponent
	{
		// Token: 0x06002817 RID: 10263 RVA: 0x0009B4F4 File Offset: 0x000996F4
		public MPPerksAgentComponent(Agent agent)
			: base(agent)
		{
			this.Agent.OnAgentHealthChanged += this.OnHealthChanged;
			if (this.Agent.HasMount)
			{
				this.Agent.MountAgent.OnAgentHealthChanged += this.OnMountHealthChanged;
			}
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x0009B548 File Offset: 0x00099748
		public override void OnMount(Agent mount)
		{
			mount.OnAgentHealthChanged += this.OnMountHealthChanged;
			mount.UpdateAgentProperties();
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
			if (perkHandler == null)
			{
				return;
			}
			perkHandler.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountChange);
		}

		// Token: 0x06002819 RID: 10265 RVA: 0x0009B582 File Offset: 0x00099782
		public override void OnDismount(Agent mount)
		{
			mount.OnAgentHealthChanged -= this.OnMountHealthChanged;
			mount.UpdateAgentProperties();
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
			if (perkHandler == null)
			{
				return;
			}
			perkHandler.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountChange);
		}

		// Token: 0x0600281A RID: 10266 RVA: 0x0009B5BC File Offset: 0x000997BC
		public override void OnItemPickup(SpawnedItemEntity item)
		{
			if (!item.WeaponCopy.IsEmpty && item.WeaponCopy.Item.ItemType == ItemObject.ItemTypeEnum.Banner)
			{
				MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
				if (perkHandler == null)
				{
					return;
				}
				perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.BannerPickUp);
			}
		}

		// Token: 0x0600281B RID: 10267 RVA: 0x0009B607 File Offset: 0x00099807
		public override void OnWeaponDrop(MissionWeapon droppedWeapon)
		{
			if (!droppedWeapon.IsEmpty && droppedWeapon.Item.ItemType == ItemObject.ItemTypeEnum.Banner)
			{
				MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
				if (perkHandler == null)
				{
					return;
				}
				perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.BannerDrop);
			}
		}

		// Token: 0x0600281C RID: 10268 RVA: 0x0009B63C File Offset: 0x0009983C
		public override void OnAgentRemoved()
		{
			if (this.Agent.HasMount)
			{
				this.Agent.MountAgent.OnAgentHealthChanged -= this.OnMountHealthChanged;
				this.Agent.MountAgent.UpdateAgentProperties();
			}
		}

		// Token: 0x0600281D RID: 10269 RVA: 0x0009B677 File Offset: 0x00099877
		private void OnHealthChanged(Agent agent, float oldHealth, float newHealth)
		{
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
			if (perkHandler == null)
			{
				return;
			}
			perkHandler.OnEvent(agent, MPPerkCondition.PerkEventFlags.HealthChange);
		}

		// Token: 0x0600281E RID: 10270 RVA: 0x0009B690 File Offset: 0x00099890
		private void OnMountHealthChanged(Agent agent, float oldHealth, float newHealth)
		{
			if (!this.Agent.IsActive() || this.Agent.MountAgent != agent)
			{
				agent.OnAgentHealthChanged -= this.OnMountHealthChanged;
				return;
			}
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
			if (perkHandler == null)
			{
				return;
			}
			perkHandler.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountHealthChange);
		}
	}
}
