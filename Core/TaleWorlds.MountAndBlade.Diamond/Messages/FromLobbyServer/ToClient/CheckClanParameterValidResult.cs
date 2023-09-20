using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class CheckClanParameterValidResult : FunctionResult
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000021C1 File Offset: 0x000003C1
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000021C9 File Offset: 0x000003C9
		public bool IsValid { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000021D2 File Offset: 0x000003D2
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000021DA File Offset: 0x000003DA
		public StringValidationError Error { get; private set; }

		// Token: 0x06000028 RID: 40 RVA: 0x000021E3 File Offset: 0x000003E3
		public CheckClanParameterValidResult(bool isValid, StringValidationError error)
		{
			this.IsValid = isValid;
			this.Error = error;
		}
	}
}
