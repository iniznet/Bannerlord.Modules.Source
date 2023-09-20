using System;

namespace TaleWorlds.Library
{
	public struct FogInformation
	{
		public void DeserializeFrom(IReader reader)
		{
			this.Density = reader.ReadFloat();
			this.Color = reader.ReadVec3();
			this.Falloff = reader.ReadFloat();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteFloat(this.Density);
			writer.WriteVec3(this.Color);
			writer.WriteFloat(this.Falloff);
		}

		public float Density;

		public Vec3 Color;

		public float Falloff;
	}
}
