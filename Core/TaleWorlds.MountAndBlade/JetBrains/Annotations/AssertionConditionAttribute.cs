using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000D0 RID: 208
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class AssertionConditionAttribute : Attribute
	{
		// Token: 0x06000864 RID: 2148 RVA: 0x0000F20B File Offset: 0x0000D40B
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x0000F21A File Offset: 0x0000D41A
		// (set) Token: 0x06000866 RID: 2150 RVA: 0x0000F222 File Offset: 0x0000D422
		public AssertionConditionType ConditionType { get; private set; }
	}
}
