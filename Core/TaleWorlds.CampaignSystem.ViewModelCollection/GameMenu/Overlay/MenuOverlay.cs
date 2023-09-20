using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	// Token: 0x020000A4 RID: 164
	public class MenuOverlay : Attribute
	{
		// Token: 0x0600105D RID: 4189 RVA: 0x00040BEB File Offset: 0x0003EDEB
		public MenuOverlay(string typeId)
		{
			this.TypeId = typeId;
		}

		// Token: 0x04000799 RID: 1945
		public new string TypeId;
	}
}
