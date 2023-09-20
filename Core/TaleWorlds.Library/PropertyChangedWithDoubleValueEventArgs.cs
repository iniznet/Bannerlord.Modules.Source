using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithDoubleValueEventArgs
	{
		public PropertyChangedWithDoubleValueEventArgs(string propertyName, double value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public double Value { get; }
	}
}
