using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000E0 RID: 224
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class AspMvcAreaAttribute : PathReferenceAttribute
	{
		// Token: 0x170001EB RID: 491
		// (get) Token: 0x0600088A RID: 2186 RVA: 0x0000F390 File Offset: 0x0000D590
		// (set) Token: 0x0600088B RID: 2187 RVA: 0x0000F398 File Offset: 0x0000D598
		[UsedImplicitly]
		public string AnonymousProperty { get; private set; }

		// Token: 0x0600088C RID: 2188 RVA: 0x0000F3A1 File Offset: 0x0000D5A1
		[UsedImplicitly]
		public AspMvcAreaAttribute()
		{
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x0000F3A9 File Offset: 0x0000D5A9
		public AspMvcAreaAttribute(string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}
	}
}
