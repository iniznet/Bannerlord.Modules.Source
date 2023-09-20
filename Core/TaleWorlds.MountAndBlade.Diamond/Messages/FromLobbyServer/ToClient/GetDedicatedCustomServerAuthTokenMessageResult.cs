using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class GetDedicatedCustomServerAuthTokenMessageResult : FunctionResult
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000025E7 File Offset: 0x000007E7
		// (set) Token: 0x06000086 RID: 134 RVA: 0x000025EF File Offset: 0x000007EF
		public string AuthToken { get; private set; }

		// Token: 0x06000087 RID: 135 RVA: 0x000025F8 File Offset: 0x000007F8
		public GetDedicatedCustomServerAuthTokenMessageResult(string authToken)
		{
			this.AuthToken = authToken;
		}
	}
}
