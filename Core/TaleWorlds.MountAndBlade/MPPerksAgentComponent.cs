using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class MPPerksAgentComponent : AgentComponent
	{
		public MPPerksAgentComponent(Agent agent)
			: base(agent)
		{
			this.Agent.OnAgentHealthChanged += this.OnHealthChanged;
			if (this.Agent.HasMount)
			{
				this.Agent.MountAgent.OnAgentHealthChanged += this.OnMountHealthChanged;
			}
		}

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

		public override void OnAgentRemoved()
		{
			if (this.Agent.HasMount)
			{
				this.Agent.MountAgent.OnAgentHealthChanged -= this.OnMountHealthChanged;
				this.Agent.MountAgent.UpdateAgentProperties();
			}
		}

		private void OnHealthChanged(Agent agent, float oldHealth, float newHealth)
		{
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this.Agent);
			if (perkHandler == null)
			{
				return;
			}
			perkHandler.OnEvent(agent, MPPerkCondition.PerkEventFlags.HealthChange);
		}

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
