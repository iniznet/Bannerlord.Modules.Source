using System;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000038 RID: 56
	public class LoadError
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001FD RID: 509 RVA: 0x0000985D File Offset: 0x00007A5D
		// (set) Token: 0x060001FE RID: 510 RVA: 0x00009865 File Offset: 0x00007A65
		public string Message { get; private set; }

		// Token: 0x060001FF RID: 511 RVA: 0x0000986E File Offset: 0x00007A6E
		internal LoadError(string message)
		{
			this.Message = message;
		}
	}
}
