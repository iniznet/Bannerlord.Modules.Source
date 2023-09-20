using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithBoolValueEventArgs
	{
		public PropertyChangedWithBoolValueEventArgs(string propertyName, bool value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public bool Value { get; }
	}
}
