using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public class CreatePremadeGameMessageResult : FunctionResult
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000023C2 File Offset: 0x000005C2
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000023CA File Offset: 0x000005CA
		public bool Successful { get; private set; }

		// Token: 0x06000055 RID: 85 RVA: 0x000023D3 File Offset: 0x000005D3
		public CreatePremadeGameMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
