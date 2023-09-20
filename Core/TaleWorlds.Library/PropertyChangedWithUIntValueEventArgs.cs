using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithUIntValueEventArgs
	{
		public PropertyChangedWithUIntValueEventArgs(string propertyName, uint value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public uint Value { get; }
	}
}
