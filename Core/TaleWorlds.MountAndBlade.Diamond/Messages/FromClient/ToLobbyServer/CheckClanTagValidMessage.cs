using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007E RID: 126
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanTagValidMessage : Message
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x00003584 File Offset: 0x00001784
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x0000358C File Offset: 0x0000178C
		public string ClanTag { get; private set; }

		// Token: 0x060001E5 RID: 485 RVA: 0x00003595 File Offset: 0x00001795
		public CheckClanTagValidMessage(string clanTag)
		{
			this.ClanTag = clanTag;
		}
	}
}
