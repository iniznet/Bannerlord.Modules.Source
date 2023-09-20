using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000E1 RID: 225
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcControllerAttribute : Attribute
	{
		// Token: 0x170001EC RID: 492
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x0000F3B8 File Offset: 0x0000D5B8
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
		[UsedImplicitly]
		public string AnonymousProperty { get; private set; }

		// Token: 0x06000890 RID: 2192 RVA: 0x0000F3C9 File Offset: 0x0000D5C9
		public AspMvcControllerAttribute()
		{
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x0000F3D1 File Offset: 0x0000D5D1
		public AspMvcControllerAttribute(string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}
	}
}
