using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200011F RID: 287
	public class LobbyClientConnectResult
	{
		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x0000AF2E File Offset: 0x0000912E
		// (set) Token: 0x06000675 RID: 1653 RVA: 0x0000AF36 File Offset: 0x00009136
		public bool Connected { get; private set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0000AF3F File Offset: 0x0000913F
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x0000AF47 File Offset: 0x00009147
		public TextObject Error { get; private set; }

		// Token: 0x06000678 RID: 1656 RVA: 0x0000AF50 File Offset: 0x00009150
		public LobbyClientConnectResult(bool connected, TextObject error)
		{
			this.Connected = connected;
			this.Error = error;
		}
	}
}
