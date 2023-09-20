using System;

namespace TaleWorlds.Library
{
	public struct PostProcessInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.MinExposure = reader.ReadFloat();
			this.MaxExposure = reader.ReadFloat();
			this.BrightpassThreshold = reader.ReadFloat();
			this.MiddleGray = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.MinExposure);
			writer.WriteFloat(this.MaxExposure);
			writer.WriteFloat(this.BrightpassThreshold);
			writer.WriteFloat(this.MiddleGray);
		}

		public float MinExposure;

		public float MaxExposure;

		public float BrightpassThreshold;

		public float MiddleGray;
	}
}
