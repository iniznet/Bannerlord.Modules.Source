using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000028 RID: 40
	[Serializable]
	public class GetOfficialServerProviderNameResult : FunctionResult
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00002607 File Offset: 0x00000807
		public string Name { get; }

		// Token: 0x06000089 RID: 137 RVA: 0x0000260F File Offset: 0x0000080F
		public GetOfficialServerProviderNameResult(string name)
		{
			this.Name = name;
		}
	}
}
