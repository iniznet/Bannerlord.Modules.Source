using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000032 RID: 50
	[Serializable]
	public class GetUserCosmeticsInfoMessageResult : FunctionResult
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00002735 File Offset: 0x00000935
		public bool Successful { get; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x0000273D File Offset: 0x0000093D
		public List<string> OwnedCosmetics { get; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00002745 File Offset: 0x00000945
		public Dictionary<string, List<string>> UsedCosmetics { get; }

		// Token: 0x060000A7 RID: 167 RVA: 0x0000274D File Offset: 0x0000094D
		public GetUserCosmeticsInfoMessageResult(bool successful, List<string> ownedCosmetics, Dictionary<string, List<string>> usedCosmetics)
		{
			this.Successful = successful;
			this.OwnedCosmetics = ownedCosmetics;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
