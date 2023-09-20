using System;

namespace TaleWorlds.Network
{
	public class WaitForSpecialCase : CoroutineState
	{
		public WaitForSpecialCase(WaitForSpecialCase.IsConditionSatisfiedDelegate isConditionSatisfiedDelegate)
		{
			this._isConditionSatisfiedDelegate = isConditionSatisfiedDelegate;
		}

		protected internal override bool IsFinished
		{
			get
			{
				return this._isConditionSatisfiedDelegate();
			}
		}

		private WaitForSpecialCase.IsConditionSatisfiedDelegate _isConditionSatisfiedDelegate;

		public delegate bool IsConditionSatisfiedDelegate();
	}
}
