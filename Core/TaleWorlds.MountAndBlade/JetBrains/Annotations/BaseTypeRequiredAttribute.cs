using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000D6 RID: 214
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[BaseTypeRequired(typeof(Attribute))]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x0000F24B File Offset: 0x0000D44B
		public BaseTypeRequiredAttribute(Type baseType)
		{
			this.BaseTypes = new Type[] { baseType };
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600086C RID: 2156 RVA: 0x0000F263 File Offset: 0x0000D463
		// (set) Token: 0x0600086D RID: 2157 RVA: 0x0000F26B File Offset: 0x0000D46B
		public Type[] BaseTypes { get; private set; }
	}
}
