using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithColorValueEventArgs
	{
		public PropertyChangedWithColorValueEventArgs(string propertyName, Color value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public Color Value { get; }
	}
}
