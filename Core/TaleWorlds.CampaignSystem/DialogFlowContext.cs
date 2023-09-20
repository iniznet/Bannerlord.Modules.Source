using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200007A RID: 122
	internal class DialogFlowContext
	{
		// Token: 0x06000F27 RID: 3879 RVA: 0x00046BC0 File Offset: 0x00044DC0
		public DialogFlowContext(string token, bool byPlayer, DialogFlowContext parent)
		{
			this.Token = token;
			this.ByPlayer = byPlayer;
			this.Parent = parent;
		}

		// Token: 0x04000530 RID: 1328
		internal readonly string Token;

		// Token: 0x04000531 RID: 1329
		internal readonly bool ByPlayer;

		// Token: 0x04000532 RID: 1330
		internal readonly DialogFlowContext Parent;
	}
}
