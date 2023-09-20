using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000DE RID: 222
	[AttributeUsage(AttributeTargets.Parameter)]
	public class PathReferenceAttribute : Attribute
	{
		// Token: 0x06000882 RID: 2178 RVA: 0x0000F340 File Offset: 0x0000D540
		public PathReferenceAttribute()
		{
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0000F348 File Offset: 0x0000D548
		[UsedImplicitly]
		public PathReferenceAttribute([PathReference] string basePath)
		{
			this.BasePath = basePath;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000884 RID: 2180 RVA: 0x0000F357 File Offset: 0x0000D557
		// (set) Token: 0x06000885 RID: 2181 RVA: 0x0000F35F File Offset: 0x0000D55F
		[UsedImplicitly]
		public string BasePath { get; private set; }
	}
}
