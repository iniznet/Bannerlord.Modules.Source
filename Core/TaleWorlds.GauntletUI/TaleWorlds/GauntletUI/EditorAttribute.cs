using System;

namespace TaleWorlds.GauntletUI
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class EditorAttribute : Attribute
	{
		public EditorAttribute(bool includeInnerProperties = false)
		{
			this.IncludeInnerProperties = includeInnerProperties;
		}

		public readonly bool IncludeInnerProperties;
	}
}
