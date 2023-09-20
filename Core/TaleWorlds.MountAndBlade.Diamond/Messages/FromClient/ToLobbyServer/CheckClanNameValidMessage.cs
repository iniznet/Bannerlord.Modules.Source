using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007D RID: 125
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanNameValidMessage : Message
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00003564 File Offset: 0x00001764
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000356C File Offset: 0x0000176C
		public string ClanName { get; private set; }

		// Token: 0x060001E2 RID: 482 RVA: 0x00003575 File Offset: 0x00001775
		public CheckClanNameValidMessage(string clanName)
		{
			this.ClanName = clanName;
		}
	}
}
