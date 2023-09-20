using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000010 RID: 16
	public class AccessObjectResult
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000266B File Offset: 0x0000086B
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002673 File Offset: 0x00000873
		public object AccessObject { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000267C File Offset: 0x0000087C
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002684 File Offset: 0x00000884
		public bool Success { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000268D File Offset: 0x0000088D
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002695 File Offset: 0x00000895
		public TextObject FailReason { get; private set; }

		// Token: 0x0600004C RID: 76 RVA: 0x0000269E File Offset: 0x0000089E
		public static AccessObjectResult CreateSuccess(object accessObject)
		{
			return new AccessObjectResult
			{
				Success = true,
				AccessObject = accessObject
			};
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000026B3 File Offset: 0x000008B3
		public static AccessObjectResult CreateFailed(TextObject failReason)
		{
			return new AccessObjectResult
			{
				Success = false,
				FailReason = failReason
			};
		}
	}
}
