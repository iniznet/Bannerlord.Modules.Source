using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithValueEventArgs
	{
		public PropertyChangedWithValueEventArgs(string propertyName, object value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public object Value { get; }
	}
}
