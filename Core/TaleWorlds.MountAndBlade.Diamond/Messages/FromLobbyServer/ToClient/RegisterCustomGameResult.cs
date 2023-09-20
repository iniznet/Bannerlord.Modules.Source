using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004F RID: 79
	[Serializable]
	public class RegisterCustomGameResult : FunctionResult
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00002D2A File Offset: 0x00000F2A
		// (set) Token: 0x06000128 RID: 296 RVA: 0x00002D32 File Offset: 0x00000F32
		public bool Success { get; private set; }

		// Token: 0x06000129 RID: 297 RVA: 0x00002D3B File Offset: 0x00000F3B
		public RegisterCustomGameResult(bool success)
		{
			this.Success = success;
		}
	}
}
