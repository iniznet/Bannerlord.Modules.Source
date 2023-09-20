using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000CC RID: 204
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		// Token: 0x0600085A RID: 2138 RVA: 0x0000F18A File Offset: 0x0000D38A
		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x0600085B RID: 2139 RVA: 0x0000F199 File Offset: 0x0000D399
		// (set) Token: 0x0600085C RID: 2140 RVA: 0x0000F1A1 File Offset: 0x0000D3A1
		[UsedImplicitly]
		public bool Required { get; set; }

		// Token: 0x0600085D RID: 2141 RVA: 0x0000F1AC File Offset: 0x0000D3AC
		public override bool Equals(object obj)
		{
			LocalizationRequiredAttribute localizationRequiredAttribute = obj as LocalizationRequiredAttribute;
			return localizationRequiredAttribute != null && localizationRequiredAttribute.Required == this.Required;
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0000F1D3 File Offset: 0x0000D3D3
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
