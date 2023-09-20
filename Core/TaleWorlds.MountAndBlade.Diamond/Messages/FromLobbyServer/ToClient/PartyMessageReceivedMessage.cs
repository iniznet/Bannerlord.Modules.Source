using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000041 RID: 65
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PartyMessageReceivedMessage : Message
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00002AA3 File Offset: 0x00000CA3
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x00002AAB File Offset: 0x00000CAB
		public string PlayerName { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00002AB4 File Offset: 0x00000CB4
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x00002ABC File Offset: 0x00000CBC
		public string Message { get; private set; }

		// Token: 0x060000F4 RID: 244 RVA: 0x00002AC5 File Offset: 0x00000CC5
		public PartyMessageReceivedMessage(string playerName, string message)
		{
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
