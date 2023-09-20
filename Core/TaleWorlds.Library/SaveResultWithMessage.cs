using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000081 RID: 129
	public struct SaveResultWithMessage
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x0000EA5A File Offset: 0x0000CC5A
		public static SaveResultWithMessage Default
		{
			get
			{
				return new SaveResultWithMessage(SaveResult.Success, string.Empty);
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0000EA67 File Offset: 0x0000CC67
		public SaveResultWithMessage(SaveResult result, string message)
		{
			this.SaveResult = result;
			this.Message = message;
		}

		// Token: 0x04000158 RID: 344
		public readonly SaveResult SaveResult;

		// Token: 0x04000159 RID: 345
		public readonly string Message;
	}
}
