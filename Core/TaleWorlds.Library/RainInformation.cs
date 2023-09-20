using System;

namespace TaleWorlds.Library
{
	public struct RainInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Density = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Density);
		}

		public float Density;
	}
}
