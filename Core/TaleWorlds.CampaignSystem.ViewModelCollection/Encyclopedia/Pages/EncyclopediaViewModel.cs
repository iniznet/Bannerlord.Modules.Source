using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B8 RID: 184
	public class EncyclopediaViewModel : Attribute
	{
		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x00046D7D File Offset: 0x00044F7D
		// (set) Token: 0x06001219 RID: 4633 RVA: 0x00046D85 File Offset: 0x00044F85
		public Type PageTargetType { get; private set; }

		// Token: 0x0600121A RID: 4634 RVA: 0x00046D8E File Offset: 0x00044F8E
		public EncyclopediaViewModel(Type pageTargetType)
		{
			this.PageTargetType = pageTargetType;
		}
	}
}
