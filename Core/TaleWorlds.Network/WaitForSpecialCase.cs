using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000008 RID: 8
	public class WaitForSpecialCase : CoroutineState
	{
		// Token: 0x06000032 RID: 50 RVA: 0x000026EA File Offset: 0x000008EA
		public WaitForSpecialCase(WaitForSpecialCase.IsConditionSatisfiedDelegate isConditionSatisfiedDelegate)
		{
			this._isConditionSatisfiedDelegate = isConditionSatisfiedDelegate;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000026F9 File Offset: 0x000008F9
		protected internal override bool IsFinished
		{
			get
			{
				return this._isConditionSatisfiedDelegate();
			}
		}

		// Token: 0x04000019 RID: 25
		private WaitForSpecialCase.IsConditionSatisfiedDelegate _isConditionSatisfiedDelegate;

		// Token: 0x02000035 RID: 53
		// (Invoke) Token: 0x06000173 RID: 371
		public delegate bool IsConditionSatisfiedDelegate();
	}
}
