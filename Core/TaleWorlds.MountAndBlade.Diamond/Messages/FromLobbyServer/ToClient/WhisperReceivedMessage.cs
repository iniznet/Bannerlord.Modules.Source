using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000059 RID: 89
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class WhisperReceivedMessage : Message
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00002E75 File Offset: 0x00001075
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00002E7D File Offset: 0x0000107D
		public string FromPlayer { get; private set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00002E86 File Offset: 0x00001086
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00002E8E File Offset: 0x0000108E
		public string ToPlayer { get; private set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00002E97 File Offset: 0x00001097
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00002E9F File Offset: 0x0000109F
		public string Message { get; private set; }

		// Token: 0x0600014B RID: 331 RVA: 0x00002EA8 File Offset: 0x000010A8
		public WhisperReceivedMessage(string fromPlayer, string toPlayer, string message)
		{
			this.FromPlayer = fromPlayer;
			this.ToPlayer = toPlayer;
			this.Message = message;
		}
	}
}
