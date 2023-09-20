using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithIntValueEventArgs
	{
		public PropertyChangedWithIntValueEventArgs(string propertyName, int value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public int Value { get; }
	}
}
