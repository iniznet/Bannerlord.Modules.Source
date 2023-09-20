using System;

namespace TaleWorlds.Library
{
	public class PropertyChangedWithVec2ValueEventArgs
	{
		public PropertyChangedWithVec2ValueEventArgs(string propertyName, Vec2 value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public string PropertyName { get; }

		public Vec2 Value { get; }
	}
}
