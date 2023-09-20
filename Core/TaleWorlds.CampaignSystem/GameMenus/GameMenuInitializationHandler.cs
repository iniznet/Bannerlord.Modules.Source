using System;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E5 RID: 229
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class GameMenuInitializationHandler : Attribute
	{
		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x00057A41 File Offset: 0x00055C41
		// (set) Token: 0x060013C6 RID: 5062 RVA: 0x00057A49 File Offset: 0x00055C49
		public string MenuId { get; private set; }

		// Token: 0x060013C7 RID: 5063 RVA: 0x00057A52 File Offset: 0x00055C52
		public GameMenuInitializationHandler(string menuId)
		{
			this.MenuId = menuId;
		}
	}
}
