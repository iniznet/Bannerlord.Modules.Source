using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class AssertionConditionAttribute : Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}

		public AssertionConditionType ConditionType { get; private set; }
	}
}
