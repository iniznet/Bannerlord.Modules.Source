using System;

namespace TaleWorlds.Library
{
	public struct SunInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Altitude = reader.ReadFloat();
			this.Angle = reader.ReadFloat();
			this.Color = reader.ReadVec3();
			this.Brightness = reader.ReadFloat();
			this.MaxBrightness = reader.ReadFloat();
			this.Size = reader.ReadFloat();
			this.RayStrength = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Altitude);
			writer.WriteFloat(this.Angle);
			writer.WriteVec3(this.Color);
			writer.WriteFloat(this.Brightness);
			writer.WriteFloat(this.MaxBrightness);
			writer.WriteFloat(this.Size);
			writer.WriteFloat(this.RayStrength);
		}

		public float Altitude;

		public float Angle;

		public Vec3 Color;

		public float Brightness;

		public float MaxBrightness;

		public float Size;

		public float RayStrength;
	}
}
