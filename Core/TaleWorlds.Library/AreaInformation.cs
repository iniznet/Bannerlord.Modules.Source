using System;

namespace TaleWorlds.Library
{
	public struct AreaInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Temperature = reader.ReadFloat();
			this.Humidity = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Temperature);
			writer.WriteFloat(this.Humidity);
		}

		public float Temperature;

		public float Humidity;
	}
}
