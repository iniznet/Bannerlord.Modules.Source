using System;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	internal class MBTextToken
	{
		// Token: 0x0600010A RID: 266 RVA: 0x00005D79 File Offset: 0x00003F79
		internal MBTextToken(TokenType tokenType)
		{
			this.TokenType = tokenType;
			this.Value = string.Empty;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00005D93 File Offset: 0x00003F93
		internal MBTextToken(TokenType tokenType, string value)
		{
			this.TokenType = tokenType;
			this.Value = value;
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00005DA9 File Offset: 0x00003FA9
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00005DB1 File Offset: 0x00003FB1
		internal TokenType TokenType { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00005DBA File Offset: 0x00003FBA
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00005DC2 File Offset: 0x00003FC2
		public string Value { get; set; }

		// Token: 0x06000110 RID: 272 RVA: 0x00005DCB File Offset: 0x00003FCB
		public MBTextToken Clone()
		{
			return new MBTextToken(this.TokenType, this.Value);
		}
	}
}
