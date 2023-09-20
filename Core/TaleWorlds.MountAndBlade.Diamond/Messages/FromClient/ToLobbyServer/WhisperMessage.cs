using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C5 RID: 197
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class WhisperMessage : Message
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x00003F6F File Offset: 0x0000216F
		// (set) Token: 0x060002CA RID: 714 RVA: 0x00003F77 File Offset: 0x00002177
		public string TargetPlayerName { get; private set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060002CB RID: 715 RVA: 0x00003F80 File Offset: 0x00002180
		// (set) Token: 0x060002CC RID: 716 RVA: 0x00003F88 File Offset: 0x00002188
		public string Message { get; private set; }

		// Token: 0x060002CD RID: 717 RVA: 0x00003F91 File Offset: 0x00002191
		public WhisperMessage(string targetPlayerName, string message)
		{
			this.TargetPlayerName = targetPlayerName;
			this.Message = message;
		}
	}
}
