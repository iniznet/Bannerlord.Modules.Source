using System;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200001A RID: 26
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class EditorAttribute : Attribute
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x0000AD91 File Offset: 0x00008F91
		public EditorAttribute(bool includeInnerProperties = false)
		{
			this.IncludeInnerProperties = includeInnerProperties;
		}

		// Token: 0x04000104 RID: 260
		public readonly bool IncludeInnerProperties;
	}
}
