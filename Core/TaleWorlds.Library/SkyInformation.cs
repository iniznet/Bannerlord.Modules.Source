using System;

namespace TaleWorlds.Library
{
	public struct SkyInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Brightness = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Brightness);
		}

		public float Brightness;
	}
}
