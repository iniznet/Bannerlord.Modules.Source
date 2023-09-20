using System;

namespace TaleWorlds.Library
{
	public struct AreaInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Temperature = reader.ReadFloat();
			this.Humidity = reader.ReadFloat();
			this.AreaType = reader.ReadInt();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Temperature);
			writer.WriteFloat(this.Humidity);
			writer.WriteInt(this.AreaType);
		}

		public float Temperature;

		public float Humidity;

		public int AreaType;
	}
}
