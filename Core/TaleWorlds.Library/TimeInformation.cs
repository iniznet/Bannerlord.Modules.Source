using System;

namespace TaleWorlds.Library
{
	public struct TimeInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.TimeOfDay = reader.ReadFloat();
			this.NightTimeFactor = reader.ReadFloat();
			this.DrynessFactor = reader.ReadFloat();
			this.WinterTimeFactor = reader.ReadFloat();
			this.Season = reader.ReadInt();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.TimeOfDay);
			writer.WriteFloat(this.NightTimeFactor);
			writer.WriteFloat(this.DrynessFactor);
			writer.WriteFloat(this.WinterTimeFactor);
			writer.WriteInt(this.Season);
		}

		public float TimeOfDay;

		public float NightTimeFactor;

		public float DrynessFactor;

		public float WinterTimeFactor;

		public int Season;
	}
}
