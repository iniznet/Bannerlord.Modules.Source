using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000D9 RID: 217
	[Flags]
	public enum ImplicitUseKindFlags
	{
		// Token: 0x040001FD RID: 509
		Default = 7,
		// Token: 0x040001FE RID: 510
		Access = 1,
		// Token: 0x040001FF RID: 511
		Assign = 2,
		// Token: 0x04000200 RID: 512
		InstantiatedWithFixedConstructorSignature = 4,
		// Token: 0x04000201 RID: 513
		InstantiatedNoFixedConstructorSignature = 8
	}
}
