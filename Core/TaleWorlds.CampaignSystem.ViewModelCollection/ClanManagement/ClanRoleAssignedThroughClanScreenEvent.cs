using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000109 RID: 265
	public class ClanRoleAssignedThroughClanScreenEvent : EventBase
	{
		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x0600195E RID: 6494 RVA: 0x0005BCAE File Offset: 0x00059EAE
		// (set) Token: 0x0600195F RID: 6495 RVA: 0x0005BCB6 File Offset: 0x00059EB6
		public SkillEffect.PerkRole Role { get; private set; }

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x06001960 RID: 6496 RVA: 0x0005BCBF File Offset: 0x00059EBF
		// (set) Token: 0x06001961 RID: 6497 RVA: 0x0005BCC7 File Offset: 0x00059EC7
		public Hero HeroObject { get; private set; }

		// Token: 0x06001962 RID: 6498 RVA: 0x0005BCD0 File Offset: 0x00059ED0
		public ClanRoleAssignedThroughClanScreenEvent(SkillEffect.PerkRole role, Hero heroObject)
		{
			this.Role = role;
			this.HeroObject = heroObject;
		}
	}
}
