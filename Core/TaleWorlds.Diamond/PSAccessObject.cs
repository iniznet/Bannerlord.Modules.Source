using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001B RID: 27
	[Serializable]
	public class PSAccessObject
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00002B1D File Offset: 0x00000D1D
		public int IssuerId { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00002B25 File Offset: 0x00000D25
		public string AuthCode { get; }

		// Token: 0x06000078 RID: 120 RVA: 0x00002B2D File Offset: 0x00000D2D
		public PSAccessObject(int issuerId, string authCode)
		{
			this.IssuerId = issuerId;
			this.AuthCode = authCode;
		}
	}
}
