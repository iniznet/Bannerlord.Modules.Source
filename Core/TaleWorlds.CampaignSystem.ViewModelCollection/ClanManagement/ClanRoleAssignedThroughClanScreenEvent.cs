using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanRoleAssignedThroughClanScreenEvent : EventBase
	{
		public SkillEffect.PerkRole Role { get; private set; }

		public Hero HeroObject { get; private set; }

		public ClanRoleAssignedThroughClanScreenEvent(SkillEffect.PerkRole role, Hero heroObject)
		{
			this.Role = role;
			this.HeroObject = heroObject;
		}
	}
}
