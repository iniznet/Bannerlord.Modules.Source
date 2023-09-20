using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithFloatValueEventArgs
	{
		public PropertyChangedWithFloatValueEventArgs(string propertyName, float value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public float Value { get; }
	}
}
