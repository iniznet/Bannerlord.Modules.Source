using System;

namespace SandBox.View
{
	// Token: 0x02000007 RID: 7
	public interface IChangeableScreen
	{
		// Token: 0x06000021 RID: 33
		bool AnyUnsavedChanges();

		// Token: 0x06000022 RID: 34
		bool CanChangesBeApplied();

		// Token: 0x06000023 RID: 35
		void ApplyChanges();

		// Token: 0x06000024 RID: 36
		void ResetChanges();
	}
}
