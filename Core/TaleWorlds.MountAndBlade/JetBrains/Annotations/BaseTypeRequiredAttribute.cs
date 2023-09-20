using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[BaseTypeRequired(typeof(Attribute))]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		public BaseTypeRequiredAttribute(Type baseType)
		{
			this.BaseTypes = new Type[] { baseType };
		}

		public Type[] BaseTypes { get; private set; }
	}
}
