using System;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x0200002D RID: 45
	public class SaveError
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x000080C4 File Offset: 0x000062C4
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x000080CC File Offset: 0x000062CC
		public string Message { get; private set; }

		// Token: 0x060001A5 RID: 421 RVA: 0x000080D5 File Offset: 0x000062D5
		internal SaveError(string message)
		{
			this.Message = message;
		}
	}
}
