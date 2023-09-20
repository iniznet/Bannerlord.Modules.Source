using System;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x02000162 RID: 354
	public abstract class EncyclopediaModelBase : Attribute
	{
		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001882 RID: 6274 RVA: 0x0007BB7D File Offset: 0x00079D7D
		// (set) Token: 0x06001883 RID: 6275 RVA: 0x0007BB85 File Offset: 0x00079D85
		public Type[] PageTargetTypes { get; private set; }

		// Token: 0x06001884 RID: 6276 RVA: 0x0007BB8E File Offset: 0x00079D8E
		public EncyclopediaModelBase(Type[] pageTargetTypes)
		{
			this.PageTargetTypes = pageTargetTypes;
		}
	}
}
