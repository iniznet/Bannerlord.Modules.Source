using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000DF RID: 223
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcActionAttribute : Attribute
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x0000F368 File Offset: 0x0000D568
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x0000F370 File Offset: 0x0000D570
		[UsedImplicitly]
		public string AnonymousProperty { get; private set; }

		// Token: 0x06000888 RID: 2184 RVA: 0x0000F379 File Offset: 0x0000D579
		public AspMvcActionAttribute()
		{
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0000F381 File Offset: 0x0000D581
		public AspMvcActionAttribute(string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}
	}
}
