using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C4 RID: 196
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateUsingClanSigil : Message
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00003F4F File Offset: 0x0000214F
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x00003F57 File Offset: 0x00002157
		public bool IsUsed { get; private set; }

		// Token: 0x060002C8 RID: 712 RVA: 0x00003F60 File Offset: 0x00002160
		public UpdateUsingClanSigil(bool isUsed)
		{
			this.IsUsed = isUsed;
		}
	}
}
