using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200001E RID: 30
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class FindGameAnswerMessage : Message
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000024B2 File Offset: 0x000006B2
		// (set) Token: 0x0600006A RID: 106 RVA: 0x000024BA File Offset: 0x000006BA
		public bool Successful { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006B RID: 107 RVA: 0x000024C3 File Offset: 0x000006C3
		// (set) Token: 0x0600006C RID: 108 RVA: 0x000024CB File Offset: 0x000006CB
		public string[] SelectedAndEnabledGameTypes { get; private set; }

		// Token: 0x0600006D RID: 109 RVA: 0x000024D4 File Offset: 0x000006D4
		public FindGameAnswerMessage(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
