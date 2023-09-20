using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000055 RID: 85
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class SystemMessage : Message
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00002DEA File Offset: 0x00000FEA
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00002DF2 File Offset: 0x00000FF2
		public ServerInfoMessage Message { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00002DFB File Offset: 0x00000FFB
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00002E03 File Offset: 0x00001003
		public List<string> Parameters { get; private set; }

		// Token: 0x0600013D RID: 317 RVA: 0x00002E0C File Offset: 0x0000100C
		public SystemMessage(ServerInfoMessage message, params string[] arguments)
		{
			this.Message = message;
			this.Parameters = new List<string>(arguments);
		}
	}
}
