using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000009 RID: 9
	public class HandlerResult
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002624 File Offset: 0x00000824
		public bool IsSuccessful { get; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000262C File Offset: 0x0000082C
		public string Error { get; }

		// Token: 0x0600002C RID: 44 RVA: 0x00002634 File Offset: 0x00000834
		protected HandlerResult(bool isSuccessful, string error = null)
		{
			this.IsSuccessful = isSuccessful;
			this.Error = error;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000264A File Offset: 0x0000084A
		public static HandlerResult CreateSuccessful()
		{
			return new HandlerResult(true, null);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002653 File Offset: 0x00000853
		public static HandlerResult CreateFailed(string error)
		{
			return new HandlerResult(false, error);
		}
	}
}
